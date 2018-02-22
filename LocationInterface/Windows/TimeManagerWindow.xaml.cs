using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for TimeManager.xaml
    /// </summary>
    public partial class TimeManagerWindow : Window
    {
        private double AutoTimeCount { get; set; } = 0;
        public double AutoTimeSpeed { get; set; } = 1d / 60d;
        public bool TimeEnabled { get; set; }
        protected DispatcherTimer AutomationTimer { get; }
        protected Action<double> AutoTimeChangeCallback { get; }
        protected Action<object, RoutedEventArgs> TimeEnabledEvent { get; }
        protected Action<object, RoutedEventArgs> TimeDisabledEvent { get; }

        /// <summary>
        /// Initialze the TimeManagerWindow
        /// </summary>
        /// <param name="AutoTimeChangeCallback">Callback for a change in the automatic timer</param>
        /// <param name="TimeEnabledEvent">Callback for when timing is enabled</param>
        /// <param name="TimeDisabledEvent">Callback for when timing is disabled</param>
        public TimeManagerWindow(Action<double> AutoTimeChangeCallback, Action<object, RoutedEventArgs> TimeEnabledEvent, Action<object, RoutedEventArgs> TimeDisabledEvent)
        {
            this.AutoTimeChangeCallback = AutoTimeChangeCallback;
            this.TimeEnabledEvent = TimeEnabledEvent;
            this.TimeDisabledEvent = TimeDisabledEvent;
            InitializeComponent();
            DataContext = this;
            TimeEnabled = false;

            // Instanciate the automation timer
            AutomationTimer = new DispatcherTimer();
            // Add the dispatch callback to the automation timer
            AutomationTimer.Tick += DispatchAutomationTimer;
            // Set the interval between calls to be 1000 / 30 ms (30 calls per second)
            AutomationTimer.Interval = TimeSpan.FromMilliseconds(1000d / 30d);

            // Change the closing behaviour to hide the window
            Closing += delegate(object sender, CancelEventArgs e)
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
        /// Action when the automatic time is enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutomaticTimeEnabled(object sender, RoutedEventArgs e)
        {
            // Star the automation timer
            AutomationTimer.Start();
        }

        /// <summary>
        /// Action when the automatic time is disabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutomaticTimeDisabled(object sender, RoutedEventArgs e)
        {
            // Stop the automation timer
            AutomationTimer.Stop();
        }

        /// <summary>
        /// Callback for each automatic time update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispatchAutomationTimer(object sender, EventArgs e)
        {
            // Check if the automatic time is enabled
            if (AutoTimeCheckbox.IsEnabled)
            {
                // Increment the automatic time count
                AutoTimeCount += AutoTimeSpeed / 30d;
                // If the automatic time count is past 24 (hours), reset the count
                if (AutoTimeCount > 24) AutoTimeCount = 0;
                // Run the automatic time callback with the current time position
                AutoTimeChangeCallback?.Invoke(AutoTimeCount);
            }
        }
    }
}
