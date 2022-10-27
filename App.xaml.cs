using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Risklib;
namespace Risk
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public List<string> avatars = new List<string>();

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            PreWindow pw = new PreWindow();
            pw.Show();
        }

    

    }
}
