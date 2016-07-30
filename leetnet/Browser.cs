using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace leetnet
{
    public partial class Browser : Form
    {
        public Browser()
        {
            InitializeComponent();
            webBrowser1.DocumentText = "Loading...";
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            LoadPage(textBox1.Text);
        }

        public void LoadPage(string text)
        {
            try
            {
                ASCIIEncoding asen = new ASCIIEncoding();
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");

                string ip = text.Split('/')[0];
                string dank = "";
                tcpclnt.Connect(ip, 13370);
                Console.WriteLine("Connected");
                Stream stm = tcpclnt.GetStream();
                string header = $"[page={text};version=1.0;]";
                stm.Write(asen.GetBytes(header), 0, header.Length);
                

                byte[] bb = new byte[100];
                int k = stm.Read(bb, 0, 100);

                for (int i = 0; i < k; i++)
                     dank += Convert.ToChar(bb[i]).ToString();
                webBrowser1.DocumentText = dank;

                tcpclnt.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("FUCKING BUBBLES: " + e.StackTrace);
            }
        }
    }
}