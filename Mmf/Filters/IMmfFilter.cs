using System.Collections.Generic;

namespace InfoReader.Mmf.Filters
{
    public interface IMmfFilter
    {
        string MmfType { get; }
        MmfBase Filter(Dictionary<string, object> config);
    }
}

