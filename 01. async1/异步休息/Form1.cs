using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 异步休息
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            using(HttpClient httpClient = new HttpClient())
            {
                string s1 = await httpClient.GetStringAsync("https://www.youzak.com");
                textBox1.Text = s1.Substring(0, 20);
                //Thread.Sleep(3000);
                await Task.Delay(3000);
                string s2 = await httpClient.GetStringAsync("https://www.baidu.com");
                textBox1.Text = s2.Substring(0, 20);
            }
        }
    }
}
