using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenSourceBotNet.Core;
using OpenSourceBotNet.Core.Helpers;

namespace OpenSourceBotNet.Core
{
    public delegate void AbortExecution();

    public class Flux : FluxObject
    {
        public event AbortExecution abortExecution;

        private FluxReactor fluxReactor;
        private FluxCapacitor fluxCapacitor;

        private bool bRunning = false;

        public Flux()
        {

        }

        public override void Start(object obj)
        {
            bRunning = true;

            // Initialize Global Variables and Settings
            FluxGlobal.Initialize();

            // Instantiate Reactor and Spark it
            fluxReactor = new FluxReactor();
            fluxReactor.abortExecution += flux_abortExecution;
            fluxReactor.pluginsLoaded += fluxReactor_pluginsLoaded;
            fluxReactor.Spark();

            // Loop and Pause as to not release the thread
            while (bRunning == true)
            {
                System.Threading.Thread.Sleep(3000);
            }
        }

        void fluxReactor_pluginsLoaded()
        {
            FluxCapacitor.Initialize();

            fluxCapacitor = new FluxCapacitor();
            fluxCapacitor.abortExecution += flux_abortExecution;
            fluxCapacitor.Spark();
        }

        void flux_abortExecution()
        {
            Stop();
        }

        public void Stop()
        {
            bRunning = false;

            abortExecution();

            Abort();
        }
    }
}
