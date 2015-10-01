using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using OpenSourceBotNet.Services;

namespace OpenSourceBotNet.Plugins.Services.ExeMonitors 
{
    public class WinPcap : ExeMonitor
    {        
        public static bool bWinPcap = false;

        public WinPcap()
        {
            sServiceProcessName = "rpcapd.exe";
            sServiceFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\WinPcap\\" + sServiceProcessName;
        }

        public override void serviceProcess_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            try
            {
                Console.WriteLine("[ERROR] " + e.Data.ToString());
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.ToString()); 
            }
        }


        public override void serviceProcess_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            try
            {
                Console.WriteLine(e.Data.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public override bool Monitor()
        {
            bool bFoundService = false;

            System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcesses();

            foreach(Process p in procs)
            {
                if(p.ProcessName == sServiceProcessName)
                {
                    bFoundService = true;
                    break;
                }
            }

            return bFoundService;
        }

        public override bool HealthCheck()
        {
            bool bServiceHealthy = true;

            // Test WinPcap Is Running Properly

            return bServiceHealthy;
        }
    }
}
