using AnalysisSDK;
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
                Assembly assembly = Assembly.LoadFrom(pluginFile);
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    Console.WriteLine(type.Name);
                    if (typeof(IAnalysisResult).IsAssignableFrom(type))
                    {
                        object analysisResultInstance = Activator.CreateInstance(type);
                        IAnalysisResult iface = (IAnalysisResult)analysisResultInstance;

                        Plugins.Add(new AnalysisPlugin()
                        {
                            Name = Path.GetFileNameWithoutExtension(pluginFile),
                            Assembly = assembly,
                            AnalysisResult = iface,
                        });

                        Console.WriteLine("Added type: " + analysisResultInstance.GetType());
                    }
                }
            }
        }
    }

    class AnalysisPlugin
    {
        public string Name { get; set; }
        public Assembly Assembly { get; set; }
        public IAnalysisResult AnalysisResult { get; set; }
    }
}
