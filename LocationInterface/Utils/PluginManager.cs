using AnalysisSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LocationInterface.Utils
{
    public class PluginManager
    {
        public static List<AnalysisPlugin> AnalysisPlugins { get; set; }
        public static List<MapperPlugin> MapperPlugins { get; set; }

        /// <summary>
        /// Initialze the plugin manager
        /// </summary>
        static PluginManager()
        {
            // Initialze the AnalysisPlugin List
            AnalysisPlugins = new List<AnalysisPlugin>();
        }

        /// <summary>
        /// Load the available plugins
        /// </summary>
        public static void Load()
        {
            AnalysisPlugins = new List<AnalysisPlugin>();
            // Fetch all the dlls in the plugin folder
            string[] pluginFiles = Directory.GetFiles(Constants.PLUGINFOLDER, "*.dll");
            // Loop through each one
            foreach (string pluginFile in pluginFiles)
                try
                {
                    // Try to load it
                    Assembly assembly = Assembly.LoadFrom(pluginFile);
                    
                    // Fetch all the types from the loaded assembly
                    Type[] types = assembly.GetTypes();
                    // Loop though each available type
                    foreach (Type type in types)
                        // If the type implements the IAnalysis interface
                        if (typeof(IAnalysis).IsAssignableFrom(type))
                        {
                            // Load it in as an instance
                            IAnalysis analysis = (IAnalysis)Activator.CreateInstance(type);
                            // If the instance is not null, add it to the analysis plugins list
                            if (analysis != null)
                                AnalysisPlugins.Add(new AnalysisPlugin()
                                {
                                    Name = Path.GetFileNameWithoutExtension(pluginFile),
                                    Assembly = assembly,
                                    Analysis = analysis,
                                });
                        }
                        // Else if the type implements the IMapper interface
                        else if (typeof(IMapper).IsAssignableFrom(type))
                        {
                            // Load it in as an instance
                            IMapper mapper = (IMapper)Activator.CreateInstance(type);
                            // If the instance is not null, add it to the mapper plugins list
                            if (mapper != null)
                                MapperPlugins.Add(new MapperPlugin()
                                {
                                    Name = Path.GetFileNameWithoutExtension(pluginFile),
                                    Assembly = assembly,
                                    Analysis = mapper,
                                });
                        }
                }
                // If loading is invalid, catch the error and move on
                catch (ReflectionTypeLoadException)
                {
                    Console.WriteLine($"Error loading assembly: { pluginFile }");
                }
        }
    }

    public class Plugin
    {
        public string Name { get; set; }
        public Assembly Assembly { get; set; }
    }

    public class AnalysisPlugin : Plugin
    {
        public IAnalysis Analysis { get; set; }
    }

    public class MapperPlugin : Plugin
    {
        public IMapper Analysis { get; set; }
    }
}
