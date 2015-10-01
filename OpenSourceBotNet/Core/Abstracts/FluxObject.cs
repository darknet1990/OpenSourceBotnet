using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSourceBotNet.Core
{
    public abstract class FluxObject
    {
        private System.Threading.Thread thread;

        public void Spark()
        {
            thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Start));
            thread.Start();
        }

        public void Abort()
        {
            try
            {
                thread.Abort();
            }
            catch(Exception ex) 
            {

            }
        }

        public abstract void Start(object obj);
    }
}
