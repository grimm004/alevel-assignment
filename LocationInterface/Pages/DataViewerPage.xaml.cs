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
using System.Windows.Input;
using LocationInterface.Windows;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for DataViewerPage.xaml
    /// </summary>
    public partial class FileManagerPage : Page
    {
        protected List<LocationDataFile> SelectedDataFiles { get; set; }
        protected bool Importing { get; set; }
        protected Common Common { get; set; }

        /// <summary>
        /// Initialise the data viewer page
        /// </summary>
        /// <param name="common">Instance of the common class</param>
        public FileManagerPage(Common common)
        {
            Common = common;
            Importing = false;
            SelectedDataFiles = new List<LocationDataFile>();
            InitializeComponent();
            LoadTables();

            // Add a keydown event to remove selected tables if the delete key is down
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Delete)) RemoveTables(); };
        }

        /// <summary>
        /// Delete any selected tables (data files) from the database
        /// </summary>
        public void RemoveTables()
        {
            // Loop through each LocationDataFile in the selected items
            foreach (LocationDataFile file in DataFilesDataGrid.SelectedItems)
                // Call to the main database to delete the coresponding table
                Common.LocationDatabase.DeleteTable(file.TableName);
            // Save changes to the table
            Common.LocationDatabase.SaveChanges();
            // Update the table
            UpdateTable();
            // Re-set the loaded tables
            Common.LoadTables(new LocationDataFile[0]);
        }

        /// <summary>
        /// Update the datagrid
        /// </summary>
        public void UpdateTable()
        {
            // If the app is not importing new tables (to prevent file access denied exceptions)
            if (!Importing)
            {
                // Verify the existance of the datafiles defined in the index
                App.DataIndex.VerifyDataFiles();
                // Load the tables (data files)
                LoadTables();
            }
        }

        /// <summary>
        /// Load the data tables
        /// </summary>
        protected void LoadTables()
        {
            // If not importing (to avoid access denied exceptions)
            if (!Importing)
            {
                // Reload the database
                Common.ReloadDatabase();

                Dispatcher.Invoke(() =>
                {
                    // Clear the items in the data grid
                    DataFilesDataGrid.Items.Clear();
                    // Loop through the location data files and add them to the data grid
                    foreach (LocationDataFile currentFile in App.DataIndex.LocationDataFiles) DataFilesDataGrid.Items.Add(currentFile);
                });
            }
        }

        /// <summary>
        /// Submit the current selection of tables (allowing other pages to access them)
        /// </summary>
        protected void SubmitSelection()
        {
            // Load the tables into the common class
            Common.LoadTables(SelectedDataFiles.ToArray());
        }

        /// <summary>
        /// Callback to safely update WPF items from the dispatcher on starting of import
        /// </summary>
        private void OnImportStart()
        {
            ImportFolderButton.Content = "Importing Folder";
            UpdateButton.IsEnabled = ImportFolderButton.IsEnabled = false;
        }

        /// <summary>
        /// Callback to safely update WPF items from the dispatcher on ending of import
        /// </summary>
        private void OnImportFinish()
        {
            ImportFolderButton.Content = "Import Folder";
            UpdateButton.IsEnabled = ImportFolderButton.IsEnabled = true;
        }

        /// <summary>
        /// Method to safely update the WPF status bar and text
        /// </summary>
        /// <param name="statusText"></param>
        /// <param name="statusBarValue"></param>
        private void UpdateStatus(string statusText, double statusBarValue)
        {
            Dispatcher.Invoke(delegate
            {
                StatusProgressBar.Value = statusBarValue;
                StatusLabel.Content = statusText;
            });
        }

        /// <summary>
        /// Import the folder of a given path
        /// </summary>
        /// <param name="path">The path to the folder to be imported</param>
        private void ImportFolder(string path)
        {
            // Mark the page as importing
            Dispatcher.Invoke(OnImportStart);
            Importing = true;
            // Clear any files in the cache
            ClearCache();
            // Get a DirectoryInfo object from the selected path
            DirectoryInfo directorySelected = new DirectoryInfo(path);
            // Initialise a file counter
            int processedFileCount = 0;
            // Get any csv (unzipped) files in the directory
            FileInfo[] fileInfos = directorySelected.GetFiles("*.csv");
            // Loop through the csv files
            for (int i = 0; i < fileInfos.Length; i++)
                // Check the file is not already in the index and does not already exist in the data cache
                if (IsUniqueDataFile(Path.GetFileNameWithoutExtension(fileInfos[i].Name)) && !File.Exists($"{ SettingsManager.Active.DataCacheFolder}\\{ fileInfos[i].Name }"))
                {
                    // Increment the file count
                    processedFileCount++;
                    // Update the status information
                    UpdateStatus(string.Format("Importing '{0}'", fileInfos[i].Name), 100d * i / fileInfos.Length);

                    // Open the file to be imported
                    using (FileStream sourceFile = fileInfos[i].OpenRead())
                    // Create and open a new file in the file cache folder
                    using (FileStream desinationFile = File.Create($"{ SettingsManager.Active.DataCacheFolder}\\{ fileInfos[i].Name }"))
                    {
                        // Get the bytes for the new file CSVDatabase table header
                        byte[] data = Encoding.UTF8.GetBytes(Constants.DATAFILEHEADER + Environment.NewLine);
                        // Write this data into the new file
                        desinationFile.Write(data, 0, data.Length);
                        // Copy the file to be imported to the new file
                        sourceFile.CopyTo(desinationFile);
                    }
                }
            // Get any gz (zipped) files in the directory
            FileInfo[] zippedFileInfos = directorySelected.GetFiles("*.gz");
            // Loop through the zipped files
            for (int i = 0; i < zippedFileInfos.Length; i++)
                // Check the file is not already in the index and does not already exist in the data cache
                if (IsUniqueDataFile(GetZipFileName(zippedFileInfos[i].Name)) && !File.Exists($"{ SettingsManager.Active.DataCacheFolder}\\{ Path.GetFileNameWithoutExtension(zippedFileInfos[i].Name) }"))
                {
                    // Increment the file count
                    processedFileCount++;
                    // Update the status information
                    UpdateStatus(string.Format("Importing '{0}'", zippedFileInfos[i].Name), 100d * i / zippedFileInfos.Length);

                    // Open the file to be imported
                    using (FileStream originalFileStream = zippedFileInfos[i].OpenRead())
                    // Create and open a new file in the file cache folder
                    using (FileStream decompressedFileStream = File.Create($"{ SettingsManager.Active.DataCacheFolder}\\{ Path.GetFileNameWithoutExtension(zippedFileInfos[i].Name) }"))
                    {
                        // Get the bytes for the new file CSVDatabase table header
                        byte[] data = Encoding.UTF8.GetBytes(Constants.DATAFILEHEADER + Environment.NewLine);
                        // Write this data into the new file
                        decompressedFileStream.Write(data, 0, data.Length);
                        // Create a GZipStream to unzip the file (a wrapper to read and interpret the zipped file)
                        using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                            // Copy the file to be imported to the new file (unzipped)
                            decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
            // If the total number of files processed and placed in the cache is zero, warn the user
            if (processedFileCount == 0) System.Windows.Forms.MessageBox.Show("Could not find any data files to import.", "Data Import");
            // Else convert the imported files
            else ConvertDataFiles();
            // Set the status as import complete
            UpdateStatus("Import Complete", 0);
            // Save the new datafile index
            App.DataIndex.SaveIndex();
            // Mark as no longer importing
            Importing = false;
            // Refresh the datagrid
            UpdateTable();
            // Mark the page as not importing
            Dispatcher.Invoke(OnImportFinish);
        }

        /// <summary>
        /// Convert the imported data files to the binary database format
        /// </summary>
        public void ConvertDataFiles()
        {
            // Initialise lists that will contain information required to convert the CSVDatabase to a BINDatabase
            List<ushort[]> varCharSizes = new List<ushort[]>();
            List<uint> recordBufferSizes = new List<uint>();
            // Load a CSVDatabase instance that represents the imported files in the DataCahce folder
            CSVDatabase database = new CSVDatabase(SettingsManager.Active.DataCacheFolder, true);

            // Initialise a counter to keep track of the current table
            int fieldIndex = 0;
            // Loop through each table in the datacache database
            foreach (Table table in database.Tables)
            {
                // Update the status
                UpdateStatus(string.Format("Calculating field sizes '{0}'.", table.Name), 100d * fieldIndex++ / database.TableCount);
                Console.WriteLine("Calculating field sizes {0}...", table.Name);
                // Initialise the currentFieldSizes variable to a new unsigned 16 bit integer array (each item represents the maximum string size for each field)
                currentFieldSizes = new ushort[table.FieldCount];
                // Loop through each field and initialise its corresponding field size to zero
                for (int i = 0; i < currentFieldSizes.Length; i++) currentFieldSizes[i] = 0x00;
                // Get the CSVTableFileds instance for the current table
                currentFields = (CSVTableFields)table.Fields;
                // Search through all records in the current table (with a callback for each record)
                table.SearchRecords(FieldSizeCalculatorCallback);
                Console.WriteLine("Done");
                // Add the calculated field sizes to the storage list
                varCharSizes.Add(currentFieldSizes);
            }

            // Loop through each table and set the record buffer sizes
            for (int i = 0; i < database.TableCount; i++)
                recordBufferSizes.Add((uint)Math.Floor(database.Tables[i].RecordCount / (100d / SettingsManager.Active.PercentagePerUpdate)));

            Console.WriteLine("Converting files...");
            // Convert the CSVDatabase to a BINDatabase with the configured location, pre-calculated string sizes and buffer sizes, along with a callback to update the status bar and label
            database.ToBINDatabase(SettingsManager.Active.LocationDataFolder, varCharSizes, recordBufferSizes, updateCommand: (table, ratio) =>
                UpdateStatus("Converting Files (may take a while)", 100d * (database.Tables.IndexOf(table) + ratio) / database.TableCount));
            Console.WriteLine("Done");

            // Update the status
            UpdateStatus("Clearing Cache", 100);
            Console.WriteLine("Clearing cache...");
            // Clear the cache
            ClearCache();
            Console.WriteLine("Done");

            // Update the status
            UpdateStatus("Updating Index File", 100);
            Console.WriteLine("Adding files to index...");
            // Loop through each data file in the locationdata folder
            foreach (string fileName in Directory.GetFiles(SettingsManager.Active.LocationDataFolder, "*.table"))
            {
                // Create a LocationDataFile object for it
                LocationDataFile locationFile = new LocationDataFile
                {
                    DateTime = GetDataFileDateTime(fileName),
                    FileName = new FileInfo(fileName).Name
                };
                // If the file is unique add it to the index
                if (IsUniqueDataFile(locationFile))
                    App.DataIndex.LocationDataFiles.Add(locationFile);
            }
            Console.WriteLine("Done");
        }

        // The current fields object for the table being processed
        private CSVTableFields currentFields;
        // The current filed sizes array for the table being processed
        private ushort[] currentFieldSizes;

        /// <summary>
        /// A callback to help process the maximum string size in each field for a table
        /// </summary>
        /// <param name="record">The current record in the table to process</param>
        private void FieldSizeCalculatorCallback(Record record)
        {
            // Get the values in the record
            object[] values = record.GetValues();
            // Loop through the fields in the current table
            for (int i = 0; i < currentFields.Fields.Length; i++)
                // If the datatype of the current field is VarChar (string)
                if (currentFields.Fields[i].DataType == Datatype.VarChar)
                    // If the size of the current value's string length is greater than the largest known size, set the largest known size to the current size
                    if (Encoding.UTF8.GetByteCount(((string)values[i])) > currentFieldSizes[i])
                        currentFieldSizes[i] = (ushort)Encoding.UTF8.GetByteCount(((string)values[i]));
        }

        /// <summary>
        /// Check if a LocationDataFile is unique in the data index
        /// </summary>
        /// <param name="file">The file to check</param>
        /// <returns>true if the file is unique</returns>
        public static bool IsUniqueDataFile(LocationDataFile file)
        {
            // Loop through the data files in the data index
            foreach (LocationDataFile currentFile in App.DataIndex.LocationDataFiles)
                // If the datetime of the current file matches that of the requested file return false
                if (currentFile.DateTime == file.DateTime) return false;
            // Return true
            return true;
        }

        /// <summary>
        /// Check if a LocationDataFile is unique in the data index
        /// </summary>
        /// <param name="tableName">The name of the file to check</param>
        /// <returns>true if the file is unique</returns>
        public static bool IsUniqueDataFile(string tableName)
        {
            // Loop through the data files in the data index
            foreach (LocationDataFile currentFile in App.DataIndex.LocationDataFiles)
                // If the table name of the current file matches that of the requested file return false
                if (currentFile.TableName == tableName) return false;
            // Return true
            return true;
        }

        /// <summary>
        /// Get the DateTime for a standard data file
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <returns>the datetime coresponding to the inputted file name</returns>
        public static DateTime GetDataFileDateTime(string fileName)
        {
            // Get the filename without its file extension
            string dataString = Path.GetFileNameWithoutExtension(fileName);
            // Return the datetime parsed from the end portion of the datastring
            return DateTime.ParseExact(dataString.Substring(dataString.Length - 10, 10), "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Get file file name of a zip file without its extensions
        /// </summary>
        /// <param name="fileName">The name of the zip file</param>
        /// <returns>the name of the unzipped file</returns>
        public static string GetZipFileName(string fileName)
        {
            return Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fileName));
        }

        /// <summary>
        /// Clear the data cache
        /// </summary>
        public void ClearCache()
        {
            // Loop through all the files in the DataCache and delete them
            foreach (string fileName in Directory.GetFiles(SettingsManager.Active.DataCacheFolder)) File.Delete(fileName);
        }

        /// <summary>
        /// Set the current selected data files
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void DataFilesSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            // Set the LocationDataFile list to the selected datafiles
            SelectedDataFiles = DataFilesDataGrid.SelectedItems.Cast<LocationDataFile>().ToList();
        }

        /// <summary>
        /// Update the datagrid table
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            // Call to the common updatetable function
            UpdateTable();
        }

        /// <summary>
        /// Submit the current file selection
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            // Call to the common submitselection method
            SubmitSelection();
        }

        /// <summary>
        /// Clear the datacache
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void ClearCacheButtonClick(object sender, RoutedEventArgs e)
        {
            // Display a confirmation dialog
            DialogResult result = System.Windows.Forms.MessageBox.Show("Are you sure you want to delete the data cache?", "Confirmation", MessageBoxButtons.YesNo);
            // If the user confirms yes, delete the cache
            if (result == DialogResult.Yes) ClearCache();
        }

        /// <summary>
        /// Start the import of a data file folder
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void ImportDataFileClick(object sender, RoutedEventArgs e)
        {
            // Open a folder browser dialog
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            // If the user does not cancel
            if (folderBrowser.ShowDialog() == DialogResult.OK)
                // Start a new background thread importing the folder
                new Thread(() => ImportFolder(folderBrowser.SelectedPath)) { IsBackground = true }.Start();
        }

        /// <summary>
        /// Show information about the current datafile selction
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void SelectionInformationButtonClick(object sender, RoutedEventArgs e)
        {
            // Open a new SelectionInformationWindow as a dialog
            new SelectionInformationWindow(SelectedDataFiles.ToArray()).ShowDialog();
        }
    }
}
