using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Wifi_QR_Code_Scanner_Legacy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        App()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture; // new System.Globalization.CultureInfo("fr-FR");

        }
    }
}
