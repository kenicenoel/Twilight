using System.Threading.Tasks;
using Microsoft.Win32;
using Quartz;
using Quartz.Impl;

namespace Twilight
{
    public class ToggleDarkTheme: IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", true);
            
            //getting the value
            var data = key.GetValue("AppsUseLightTheme").ToString();

            //adding/editing a value 
            key.SetValue("AppsUseLightTheme", 0);
            key.Close();
        }

       
    }

    

}
