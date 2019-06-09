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
        public override string ToString()
        {
            return "Network Name (SSID): " + ssid + System.Environment.NewLine + "Password: " + password;
        }
    }
}
