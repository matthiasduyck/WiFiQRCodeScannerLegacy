using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wifi_QR_Code_Scanner_Legacy.Business
{
    public class WifiAccessPointData
    {
        public string ssid { get; set; }
        public string password { get; set; }

        public WifiAccessPointSecurity wifiAccessPointSecurity { get; set; }

        public override string ToString()
        {
            return Properties.Resources.SSIDLabel+": " + ssid + System.Environment.NewLine + 
                Properties.Resources.PasswordLabel + ": " + password + System.Environment.NewLine +
                Properties.Resources.AuthenticationLabel + ": " + this.wifiAccessPointSecurity;
        }
    }
    public enum WifiAccessPointSecurity
    {
        WEP,
        WPA,
        nopass
    }
}
