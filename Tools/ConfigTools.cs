﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InfoReader.Command;
using InfoReader.Command.Parser;
using InfoReader.Configuration;
using InfoReader.Configuration.Attributes;
using InfoReader.Configuration.Converter;
using InfoReader.Configuration.Elements;
using InfoReader.Configuration.Gui;
using InfoReader.Mmf;
using InfoReader.Tools.I8n;

namespace InfoReader.Tools
{
    public static class ConfigTools
    {

        public static Dictionary<Type, IConfigConverter> Converters { get; } = new();
        public static void SetConfig(string configPath, object val, IConfigElement element)
        {
            var parts = configPath.Split('.');
            IConfigElement tmp = element;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                tmp = tmp[parts[i]];
            }

            tmp.SetValue(parts.Last(), val);
        }

        public static void RefreshConfig(InfoReaderPlugin instance)
        {
            MmfManager.CreateNewInstance(instance);
            Dictionary<Type, object?[]> converterTypesArgs = new Dictionary<Type, object?[]>
            {
                {typeof(MmfListConverter), new object[] {instance}},
            };
            ReadConfigFile(instance.Configurables.Values.ToArray(), converterTypesArgs);
            var mmfMgr = MmfManager.GetInstance(instance);
            mmfMgr.StopUpdate();
            mmfMgr.StartUpdate(100);
        }

        public static void ReadConfigFile(IConfigurable[] configurableItems, Dictionary<Type, object?[]>? converterTypesArgs)
        {
            Dictionary<int, IConfigElement> configElements = new Dictionary<int, IConfigElement>();
            foreach (var configurable in configurableItems)
            {
                IConfigElement? element;
                if (configElements.ContainsKey(configurable.ConfigFilePath.GetHashCode()))
                {
                    element = configElements[configurable.ConfigFilePath.GetHashCode()];
                }
                else
                {
                    element = (IConfigElement?)ReflectionTools.CreateInstance(configurable.ConfigElementType,
                        new object?[] { configurable.ConfigFilePath }) ?? throw new ArgumentException();
                    configElements.Add(configurable.ConfigFilePath.GetHashCode(), element);
                }
                InitConfig(configurable, element, converterTypesArgs);
            }
        }


        public static void InitConfig(IConfigurable configurable, IConfigElement element, Dictionary<Type, object?[]>? converterTypeArgs = null)
        {
            if (configurable == null)
            {
                throw new ArgumentNullException(nameof(configurable));
            }

            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            var props = ReflectionTools.GetPropertiesWithAttribute<ConfigItemAttribute>
                (configurable.GetType(), BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in props)
            {
                IConfigElement lastElement = element;
                var cfgInfo = property.Item2[0];
                string[] paths = string.IsNullOrEmpty(cfgInfo.ConfigPath)
                    ? new[] { property.Item1.Name }
                    : cfgInfo.ConfigPath.Split('.');
                lastElement = paths.Aggregate(lastElement, (current, path) => current[path]);

                Type? converter = cfgInfo.ConverterType;
                object? cfgVal;
                if (converter == null)
                {
                    cfgVal = lastElement.GetValue(property.Item1.PropertyType);
                }
                else
                {
                    if (!Converters.ContainsKey(converter))
                    {
                        var args = Array.Empty<object?>();
                        if (converterTypeArgs != null && converterTypeArgs.ContainsKey(converter))
                        {
                            args = converterTypeArgs[converter];
                        }
                        var converterIns =
                            (IConfigConverter?)ReflectionTools.CreateInstance(converter, args);
                        Converters.Add(converter, converterIns ?? throw new InvalidOperationException());
                    }

                    cfgVal = lastElement.GetValue(Converters[converter]);
                }

                property.Item1.SetValue(configurable, cfgVal);
            }
        }
    }
}