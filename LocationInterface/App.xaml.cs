using System.Windows;
using System.IO;
using LocationInterface.Utils;

namespace LocationInterface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static DataIndex DataIndex { get; protected set; }
        
        public App()
        {
            if (!Directory.Exists(@"DataCache")) Directory.CreateDirectory(@"DataCache");
            if (!Directory.Exists(@"LocationData")) Directory.CreateDirectory(@"LocationData");
            if (!File.Exists(@"LocationData\index.json") || (DataIndex = DataIndex.LoadIndex()) == null) { File.Create(@"LocationData\index.json").Close(); DataIndex = new DataIndex(); }
            else VerifyFiles();
            DataIndex.SaveIndex();
        }

        public static void VerifyFiles()
        {
            for (int i = DataIndex.LocationDataFiles.Count - 1; i >= 0; i--) if (!DataIndex.LocationDataFiles[i].Exists) DataIndex.LocationDataFiles.RemoveAt(i);
            DataIndex.SaveIndex();
        }
    }
}
