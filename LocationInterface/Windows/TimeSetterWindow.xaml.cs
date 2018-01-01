using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace LocationInterface.Windows
{
    /// <summary>
    /// Interaction logic for TimeSetterWindow.xaml
    /// </summary>
    public partial class TimeSetterWindow : Window
    {
        protected Action<double> TimeChangedCallback { get; }

        public TimeSetterWindow(Action<double> TimeChangedCallback)
        {
            this.TimeChangedCallback = TimeChangedCallback;
            InitializeComponent();
            
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

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeChangedCallback?.Invoke(((Slider)sender).Value);
        }

        public void AutoTimeChange(double newTime)
        {
            TimeSlider.Value = newTime;
        }
    }
}
