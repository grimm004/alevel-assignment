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
                return new Settings
                {
                    RawDataRecordBuffer = 1000,
                    PercentagePerUpdate = 10,
                    DataCacheFolder = "DataCache",
                    LocationDataFolder = "LocationData",
                    EmailDatabase = "Email",

                    EmailServer = "smtp.gmail.com",
                    EmailPort = 587,
                    DisplayName = "",
                    EmailAddress = "",
                    Password = "",
                };
            }
        }

        static SettingsManager()
        {
            Serializer = new XmlSerializer(typeof(Settings));
            Active = Defaults;
            if (File.Exists(Constants.CONFIGFILE)) Load();
            else Save();
        }

        public static void Save()
        {
            using (FileStream writer = new FileStream(Constants.CONFIGFILE, FileMode.Create)) Serializer.Serialize(writer, Active);
        }

        public static void Load()
        {
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

        public string EmailServer { get; set; }
        public int EmailPort { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }

        public bool Equals(Settings settings)
        {
            bool equal = true;
            foreach (PropertyInfo property in typeof(Settings).GetProperties())
                equal = property.GetValue(settings) != property.GetValue(this) ? false : equal;
            return equal;
        }
        public override int GetHashCode()
        {
            int hash = 0x00;
            foreach (PropertyInfo property in typeof(Settings).GetProperties())
                hash ^= property.GetValue(this).GetHashCode();
            return hash;
        }
    }

    public class Constants
    {
        public const string CONFIGFILE = "config.xml";
        public const string EMAILREGEX = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
        public const string MACVENDORAPISITE = "api.macvendors.com";
    }
}
