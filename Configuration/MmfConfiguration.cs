using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using InfoReader.Configuration.Attributes;
using InfoReader.Configuration.Converter;
using InfoReader.Configuration.Elements;
using InfoReader.Configuration.Gui;
using InfoReader.Mmf;
using InfoReader.Tools;
using InfoReader.Tools.I8n;
using InfoReader.Window;
using Nett;

namespace InfoReader.Configuration
{
    public class MmfConfiguration: IGuiConfigurable
    {
        
        public string ConfigFilePath => DefaultFilePath.CurrentConfigFile;
        public Type ConfigElementType => typeof(TomlConfigElement);
        public string ConfigArgName => "mmf";
        [ConfigItem("MmfConfigs.Mmfs",converterType: typeof(MmfListConverter))]
        public List<MmfBase> MmfList { get; set; } = new();
        [ConfigItem("MmfConfigs.Encoding", "L::LANG_CFG_MMFENCODING",typeof(EncodingConverter))]
        public Encoding MmfEncoding { get; set; } = Encoding.UTF8;
        public void Save(IConfigElement element, Dictionary<Type,object?[]>? typeConverterArgs)
        {
            Type cfgType = typeof(MmfConfiguration);
            var bindingAttr = BindingFlags.Instance | BindingFlags.Public;
            foreach (var propertyInfo in ReflectionTools.GetPropertiesWithAttribute<ConfigItemAttribute>(cfgType, bindingAttr))
            {
                IConfigElement tmp = element;
                var cfgInfo = propertyInfo.Item2[0];
                string[] parts = cfgInfo.ConfigPath.Split('.');
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    tmp = tmp[parts[i]];
                }

                object? currentValue = propertyInfo.Item1.GetValue(this);
                if (cfgInfo.ConverterType != null)
                {
                    object?[] args = Array.Empty<object>();
                    if (typeConverterArgs != null && typeConverterArgs.ContainsKey(cfgInfo.ConverterType))
                    {
                        args = typeConverterArgs[cfgInfo.ConverterType];
                    }
                    currentValue = (IConfigConverter?) ReflectionTools.CreateInstance(cfgInfo.ConverterType, args);
                }

                tmp.SetValue(parts.Last(), currentValue);
            }
        }

        public Form CreateConfigWindow()
        {
            Form form = new Form();
            AddControls(form);
            return form;
        }

        public void AddControls(Form form)
        {
            form.Text = LocalizationInfo.Current.Translations["LANG_WINDOW_MMF_CONFIG_TITLE"];
            List<Control> controls = new List<Control>();
            int mmfWidth = 0, mmfHeight = 0;
            Label l = new Label();
            l.Top = 5;
            l.Left = 0;
            for (int i = 0; i < MmfList.Count; i++)
            {
                MmfConfigControl control = new(MmfList[i]);
                mmfWidth = control.Width;
                mmfHeight = control.Height;
                control.Top = i * control.Height + 10 * i + 50;
                control.Visible = true;
                controls.Add(control);
            }
            form.AutoScroll = true;
            form.Width = mmfWidth + 50;
            form.Height = mmfHeight * (MmfList.Count > 3 ? 3 : MmfList.Count) + 30 * 3;
            form.Controls.AddRange(controls.ToArray());
        }
    }
}
