using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for TimeSetterWindow.xaml
    /// </summary>
    public partial class TimeSetterWindow : Window
    {
        protected Action<double> TimeChangedCallback { get; }

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

        /// <summary>
        /// Initialize the time setter window
        /// </summary>
        /// <param name="TimeChangedCallback">The callback for when the selected time is changed</param>
        public TimeSetterWindow(Action<double> TimeChangedCallback)
        {
            this.TimeChangedCallback = TimeChangedCallback;
            InitializeComponent();
            
            // Change the closing behaviour to hide the window
            Closing += delegate (object sender, CancelEventArgs e)
            {
                e.Cancel = true;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    (DispatcherOperationCallback)(arg =>
                    {
                        Hide();
                        return null;
                    }), null);
            };
        }

        /// <summary>
        /// Event for when the time selection slider is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Run the callback with the sldier's selected value
            TimeChangedCallback?.Invoke(((Slider)sender).Value);
        }

        /// <summary>
        /// Change the selected time
        /// </summary>
        /// <param name="newTime">The time to change to</param>
        public void TimeChange(double newTime)
        {
            // Set the sliders value to the new time
            TimeSlider.Value = newTime;
        }
    }
}
