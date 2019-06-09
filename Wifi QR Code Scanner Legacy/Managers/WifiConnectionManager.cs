using NativeWifi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wifi_QR_Code_Scanner_Legacy.Business;

namespace Wifi_QR_Code_Scanner_Legacy.Managers
{
    class WifiConnectionManager
    {
        WlanClient client = new WlanClient();
        public void Connect(WifiAccessPointData wifiAccessPointData)
        {
            var wifiInterface = client.Interfaces.First();
            //Wlan.WlanAvailableNetwork[] networks = wifiInterface.GetAvailableNetworkList(0);
            //foreach (Wlan.WlanAvailableNetwork network in networks)
            //{
            //    if (network.dot11DefaultAuthAlgorithm == Wlan.Dot11AuthAlgorithm.WPA_PSK)
            //    {
            //        Console.WriteLine("Found WPA network with SSID {0}.", System.Text.Encoding.UTF8.GetString(network.dot11Ssid.SSID, 0, network.dot11Ssid.SSID.Length));
            //    }
            //}

            //// Retrieves XML configurations of existing profiles.
            //// This can assist you in constructing your own XML configuration
            //// (that is, it will give you an example to follow).
            //foreach (Wlan.WlanProfileInfo profileInfo in wifiInterface.GetProfiles())
            //{
            //    string name = profileInfo.profileName; // this is typically the network's SSID
            //    string xml = wifiInterface.GetProfileXml(profileInfo.profileName);
            //}

            // Connects to a known network with WPA security
            string profileName = wifiAccessPointData.ssid; // this is also the SSID
            //string mac = "52544131303235572D454137443638";
            string key = wifiAccessPointData.password;
            string profileXml = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>auto</connectionMode><MSM><security><authEncryption><authentication>WPA2PSK</authentication><encryption>AES</encryption><useOneX>false</useOneX></authEncryption><sharedKey><keyType>passPhrase</keyType><protected>false</protected><keyMaterial>{1}</keyMaterial></sharedKey></security></MSM></WLANProfile>", profileName, key);
            var result = wifiInterface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);
            wifiInterface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName);
        }
    }
}
