using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Wifi_QR_Code_Scanner_Legacy.Business
{
    public class WifiStringParser
    {
        public static WifiAccessPointData parseWifiString(string wifiString)
        {
            var result = new WifiAccessPointData();
            //expected format:
            //WIFI:S:<SSID>;T:<WPA|WEP|>;P:<password>;;
            // regex for ssid:   S:(.*?)((?<!\\);)   might still need to strip quotes if ascii? unescape special chars from \    \, ;, , and :
            // regex for pass:   P:(.*?)((?<!\\);)   
            // regex for security:   T:(.*?)((?<!\\);)   //can be WEP WPA or nopass, might want to check for WPA2 just in case
            // regex for hidden:   H:(.*?)((?<!\\);)
            try
            {
                Regex ssidRegex = new Regex(@"S:(.*?)((?<!\\);)");
                Match ssidMatch = ssidRegex.Match(wifiString);
                if (!ssidMatch.Success)
                {
                    throw new Exception("No ssid found in QR code.");
                }
                result.ssid = unescapeSpecialChars(ssidMatch.Value);
                result.ssid = result.ssid.Substring(2);
                result.ssid = result.ssid.Substring(0, result.ssid.Length - 1);

                Regex passwordRegex = new Regex(@"P:(.*?)((?<!\\);)");
                Match passwordMatch = passwordRegex.Match(wifiString);
                if (passwordMatch.Success)
                {
                    result.password = unescapeSpecialChars(passwordMatch.Value);
                    result.password = result.password.Substring(2);
                    result.password = result.password.Substring(0, result.password.Length - 1);
                }

                Regex securityRegex = new Regex(@"T:(.*?)((?<!\\);)");
                Match securityMatch = securityRegex.Match(wifiString);
                if (securityMatch.Success)
                {
                    if (securityMatch.Value.Contains("WEP"))
                        result.wifiAccessPointSecurity = WifiAccessPointSecurity.WEP;

                    if (securityMatch.Value.Contains("WPA"))
                        result.wifiAccessPointSecurity = WifiAccessPointSecurity.WPA;

                    if (securityMatch.Value.Contains("nopass"))
                        result.wifiAccessPointSecurity = WifiAccessPointSecurity.nopass;
                }
                else
                {
                    result.wifiAccessPointSecurity = WifiAccessPointSecurity.nopass;
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string unescapeSpecialChars(string input)
        {
            var result = input;
            result = result.Replace(@"\\", @"\");
            result = result.Replace(@"\;", @";");
            result = result.Replace(@"\,", @",");
            result = result.Replace(@"\:", @":");
            result = result.Replace(@"\""", @"""");
            return result;
        }
    }
}
