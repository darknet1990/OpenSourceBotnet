using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenSourceBotNet;
using OpenSourceBotNet.Core;
using OpenSourceBotNet.Core.Helpers;

namespace OpenSourceBotNet.Services
{
    public abstract class ExeMonitor : FluxService
    {        
        public System.IO.StreamWriter swInput;

        public string sServiceFilePath;
        public string sServiceProcessName;

        public System.Diagnostics.Process serviceProcess;

        public bool bExecutableFailure = false;

        public bool bExeIsRunning = false;

        public ExeMonitor()
        {

        }

        // Start the process
        public void StartProcess()
        {
            if (System.IO.File.Exists(sServiceFilePath) == false)
            {
                bExecutableFailure = true;
                return;
            }

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(sServiceFilePath);
            psi.UseShellExecute = false;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;

            serviceProcess = new System.Diagnostics.Process();
            serviceProcess.StartInfo = psi;
            serviceProcess.Start();
            serviceProcess.OutputDataReceived += serviceProcess_OutputDataReceived;
            serviceProcess.ErrorDataReceived += serviceProcess_ErrorDataReceived;
            serviceProcess.BeginErrorReadLine();
            serviceProcess.BeginOutputReadLine();

            swInput = serviceProcess.StandardInput;
        }

        public void WriteInput(string input)
        {
            swInput.WriteLine(input);
        }

        public abstract void serviceProcess_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e);
        public abstract void serviceProcess_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e);

        // Kill the process
        public void Stop()
        {
            try
            {
                serviceProcess.Kill();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                serviceProcess.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public override void HeartBeat()
        {
            Checks();
        }

        // Monitor the process
        public abstract bool Monitor();

        // Service health check
        public abstract bool HealthCheck();

        // Restart the service
        public void Restart()
        {
            try
            {
                Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                StartProcess();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Checks()
        {
            try
            {
                monitor();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                healthCheck();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void monitor()
        {
            if (bExecutableFailure == true)
            {
                return;
            }

            bExeIsRunning = Monitor();

            if(bExeIsRunning == false)
            {
                Restart();
            }
        }

        public void healthCheck()
        {
            if (bExecutableFailure == true)
            {
                return;
            }

            bExeIsRunning = HealthCheck();

            if (bExeIsRunning == false)
            {
                Restart();
            }
        }
    }
}

