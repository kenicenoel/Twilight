using Quartz;
using Quartz.Impl;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Twilight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private object on;
        private object off;
        private IScheduler scheduler;
        private System.Windows.Forms.NotifyIcon MyNotifyIcon;
        public MainWindow()
        {
            InitializeComponent();
             on = FindResource("On");
             off = FindResource("Off");
            ToggleAutoThemeButton.Content = Properties.Settings.Default.AutoThemeSwitch == true ? on : off;
            if (Properties.Settings.Default.AutoThemeSwitch)
            {
                Console.WriteLine("Auto switch enabled. Setting up...");
                SetupScheduler();

            }
            
            SetupMinimizeToTray();

        }

        private void SetupMinimizeToTray()
        {
            MyNotifyIcon = new System.Windows.Forms.NotifyIcon();
            MyNotifyIcon.Icon = new System.Drawing.Icon(
                            @"../../bulb.ico");
            MyNotifyIcon.MouseDoubleClick +=
                new System.Windows.Forms.MouseEventHandler
                    (MyNotifyIcon_MouseDoubleClick);
        }

        void MyNotifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void SetupScheduler()
        {
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

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

            scheduler.ScheduleJob(jobA, morningTrigger);
            scheduler.ScheduleJob(jobB, eveningTrigger);

            if (scheduler.IsStarted) Console.WriteLine("Scheduler has been started");
        }

        private void ToggleAutoThemeButton_Click(object sender, RoutedEventArgs e)
        {
            
            ToggleAutoThemeButton.Content =  ToggleAutoThemeButton.Content == off ? on : off;
            Properties.Settings.Default.AutoThemeSwitch = ToggleAutoThemeButton.Content == off ? false : true;
            Properties.Settings.Default.Save();
            if (!Properties.Settings.Default.AutoThemeSwitch)
            {

                if (scheduler != null)
                {
                    scheduler.Shutdown();
                }
            }
            else
            {
                SetupScheduler();
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                MyNotifyIcon.BalloonTipTitle = "Twilight";
                MyNotifyIcon.BalloonTipText = "We're gonna stay tucked away here to ensure that your theme changes automagically.";
                MyNotifyIcon.ShowBalloonTip(600);
                MyNotifyIcon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                MyNotifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }
    }
}
