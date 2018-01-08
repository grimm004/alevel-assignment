using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace LocationInterface.Utils
{
    public static class SettingsManager
    {
        private static XmlSerializer Serializer { get; set; }

        public static Settings Active { get; set; }

        public static Settings Defaults
        {
            get
            {
                // Load the default settings using the empty contructor for Settings
                return new Settings();
            }
        }

        /// <summary>
        /// Statically load members of the SettingsManager
        /// </summary>
        static SettingsManager()
        {
            Serializer = new XmlSerializer(typeof(Settings));
            Active = Defaults;
            if (File.Exists(Constants.CONFIGFILE)) Load();
            else Save();
        }

        /// <summary>
        /// Save the active settings to the config file
        /// </summary>
        public static void Save()
        {
            // Create a file and XML serialise the active settings instance to it
            using (FileStream writer = new FileStream(Constants.CONFIGFILE, FileMode.Create)) Serializer.Serialize(writer, Active);
        }

        /// <summary>
        /// Load the active settings from the config file
        /// </summary>
        public static void Load()
        {
            // Open the config file and XML deserialise the contents to the active settings instance
            using (StreamReader reader = new StreamReader(Constants.CONFIGFILE)) Active = (Settings)Serializer.Deserialize(reader);
        }
    }

    public class Settings
    {
        public int RawDataRecordBuffer { get; set; }
        public double PercentagePerUpdate { get; set; }
        public string DataCacheFolder { get; set; }
        public string LocationDataFolder { get; set; }
        public string EmailDatabase { get; set; }
        public string ImageFolder { get; set; }
        public string AnalysisFolder { get; set; }

        public string EmailServer { get; set; }
        public int EmailPort { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Initialise a Settings object, loading defualt values
        /// </summary>
        public Settings()
        {
            RawDataRecordBuffer = 1000;
            PercentagePerUpdate = 10;
            DataCacheFolder = "DataCache";
            LocationDataFolder = "LocationData";
            EmailDatabase = "Email";
            ImageFolder = "Images";
            AnalysisFolder = "Analysis";

            EmailServer = "smtp.gmail.com";
            EmailPort = 587;
            DisplayName = "";
            EmailAddress = "";
            Password = "";
        }

        /// <summary>
        /// Check if this instance is equal to another settings instance
        /// </summary>
        /// <param name="settings">The settings instance to compare</param>
        /// <returns>true if this instance is equal to the supplied instance</returns>
        public bool Equals(Settings settings)
        {
            // Loop through each property in the settings
            foreach (PropertyInfo property in typeof(Settings).GetProperties())
                // If the current property does not equal the corresponding property of the supplied instance, return false
                if (property.GetValue(settings) != property.GetValue(this)) return false;
            // Return true
            return true;
        }
        public override int GetHashCode()
        {
            // Initialise the hash code for this instance
            int hash = 0x00;
            // Loop through each property in this instance
            foreach (PropertyInfo property in typeof(Settings).GetProperties())
                // XOR the hash code of the property with the hash code for this instance
                hash ^= property.GetValue(this).GetHashCode();
            // Return the calculated hash code
            return hash;
        }
    }

    public class Constants
    {
        public const string CONFIGFILE = "config.xml";
        public const string PLUGINFOLDER = "Plugins";
        public const string EMAILREGEX = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
        public const string DATAFILEHEADER = "MAC:string,Unknown1:string,Date:datetime,Unknown2:string,Location:string,Vendor:string,Ship:string,Deck:string,X:number,Y:number";
        public const int MAPUPS = 30;
    }
}
