﻿using System;
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

        /// <summary>
        /// Initialise the Analysis Page
        /// </summary>
        /// <param name="common">The current instance of the common class</param>
        /// <param name="ShowPreviousPage">A callback to show the previous page</param>
        public AnalysisPage(Common common, Action ShowPreviousPage)
        {
            InitializeComponent();

            Common = common;
            this.ShowPreviousPage = ShowPreviousPage;
        }

        /// <summary>
        /// Show the previous page
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        public void BackButtonClick(object sender, RoutedEventArgs e)
        {
            ShowPreviousPage?.Invoke();
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
        private void VendorAnalysisButtonClick(object sender, RoutedEventArgs e)
        {
            // Create a new instance of the vendor analysis window and show it as a dialog
            new VendorAnalysisWindow(Common).ShowDialog();
        }

        /// <summary>
        /// Show the export as PDF window
        /// </summary>
        /// <param name="sender">The instance of the object that triggered the event</param>
        /// <param name="e">Information about the event</param>
        private void ExportPdfButtonClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}