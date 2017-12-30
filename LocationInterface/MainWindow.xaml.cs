using System.Windows;
using System.Windows.Controls;
using LocationInterface.Utils;

namespace LocationInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected Common Common { get; set; }

        /// <summary>
        /// Initialize the MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Create a new instance of the common properties and methods class
            Common = new Common(ShowPage);

            // Set the window to launch in the center of the screen
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Show the hompage (as the first page when the application opens)
            Common.ShowHomePage();
        }

        /// <summary>
        /// Show a given page
        /// </summary>
        /// <param name="page">The page to display on the main window</param>
        protected void ShowPage(Page page)
        {
            // Log the current page as previous page
            Common.PreviousPage = Common.CurrentPage;
            // Switch the content of the inner frame to the current page
            DataContext = frame.Content = Common.CurrentPage = page;
        }
    }
}
