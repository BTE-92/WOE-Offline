#!/usr/bin/env python3
"""Split a universal (armv7 + arm64) iOS .ipa into two thin .ipa files, one per architecture.

Why: Sideloadly 0.70.1 signs the 32-bit slice alone and the 64-bit slice alone, but fails on the combined
universal Mach-O ("macho sign/edit failed (-18)"), whatever the slice order or alignment. Splitting the same
build gives two installable files.

  python split_universal_ipa.py WhatOnEarth_Universal.ipa [output_dir]

Output: <name>_arm64_64bit.ipa and <name>_armv7_32bit.ipa. Only the main executable and the
UIRequiredDeviceCapabilities entry of Info.plist change. No third-party packages needed.
"""
import os
import plistlib
import struct
import sys
import zipfile

CPU_ARMV7 = 0x0000000C
CPU_ARM64 = 0x0100000C
FAT_MAGIC = 0xCAFEBABE


def read_slices(fat):
    if struct.unpack('>I', fat[:4])[0] != FAT_MAGIC:
        raise SystemExit('The executable is not a universal (fat) binary: nothing to split.')
    count = struct.unpack('>I', fat[4:8])[0]
    slices = {}
    for i in range(count):
        cputype, _sub, offset, size, _align = struct.unpack('>IIIII', fat[8 + i * 20:28 + i * 20])
        slices[cputype] = fat[offset:offset + size]
    return slices


def main():
    if len(sys.argv) < 2:
        raise SystemExit(__doc__)
    src = sys.argv[1]
    out_dir = sys.argv[2] if len(sys.argv) > 2 else os.path.dirname(os.path.abspath(src))
    os.makedirs(out_dir, exist_ok=True)
    stem = os.path.splitext(os.path.basename(src))[0]

    zin = zipfile.ZipFile(src)
    plist_name = [n for n in zin.namelist() if n.count('/') == 2 and n.endswith('.app/Info.plist')][0]
    app_dir = plist_name[:-len('Info.plist')]
    info = plistlib.loads(zin.read(plist_name))
    exe_name = app_dir + info['CFBundleExecutable']
    slices = read_slices(zin.read(exe_name))
    missing = [name for name, cpu in (('armv7', CPU_ARMV7), ('arm64', CPU_ARM64)) if cpu not in slices]
    if missing:
        raise SystemExit('Missing slice(s) in the universal binary: ' + ', '.join(missing))

    for suffix, cpu, caps in (('arm64_64bit', CPU_ARM64, ['arm64']), ('armv7_32bit', CPU_ARMV7, ['armv7'])):
        new_info = dict(info)
        new_info['UIRequiredDeviceCapabilities'] = caps
        dst = os.path.join(out_dir, '%s_%s.ipa' % (stem, suffix))
        with zipfile.ZipFile(dst, 'w', zipfile.ZIP_DEFLATED, compresslevel=6) as zout:
            for item in zin.infolist():
                if item.filename == exe_name:
                    data = slices[cpu]
                elif item.filename == plist_name:
                    data = plistlib.dumps(new_info, fmt=plistlib.FMT_BINARY)
                else:
                    data = zin.read(item.filename)
                out = zipfile.ZipInfo(item.filename, item.date_time)
                out.compress_type = zipfile.ZIP_DEFLATED
                out.external_attr = (0o100755 << 16) if item.filename == exe_name else (item.external_attr or (0o100644 << 16))
                zout.writestr(out, data)
        print('%s  (%d MB)' % (dst, os.path.getsize(dst) // (1024 * 1024)))


if __name__ == '__main__':
    main()
