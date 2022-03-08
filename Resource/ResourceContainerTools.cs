using System;
using System.Collections.Generic;
using System.IO;

namespace InfoReader.Resource;

public static class ResourceContainerTools
{
    public static List<ResourceFileInfo> GeneralParser(Stream stream, bool autoClose)
    {
        if (stream.CanSeek && stream.Position != 0)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }
        BinaryReader reader = new BinaryReader(stream);
        List<ResourceFileInfo> resources = new List<ResourceFileInfo>();
        int fileCount = reader.ReadInt32();
        byte tag = reader.ReadByte();
        if (tag != 0xef)
        {
            throw new InvalidOperationException("Not a valid resource file.");
        }

        for (int i = 0; i < fileCount; i++)
        {
            string filePath = reader.ReadString();
            long byteLength = reader.ReadInt64();
            List<byte[]> dataSegments = new List<byte[]>();
            long segCount = (long)Math.Ceiling(byteLength / 16384.0);
            int offset = 0;
            for (int j = 0; j < segCount; j++)
            {
                byte[] data = new byte[16384];
                //int readSize = byteLength - offset > 16384 ? 16384 : (int)byteLength;
                int readSize = byteLength - offset > 16384 ? 16384 : (int)(byteLength - offset);
                if (readSize <= 0)
                    break;
                int realRead = reader.Read(data, 0, readSize);
                if (realRead != readSize)
                {
                    throw new InvalidOperationException("Not a valid resource file.");
                }
                dataSegments.Add(data);
                offset += readSize;
            }
            byte endFlag = reader.ReadByte();
            if (endFlag != 0xff)
            {
                throw new InvalidOperationException("Not a valid resource file.");
            }
            resources.Add(new ResourceFileInfo(filePath, byteLength, new ResourceStream(dataSegments, byteLength)));
        }

        if (autoClose)
        {
            //Console.WriteLine("Read stream closed");
            reader.Close();
        }

        return resources;
    }

    public static BinaryWriter? GeneralWriter(List<string> files, Stream containerStream, bool autoClose = true)
    {
        bool fileNotExists = false;
        BinaryWriter writer = new BinaryWriter(containerStream);
        int fileCount = files.Count;
        writer.Write(fileCount);
        writer.Write((byte)0xef);
        foreach (var file in files)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine("File not exist.");
                fileNotExists = true;
            }
            byte[] bts = File.ReadAllBytes(file);
            long len = bts.LongLength;
            writer.Write(file);
            writer.Write(len);
            writer.Write(bts);
            writer.Write((byte)0xff);
        }

        if (autoClose)
        {
            //Console.WriteLine("Write stream closed");
            writer.Close();
        }

        if (fileNotExists)
        {
            Console.WriteLine("The resource file may be incomplete.");
        }
        return autoClose ? null : writer;
    }
}