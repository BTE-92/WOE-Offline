# Building the offline game for iOS (32-bit and 64-bit)

The iOS build is exported as an Xcode project by Unity (IL2CPP) and compiled on a Mac. Nothing here needs a paid
Apple developer account: the IPA is built unsigned and signed afterwards by Sideloadly with a free Apple ID.

## Why two IPAs (32-bit and 64-bit) and not one universal IPA

A single universal IPA (armv7 + arm64) compiles fine, but **Sideloadly 0.70.1 refuses to sign it**
(`macho sign/edit failed (-18)`). What was tested:

| File | Sideloadly signing |
|---|---|
| armv7 slice alone | OK |
| arm64 slice alone | OK |
| arm64 slice padded to 115 MB | OK (so it is not a size limit) |
| Universal, armv7 first, 16 KB alignment | fails |
| Universal, arm64 first, 16 KB alignment | fails |
| Universal, armv7 first, lipo-standard alignment | fails |
| Universal, arm64 first, lipo-standard alignment | fails |

So the same build is split into one IPA per architecture with `Tools/ios/split_universal_ipa.py`.
arm64 is what iOS 11+ devices run; the armv7 IPA is for 32-bit devices (iPhone 4s/5/5c, iOS 10 or older).

## Requirements that are NOT in this repository

* ChipmunkPro 6.1.5 + GPC sources (commercially licensed, not redistributed here), placed in
  `Assets/Plugins/iOS/Chipmunk/` (`src/*.c`, `src/constraints/*.c`, `include/**`, and `src/prime.h`).
  `include/chipmunk/chipmunk_types.h` must start with:

  ```c
  #ifndef NDEBUG
  #define NDEBUG 1          // the shipping Android/Windows libraries were built with NDEBUG
  #endif
  #undef CP_USE_CGPOINTS
  #define CP_USE_CGPOINTS 0 // cpVect must be 2 floats (like Unity's Vector2), not CGPoint (2 doubles on arm64)
  ```

  Without the second change, arm64 iOS passes `cpVect` by value and reads `Vector2[]` arrays with the wrong size,
  which crashes the level editor. Without the first one, Chipmunk `abort()`s on soft assertions.
* The FMOD static libraries in `Assets/Plugins/iOS/` are already in the repo (armv7 + arm64).
* Facebook is replaced by no-op natives (`Assets/Plugins/iOS/FacebookIOSStubs.c`); Game Center is skipped on iOS
  (`GameCenterManager.Login`), and Push / Game Center capabilities are stripped from the Xcode project.

## 1. Export the Xcode project (Windows or Mac, Unity 2018.4.8f1 with iOS Build Support)

`Assets/Editor/BuildIOS.cs` does the export and the Xcode post-processing. Environment variables:

* `WOE_IOS_ARCH`: `arm64` (default), `armv7` or `universal`
* `WOE_IOS_OUT`: output folder (default `Builds/iOS`)

```bat
set WOE_IOS_ARCH=universal
set WOE_IOS_OUT=C:/out/WOE-iOS
Unity.exe -batchmode -nographics -quit -projectPath <this repo> -buildTarget iOS -executeMethod BuildIOS.Build -logFile ios.log
```

The log should end with `BUILD RESULT: iOS Succeeded`. Two shader errors for `Character/Disintegrate Diffuse`
(shadow collector pass) also appear in the Android and Windows builds and are harmless.

## 2. Compile on the Mac (Xcode 10.1 was used; iOS SDK 12.1)

macOS does not keep the script permissions from a zip, so first:

```sh
chmod +x MapFileParser.sh process_symbols.sh
```

Build both architectures unsigned (20 to 60 minutes):

```sh
xcodebuild -project Unity-iPhone.xcodeproj -scheme Unity-iPhone -configuration Release -sdk iphoneos \
  -derivedDataPath build/dd CODE_SIGNING_ALLOWED=NO CODE_SIGNING_REQUIRED=NO CODE_SIGN_IDENTITY="" \
  ARCHS="armv7 arm64" VALID_ARCHS="armv7 arm64" ONLY_ACTIVE_ARCH=NO

rm -rf Payload && mkdir Payload && cp -R build/dd/Build/Products/Release-iphoneos/whatonearth.app Payload/
zip -qry WhatOnEarth_Universal.ipa Payload
lipo -info Payload/whatonearth.app/whatonearth        # expected: armv7 arm64
```

## 3. Split into 32-bit and 64-bit IPAs (any machine with Python 3.6+)

```sh
python3 Tools/ios/split_universal_ipa.py WhatOnEarth_Universal.ipa out/
```

This writes `*_arm64_64bit.ipa` and `*_armv7_32bit.ipa`. Only the executable and
`UIRequiredDeviceCapabilities` (`arm64` / `armv7`) differ; everything else is identical.

## 4. Install

Sideloadly with a free Apple ID (the app expires after 7 days). On the device: Settings, General, Device Management,
trust the profile; on iOS 16+ also enable Developer Mode. 32-bit devices cannot run iOS 11 or later.

## Status

* 64-bit: signs and installs on an iPhone 15 (iOS 26); an earlier build launched there. The Chipmunk crash fixes
  described above are not yet confirmed on a device.
* 32-bit: compiles and signs; not yet confirmed running on a real 32-bit device. These devices have 512 MB to 1 GB of
  RAM, so the game may be slow or be closed by iOS when memory runs out.
