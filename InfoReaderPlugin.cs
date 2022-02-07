using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoReader.FileWatcher;
using Sync.Plugins;

namespace InfoReader
{
    public class InfoReaderPlugin: Plugin
    {
        public InfoReaderPlugin() : base("InfoReader", "Someone999")
        {
        }
    }

    public class MainClass
    {
        static void Main(string[] args)
        {
            
            PluginFileInfo fileInfo = new PluginFileInfo("F:\\sync app\\plugins\\RealTimePPDisplayer.dll");
           
            
        }
    }
}
