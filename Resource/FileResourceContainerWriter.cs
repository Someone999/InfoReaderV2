using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoReader.Resource
{
    public class FileResourceContainerWriter : IResourceContainerWriter
    {
        private List<Stream> _streams = new List<Stream>();
        public List<string> Files { get; set; } = new();

        public FileStream? WriteToFile(string path, bool autoClose = true) => (FileStream?)Write(File.Create(path), autoClose);
        public void Close()
        {
            int count = 0;
            foreach (var stream in _streams)
            {
                stream.Close();
                //Console.WriteLine($"[{nameof(FileResourceContainerWriter)}] Stream{count++} closed");
            }
        }

        public Stream? Write(Stream outputStream, bool autoClose = true)
        {
            ResourceContainerTools.GeneralWriter(Files, outputStream, autoClose);
            _streams.Add(outputStream);
            return autoClose ? null : outputStream;
        }
    }
}
