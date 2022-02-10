using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InfoReader.Tools;

namespace InfoReader.Mmf
{
    public delegate void EnabledStateChangedEventHandler(MmfBase mmf, bool enabled);
    public class MmfManager
    {
        readonly List<MmfBase> _mmfList = new List<MmfBase>();
        readonly InfoReaderPlugin _plugin;

        private readonly Dictionary<string, (MmfBase, CancellationTokenSource)> _updatingMmfs = new();
        private MmfManager(InfoReaderPlugin infoReader)
        {
            _plugin = infoReader;
        }
        private static MmfManager? _mgr;
        private static readonly object StaticLocker = new();
        public MmfBase? FindMmf(string mmf) => Mmfs.FirstOrDefault(m => m.Name == mmf);
       

        public static MmfManager GetInstance(InfoReaderPlugin infoReader)
        {
            return _mgr ??= CreateNewInstance(infoReader);
        }

        internal static MmfManager CreateNewInstance(InfoReaderPlugin infoReader)
        {
            lock (StaticLocker)
            {
                return _mgr = new MmfManager(infoReader);
            }
        }

        public bool Add(MmfBase mmfBase)
        {
            if (_mmfList.Any(m => m.Name == mmfBase.Name))
                return false;
            if (mmfBase.Enabled)
                _updatingMmfs.Add(mmfBase.Name, (mmfBase, new CancellationTokenSource()));
            mmfBase.OnEnabledStateChanged += MmfBase_OnEnabledStateChanged;
            _mmfList.Add(mmfBase);
            return true;
        }

        private void MmfBase_OnEnabledStateChanged(MmfBase mmf, bool enabled)
        {
            if (enabled)
            {
                if (_updatingMmfs.ContainsKey(mmf.Name))
                    return;
                var val = (mmf, new CancellationTokenSource());
                _updatingMmfs.Add(mmf.Name, val);
                Task.Run(() => UpdateMmf(val.mmf, val.Item2.Token));
                Logger.LogNotification($"[Mmf::{mmf.Name}] Enabled.");
            }
            else
            {
                _updatingMmfs[mmf.Name].Item2.Cancel();
                _updatingMmfs.Remove(mmf.Name);
                Logger.LogNotification($"[Mmf::{mmf.Name}] Disabled.");
            }
        }

        public bool Remove(MmfBase mmfBase)
        {
            return _mmfList.Remove(mmfBase);
        }

        public MmfBase[] Mmfs => _mmfList.ToArray();

        void UpdateMmf(MmfBase mmf, CancellationToken cancellationToken)
        {
            while (mmf.Enabled)
            {
                Thread.Sleep(mmf.UpdateInterval);
                mmf.Update(_plugin);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
        public void StartUpdate(int interval)
        {
            foreach (var mmfBase in _updatingMmfs)
            {
                Task.Run(() => UpdateMmf(mmfBase.Value.Item1, mmfBase.Value.Item2.Token));
            }
        }

        public void StopUpdate()
        {
            foreach (var updatingMmf in _updatingMmfs)
            {
                updatingMmf.Value.Item2.Cancel();
            }
        }

    }
}
