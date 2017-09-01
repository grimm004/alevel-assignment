using LocationInterface.Utils;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for VendorAnalysisWindow.xaml
    /// </summary>
    public partial class VendorAnalysisWindow : Window
    {
        protected bool Analysing { get; set; }
        protected Common Common { get; set; }

        public VendorAnalysisWindow(Common common)
        {
            Common = common;
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
            new VendorAnalysis((ratio) => Dispatcher.Invoke(() => analysisProgressBar.Value = 100 * ratio), $"VendorCounts-{ DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") }").RunAnalysis(Common.LoadedDataTables);
            Analysing = false;
            Dispatcher.Invoke(() =>
            {
                startAnalysisButton.IsEnabled = true;
                startAnalysisButton.Content = "Start Analysis";
            });
        }
    }
}
