using AnalysisSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LocationInterface.Utils
{
    public class PluginManager
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
                Assembly assembly = Assembly.LoadFrom(pluginFile);
                
                IAnalysis analysis = null;
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                    if (typeof(IAnalysis).IsAssignableFrom(type))
                    {
                        analysis = (IAnalysis)Activator.CreateInstance(type);
                        break;
                    }

                if (analysis != null)
                    Plugins.Add(new AnalysisPlugin()
                    {
                        Name = Path.GetFileNameWithoutExtension(pluginFile),
                        Assembly = assembly,
                        Analysis = analysis,
                    });
            }
        }
    }

    public class AnalysisPlugin
    {
        public string Name { get; set; }
        public Assembly Assembly { get; set; }
        public IAnalysis Analysis { get; set; }
    }
}
