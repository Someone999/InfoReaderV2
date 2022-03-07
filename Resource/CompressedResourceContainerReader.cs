using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.GZip;

namespace InfoReader.Resource
{
    public class CompressedResourceContainerReader:IResourceContainerReader
    {

        private List<ResourceFileInfo> _resources = new List<ResourceFileInfo>();
        private Stream _innerStream;
        public CompressedResourceContainerReader(Stream stream, bool autoClose)
        {
            _innerStream = ReadFromStream(stream, autoClose);
        }

        public CompressedResourceContainerReader(string path, bool autoClose)
        {
            _innerStream = ReadFromStream(File.OpenRead(path), autoClose);
        }

        Stream ReadFromStream(Stream stream, bool autoClose = true)
        {
            MemoryStream decompressed = new MemoryStream();
            GZip.Decompress(stream, decompressed, false);
            _resources = ResourceContainerTools.GeneralParser(decompressed, autoClose);
            decompressed.Close();
            return autoClose ? Stream.Null : stream;
        }

        public int Count => _resources.Count;
        public bool IsCompressed => true;
        public ResourceFileInfo? GetResource(string resourceName)
        {
            return _resources.FirstOrDefault(r => r.ResourceName.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase));
        }

        public ResourceFileInfo[] GetResources()
        {
            return _resources.ToArray();
        }

        public void Close()
        {
            //Console.WriteLine($"[{nameof(CompressedResourceContainerReader)}] Stream closed");
            _innerStream.Close();
        }
    }
}
