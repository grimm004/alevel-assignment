using System;
using System.Windows;
using LocationInterface.Utils;
using DatabaseManagerLibrary;
using DatabaseManagerLibrary.CSV;
using System.Windows.Controls;
using System.Windows.Input;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for EmailPresetWindow.xaml
    /// </summary>
    public partial class EmailPresetWindow : Window
    {
        protected Database Database { get; set; }
        public EmailPreset SelectedPreset { get; protected set; }
        public bool PresetLoaded { get; protected set; }
        protected EmailPreset[] Presets { get; set; }
        protected string Subject { get; set; }
        protected string Body { get; set; }

        /// <summary>
        /// Initialse the email presets manager window
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public EmailPresetWindow(string subject, string body)
        {
            Subject = subject;
            Body = body;
            Database = new CSVDatabase(SettingsManager.Active.EmailDatabase, tableFileExtention: ".csv");
            PresetLoaded = false;
            InitializeComponent();
            UpdateTable();
            LoadPresetButton.IsEnabled = false;

            // Add a key listener for the delete key
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Delete)) RemovePreset(); };
        }

        /// <summary>
        /// Update the datagrid with the available presets
        /// </summary>
        public void UpdateTable()
        {
            // Clear the current items in the datagrid
            PresetsDataGrid.Items.Clear();
            // Loop through each preset in the presets table
            foreach (EmailPreset preset in (Presets = Database.GetRecords<EmailPreset>("Presets")))
                // Add each one to the presets data grid
                PresetsDataGrid.Items.Add(preset);
            // Enable the load presets button if the number of available presets is greater than zero
            LoadPresetButton.IsEnabled = Presets.Length > 0;
        }

        /// <summary>
        /// Load the selected preset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadPresetButtonClick(object sender, RoutedEventArgs e)
        {
            // Mark a preset to have been loaded
            PresetLoaded = true;
            // If the selected index of the preset in the datagrid is not -1 (one is selected), store the selected preset to be retreivd upon close
            if (PresetsDataGrid.SelectedIndex != -1) SelectedPreset = Presets[PresetsDataGrid.SelectedIndex];
            // Close the window
            Close();
        }
        
        /// <summary>
        /// Save the current subject and body as a preset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavePresetButtonClick(object sender, RoutedEventArgs e)
        {
            // Create a preset name selector window
            PresetNameWindow window = new PresetNameWindow(IsUniqueName);
            // Show the window as a dialog
            window.ShowDialog();
            // If the preset name selection has been submitted
            if (window.Submitted)
            {
                // Enable disable the preset save button
                SavePresetButton.IsEnabled = false;
                // Change the text in the save preset button
                SavePresetButton.Content = "Preset Saved";
                // Fetch the presets table from the database
                Table presetsTable = Database.GetTable("Presets");
                // Add the preset as a new record to the presets table
                presetsTable.AddRecord(new object[] { window.Text, Subject, Body });
                // Save the changes to the database
                Database.SaveChanges();
                // Updat the DataGrid with the new preset
                UpdateTable();
            }
        }

        /// <summary>
        /// Callback to check if a preset name is unique
        /// </summary>
        /// <param name="name">The name to check</param>
        /// <returns>true if the name is unique</returns>
        private bool IsUniqueName(string name)
        {
            // Fetch the rresets table and get the records where the name is equal the name being checked,
            // if an empty array is returned then the name is unique
            return Database.GetTable("Presets").GetRecords("Name", name).Length == 0;
        }

        /// <summary>
        /// Remove a preset
        /// </summary>
        public void RemovePreset()
        {
            // Fetch the presets table from the database
            Table contactsTable = Database.GetTable("Presets");
            // Loop through each preset in the selected presets
            foreach (EmailPreset preset in PresetsDataGrid.SelectedItems)
                // Delete it from the database table
                contactsTable.DeleteRecord((uint)Array.IndexOf(Presets, preset));
            // Save the changes to the table
            Database.SaveChanges();
            // Update the datagrid
            UpdateTable();
        }

        /// <summary>
        /// Cancel preset selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            PresetLoaded = false;
            Close();
        }

        /// <summary>
        /// Enable the ability to load a preset upon change of selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PresetsDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadPresetButton.IsEnabled = true;
        }
    }
}
