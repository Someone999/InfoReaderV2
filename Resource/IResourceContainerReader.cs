using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoReader.Resource
{
    public interface IResourceContainerReader
    {
        int Count { get; }
        bool IsCompressed { get; }
        ResourceFileInfo? GetResource(string resourceName);
        ResourceFileInfo[] GetResources();
        void Close();
    }
}
