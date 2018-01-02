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

        public TimeManagerWindow(Action<double> AutoTimeChangeCallback, Action<object, RoutedEventArgs> TimeEnabledEvent, Action<object, RoutedEventArgs> TimeDisabledEvent)
        {
            this.AutoTimeChangeCallback = AutoTimeChangeCallback;
            this.TimeEnabledEvent = TimeEnabledEvent;
            this.TimeDisabledEvent = TimeDisabledEvent;
            InitializeComponent();
            DataContext = this;
            TimeEnabled = false;

            AutomationTimer = new DispatcherTimer();
            AutomationTimer.Tick += DispatchAutomationTimer;
            AutomationTimer.Interval = TimeSpan.FromMilliseconds(1000d / 30d);

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

        private void AutomaticTimeEnabled(object sender, RoutedEventArgs e)
        {
            AutomationTimer.Start();
        }

        private void AutomaticTimeDisabled(object sender, RoutedEventArgs e)
        {
            AutomationTimer.Stop();
        }

        private void DispatchAutomationTimer(object sender, EventArgs e)
        {
            if (AutoTimeCheckbox.IsEnabled)
            {
                AutoTimeCount += AutoTimeSpeed / 30d;
                if (AutoTimeCount > 24) AutoTimeCount = 0;
                AutoTimeChangeCallback?.Invoke(AutoTimeCount);
            }
        }
    }
}
