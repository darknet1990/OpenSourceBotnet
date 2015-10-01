using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSourceBotNet.Core
{
    public abstract class FluxService : FluxPlugin
    {
        private System.Threading.Timer tmrServiceHeartbeat;

        public int delay = 30000;
        public int pause = 60000;

        public bool bHeartBeatBusy = false;

        public override void Start(object obj)
        {
            tmrServiceHeartbeat = new System.Threading.Timer(new System.Threading.TimerCallback(heartBeat), null, delay, pause);
        }

        public void heartBeat(object obj)
        {
            if (bHeartBeatBusy == true)
            {
                return;
            }

            bHeartBeatBusy = true;

            HeartBeat();

            bHeartBeatBusy = false;
        }

        public abstract void HeartBeat();
    }
}
