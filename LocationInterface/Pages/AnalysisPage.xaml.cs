using System;
using System.Windows;
using System.Windows.Controls;
using LocationInterface.Utils;
using LocationInterface.Windows;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for AnalysisPage.xaml
    /// </summary>
    public partial class AnalysisPage : Page
    {
        protected Action ShowPreviousPage { get; set; }
        protected Common Common { get; }
        public bool AnalysisRunning { get; protected set; }

        public AnalysisPage(Common common, Action ShowPreviousPage)
        {
            Common = common;
            this.ShowPreviousPage = ShowPreviousPage;
            InitializeComponent();
        }

        public void BackButtonClick(object sender, RoutedEventArgs e)
        {
            ShowPreviousPage?.Invoke();
        }

        private void SendEmailButtonClick(object sender, RoutedEventArgs e)
        {
            new Windows.EmailWindow().ShowDialog();
        }

        private void VendorAnalysisButtonClick(object sender, RoutedEventArgs e)
        {
            new VendorAnalysisWindow(Common).ShowDialog();
        }

        private void ExportPdfButtonClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
