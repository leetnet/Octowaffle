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
                byte[] bb = new byte[100];
                int k = stm.Read(bb, 0, 100);
                
                switch (Convert.ToChar(bb[0]).ToString())
                {
                    case "100":
                        // 100 OKAY
                        for (int i = 1; i < k; i++)
                            dank += Convert.ToChar(bb[i]).ToString();
                        webBrowser1.DocumentText = dank;

                        tcpclnt.Close();
                        break;
                    case "101":
                        // 101 REDIRECT
                        break;
                    case "102":
                        // 102 BROWSER SPECIFIC
                        break;
                    case "200":
                        // 200 FILE NOT FOUND
                        webBrowser1.DocumentText = "<H1>ERROR 200</H1><P>File Not Found</P>";
                        break;
                    case "201":
                        // 201 GONE
                        webBrowser1.DocumentText = "<H1>ERROR 201</H1><P>Gone</P>";
                        break;
                    case "202":
                        // 202 GENERIC
                        webBrowser1.DocumentText = "<H1>ERROR 202</H1><P>Generic<P>";
                        break;
                    case "300":
                        // 300 SERVER DOWN
                        webBrowser1.DocumentText = "<H1>ERROR 300</H1><P>Server Down</P>";
                        break;
                    case "301":
                        // 301 SERVER ERROR
                        webBrowser1.DocumentText = "<H1>ERROR 301</H1><P>Server Error</P>";
                        break;
                    default:
                        // 100 OKAY
                        for (int i = 1; i < k; i++)
                            dank += Convert.ToChar(bb[i]).ToString();
                        webBrowser1.DocumentText = dank;

                        tcpclnt.Close();
                        break;
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("FUCKING BUBBLES: " + e.StackTrace);
            }
        }
    }
}