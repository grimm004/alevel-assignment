using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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
        protected DispatcherTimer AutomationTimer { get; set; }
        protected Action<double> TimeChangedCallback { get; }

        public TimeManagerWindow(Action<double> TimeChangedCallback)
        {
            this.TimeChangedCallback = TimeChangedCallback;
            InitializeComponent();

            AutomationTimer = new DispatcherTimer();
            AutomationTimer.Tick += DispatchAutomationTimer;
            AutomationTimer.Interval = TimeSpan.FromMilliseconds(1000d / 60d);

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

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeChangedCallback?.Invoke(((Slider)sender).Value);
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
                AutoTimeCount += AutoTimeSpeed / 60d;
                if (AutoTimeCount > 24) AutoTimeCount = 0;
                TimeSlider.Value = AutoTimeCount;
            }
        }
    }
}
