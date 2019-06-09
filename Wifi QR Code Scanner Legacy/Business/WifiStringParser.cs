using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wifi_QR_Code_Scanner_Legacy.Business
{
    public class WifiStringParser
    {
        public static WifiAccessPointData parseWifiString(string wifiString)
        {
            var result = new WifiAccessPointData();
            try
            {
                var splitWifiString = wifiString.Split(';');
                result.ssid = splitWifiString[0].Split(':')[2];
                result.password = splitWifiString[2].Split(':')[1];
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }
    }
}
