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

        private List<Resource> _resources = new List<Resource>();
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
            GZip.Decompress(stream, decompressed, autoClose);
            _resources = ResourceContainerTools.GeneralParser(decompressed);
            decompressed.Close();
            return stream;
        }

        public int Count => _resources.Count;
        public bool IsCompressed => true;
        public Resource? GetResource(string resourceName)
        {
            return _resources.FirstOrDefault(r => r.ResourceName.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase));
        }

        public Resource[] GetResources()
        {
            return _resources.ToArray();
        }

        public void Close()
        {
            _innerStream.Close();
        }
    }
}
