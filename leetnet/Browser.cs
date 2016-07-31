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
using CommonMark;

namespace leetnet
{
    public partial class Browser : Form
    {
        public Browser()
        {
            InitializeComponent();
            webBrowser1.DocumentText = "";
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
                Console.WriteLine("Connecting...");

                string ip = text.Split('/')[0];
                string markdown = "";
                tcpclnt.Connect(ip, 13370);
                Console.WriteLine("Connected");
                Stream stm = tcpclnt.GetStream();
                for (int b = 0; b != 4; b = stm.ReadByte())
                {
                    char s = (char)b;
                    markdown += s;
                    Console.Write(s);
                }
                webBrowser1.DocumentText = CommonMarkConverter.Convert(markdown.Remove(0, 1));
                tcpclnt.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }
    }
}