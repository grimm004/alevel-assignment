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
        protected Common Common { get; }
        
        /// <summary>
        /// A property that represents the entered settings.
        /// It sets and gets data directly to and from the wpf entry boxes
        /// </summary>
        protected Settings EnteredSettings
        {
            get
            {
                // Create and return a new settings object with all the entered data
                return new Settings
                {
                    PercentagePerUpdate = Convert.ToDouble(percentagePerUpdateInput.Text),
                    RawDataRecordBuffer = Convert.ToInt32(rawDataRecordBufferInput.Text),
                    DataCacheFolder = dataCacheFolderInput.Text,
                    LocationDataFolder = locationDataFolderInput.Text,
                    EmailDatabase = emailDatabaseFolder.Text,
                    ImageFolder = imageFolder.Text,
                    AnalysisFolder = analysisFolder.Text,
                    EmailServer = emailServerInput.Text,
                    EmailPort = Convert.ToInt32(emailPortInput.Text),
                    EmailAddress = emailAddressInput.Text,
                    DisplayName = emailNameInput.Text,
                    Password = emailPasswordInput.Password
                };
            }
            set
            {
                // Set the values of each entry box from the supplied settings object

                percentagePerUpdateInput.Text = value.PercentagePerUpdate.ToString();
                rawDataRecordBufferInput.Text = value.RawDataRecordBuffer.ToString();
                dataCacheFolderInput.Text = value.DataCacheFolder;
                locationDataFolderInput.Text = value.LocationDataFolder;
                emailDatabaseFolder.Text = value.EmailDatabase;
                imageFolder.Text = value.ImageFolder;
                analysisFolder.Text = value.AnalysisFolder;
                emailServerInput.Text = value.EmailServer;
                emailPortInput.Text = value.EmailPort.ToString();
                emailAddressInput.Text = value.EmailAddress;
                emailNameInput.Text = value.DisplayName;
                emailPasswordInput.Password = value.Password;
            }
        }

        /// <summary>
        /// Initialise the Settings Page
        /// </summary>
        public SettingsPage(Common common)
        {
            Common = common;
            InitializeComponent();
            
            applyButton.IsEnabled = loadDefaultsButton.IsEnabled = false;
        }

        /// <summary>
        /// Load the settings to the entry boxes
        /// </summary>
        public void LoadSettings()
        {
            // Set the entered settings to the active settings
            EnteredSettings = SettingsManager.Active;
            // Mark the fields as having changed
            FieldChange();
        }
        
        /// <summary>
        /// Verify if the entered settings are the same as the active ones or the default ones
        /// </summary>
        private void FieldChange()
        {
            // Verify the wpf items are initialised
            if (IsInitialized)
                try
                {
                    // If the hash code of the entered settings is equal to the hash code of the default settings disable the load defaults button
                    loadDefaultsButton.IsEnabled = EnteredSettings.GetHashCode() != SettingsManager.Defaults.GetHashCode();
                    // If the hash code of the entered settings is equal to the hash code of the active settings disable the apply button
                    applyButton.IsEnabled = EnteredSettings.GetHashCode() != SettingsManager.Active.GetHashCode();
                } catch (FormatException)
                {
                    // If a formatexception occurrs enable both buttons
                    loadDefaultsButton.IsEnabled = true;
                    applyButton.IsEnabled = false;
                }
        }

        /// <summary>
        /// Return to the previous page
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            // Run the callback to show the previous page
            Common.ShowPreviousPage();
        }
        /// <summary>
        /// Apply the changes to the settings
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void ApplyButtonClick(object sender, RoutedEventArgs e)
        {
            // Set the active settings to the entered settings
            SettingsManager.Active = EnteredSettings;
            // Save the settings to the config file
            SettingsManager.Save();
            // Mark the field changes
            FieldChange();
        }
        /// <summary>
        /// Load the default settings
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void LoadDefaultsButtonClick(object sender, RoutedEventArgs e)
        {
            // Set the entered settings to the default settings
            EnteredSettings = SettingsManager.Defaults;
            // Mark the fields as having changed
            FieldChange();
        }
        /// <summary>
        /// Mark the fields as having changed
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void SettingsTextChanged(object sender, TextChangedEventArgs e)
        {
            // Mark the fields as having changed
            FieldChange();
        }
        /// <summary>
        /// Mark the fields as having changed
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void SettingsPasswordChanged(object sender, RoutedEventArgs e)
        {
            // Mark the fields as having changed
            FieldChange();
        }
        /// <summary>
        /// Validate that entered text is a number
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            // Mark the validation as having been handled if the text is numerical
            e.Handled = !IsNumericalText(e.Text);
            // If the length of the text is zero, set the value of the text to the number zero
            if (e.Text.Length == 0) { e.Handled = true; ((TextBox)sender).Text = "0"; }
        }
        /// <summary>
        /// Ensure pasted data is numerical
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void PasteNumberValidation(object sender, DataObjectPastingEventArgs e)
        {
            // If the datatype of the current sender is a string
            if (e.DataObject.GetDataPresent(typeof(String)))
                // If the entered text is not numerical cancel the paste
                if (!IsNumericalText((String)e.DataObject.GetData(typeof(String)))) e.CancelCommand();
            else e.CancelCommand();
        }

        /// <summary>
        /// Check if text is numerical
        /// </summary>
        /// <param name="text">The text to check</param>
        /// <returns>true if the text entered is numerical</returns>
        private bool IsNumericalText(string text)
        {
            // Define a regular expression object to check if text is not numerical
            Regex regex = new Regex("[^0-9.-]+");
            // Return true if the text is not a match
            return !regex.IsMatch(text);
        }
    }
}
