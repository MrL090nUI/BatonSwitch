using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace run
{
    public partial class popup : Form
    {
        public popup()
        {
            InitializeComponent();
        }

        private void popup_Load(object sender, EventArgs e)
        {
            try
            {
                Random r = new Random();
                this.Location = new Point(r.Next(1, data.x), r.Next(1, data.y));
                int id = r.Next(0, 10);
                string[] texts = { "Ерторик милашка?", "MrL090nUI дебил?", "Вирусчек гей?", "Клишин лох?", $"{Environment.UserName} лох сел на лавочку и сдох", "Свинка пепа никитос у тебя сейчас понос", "Нука сасик ты не плач я тебе в очко засуну мяч!", "Ещё используешь винду? Переходи на ебунту", "Его нельзя смотреть из-за жалобы на нарушение авторских прав от пользователя VirusCheck." };
                label1.Text = texts[id];
            }
            catch
            {
                try
                {
                    this.Hide();
                }
                catch { }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
            }
            catch { }
        }
    }
}
