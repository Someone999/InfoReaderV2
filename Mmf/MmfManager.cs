using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InfoReader.Mmf
{
    public class MmfManager
    {
        readonly List<MmfBase> _mmfList = new List<MmfBase>();
        readonly InfoReaderPlugin _plugin;
        private MmfManager(InfoReaderPlugin infoReader)
        {
            _plugin = infoReader;
        }
        private static MmfManager? _mgr;
        private static readonly object StaticLocker = new();
        public static MmfManager GetInstance(InfoReaderPlugin infoReader)
        {
            if (_mgr == null)
            {
                lock (StaticLocker)
                {
                    _mgr = new MmfManager(infoReader);
                }
            }
            return _mgr;
        }

        public bool Add(MmfBase mmfBase)
        {
            if (_mmfList.Any(m => m.Name == mmfBase.Name))
                return false;
            _mmfList.Add(mmfBase);
            return true;
        }

        public bool Remove(MmfBase mmfBase)
        {
            return _mmfList.Remove(mmfBase);
        }

        public MmfBase[] Mmfs => _mmfList.ToArray();
        private CancellationTokenSource? _cancellationTokenSource;
        void UpdateThread(MmfBase mmf, int interval)
        {
            while (true)
            {
                Thread.Sleep(interval);
                mmf.Update(_plugin);
            }
        }
        public void StartUpdate(int interval)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            foreach (var mmfBase in Mmfs)
            {
                Task.Run(() => UpdateThread(mmfBase, interval), _cancellationTokenSource.Token);
            }
        }

        public void StopUpdate()
        {
            _cancellationTokenSource?.Cancel();
        }

    }
}
