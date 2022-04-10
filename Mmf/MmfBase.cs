using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using InfoReader.ExpressionMatcher;
using InfoReader.Tools;
using InfoReader.Tools.I8n;
using Lexer;
using Lexer.Expressions;
using Lexer.RpnExpression;
using Nett;
using osuTools.OrtdpWrapper;
using RpnTools = InfoReader.ExpressionParser.Tools.RpnTools;

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
        public int UpdateInterval { get; set; } = 5;
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
            if (string.IsNullOrEmpty(FormatFile))
            {
                return;
            }
            if (!File.Exists(FormatFile))
            {
                FileTools.ConfirmDirectory(FormatFile);
                File.Create(FormatFile).Close();
            }

            try
            {
                Format = Enabled ? File.ReadAllText(FormatFile) : string.Empty;
            }
            catch (Exception)
            {
                //None
            }
            
        }

        public virtual void Update(InfoReaderPlugin instance)
        {
            if (!RpnGlobalTypes.VariableReflectTypes.ContainsKey(typeof(OrtdpWrapper)))
            {
                RpnGlobalTypes.VariableReflectTypes.Add(typeof(OrtdpWrapper), instance.MemoryDataSource);
            }
            ValueExpressionMatcher matcher = new ValueExpressionMatcher();
            var vals = matcher.Match(Format);
            StringBuilder innerFormat = new StringBuilder(Format);
            foreach (var val in vals)
            {
                string tmpVal = val;
                if (tmpVal.StartsWith("${"))
                {
                    tmpVal = val.Substring(2);
                }

                if (tmpVal.EndsWith("}"))
                {
                    //tmpVal = val.Substring(0,tmpVal.Length - 1);
                    tmpVal = tmpVal.Substring(0, tmpVal.Length - 1);
                }
                string[] parts = tmpVal.Split(':');
                string format = "";
                if (parts.Length > 1)
                {
                    format = parts[1];
                }
                IRpnValue rslt = new RpnString("failed");

                parts[0] = instance.LowerCasedVariables[parts[0].ToLower()].Name;
                try
                {
                    CodeLexer lexer = new CodeLexer(new StringReader(parts[0]));
                    lexer.Parse();
                    rslt = new CalculationExpression(lexer.Tokens).GetRpnValue();
                }
                catch
                {
                    // ignored
                }


                //var rslt = RpnTools.CalcRpnStack(RpnTools.ToRpnExpression(parts[0]), instance.MemoryDataSource);
                //var rslt = RpnTools.CalcRpnStack(RpnTools.ToRpnExpression(parts[0].Trim('$','{','}')), instance.MemoryDataSource);
                innerFormat.Replace(val, rslt.ToString(format, LocalizationInfo.Current.Culture));
            }

            innerFormat.Append('\0', 4);
            string formatted = innerFormat.ToString();
            byte[] bts = Encoding.UTF8.GetBytes(formatted);
            MappedFileStream.Write(bts, 0, bts.Length);
        }
    }
}
