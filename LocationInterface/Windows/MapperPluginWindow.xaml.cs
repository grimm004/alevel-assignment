using LocationInterface.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for MapperPluginWindow.xaml
    /// </summary>
    public partial class MapperPluginWindow : Window
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

        private List<MapperPlugin> SelectedPlugins { get; set; }

        private Action<MapperPlugin[]> PluginsEnabledCallback { get; }
        private Action PluginsDisabledCallback { get; }

        public MapperPluginWindow(Action<MapperPlugin[]> PluginsEnabledCallback, Action PluginsDisabledCallback)
        {
            InitializeComponent();

            this.PluginsEnabledCallback = PluginsEnabledCallback;
            this.PluginsDisabledCallback = PluginsDisabledCallback;

            DataContext = this;
            SelectedPlugins = new List<MapperPlugin>();
        }

        private void SelectPluginsButtonClick(object sender, RoutedEventArgs e)
        {
            PluginSelectorWindow pluginSelector = new PluginSelectorWindow(PluginManager.MapperPlugins.ToArray(), SelectedPlugins.ToArray());
            pluginSelector.ShowDialog();
            if (pluginSelector.SelectedPlugins.Length > 0)
                SelectedPlugins = pluginSelector.SelectedPlugins.Cast<MapperPlugin>().ToList();

            UpdateList();
        }

        private void UpdateList()
        {
            PluginListBox.Items.Clear();
            foreach (MapperPlugin selectedPlugin in SelectedPlugins)
                PluginListBox.Items.Add(selectedPlugin);
        }

        private void PluginListKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete)
                DeleteSelected();
        }

        private void DeleteSelected()
        {
            for (int i = PluginListBox.SelectedItems.Count - 1; i > -1; i--)
                SelectedPlugins.Remove(PluginListBox.SelectedItems[i] as MapperPlugin);
            UpdateList();
        }

        private void PluginsEnabled(object sender, RoutedEventArgs e)
        {
            PluginsEnabledCallback?.Invoke(SelectedPlugins.ToArray());
        }

        private void PluginsDisabled(object sender, RoutedEventArgs e)
        {
            PluginsDisabledCallback?.Invoke();
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
