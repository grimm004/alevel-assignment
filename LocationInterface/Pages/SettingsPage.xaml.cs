using System;
using System.Windows.Controls;
using LocationInterface.Utils;
using System.Windows;

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
                    PercentagePerUpdate = Convert.ToInt32(percentagePerUpdateInput.Text),
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
        }
        
        private void FieldChange()
        {
            if (IsInitialized)
            {
                loadDefaultsButton.IsEnabled = EnteredSettings.GetHashCode() != SettingsManager.Defaults.GetHashCode();
                applyButton.IsEnabled = EnteredSettings.GetHashCode() != SettingsManager.Active.GetHashCode();
            }
        }

        private void BackButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            ShowPreviousPage?.Invoke();
        }
        private void ApplyButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            SettingsManager.Active = EnteredSettings;
            UpdateSettings?.Invoke();
        }
        private void LoadDefaultsButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadSettings(true);
        }
        private void SettingsTextChanged(object sender, TextChangedEventArgs e)
        {
            FieldChange();
        }
        private void SettingsPasswordChanged(object sender, RoutedEventArgs e)
        {
            FieldChange();
        }
    }
}
