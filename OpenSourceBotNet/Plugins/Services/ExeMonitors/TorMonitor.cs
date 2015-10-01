using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using OpenSourceBotNet;
using OpenSourceBotNet.Core;
using OpenSourceBotNet.Core.Helpers;
using OpenSourceBotNet.Services;

namespace OpenSourceBotNet.Services.ExeMonitors
{
    public class TorMonitor : ExeMonitor
    {
        public static bool bTorIsRunning = false;

        public TorMonitor()
        {
            sServiceProcessName = "tor.exe";
            sServiceFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\tor\\" + sServiceProcessName;
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

            // Use Socket to test tor connection

            return bServiceHealthy;
        }
    }
}
