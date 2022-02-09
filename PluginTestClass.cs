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
                var x = PathTools.GetInvalidFileNameCharsIndexes("1<2>3.<>4");
                var c = PathTools.ClearInvalidCharacters("1<2>3.<>4", x);

            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
            }

        }
    }
}