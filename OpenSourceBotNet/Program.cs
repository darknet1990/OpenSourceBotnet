using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;

using OpenSourceBotNet.Core;

namespace OpenSourceBotNet
{
    static class Program
    {
        private static bool bRunning = true;
        static void Main(string[] args)
        {
            App();

            if (mutex != null)
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

                Flux flux = new Flux();
                flux.abortExecution += flux_abortExecution;
                flux.Spark();

                while (bRunning == true)
                {
                    System.Threading.Thread.Sleep(3000);
                }
            }
        }

        static Mutex mutex;
        static void App()
        {
            mutex = new Mutex(false, "SINGLE_INSTANCE_MUTEX");
            if (!mutex.WaitOne(0, false))
            {
                mutex.Close();
                mutex = null;
            }
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }

        static void flux_abortExecution()
        {
            bRunning = false;
        }
    }
}
