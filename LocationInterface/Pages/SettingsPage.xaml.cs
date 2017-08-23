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

        public void LoadSettings()
        {
            SettingsManager.Load();

            percentagePerUpdateInput.Text = SettingsManager.Active.PercentagePerUpdate.ToString();
            rawDataRecordBufferInput.Text = SettingsManager.Active.RawDataRecordBuffer.ToString();
            dataCacheFolderInput.Text = SettingsManager.Active.DataCacheFolder;
            locationDataFolderInput.Text = SettingsManager.Active.LocationDataFolder;
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

            UpdateSettings?.Invoke();
        }
    }
}
