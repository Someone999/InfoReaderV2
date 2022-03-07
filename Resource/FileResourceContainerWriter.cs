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
        public List<string> Files { get; set; } = new();

        public FileStream? WriteToFile(string path, bool autoClose = true) => (FileStream?)Write(File.Create(path), autoClose);
        public Stream? Write(Stream outputStream, bool autoClose = true)
        {
            ResourceContainerTools.GeneralWriter(Files, outputStream, autoClose);
            return autoClose ? null : outputStream;
        }
    }
}
