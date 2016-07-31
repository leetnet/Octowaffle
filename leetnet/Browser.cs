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
            //webBrowser1.DocumentText = "";
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
                WebBrowser wbControl = tabControl1.SelectedTab.Controls.OfType<WebBrowser>().FirstOrDefault();
                wbControl.DocumentText = CommonMarkConverter.Convert(markdown.Remove(0, 1));
                tabControl1.SelectedTab.Text = textBox1.Text;
                tcpclnt.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }

        private void Browser_Load(object sender, EventArgs e)
        {
            TabPage addedTabPage = new TabPage("about:blank"); //create the new tab
            tabControl1.TabPages.Add(addedTabPage); //add the tab to the TabControl

            WebBrowser addedWebBrowser = new WebBrowser()
            {
                Parent = addedTabPage, //add the new webBrowser to the new tab
                Dock = DockStyle.Fill
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TabPage addedTabPage = new TabPage("about:blank"); //create the new tab
            tabControl1.TabPages.Add(addedTabPage); //add the tab to the TabControl

            WebBrowser addedWebBrowser = new WebBrowser()
            {
                Parent = addedTabPage, //add the new webBrowser to the new tab
                Dock = DockStyle.Fill
            };
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}