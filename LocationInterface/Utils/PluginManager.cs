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

        /// <summary>
        /// Initialze the plugin manager
        /// </summary>
        static PluginManager()
        {
            // Initialze the AnalysisPlugin List
            Plugins = new List<AnalysisPlugin>();
        }

        /// <summary>
        /// Load the available plugins
        /// </summary>
        public static void Load()
        {
            Plugins = new List<AnalysisPlugin>();
            // Fetch all the dlls in the plugin folder
            string[] pluginFiles = Directory.GetFiles(Constants.PLUGINFOLDER, "*.dll");
            // Loop through each one
            foreach (string pluginFile in pluginFiles)
                try
                {
                    // Try to load it
                    Assembly assembly = Assembly.LoadFrom(pluginFile);

                    IAnalysis analysis = null;
                    // Fetch all the types from the loaded assembly
                    Type[] types = assembly.GetTypes();
                    // Loop though each available type
                    foreach (Type type in types)
                        // If the type implements the IAnalysis interface
                        if (typeof(IAnalysis).IsAssignableFrom(type))
                        {
                            // Load it in and break out of the loop
                            analysis = (IAnalysis)Activator.CreateInstance(type);
                            break;
                        }

                    // if a valid analysis class is found, add it to the plugin list
                    if (analysis != null)
                        Plugins.Add(new AnalysisPlugin()
                        {
                            Name = Path.GetFileNameWithoutExtension(pluginFile),
                            Assembly = assembly,
                            Analysis = analysis,
                        });
                }
                // If loading is invalid, catch the error and move on
                catch (ReflectionTypeLoadException)
                {
                    Console.WriteLine($"Error loading assembly: { pluginFile }");
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
