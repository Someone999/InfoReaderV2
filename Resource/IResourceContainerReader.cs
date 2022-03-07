using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoReader.Resource
{
    public interface IResourceContainerReader
    {
        int Count { get; }
        bool IsCompressed { get; }
        Resource? GetResource(string resourceName);
        Resource[] GetResources();
        void Close();
    }
}
