using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

using OpenSourceBotNet;
using OpenSourceBotNet.Core;
using OpenSourceBotNet.Core.Helpers;
using OpenSourceBotNet.Plugins.Engines;
using OpenSourceBotNet.Services;

namespace OpenSourceBotNet.Core
{
    public abstract class FluxEngine : FluxPlugin
    {
        public event IPluginResponseTime _fireResponseTime;

        public event IPluginEvent _fireSuccess;
        public event IPluginEvent _fireFailed;
        public event IPluginEvent _fireSaveWork;

        public string sGetWorkPath;

        public bool bRunningPlugin = false;

        public FluxEngineWorkObject ewObject;

        public FluxEnginePlugin pluginClassObject;

        public System.Threading.Thread threadIgnition;

        public FluxEngine(FluxEngineWorkObject ewobject)
        {
            ewObject = ewobject;
        }

        public override void Start(object obj)
        {
            Type tThisPluginType = Type.GetType(ewObject.sPluginName);

            Assembly thisAssembly = Assembly.GetExecutingAssembly();

            foreach (Type type in thisAssembly.GetTypes())
            {
                if (type.IsClass == true && type == tThisPluginType)
                {
                    pluginClassObject = (FluxEnginePlugin)Activator.CreateInstance(type, new object[] { ewObject, this });
                    pluginClassObject._fireFailed += new IPluginEvent(this.fireFailed);
                    pluginClassObject._fireSuccess += new IPluginEvent(this.fireSuccess);
                    pluginClassObject._fireSaveWork += new IPluginEvent(this.fireSaveWork);
                    pluginClassObject._fireResponseTime += new IPluginResponseTime(fireResponseTime);
                    pluginClassObject.oParentEngine = this;

                    break;
                }
            }

            threadIgnition = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Ignition));
            threadIgnition.Start();
        }

        public void Stop()
        {
            try
            {
                threadIgnition.Abort();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Abort();
        }

        public void fireSuccess(int iThreadID, string sTaskUUID, string results)
        {
            _fireSuccess(iThreadID, sTaskUUID, results);
        }

        public void fireSaveWork(int iThreadID, string sTaskUUID, string results)
        {
            _fireSaveWork(iThreadID, sTaskUUID, results);
        }

        public void fireFailed(int iThreadID, string sTaskUUID, string results)
        {
            _fireFailed(iThreadID, sTaskUUID, results);
        }

        public void fireResponseTime(int iThreadID, double iLastResponseTime, string host)
        {
            _fireResponseTime(iThreadID, iLastResponseTime, host);
        }

        public abstract void Ignition(object obj);
    }
}
