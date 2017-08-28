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

        public EmailPresetWindow(string subject, string body)
        {
            Subject = subject;
            Body = body;
            Database = new CSVDatabase(SettingsManager.Active.EmailDatabase, tableFileExtention: ".csv");
            PresetLoaded = false;
            InitializeComponent();
            UpdateTable();
            loadPresetButton.IsEnabled = false;

            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Delete)) RemovePreset(); };
        }

        public void UpdateTable()
        {
            presetsDataGrid.Items.Clear();
            foreach (EmailPreset contact in (Presets = Database.GetRecords<EmailPreset>("Presets")))
                presetsDataGrid.Items.Add(contact);
            loadPresetButton.IsEnabled = Presets.Length > 0;
        }

        private void LoadPresetButtonClick(object sender, RoutedEventArgs e)
        {
            PresetLoaded = true;
            if (presetsDataGrid.SelectedIndex != -1) SelectedPreset = Presets[presetsDataGrid.SelectedIndex];
            Close();
        }

        private void SavePresetButtonClick(object sender, RoutedEventArgs e)
        {
            PresetNameWindow window = new PresetNameWindow(IsUniqueName);
            window.ShowDialog();
            if (window.Submitted)
            {
                savePresetButton.IsEnabled = false;
                savePresetButton.Content = "Preset Saved";
                Table presetsTable = Database.GetTable("Presets");
                presetsTable.AddRecord(new object[] { window.Text, Subject, Body });
                Database.SaveChanges();
                UpdateTable();
            }
        }

        private bool IsUniqueName(string name)
        {
            Table presetsTable = Database.GetTable("Presets");
            return presetsTable.GetRecords("Name", name).Length == 0;
        }

        public void RemovePreset()
        {
            Table contactsTable = Database.GetTable("Presets");
            foreach (EmailPreset contact in presetsDataGrid.SelectedItems)
                contactsTable.DeleteRecord((uint)Array.IndexOf(Presets, contact));
            Database.SaveChanges();
            UpdateTable();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            PresetLoaded = false;
            Close();
        }

        private void PresetsDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadPresetButton.IsEnabled = true;
        }
    }
}
