using LocationInterface.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DatabaseManagerLibrary.CSV;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Text;
using DatabaseManagerLibrary;
using DatabaseManagerLibrary.BIN;

// TODO: ADD SETTINGS TO SAVE MULTIPLIER AND OFFSET PER FILE

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for DataViewerPage.xaml
    /// </summary>
    public partial class DataViewerPage : Page
    {
        public Database Database { get; protected set; }
        protected Action ShowPreviousPage { get; }
        protected Action<LocationDataFile[]>[] SetTablesActions { get; }
        protected List<LocationDataFile> SelectedDataFiles { get; set; }
        private const string HEADER = "MAC:string,Unknown1:string,Date:string,Unknown2:string,Location:string,Vendor:string,Ship:string,Deck:string,X:number,Y:number";
        private const double PERCENTAGEPERUPDATE = 10;

        public DataViewerPage(Action ShowPreviousPage, Action<LocationDataFile[]>[] SetTablesActions)
        {
            this.SetTablesActions = SetTablesActions;
            this.ShowPreviousPage = ShowPreviousPage;
            SelectedDataFiles = new List<LocationDataFile>();
            Database = new BINDatabase("LocationData");
            InitializeComponent();
            LoadTables();
        }

        public void UpdateTable()
        {
            App.VerifyFiles();
            LoadTables();
        }
        protected void LoadTables()
        {
            Dispatcher.Invoke(() =>
            {
                dataFiles.Items.Clear();
                foreach (LocationDataFile currentFile in App.DataIndex.LocationDataFiles) dataFiles.Items.Add(currentFile);
            });
        }
        protected void SubmitSelection()
        {
            foreach (Action<LocationDataFile[]> LoadTablesCommand in SetTablesActions) LoadTablesCommand?.Invoke(SelectedDataFiles.ToArray());
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
            Dispatcher.Invoke(OnStart);
            ClearCache();
            DirectoryInfo directorySelected = new DirectoryInfo(path);
            int processedFileCount = 0;
            FileInfo[] fileInfos = directorySelected.GetFiles("*.csv");
            for (int i = 0; i < fileInfos.Length; i++)
                if (IsUniqueDataFile(Path.GetFileNameWithoutExtension(fileInfos[i].Name)))
                    if (!File.Exists(@"DataCache\" + fileInfos[i].Name))
                    {
                        processedFileCount++;
                        UpdateStatus(string.Format("Importing '{0}'", fileInfos[i].Name), 100d * i / fileInfos.Length);
                        using (FileStream sourceFile = new FileStream(fileInfos[i].FullName, FileMode.Open))
                        using (FileStream desinationFile = new FileStream(@"DataCache\" + fileInfos[i].Name, FileMode.Create))
                        {
                            byte[] data = Encoding.UTF8.GetBytes(HEADER + Environment.NewLine);
                            desinationFile.Write(data, 0, data.Length);
                            sourceFile.CopyTo(desinationFile);
                        }
                    }
            FileInfo[] zippedFileInfos = directorySelected.GetFiles("*.gz");
            for (int i = 0; i < zippedFileInfos.Length; i++)
                if (IsUniqueDataFile(Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(zippedFileInfos[i].Name))))
                    if (!File.Exists(@"DataCache\" + Path.GetFileNameWithoutExtension(zippedFileInfos[i].Name)))
                    {
                        processedFileCount++;
                        UpdateStatus(string.Format("Importing '{0}'", zippedFileInfos[i].Name), 100d * i / zippedFileInfos.Length);
                        using (FileStream originalFileStream = zippedFileInfos[i].OpenRead())
                        using (FileStream decompressedFileStream = File.Create(@"DataCache\" + Path.GetFileNameWithoutExtension(zippedFileInfos[i].Name)))
                        {
                            byte[] data = Encoding.UTF8.GetBytes(HEADER + Environment.NewLine);
                            decompressedFileStream.Write(data, 0, data.Length);
                            using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                                decompressionStream.CopyTo(decompressedFileStream);
                        }
                    }
            if (processedFileCount == 0) System.Windows.Forms.MessageBox.Show("Could not find any data files to import.", "Data Import");
            else ConvertDataFiles();
            UpdateStatus("Import Complete", 0);
            App.DataIndex.SaveIndex();
            Dispatcher.Invoke(OnFinish);
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

            for (int i = 0; i < database.TableCount; i++) recordBufferSizes.Add((uint)Math.Floor(database.Tables[i].RecordCount / (100d / PERCENTAGEPERUPDATE)));

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
                    DateTime = GetDataFileDateTime(fileName),
                    FileName = new FileInfo(fileName).Name
                };
                if (IsUniqueDataFile(locationFile))
                    App.DataIndex.LocationDataFiles.Add(locationFile);
            }
            UpdateTable();
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

        public static bool IsUniqueDataFile(string tableName)
        {
            foreach (LocationDataFile currentFile in App.DataIndex.LocationDataFiles) if (currentFile.TableName == tableName)
                { Console.WriteLine("Not New"); return false; }
            Console.WriteLine("New");
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

        private void DataFilesSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            SelectedDataFiles = dataFiles.SelectedItems.Cast<LocationDataFile>().ToList();
        }
        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            UpdateTable();
        }
        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            SubmitSelection();
        }
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            ShowPreviousPage?.Invoke();
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
                new Thread(() => ImportFolder(path)) { IsBackground = true }.Start();
            }
        }
    }
}
