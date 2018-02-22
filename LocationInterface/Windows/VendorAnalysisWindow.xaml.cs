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

        /// <summary>
        /// Initialze the analysis window
        /// </summary>
        /// <param name="common">The common data instance</param>
        /// <param name="analysis">The analysis to run</param>
        public AnalysisWindow(Common common, IAnalysis analysis)
        {
            Common = common;
            Analysis = analysis;
            Analysing = false;
            InitializeComponent();
        }

        /// <summary>
        /// Event for when the window is closing
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            // If analysis is running, cancel the window close if the user wants to do so
            e.Cancel = Analysing ? MessageBox.Show("Are you sure you want to exit and cancel analysis?", "Program Analysing", MessageBoxButton.YesNo) != MessageBoxResult.Yes : false;
        }

        /// <summary>
        /// Begin startin analysis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void StartButtonClick(object sender, RoutedEventArgs e)
        {
            // Mark the window as analysing
            Analysing = true;
            // Disable the start analysis button
            StartAnalysisButton.IsEnabled = false;
            // Set the content of the start analysis window
            StartAnalysisButton.Content = "Running Analysis";
            // Run the analysis in a new background thread
            new Thread(() => RunAnalysis()) { IsBackground = false }.Start();
        }
        
        /// <summary>
        /// Run the desired analysis
        /// </summary>
        protected void RunAnalysis()
        {
            // Run the analysis
            Analysis.Run(Common.LoadedDataTables, (ratio) => Dispatcher.Invoke(() => analysisProgressBar.Value = 100 * ratio));
            // Do a thread-safe cll to re-set the start analysis button
            Dispatcher.Invoke(() =>
            {
                StartAnalysisButton.IsEnabled = true;
                StartAnalysisButton.Content = "Start Analysis";
            });
            // Mark the analysis as stopped
            Analysing = false;
        }
    }
}
