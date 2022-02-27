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
        public IConfigElement? ConfigElement { get; set; }

        [ConfigItem("MmfConfigs.Mmfs",converterType: typeof(MmfListConverter))]
        public List<MmfBase> MmfList { get; set; } = new();
        [ConfigItem("MmfConfigs.Encoding", "L::LANG_CFG_MMFENCODING",typeof(EncodingConverter))]
        public Encoding MmfEncoding { get; set; } = Encoding.UTF8;
        public void Save(Dictionary<Type,object?[]>? typeConverterArgs = null)
        {
            Type cfgType = typeof(MmfConfiguration);
            var bindingAttr = BindingFlags.Instance | BindingFlags.Public;
            foreach (var propertyInfo in ReflectionTools.GetPropertiesWithAttribute<ConfigItemAttribute>(cfgType, bindingAttr))
            {
                typeConverterArgs ??= new Dictionary<Type, object?[]>()
                {
                    {typeof(MmfListConverter), new object?[] {null}}
                };
                IConfigElement tmp = ConfigElement ?? throw new InvalidOperationException();
                var cfgInfo = propertyInfo.Item2[0];
                string[] parts = cfgInfo.ConfigPath.Split('.');
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    tmp = tmp[parts[i]];
                }
                if (GetType() != propertyInfo.Item1.DeclaringType)
                {
                    return;
                }

                object? currentValue = propertyInfo.Item1.GetValue(this);
                object?[] args = Array.Empty<object>();
                if (cfgInfo.ConverterType != null && typeConverterArgs.ContainsKey(cfgInfo.ConverterType))
                {
                    args = typeConverterArgs[cfgInfo.ConverterType];
                }
                var converter = (IConfigConverter?) ReflectionTools.CreateInstance(cfgInfo.ConverterType ?? throw new InvalidOperationException(), args);
                currentValue = converter?.ToDictionary(currentValue) ?? converter?.ToValue(currentValue) ?? throw new ArgumentNullException();
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
            Button addMmfBtn = new Button();
            addMmfBtn.Height = 15;
            addMmfBtn.Width = 20;
            addMmfBtn.Top = 5;
            addMmfBtn.Text = LocalizationInfo.Current.Translations["LANG_UI_ADD_MMF"];
            addMmfBtn.Click += (_, _) =>
            {
                MmfSelectWindow selectWindow = new MmfSelectWindow();
                selectWindow.ShowDialog();
                MmfBase? mmfBase = selectWindow.GetCreatedMmf();
                if (mmfBase == null)
                    return;
                MmfManager.GetInstance().Add(mmfBase);
                MmfConfigControl control = new MmfConfigControl(mmfBase);
                control.Top = (form.Controls.Count - 1) * control.Height + 10 * (form.Controls.Count - 1) + 50;
                control.Visible = true;
                form.Controls.Add(control);
                form.Refresh();
            };
            form.Controls.Add(addMmfBtn);
            Label l = new Label();
            l.Top = 50;
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
            addMmfBtn.Left = mmfWidth - 50;
            form.Height = mmfHeight * (MmfList.Count > 3 ? 3 : MmfList.Count) + 30 * 3;
            form.Controls.AddRange(controls.ToArray());
        }
    }
}
