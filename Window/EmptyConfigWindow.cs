using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InfoReader.Configuration.Attributes;
using InfoReader.Tools;
using InfoReader.Configuration;
using InfoReader.Configuration.Gui;

namespace InfoReader.Window
{
    public partial class EmptyConfigWindow : Form
    {
        public EmptyConfigWindow(IConfigurable configurable)
        {
            InitializeComponent();
            AddComponents(configurable);

        }

        Control ProcessNormalValue(PropertyInfo propertyInfo, ConfigItemAttribute attr)
        {
            Label l = new Label();
            l.Text = attr.DisplayName ?? propertyInfo.Name;
            l.Font = new Font(new FontFamily("微软雅黑"), 10);
            l.Enabled = true;
            l.Visible = true;
            return l;
        }
        void AddComponents(IConfigurable configurable)
        {
            if (configurable is IGuiConfigurable guiConfigurable)
            {
                //guiConfigurable.CreateConfigWindow(this);
                return;
            }
            var props = ReflectionTools.GetPropertiesWithAttribute<ConfigItemAttribute>(configurable.GetType(),
                BindingFlags.Public | BindingFlags.Instance);
            for (var i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                var attr = prop.Item2[0];
                var control = ProcessNormalValue(prop.Item1, attr);
                control.Top = i * 30;
                Controls.Add(control);
            }
        }
    }
}
