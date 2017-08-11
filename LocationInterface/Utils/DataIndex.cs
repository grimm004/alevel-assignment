using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace LocationInterface.Utils
{
    public class DataIndex
    {
        public List<LocationDataFile> LocationDataFiles { get; set; }

        public DataIndex()
        {
            LocationDataFiles = new List<LocationDataFile>();
        }

        public static DataIndex LoadIndex()
        {
            byte[] data = File.ReadAllBytes(@"LocationData\index.json");
            return JsonConvert.DeserializeObject<DataIndex>(Encoding.UTF8.GetString(data));
        }

        public void SaveIndex()
        {
            byte[] data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this, Formatting.Indented));
            File.WriteAllBytes(@"LocationData\index.json", data);
        }
    }
}
