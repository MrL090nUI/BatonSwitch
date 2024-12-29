using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace run
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        Random r = new Random();
        int numimage = 0;
        private void Form2_Load(object sender, EventArgs e)
        {
            if (data.topmost)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
            this.Text = r.Next(111111111, 999999999).ToString();
            numimage = r.Next(1, 9);
            if (numimage == 1)
            {
                this.BackgroundImage = Properties.Resources.image1;
            }
            else if (numimage == 2)
            {
                this.BackgroundImage = Properties.Resources.image2;
            }
            else if (numimage == 3)
            {
                this.BackgroundImage = Properties.Resources.image3;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.Location = new Point(r.Next(1, data.x), r.Next(1, data.y));
            if (data.topmost)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
            try
            {
                Process[] notice = Process.GetProcessesByName("notice");
                if (notice.Length > 0)
                {
                    foreach (Process process in notice)
                    {
                        if (process.MainModule.FileVersionInfo.FileDescription == "Уведомления о нарушении политик ВАО MrL090nUI")
                        {
                            data.topmost = false;
                        }
                    }
                }
            }
            catch { }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            
        }
    }
}
