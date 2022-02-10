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
                InfoReaderPlugin p = new();
                MmfManager.GetInstance(p).Mmfs[0].Enabled = false;
                while(true){}
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
            }

        }
    }
}