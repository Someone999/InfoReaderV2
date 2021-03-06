using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using InfoReader.Configuration;
using InfoReader.Configuration.Attributes;
using InfoReader.Configuration.Converter;
using InfoReader.Tools.I8n;

namespace InfoReader.Tools
{
    public static class WindowTools
    {
        public static (Thread, Form) StartMessageLoop(Form form)
        {
            Thread msgThread = new Thread(() => Application.Run(form));
            msgThread.SetApartmentState(ApartmentState.STA);
            msgThread.Start();
            return (msgThread, form);
        }

        public static int IndexProcessor(int len, int idx)
        {
            if (idx < 0)
            {
                if (len + idx < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                return len + idx;
            }
            if (idx > len - 1)
            {
                throw new IndexOutOfRangeException();
            }

            return idx;
        }

        public static (Control?, Control?) CreatePairedControl((PropertyInfo, ConfigItemAttribute[]) info, 
            object ins, ConfigTypeAttribute? typeAttr = null, Dictionary<Type,object?[]>? converterTypes = null)
        {
            var configItemAttr = info.Item2[0];
            string displayName = configItemAttr.DisplayName ?? info.Item1.Name;
            if (info.Item1.PropertyType == typeof(bool))
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Text = displayName;
                checkBox.Font = FontTools.MicrosoftYaHei;
                checkBox.Checked = (bool)info.Item1.GetValue(ins);
                checkBox.CheckStateChanged += (_, _) =>
                {
                    try
                    {
                        info.Item1.SetValue(ins, checkBox.Checked);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                };
                return (null, checkBox);
            }

            if (typeAttr is ListAttribute lst)
            {
                Label l = new Label();
                l.Text = displayName;
                l.Font = FontTools.MicrosoftYaHei;
                l.AutoSize = true;
                ComboBox comboBox = new ComboBox();
                comboBox.DataSource = lst.Items;
                comboBox.SelectedItem = lst.DefaultSelection;
                comboBox.Width = 100;
                comboBox.Font = FontTools.MicrosoftYaHei;
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                return (l, comboBox);
            }

            if (typeAttr == null)
            {
                Label l = new Label();
                l.Text = displayName;
                l.Font = FontTools.MicrosoftYaHei;
                l.AutoSize = true;
                l.Width += 50;
                TextBox t = new TextBox();
                t.Left = l.Width + l.Left + 100;
                t.Font = FontTools.MicrosoftYaHei;
                t.Text = info.Item1.GetValue(ins).ToString();
                t.TextChanged += (_, _) =>
                {
                    try
                    {
                        object val = t.Text;
                        var converterType = configItemAttr.ConverterType;
                        if (converterType != null)
                        {
                            object?[] converterArgs = Array.Empty<object?>();
                            if (converterTypes != null && converterTypes.ContainsKey(converterType))
                            {
                                converterArgs = converterTypes[converterType];
                            }

                            IConfigConverter? converter = (IConfigConverter?)
                                ReflectionTools.CreateInstance(converterType, converterArgs);
                            val = converter?.Convert(val) ?? throw new InvalidCastException();
                        } info.Item1.SetValue(ins, val);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                };
                return (l, t);
            }
            return (null, null);
        }

        public static T LastOf<T>(IEnumerable enumerable)
        {
            List<T> list = new List<T>();
            foreach (var obj in enumerable)
            {
                list.Add((T)obj);
            }

            return list.Last();
        }
        public static Form CreateDefaultConfigForm(IConfigurable configurable, Dictionary<Type, object?[]>? converterTypes = null)
        {
            List<int> widths = new List<int>();
            Form f = new Form();
            Type configType = configurable.GetType();
            var properties =
                ReflectionTools.GetPropertiesWithAttribute<ConfigItemAttribute>(configType,
                    BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                ConfigTypeAttribute? typeAttribute = null;
                if (property.Item1.IsDefined(typeof(ConfigTypeAttribute), true))
                {
                    var attrs = property.Item1.GetCustomAttributes(typeof(ConfigTypeAttribute), true);
                    if (attrs.Length > 1)
                    {
                        Logger.LogError(LocalizationInfo.Current.Translations["LANG_ERR_TOOMANYATTR"]);
                        foreach (var attr in attrs)
                        {
                            Logger.LogError(attr.GetType().ToString());
                        }
                    }

                    typeAttribute = (ConfigTypeAttribute) attrs[0];
                }

                
                int width = 0;
                (Control?, Control?) controlTuple =
                    CreatePairedControl(property, configurable, typeAttribute, converterTypes);
                if (controlTuple.Item1 != null)
                {
                    if (f.Controls.Count > 0)
                    {
                        Control lastCtrl = LastOf<Control>(f.Controls);
                        controlTuple.Item1.Top += lastCtrl.Top + lastCtrl.Height + 20;
                    }
                    f.Controls.Add(controlTuple.Item1);
                    width += controlTuple.Item1.Width + controlTuple.Item1.Left;
                }

                if (controlTuple.Item2 != null)
                {
                    Label? lb = controlTuple.Item1 as Label;
                    if (f.Controls.Count > 0)
                    {
                        controlTuple.Item2.Top = lb?.Top ?? LastOf<Control>(f.Controls).Top + controlTuple.Item2.Height + 15;
                        controlTuple.Item2.Left = lb?.Left + lb?.Width + 20 ?? controlTuple.Item2.Left;
                        controlTuple.Item2.Width = 200;
                    }
                    
                    f.Controls.Add(controlTuple.Item2);
                    width += lb?.Left + lb?.Width + 70 ?? controlTuple.Item2.Left + controlTuple.Item2.Width + 70;
                }
                widths.Add(width);
            }
            var lastControl = LastOf<Control>(f.Controls);
            f.Width = widths.Max() + 50;
            f.AutoSize = true;
            f.Height = widths.Max() + 50;
            var saveBtn = new Button
            {
                Text = LocalizationInfo.Current.Translations["LANG_UI_BTN_SAVE"],
                Width = 100,
                Height = 60,
                Top = lastControl.Top + 50,
                Left = f.Width - 150,
                Font = FontTools.MicrosoftYaHei
            };
            saveBtn.Click += (_, _) =>
            {
                configurable.Save();
                ConfigTools.InitConfig(configurable,
                    configurable.ConfigElement ?? throw new InvalidOperationException(), converterTypes);
                //configurable.Save();
            };
            f.Controls.Add(saveBtn);

            return f;
        }
    }

    
}
