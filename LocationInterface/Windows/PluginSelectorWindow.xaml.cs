using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using LocationInterface.Utils;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for PluginSelectorWindow.xaml
    /// </summary>
    public partial class PluginSelectorWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
        
        protected Plugin[] Plugins { get; set; }
        protected Plugin[] IgnoreablePlugins { get; set; }
        public Plugin[] SelectedPlugins { get; protected set; }

        public PluginSelectorWindow(Plugin[] plugins, Plugin[] ignoreablePlugins)
        {
            InitializeComponent();
            DataContext = this;

            Plugins = plugins;
            SelectedPlugins = new Plugin[0];
            IgnoreablePlugins = ignoreablePlugins;
            PopulateTable();
        }

        /// <summary>
        /// Populate the DataGrid with the available plugins
        /// </summary>
        public void PopulateTable()
        {
            // Clear the current items in the data grid's list
            PluginsDataGrid.Items.Clear();
            // Loop through each plugins in the contacts list
            foreach (Plugin plugin in Plugins)
            {
                bool insertPlugin = true;
                // Loop through each ignorable plugin
                foreach (Plugin ignorablePlugin in IgnoreablePlugins)
                    // If the name of the current plugin is equal to that of the current ignorable plugin
                    if (ignorablePlugin.Name == plugin.Name)
                        // Mark the plugin to not be added to list
                        insertPlugin = false;
                // If the contact can be added to the list, add it
                if (insertPlugin) PluginsDataGrid.Items.Add(plugin);
            }
        }

        /// <summary>
        /// Store a local set of selected plugins (for when the window is closed)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPlugins = new Plugin[PluginsDataGrid.SelectedItems.Count];
            for (int i = 0; i < SelectedPlugins.Length; i++) SelectedPlugins[i] = PluginsDataGrid.SelectedItems[i] as Plugin;
        }

        /// <summary>
        /// Submit the selected plugins and close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitSelectionButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Cancel the selection and close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            SelectedPlugins = new Plugin[0];
            Close();
        }
    }
}
