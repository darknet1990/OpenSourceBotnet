using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSourceBotNet.Core
{
    public delegate void IPluginEvent(int iThreadID, string sTaskUUID, string results);
    public delegate void IPluginResponseTime(int iThreadID, double iLastResponseTime, string host);

    public interface IPlugin
    {
        IPluginHost iPluginHost { get; set; }
        string Name { get; set; }

        void Spark();

        void Abort();
    }

    public interface IPluginHost
    {
        void Register(IPlugin iPlugin);
    }

    public abstract class FluxPlugin : FluxObject,IPlugin
    {
        private string sQueueItem;
        private string name;

        private IPluginHost iPluginHostVar;

        public IPluginHost iPluginHost
        {
            get
            {
                return iPluginHostVar;
            }
            set
            {
                iPluginHostVar = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
    }
}
