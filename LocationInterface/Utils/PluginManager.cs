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
            MapperPlugins = new List<MapperPlugin>();
        }

        /// <summary>
        /// Load the available plugins
        /// </summary>
        public static void Load()
        {
            AnalysisPlugins = new List<AnalysisPlugin>();
            MapperPlugins = new List<MapperPlugin>();
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
                        if (typeof(IAnalysis).IsAssignableFrom(type) && Activator.CreateInstance(type) is IAnalysis analysis)
                            // If the instance is not null, add it to the analysis plugins list
                            AnalysisPlugins.Add(new AnalysisPlugin()
                            {
                                Name = analysis.Name,
                                Assembly = assembly,
                                Analysis = analysis,
                            });
                        // Else if the type implements the IMapper interface
                        else if (typeof(IMapper).IsAssignableFrom(type) && Activator.CreateInstance(type) is IMapper mapper)
                            // If the instance is not null, add it to the mapper plugins list
                            MapperPlugins.Add(new MapperPlugin()
                            {
                                Name = mapper.Name,
                                Assembly = assembly,
                                Mapper = mapper,
                            });
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

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public class AnalysisPlugin : Plugin
    {
        public IAnalysis Analysis { get; set; }
    }

    public class MapperPlugin : Plugin
    {
        public IMapper Mapper { get; set; }
    }
}
