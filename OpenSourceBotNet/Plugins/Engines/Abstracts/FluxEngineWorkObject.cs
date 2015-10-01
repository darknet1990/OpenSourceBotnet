using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenSourceBotNet;
using OpenSourceBotNet.Core;
using OpenSourceBotNet.Core.Helpers;
using OpenSourceBotNet.Services;

namespace OpenSourceBotNet.Plugins.Engines
{
    public class FluxEngineWorkObject
    {
        public string sEngineName;
        public string sPluginName;
        public string sTaskUUID;
        public string sPluginWork;
        public int iThreadID;
    }
}