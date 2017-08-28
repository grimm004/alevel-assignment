using System.IO;
using System.Xml.Serialization;

namespace LocationInterface.Utils
{
    public static class SettingsManager
    {
        public static Settings Active { get; private set; }

        static SettingsManager()
        {
            LoadDefaultSettings();
            if (File.Exists(Constants.CONFIGFILE)) Load();
            else Save();
        }

        public static void LoadDefaultSettings()
        {
            Active = new Settings
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

        public static void Save()
        {
            XmlSerializer serializer = new XmlSerializer(Active.GetType());
            using (FileStream writer = new FileStream(Constants.CONFIGFILE, FileMode.Create)) serializer.Serialize(writer, Active);
        }

        public static void Load()
        {
            XmlSerializer serializer = new XmlSerializer(Active.GetType());
            using (StreamReader reader = new StreamReader(Constants.CONFIGFILE)) Active = (Settings)serializer.Deserialize(reader);
        }
    }

    public class Settings
    {
        public int RawDataRecordBuffer { get; set; }
        public int PercentagePerUpdate { get; set; }
        public string DataCacheFolder { get; set; }
        public string LocationDataFolder { get; set; }
        public string EmailDatabase { get; set; }

        public string EmailServer { get; set; }
        public int EmailPort { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    public class Constants
    {
        public const string CONFIGFILE = "config.xml";
        public const string EMAILREGEX = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
    }
}
