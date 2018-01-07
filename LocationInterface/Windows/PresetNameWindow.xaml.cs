using System;
using System.Windows;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for PresetNameWindow.xaml
    /// </summary>
    public partial class PresetNameWindow : Window
    {
        public bool Submitted { get; protected set; }
        public string Text { get; protected set; }
        protected Func<string, bool> ValidityCallback { get; set; }

        /// <summary>
        /// Initialize the preset name window
        /// </summary>
        /// <param name="ValidityCallback"></param>
        public PresetNameWindow(Func<string, bool> ValidityCallback)
        {
            this.ValidityCallback = ValidityCallback;
            InitializeComponent();
        }

        /// <summary>
        /// Submit the entered name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            // Check that the name has been entered
            bool valid = !string.IsNullOrWhiteSpace(valueEntry.Text);
            // Check the preset does not already exist
            valid = valid && ValidityCallback(valueEntry.Text);
            
            // If valid
            if (valid)
            {
                // Submit the name
                Submitted = true;
                Text = valueEntry.Text;
                Close();
            }
            else MessageBox.Show("You must enter a valid unique preset name.", "Value Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Cancel name submission
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Submitted = false;
            Close();
        }
    }
}
