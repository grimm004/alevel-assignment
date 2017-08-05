using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private Action ShowDataViewerPage;

        public HomePage(Action ShowDataViewerPage)
        {
            this.ShowDataViewerPage = ShowDataViewerPage;
            InitializeComponent();
        }

        private void ImportDataFileClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                string path = folderBrowser.SelectedPath;
                new Thread(new ParameterizedThreadStart(delegate { ImportFolder(path); })) { IsBackground = true }.Start();
            }
        }

        private void OnStart()
        {
            importFolderButton.Content = "Importing Folder";
            importFolderButton.IsEnabled = false;
        }

        private void OnFinish()
        {
            importFolderButton.Content = "Import Folder";
            importFolderButton.IsEnabled = true;
        }

        private void UpdateStatus(string statusText, double statusBarValue)
        {
            Dispatcher.Invoke(
                delegate
           {
               statusProgressBar.Value = statusBarValue;
               statusLabel.Content = statusText;
           });
        }

        private void ImportFolder(string path)
        {
            Dispatcher.Invoke(delegate { OnStart(); });
            DirectoryInfo directorySelected = new DirectoryInfo(path);
            FileInfo[] fileInfos = directorySelected.GetFiles("*.gz");
            totalFiles = fileInfos.Length;
            if (totalFiles == 0) System.Windows.Forms.MessageBox.Show("Could not find any data files to import.", "Data Import");
            foreach (FileInfo fileInfo in fileInfos) Decompress(fileInfo);
            UpdateStatus("Import Complete", 0);
            App.dataIndex.SaveIndex();
            Dispatcher.Invoke(delegate { OnFinish(); });
            currentFileNumber = 0;
            totalFiles = 0;
        }

        int currentFileNumber = 0;
        int totalFiles = 0;
        public void Decompress(FileInfo fileToDecompress)
        {
            if (!File.Exists(@"LocationData\" + System.IO.Path.GetFileNameWithoutExtension(fileToDecompress.Name)))
            {
                Dispatcher.Invoke(delegate { UpdateStatus(string.Format("Importing '{0}'", fileToDecompress.Name), (double)100 * (double)currentFileNumber / (double)totalFiles); });
                using (FileStream originalFileStream = fileToDecompress.OpenRead())
                using (FileStream decompressedFileStream = File.Create(@"LocationData\" + System.IO.Path.GetFileNameWithoutExtension(fileToDecompress.FullName)))
                using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(decompressedFileStream);
                    LocationDataFile next = new LocationDataFile { LocationIdentifier = GetDataFileLocationIdentifier(decompressedFileStream.Name), DateTime = GetDataFileDateTime(decompressedFileStream.Name), FileName = new FileInfo(decompressedFileStream.Name).Name };
                    if (IsUniqueDataFile(next)) App.dataIndex.LocationDataFiles.Add(next);
                }
            }
            currentFileNumber++;
        }

        public static bool IsUniqueDataFile(LocationDataFile file)
        {
            foreach (LocationDataFile currentFile in App.dataIndex.LocationDataFiles) if (currentFile.DateTime == file.DateTime) return false;
            return true;
        }

        public static DateTime GetDataFileDateTime(string fileName)
        {
            string dataString = GetZipFileName(fileName);
            return DateTime.ParseExact(dataString.Substring(dataString.Length - 10, 10), "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string GetDataFileLocationIdentifier(string fileName)
        {
            string dataString = GetZipFileName(fileName);
            return dataString.Substring(0, dataString.Length - 11);
        }

        public static string GetZipFileName(string fileName)
        {
            return System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileNameWithoutExtension(fileName));
        }

        private void ViewImportedFilesClick(object sender, RoutedEventArgs e)
        {
            ShowDataViewerPage();
        }
    }
}
