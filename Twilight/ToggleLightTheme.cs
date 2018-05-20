using Microsoft.Win32;
using Quartz;

namespace Twilight
{
    class ToggleLightTheme : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", true);
            
            //getting the value
            var data = key.GetValue("AppsUseLightTheme").ToString(); 

            
            //adding/editing a value 
            key.SetValue("AppsUseLightTheme", 1); 
            key.Close();
        }
    }
}
