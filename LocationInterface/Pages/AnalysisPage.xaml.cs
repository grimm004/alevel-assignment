using System;
using System.Windows;
using System.Windows.Controls;
using DatabaseManagerLibrary;
using DatabaseManagerLibrary.CSV;
using LocationInterface.Utils;
using System.Net.Mail;

namespace LocationInterface.Pages
{
    /// <summary>
    /// Interaction logic for AnalysisPage.xaml
    /// </summary>
    public partial class AnalysisPage : Page
    {
        protected Action ShowPreviousPage { get; set; }

        public AnalysisPage(Action ShowPreviousPage)
        {
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
    }
}
