using System;
using System.Collections;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

public static class FilePacker
{
	public static MemoryStream ZipStream(MemoryStream inputStream, int compressLevel = 3)
	{
		inputStream.Position = 0L;
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(inputStream.GetBuffer().Length);
		ZipOutputStream zipOutputStream = new ZipOutputStream(memoryStream);
		zipOutputStream.SetLevel(compressLevel);
		ZipEntry entry = new ZipEntry("LevelData");
		zipOutputStream.PutNextEntry(entry);
		StreamUtils.Copy(inputStream, zipOutputStream, new byte[4096]);
		zipOutputStream.CloseEntry();
		zipOutputStream.IsStreamOwner = false;
		zipOutputStream.Close();
		memoryStream.Position = 0L;
		return memoryStream;
	}

	public static byte[] ZipBytes(byte[] inputBytes, int compressLevel = 3)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(inputBytes.Length);
		ZipOutputStream zipOutputStream = new ZipOutputStream(memoryStream);
		zipOutputStream.SetLevel(compressLevel);
		ZipEntry entry = new ZipEntry("LevelData");
		zipOutputStream.PutNextEntry(entry);
		zipOutputStream.Write(inputBytes, 0, inputBytes.Length);
		zipOutputStream.CloseEntry();
		zipOutputStream.IsStreamOwner = false;
		zipOutputStream.Close();
		memoryStream.Position = 0L;
		return memoryStream.ToArray();
	}

	public static byte[] UnZipBytes(byte[] inputBytes)
	{
		MemoryStream memoryStream = new MemoryStream(inputBytes, 0, inputBytes.Length, false, true);
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		int num = binaryReader.ReadInt32();
		byte[] array = new byte[num];
		ZipInputStream zipInputStream = new ZipInputStream(memoryStream);
		zipInputStream.GetNextEntry();
		zipInputStream.Read(array, 0, num);
		zipInputStream.CloseEntry();
		zipInputStream.Close();
		return array;
	}

	public static byte[] CombineByteArrays(byte[][] _arraysToCombine, Hashtable _header)
	{
		int num = 0;
		for (int i = 0; i < _arraysToCombine.Length; i++)
		{
			num += _arraysToCombine[i].Length;
		}
		byte[] array = new byte[num];
		string text = string.Empty;
		int num2 = 0;
		for (int j = 0; j < _arraysToCombine.Length; j++)
		{
			Buffer.BlockCopy(_arraysToCombine[j], 0, array, num2, _arraysToCombine[j].Length);
			num2 += _arraysToCombine[j].Length;
			text = text + _arraysToCombine[j].Length + ((j >= _arraysToCombine.Length - 1) ? string.Empty : ",");
		}
		_header.Add("Content-Type", "application/octet-stream");
		_header.Add("FILE_SIZES", text);
		return array;
	}

	public static byte[] StringToByteArray(string str)
	{
		return Encoding.UTF8.GetBytes(str);
	}

	public static string ByteArrayToString(byte[] bytes)
	{
		return Encoding.UTF8.GetString(bytes);
	}
}
