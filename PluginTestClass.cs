using System;
using System.Collections.Generic;
using InfoReader.Configuration;
using InfoReader.Configuration.Converter;
using InfoReader.Configuration.Elements;
using InfoReader.Mmf;
using InfoReader.Tools;
using Nett;

namespace InfoReader
{
    public class PluginTestClass
    {
        static void Main(string[] args)
        {
            try
            {
                var vs = VariableTools.GetAvailableVariables(typeof(osuTools.OrtdpWrapper.OrtdpWrapper));
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
            }

        }
    }
}