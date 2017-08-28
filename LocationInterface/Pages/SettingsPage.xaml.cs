using System;
using System.Windows.Controls;
using LocationInterface.Utils;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        protected Action ShowPreviousPage { get; set; }
        protected Action UpdateSettings { get; set; }

        public SettingsPage(Action ShowPreviousPage, Action UpdateSettings)
        {
            this.ShowPreviousPage = ShowPreviousPage;
            this.UpdateSettings = UpdateSettings;
            InitializeComponent();
        }

        public void LoadSettings(bool loadDefaults = false)
        {
            if (!loadDefaults) SettingsManager.Load();
            else SettingsManager.LoadDefaultSettings();

            percentagePerUpdateInput.Text = SettingsManager.Active.PercentagePerUpdate.ToString();
            rawDataRecordBufferInput.Text = SettingsManager.Active.RawDataRecordBuffer.ToString();
            dataCacheFolderInput.Text = SettingsManager.Active.DataCacheFolder;
            locationDataFolderInput.Text = SettingsManager.Active.LocationDataFolder;
            emailDatabaseFolder.Text = SettingsManager.Active.EmailDatabase;

            emailServerInput.Text = SettingsManager.Active.EmailServer;
            emailPortInput.Text = SettingsManager.Active.EmailPort.ToString();
            emailAddressInput.Text = SettingsManager.Active.EmailAddress;
            emailNameInput.Text = SettingsManager.Active.DisplayName;
            emailPasswordInput.Password = SettingsManager.Active.Password;
        }

        private void BackButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            ShowPreviousPage?.Invoke();
        }
        private void ApplyButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            SettingsManager.Active.PercentagePerUpdate = Convert.ToInt32(percentagePerUpdateInput.Text);
            SettingsManager.Active.RawDataRecordBuffer = Convert.ToInt32(rawDataRecordBufferInput.Text);
            SettingsManager.Active.DataCacheFolder = dataCacheFolderInput.Text;
            SettingsManager.Active.LocationDataFolder = locationDataFolderInput.Text;
            SettingsManager.Active.EmailDatabase = emailDatabaseFolder.Text;

            SettingsManager.Active.EmailServer = emailServerInput.Text;
            SettingsManager.Active.EmailPort = Convert.ToInt32(emailPortInput.Text);
            SettingsManager.Active.EmailAddress = emailAddressInput.Text;
            SettingsManager.Active.DisplayName = emailNameInput.Text;
            SettingsManager.Active.Password = emailPasswordInput.Password;

            UpdateSettings?.Invoke();
        }
        private void LoadDefaultsButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            SettingsManager.LoadDefaultSettings();
            LoadSettings(true);
        }
    }
}
