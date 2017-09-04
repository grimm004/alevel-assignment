using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace LocationInterface.Utils
{
    public class DataIndex
    {
        public List<LocationDataFile> LocationDataFiles { get; set; }

        /// <summary>
        /// Initialise the data index
        /// </summary>
        public DataIndex()
        {
            LocationDataFiles = new List<LocationDataFile>();
        }

        /// <summary>
        /// Load the data index from the file
        /// </summary>
        /// <returns>The loaded DataIndex instance</returns>
        public static DataIndex LoadIndex()
        {
            // Read the data in the data index files, convert it to a string, then deserialize to a DataIndex object with JsonConvert
            return JsonConvert.DeserializeObject<DataIndex>(Encoding.UTF8.GetString(File.ReadAllBytes($"{ SettingsManager.Active.LocationDataFolder }\\index.json")));
        }

        /// <summary>
        /// Save the current dataindex instance to the index file
        /// </summary>
        public void SaveIndex()
        {
            // Serialize the current DataIndex instance to a string, then write it to the index file
            File.WriteAllBytes($"{ SettingsManager.Active.LocationDataFolder }\\index.json", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this, Formatting.Indented)));
        }

        /// <summary>
        /// Verify the existance of the data files
        /// </summary>
        public void VerifyDataFiles()
        {
            // Loop through each data file and if it does not exist remove it from the index
            for (int i = LocationDataFiles.Count - 1; i >= 0; i--) if (!LocationDataFiles[i].Exists) LocationDataFiles.RemoveAt(i);
            // Save the data index
            SaveIndex();
        }
    }
}
