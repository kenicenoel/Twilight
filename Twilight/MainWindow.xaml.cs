using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Quartz;
using Quartz.Impl;
using Twilight.Properties;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Twilight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private object _on;
        private object _off;
        private IScheduler _scheduler;
        private NotifyIcon _myNotifyIcon;
        public MainWindow()
        {
            InitializeComponent();
             _on = FindResource("On");
             _off = FindResource("Off");
            ToggleAutoThemeButton.Content = Settings.Default.AutoThemeSwitch ? _on : _off;
            if (Settings.Default.AutoThemeSwitch)
            {
                Console.WriteLine("Auto switch enabled. Setting up...");
                SetupScheduler();

            }
            
            SetupMinimizeToTray();

        }

        private void SetupMinimizeToTray()
        {
            _myNotifyIcon = new NotifyIcon
            {
                Icon = new Icon(
                            @"../../twilight.ico")
            };
                _myNotifyIcon.DoubleClick += 
                delegate(object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
        }

        private void SetupScheduler()
        {
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            _scheduler.Start();

            var jobB = JobBuilder.Create<ToggleDarkTheme>().Build();
            var jobA = JobBuilder.Create<ToggleLightTheme>().Build();
            var morningTrigger = TriggerBuilder.Create()
                    .WithIdentity("morningTrigger")
                    .StartAt(DateBuilder.DateOf(6, 0, 0))
                    .WithSimpleSchedule(x => x
                    .WithIntervalInHours(24)
                    .RepeatForever())
                    .WithPriority(1)
                    .Build();

            var eveningTrigger = TriggerBuilder.Create()
                   .WithIdentity("nightTrigger")
                   .StartAt(DateBuilder.DateOf(19, 0, 0))
                   .WithPriority(1)
                    .WithSimpleSchedule(x => x
                    .WithIntervalInHours(24)
                    .RepeatForever())
                   .Build();

            _scheduler.ScheduleJob(jobA, morningTrigger);
            _scheduler.ScheduleJob(jobB, eveningTrigger);

            if (_scheduler.IsStarted) Console.WriteLine("Scheduler has been started");
        }

        private void ToggleAutoThemeButton_Click(object sender, RoutedEventArgs e)
        {
            
            ToggleAutoThemeButton.Content =  ToggleAutoThemeButton.Content == _off ? _on : _off;
            Settings.Default.AutoThemeSwitch = ToggleAutoThemeButton.Content == _off ? false : true;
            Settings.Default.Save();
            if (!Settings.Default.AutoThemeSwitch)
            {
                _scheduler?.Shutdown();
            }
            else
            {
                SetupScheduler();
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                ShowInTaskbar = false;
                _myNotifyIcon.BalloonTipTitle = "Twilight";
                _myNotifyIcon.BalloonTipText = "We're gonna stay tucked away here to ensure that your theme changes automagically.";
                _myNotifyIcon.ShowBalloonTip(600);
                _myNotifyIcon.Visible = true;
            }
//            else if (WindowState == WindowState.Normal)
//            {
//                _myNotifyIcon.Visible = false;
//                ShowInTaskbar = true;
//            }
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
