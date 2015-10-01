using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Diagnostics;

using OpenSourceBotNet.Core;
using OpenSourceBotNet.Services;

namespace OpenSourceBotNet.Services.Resources
{
    public class CPUThrottle : FluxService
    {    
        public static int iThrottleCPU = 10;

        private ArrayList alUsages = new ArrayList();

        private bool bInUse = false;

        public CPUThrottle()
        {
            pause = 1000;
        }

        public override void HeartBeat()
        {
            if(bInUse == true)
            {
                return;
            }

            bInUse = true;

            // Do if CPU > 75% for one minute...
            PerformanceCounter cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            // will always start at 0
            float firstValue = cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            // now matches task manager reading
            float secondValue = cpuCounter.NextValue();

            float cpuUsage = secondValue;

            if (alUsages.Count == 60)
            {
                alUsages.RemoveAt(0);
            }

            alUsages.Add(cpuUsage);

            float totalUsage = 0;

            foreach(float cpu in alUsages)
            {
                totalUsage += cpu;
            }

            float myUsage = totalUsage / 60;

            if(myUsage > 75)
            {
                iThrottleCPU += 10;
            }

            bInUse = false;
        }

        public static void ThrottleCPU()
        {
            System.Threading.Thread.Sleep(iThrottleCPU);
        }
    }
}
