using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenSourceBotNet;
using OpenSourceBotNet.Core;
using OpenSourceBotNet.Core.Helpers;
using OpenSourceBotNet.Services;

namespace OpenSourceBotNet.Plugins.Engines
{
    public abstract class FluxEnginePlugin
    {
        public event IPluginResponseTime _fireResponseTime;

        public event IPluginEvent _fireSuccess;
        public event IPluginEvent _fireFailed;
        public event IPluginEvent _fireSaveWork;

        public object oParentEngine;
    }
}
