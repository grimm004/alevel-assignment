using System.Windows;
using System.Windows.Controls;
using LocationInterface.Utils;
using LocationInterface.Windows;
using System.Collections.Generic;
using PdfSharp;
using System.Diagnostics;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for AnalysisPage.xaml
    /// </summary>
    public partial class AnalysisPage : Page
    {
        protected Common Common { get; }
        public bool AnalysisRunning { get; protected set; }

        public List<AnalysisPlugin> AnalysisOptions { get; set; }
        public AnalysisPlugin SelectedAnalysis { get; set; }

        /// <summary>
        /// Initialise the Analysis Page
        /// </summary>
        /// <param name="common">The current instance of the common class</param>
        public AnalysisPage(Common common)
        {
            InitializeComponent();
            
            Common = common;
            AnalysisOptions = PluginManager.Plugins;
        }

        /// <summary>
        /// Show the previous page
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        public void BackButtonClick(object sender, RoutedEventArgs e)
        {
            Common.ShowPreviousPage();
        }

        /// <summary>
        /// Show the email window
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void SendEmailButtonClick(object sender, RoutedEventArgs e)
        {
            // Create a new instance of the email window and show it as a dialog
            new EmailWindow().ShowDialog();
        }

        /// <summary>
        /// Show the vendor analysis window
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void OnAnalysisButtonClick(object sender, RoutedEventArgs e)
        {
            // Create a new instance of the vendor analysis window and show it as a dialog
            if (SelectedAnalysis != null) new AnalysisWindow(Common, SelectedAnalysis.Analysis).ShowDialog();
        }
        
        private void OpenPluginFolderButtonClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Constants.PLUGINFOLDER);
        }
    }
}
