using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Threading;
using DatabaseManagerLibrary;
using DatabaseManagerLibrary.CSV;
using DatabaseManagerLibrary.BIN;
using LocationInterface.Utils;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public Database Database { get; protected set; }
        private Action ShowDataViewerPage { get; set; }
        private Action ShowMapPage { get; set; }
        private const string HEADER = "MAC:string,Unknown1:string,Date:string,Unknown2:string,Location:string,Vendor:string,Ship:string,Deck:string,X:number,Y:number";
        private const double percentagePerUpdate = 10;

        public HomePage(Action ShowDataViewerPage, Action ShowMapPage)
        {
            this.ShowDataViewerPage = ShowDataViewerPage;
            this.ShowMapPage = ShowMapPage;
            Database = new BINDatabase("DataCache");
            InitializeComponent();
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
            Dispatcher.Invoke(delegate
           {
               statusProgressBar.Value = statusBarValue;
               statusLabel.Content = statusText;
           });
        }

        private void ImportFolder(string path)
        {
            Dispatcher.Invoke(delegate { OnStart(); });
            DirectoryInfo directorySelected = new DirectoryInfo(path);
            FileInfo[] zippedFileInfos = directorySelected.GetFiles("*.gz");
            FileInfo[] fileInfos = directorySelected.GetFiles("*.csv");
            if (zippedFileInfos.Length + fileInfos.Length == 0) System.Windows.Forms.MessageBox.Show("Could not find any data files to import.", "Data Import");
            Console.WriteLine(fileInfos.Length);
            for (int i = 0; i < fileInfos.Length; i++)
            {
                string newFileName = @"DataCache\" + fileInfos[i].Name;
                if (!File.Exists(newFileName))
                {
                    UpdateStatus(string.Format("Importing '{0}'", fileInfos[i].Name), 100d * i / fileInfos.Length);
                    using (FileStream sourceFile = new FileStream(fileInfos[i].FullName, FileMode.Open))
                        using (FileStream desinationFile = new FileStream(newFileName, FileMode.Create))
                        {
                            byte[] data = Encoding.UTF8.GetBytes(HEADER + Environment.NewLine);
                            desinationFile.Write(data, 0, data.Length);
                            sourceFile.CopyTo(desinationFile);
                        }
                }
            }
            for (int i = 0; i < zippedFileInfos.Length; i++)
            {
                UpdateStatus(string.Format("Importing '{0}'", zippedFileInfos[i].Name), 100d * i / zippedFileInfos.Length);
                string newFileName = @"DataCache\" + System.IO.Path.GetFileNameWithoutExtension(zippedFileInfos[i].Name);
                if (!File.Exists(newFileName))
                    using (FileStream originalFileStream = zippedFileInfos[i].OpenRead())
                    using (FileStream decompressedFileStream = File.Create(newFileName))
                    {
                        byte[] data = Encoding.UTF8.GetBytes(HEADER + Environment.NewLine);
                        decompressedFileStream.Write(data, 0, data.Length);
                        using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                            decompressionStream.CopyTo(decompressedFileStream);
                    }
            }
            ConvertDataFiles();
            UpdateStatus("Import Complete", 0);
            App.DataIndex.SaveIndex();
            Dispatcher.Invoke(delegate { OnFinish(); });
        }

        public void ConvertDataFiles()
        {
            List<ushort[]> varCharSizes = new List<ushort[]>();
            List<uint> recordBufferSizes = new List<uint>();
            CSVDatabase database = new CSVDatabase("DataCache", true, ".csv");

            int fieldIndex = 0;
            foreach (Table table in database.Tables)
            {
                UpdateStatus(string.Format("Calculating field sizes '{0}'.", table.Name), 100d * fieldIndex++ / database.TableCount);
                Console.WriteLine("Calculating field sizes {0}...", table.Name);
                currentFieldSizes = new ushort[table.FieldCount];
                for (int i = 0; i < currentFieldSizes.Length; i++) currentFieldSizes[i] = 0x00;
                currentFields = (CSVTableFields)table.Fields;
                table.SearchRecords(FieldSizeCalculatorCallback);
                //for (int i = 0; i < fieldSizes.Length; i++) fieldSizes[i] += (ushort)(fieldSizes[i] > 0x00 ? 0x02 : 0x00);
                Console.WriteLine("Done");
                varCharSizes.Add(currentFieldSizes);
            }

            for (int i = 0; i < database.TableCount; i++) recordBufferSizes.Add((uint)Math.Floor(database.Tables[i].RecordCount / (100d / percentagePerUpdate)));
            
            Console.WriteLine("Converting files...");
            database.ToBINDatabase("LocationData", varCharSizes, recordBufferSizes, updateCommand: (table, ratio) =>
                UpdateStatus("Converting Files (may take a while)", 100d * (database.Tables.IndexOf(table) + ratio) / database.TableCount));
            Console.WriteLine("Done");

            UpdateStatus("Clearing Cache", 100);
            Console.WriteLine("Clearing cache...");
            ClearCache();
            Console.WriteLine("Done");

            UpdateStatus("Updating Index File", 100);
            Console.WriteLine("Adding files to index...");
            foreach (string fileName in Directory.GetFiles("LocationData", "*.table"))
            {
                LocationDataFile locationFile = new LocationDataFile
                {
                    LocationIdentifier = GetDataFileLocationIdentifier(fileName),
                    DateTime = GetDataFileDateTime(fileName),
                    FileName = new FileInfo(fileName).Name
                };
                if (IsUniqueDataFile(locationFile))
                    App.DataIndex.LocationDataFiles.Add(locationFile);
            }
            Console.WriteLine("Done");
        }

        private CSVTableFields currentFields;
        private ushort[] currentFieldSizes;
        private void FieldSizeCalculatorCallback(Record record)
        {
            object[] values = record.GetValues();
            for (int i = 0; i < currentFields.Fields.Length; i++)
                if (currentFields.Fields[i].DataType == Datatype.VarChar)
                    if (Encoding.UTF8.GetByteCount(((string)values[i])) > currentFieldSizes[i]) currentFieldSizes[i] = (ushort)Encoding.UTF8.GetByteCount(((string)values[i]));
        }

        public static bool IsUniqueDataFile(LocationDataFile file)
        {
            foreach (LocationDataFile currentFile in App.DataIndex.LocationDataFiles) if (currentFile.DateTime == file.DateTime) return false;
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

        public void ClearCache()
        {
            foreach (string fileName in Directory.GetFiles("DataCache")) File.Delete(fileName);
        }

        private void ViewImportedFilesClick(object sender, RoutedEventArgs e)
        {
            ShowDataViewerPage();
        }
        private void ViewMapClick(object sender, RoutedEventArgs e)
        {
            ShowMapPage();
        }
        private void ClearCacheButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show("Are you sure you want to delete the data cache?", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes) ClearCache();
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
    }
}
