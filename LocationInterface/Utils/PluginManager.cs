using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LocationInterface.Utils
{
    class PluginManager
    {
        public static List<AnalysisPlugin> Plugins { get; set; }

        static PluginManager()
        {
            Plugins = new List<AnalysisPlugin>();
        }

        public static void Load()
        {
            Plugins = new List<AnalysisPlugin>();
            string[] pluginFiles = Directory.GetFiles(Constants.PLUGINFOLDER, "*.dll");
            foreach (string pluginFile in pluginFiles)
            {
                Assembly assembly = Assembly.Load(pluginFile);
                Type[] types = assembly.GetTypes();
                Plugins.Add(new AnalysisPlugin() { Name = Path.GetFileNameWithoutExtension(pluginFile), Assembly =  });
            }
        }
    }

    class AnalysisPlugin
    {
        public string Name { get; set; }
        public Assembly Assembly { get; set; }
    }
}
