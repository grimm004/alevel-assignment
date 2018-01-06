using LocationInterface.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        public InsertAnalysisWindow(EmailProcessor emailProcessor)
        {
            InitializeComponent();
            DataContext = this;
            Selected = false;
            AnalysisSelectionBox.ItemsSource = emailProcessor.BindableVariables.Keys.ToArray();
        }

        private void SubmitButtonClick(object sender, RoutedEventArgs e)
        {
            Selected = true;
            Close();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Selected = false;
            Close();
        }
    }
}
