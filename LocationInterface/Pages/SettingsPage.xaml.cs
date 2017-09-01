using System;
using System.Windows.Controls;
using LocationInterface.Utils;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        protected Action ShowPreviousPage { get; set; }
        protected Action UpdateSettings { get; set; }
        protected Settings EnteredSettings
        {
            get
            {
                return new Settings
                {
                    PercentagePerUpdate = Convert.ToDouble(percentagePerUpdateInput.Text),
                    RawDataRecordBuffer = Convert.ToInt32(rawDataRecordBufferInput.Text),
                    DataCacheFolder = dataCacheFolderInput.Text,
                    LocationDataFolder = locationDataFolderInput.Text,
                    EmailDatabase = emailDatabaseFolder.Text,

                    EmailServer = emailServerInput.Text,
                    EmailPort = Convert.ToInt32(emailPortInput.Text),
                    EmailAddress = emailAddressInput.Text,
                    DisplayName = emailNameInput.Text,
                    Password = emailPasswordInput.Password
                };
            }
            set
            {
                percentagePerUpdateInput.Text = value.PercentagePerUpdate.ToString();
                rawDataRecordBufferInput.Text = value.RawDataRecordBuffer.ToString();
                dataCacheFolderInput.Text = value.DataCacheFolder;
                locationDataFolderInput.Text = value.LocationDataFolder;
                emailDatabaseFolder.Text = value.EmailDatabase;

                emailServerInput.Text = value.EmailServer;
                emailPortInput.Text = value.EmailPort.ToString();
                emailAddressInput.Text = value.EmailAddress;
                emailNameInput.Text = value.DisplayName;
                emailPasswordInput.Password = value.Password;
            }
        }

        public SettingsPage(Action ShowPreviousPage, Action UpdateSettings)
        {
            this.ShowPreviousPage = ShowPreviousPage;
            this.UpdateSettings = UpdateSettings;
            InitializeComponent();
            loadDefaultsButton.IsEnabled = false;
            applyButton.IsEnabled = false;
        }

        public void LoadSettings(bool loadDefaults = false)
        {
            if (!loadDefaults) SettingsManager.Load();
            else SettingsManager.Active = SettingsManager.Defaults;
            EnteredSettings = SettingsManager.Active;
            FieldChange();
        }
        
        private void FieldChange()
        {
            if (IsInitialized)
            {
                try
                {
                    loadDefaultsButton.IsEnabled = EnteredSettings.GetHashCode() != SettingsManager.Defaults.GetHashCode();
                    applyButton.IsEnabled = EnteredSettings.GetHashCode() != SettingsManager.Active.GetHashCode();
                } catch (FormatException)
                {
                    loadDefaultsButton.IsEnabled = true;
                    applyButton.IsEnabled = false;
                }
            }
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            ShowPreviousPage?.Invoke();
        }
        private void ApplyButtonClick(object sender, RoutedEventArgs e)
        {
            SettingsManager.Active = EnteredSettings;
            UpdateSettings?.Invoke();
            FieldChange();
        }
        private void LoadDefaultsButtonClick(object sender, RoutedEventArgs e)
        {
            EnteredSettings = SettingsManager.Defaults;
            FieldChange();
        }
        private void SettingsTextChanged(object sender, TextChangedEventArgs e)
        {
            FieldChange();
        }
        private void SettingsPasswordChanged(object sender, RoutedEventArgs e)
        {
            FieldChange();
        }
        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumericalText(e.Text);
            if (e.Text.Length == 0) { e.Handled = true; ((TextBox)sender).Text = "0"; }
        }
        private void PasteNumberValidation(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsNumericalText(text)) e.CancelCommand();
            }
            else e.CancelCommand();
        }

        private bool IsNumericalText(string text)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(text);
        }
    }
}
