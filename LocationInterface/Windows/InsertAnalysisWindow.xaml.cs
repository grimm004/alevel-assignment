using LocationInterface.Utils;
using System.Linq;
using System.Windows;

namespace LocationInterface.Windows
{
    public class AnalysisTypeBind
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Interaction logic for InsertAnalysisWindow.xaml
    /// </summary>
    public partial class InsertAnalysisWindow : Window
    {
        public string SelectedAnalysis { get; set; }
        public string AnalysisMetadata { get; set; }
        public string AnalysisBindString { get { return $"{{ { SelectedAnalysis }{ (string.IsNullOrWhiteSpace(AnalysisMetadata) ? "" : ":") }{ AnalysisMetadata } }}"; } }
        public bool Selected { get; set; }

        /// <summary>
        /// Initialze the insert analysis window
        /// </summary>
        /// <param name="emailProcessor"></param>
        public InsertAnalysisWindow(EmailProcessor emailProcessor)
        {
            InitializeComponent();
            DataContext = this;
            Selected = false;
            AnalysisSelectionBox.ItemsSource = emailProcessor.BindableVariables.Keys.ToArray();
        }

        /// <summary>
        /// Submit the selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            Selected = true;
            Close();
        }

        /// <summary>
        /// Cancel the selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Selected = false;
            Close();
        }
    }
}
