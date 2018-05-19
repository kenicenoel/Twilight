using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
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
        public MainWindow()
        {
            InitializeComponent();
             on = FindResource("On");
             off = FindResource("Off");
            ToggleAutoThemeButton.Content = Properties.Settings.Default.ServiceInstalled == true ? on : off;
            if(ServiceExists("Twilight"))
            {
                ServiceStatusLabel.Content = "Twilight Service not installed";
            }
            else
            {
                var warningMessage = "In order to automatically switch themes, Twilight needs to install a service. Should this be done?";
                var caption = "Install Service?";
                var buttons = MessageBoxButton.YesNo;
                var result = MessageBox.Show(warningMessage, caption, buttons, MessageBoxImage.Information);
                if (result != MessageBoxResult.Yes) Close();
                InstallService();
                ServiceStatusLabel.Content = "Twilight Service installed";
            }
        }

        private void ToggleAutoThemeButton_Click(object sender, RoutedEventArgs e)
        {
            
            ToggleAutoThemeButton.Content =  ToggleAutoThemeButton.Content == off ? on : off;
            
           if(ToggleAutoThemeButton.Content == FindResource("On"))
           {
              UninstallService();
           }
           else
           {
              InstallService();
           }
            Properties.Settings.Default.Save();
        }
        private void InstallService()
        {
            Process proc = null;
            try
            {
                string batDir = string.Format(@"Service");
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batDir;
                proc.StartInfo.FileName = "InstallService.bat";
                proc.StartInfo.CreateNoWindow = false;
                proc.Start();
                proc.WaitForExit();
                Console.WriteLine("Bat file executed !!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
            Properties.Settings.Default.ServiceInstalled = true;
        }

        private void UninstallService()
        {
            Properties.Settings.Default.ServiceInstalled = false;
        }

        /// <summary>
        /// Verify if a service exists
        /// </summary>
        /// <param name="ServiceName">Service name</param>
        /// <returns></returns>
        public bool ServiceExists(string ServiceName)
        {
            return ServiceController.GetServices().Any(serviceController => serviceController.ServiceName.Equals(ServiceName));
        }

    

    }
}
