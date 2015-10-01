using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using OpenSourceBotNet.Core.Helpers;

namespace OpenSourceBotNet.Core
{
    public class FluxReactor : FluxObject
    {
        public event AbortExecution abortExecution;

        public delegate void PluginsLoaded();
        public event PluginsLoaded pluginsLoaded;

        public Hashtable htPlugins = new Hashtable();

        public System.Threading.Thread thread;

        public override void Start(object obj)
        {
            LoadPlugins();
        }

        public void Register(IPlugin iPlugin)
        {
            if (htPlugins.ContainsKey(iPlugin.Name) == false)
            {
                htPlugins.Add(iPlugin.Name, iPlugin);
            }
            else
            {
                iPlugin.Abort();
            }
        }

        public void LoadPlugins()
        {
            // Load Services
            LoadPluginsByNamespace("OpenSourceBotNet.Services");
        }

        public void LoadPluginsByNamespace(string sNamespace)
        {
            List<Type> typelist = GetTypesInNamespace(Assembly.GetExecutingAssembly(), sNamespace);
            for (int i = 0; i < typelist.Count; i++)
            {
                if (typelist[i].IsAbstract == false)
                {
                    IPlugin plugin = (IPlugin)Activator.CreateInstance(typelist[i]);
                    plugin.Spark();
                }
            }
        }

        private List<Type> GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            Type[] types = assembly.GetTypes();
            List<Type> myTypes = new List<Type>();
            foreach (Type t in types)
            {
                if (t.Namespace != null && t.Namespace.Contains(nameSpace) == true)
                    myTypes.Add(t);
            }

            return myTypes;
        }
    }
}
