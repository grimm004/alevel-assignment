using AnalysisSDK;
using LocationInterface.Utils;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for VendorAnalysisWindow.xaml
    /// </summary>
    public partial class AnalysisWindow : Window
    {
        protected bool Analysing { get; set; }
        protected Common Common { get; set; }
        public IAnalysis Analysis { get; protected set; }

        public AnalysisWindow(Common common, IAnalysis analysis)
        {
            Common = common;
            Analysis = analysis;
            Analysing = false;
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = Analysing ? MessageBox.Show("Are you sure you want to exit and cancel analysis?", "Program Analysing", MessageBoxButton.YesNo) != MessageBoxResult.Yes : false;
        }

        protected void StartButtonClick(object sender, RoutedEventArgs e)
        {
            Analysing = true;
            startAnalysisButton.IsEnabled = false;
            startAnalysisButton.Content = "Running Analysis";
            new Thread(() => RunAnalysis()) { IsBackground = false }.Start();
        }
        
        protected void RunAnalysis()
        {
            Analysis.Run(Common.LoadedDataTables, (ratio) => Dispatcher.Invoke(() => analysisProgressBar.Value = 100 * ratio));
            Analysing = false;
            Dispatcher.Invoke(() =>
            {
                startAnalysisButton.IsEnabled = true;
                startAnalysisButton.Content = "Start Analysis";
            });
        }
    }
}
