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

        public PresetNameWindow(Func<string, bool> ValidityCallback)
        {
            this.ValidityCallback = ValidityCallback;
            InitializeComponent();
        }

        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            bool valid = !string.IsNullOrWhiteSpace(valueEntry.Text);
            valid = valid && ValidityCallback(valueEntry.Text);
            
            if (valid)
            {
                Submitted = true;
                Text = valueEntry.Text;
                Close();
            }
            else MessageBox.Show("You must enter a valid unique preset name.", "Value Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Submitted = false;
            Close();
        }
    }
}
