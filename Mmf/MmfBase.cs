using System.Collections.Generic;
using System.IO;
using System.Text;
using InfoReader.ExpressionMatcher;
using InfoReader.ExpressionParser.Tools;
using InfoReader.Tools.I8n;
using Nett;
using YamlDotNet.Serialization;

namespace InfoReader.Mmf
{
    public abstract class MmfBase: EqualityComparer<MmfBase>, IMmf
    {
        private readonly System.Timers.Timer _fileReadTimer = new System.Timers.Timer();
        public abstract string MmfType { get; }
        public string Name { get; set; }
        [TomlIgnore]
        public System.IO.MemoryMappedFiles.MemoryMappedFile MappedFile { get; set; }
        [TomlIgnore]
        public Stream MappedFileStream => MappedFile.CreateViewStream();
        public event EnabledStateChangedEventHandler? OnEnabledStateChanged;
        public int UpdateInterval { get; set; } = 100;
        protected bool InternalEnabled;
        public bool Enabled
        {
            get => InternalEnabled;
            set
            {
                InternalEnabled = value;
                OnEnabledStateChanged?.Invoke(this, value);
            }
        }
        public string FormatFile { get; set; } = "";
        [TomlIgnore]
        public string Format { get; private set; } = "";
        protected MmfBase(string name)
        {
            MappedFile = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateOrOpen(name, 2048);
            Name = name;
            _fileReadTimer.Interval = 500;
            _fileReadTimer.AutoReset = true;
            _fileReadTimer.Elapsed += _fileReadTimer_Elapsed;
            _fileReadTimer.Enabled = true;
        }

        public override bool Equals(MmfBase? x, MmfBase? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Name == y.Name;
        }

        public override int GetHashCode(MmfBase obj)
        {
            return obj.Name.GetHashCode();
        }
        protected bool Equals(MmfBase other)
        {
            return Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MmfBase)obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public static bool operator ==(MmfBase? a, MmfBase? b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            {
                return true;
            }
            
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Equals(a, b);
        }

        public static bool operator !=(MmfBase? a, MmfBase? b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            {
                return false;
            }

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return true;
            }

            return !a.Equals(a, b);
        }


        private void _fileReadTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!File.Exists(FormatFile))
            {
                File.Create(FormatFile).Close();
            }
            Format = Enabled ? File.ReadAllText(FormatFile) : string.Empty;
        }

        public virtual void Update(InfoReaderPlugin instance)
        {
            ValueExpressionMatcher matcher = new ValueExpressionMatcher();
            var vals = matcher.Match(Format);
            StringBuilder innerFormat = new StringBuilder(Format);
            foreach (var val in vals)
            {
                string[] parts = val.Split(':');
                string format = "";
                if (parts.Length > 1)
                {
                    format = parts[1];
                }
                
                var rslt = RpnTools.CalcRpnStack(RpnTools.ToRpnExpression(parts[0].Trim('$','{','}')), instance.MemoryDataSource);
                innerFormat.Replace(val, rslt.ToString(format,LocalizationInfo.Current.Culture));
            }

            byte[] bts = Encoding.UTF8.GetBytes(innerFormat.ToString());
            MappedFileStream.Write(bts, 0, bts.Length);
        }
    }
}
