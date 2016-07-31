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
using NetSockets;

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

        NetObjectClient clnt = null;
        int thisID = 0;

        public void LoadPage(string text)
        {
            text = text.Replace("ltp://", "");
            var textsplit = text.Split('/');
            string ip_addr = textsplit[0];
            string fpath = text.Replace(ip_addr, "");
            if(clnt == null || clnt.RemoteHost != ip_addr)
            {
                ConnectToClient(ip_addr);
            }
            else
            {
                SendToServer(StatusCode.DocumentRequest, fpath);
            }

        }

        public void SetMD(string md)
        {
            WebBrowser wbControl = tabControl1.SelectedTab.Controls.OfType<WebBrowser>().FirstOrDefault();
            wbControl.DocumentText = CommonMark.CommonMarkConverter.Convert(md);
        }

        public void SendToServer(StatusCode statusCode, object obj)
        {
            clnt.Send(new NetObject(this.thisID + " " + ((int)statusCode).ToString(), obj));
        }

        public void ConnectToClient(string ip)
        {
            clnt = new NetObjectClient();
            clnt.OnReceived += new NetReceivedEventHandler<NetObject>(this.OnReceived);
            clnt.Connect(ip, 13370);
        }

        private void OnReceived(object sender, NetReceivedEventArgs<NetObject> e)
        {
            try
            {
                int dataheader = Convert.ToInt32(e.Data.Name);
                switch(dataheader)
                {
                    case 100:
                        SetMD(e.Data.Object as string);
                        break;
                    case 201:
                        SetMD(@"# LTP 201: Not found.

The file you requested is not found on the server.");
                        break;        
                }
            }
            catch (Exception ex)
            {
                SetMD($@"# Client error

An error has occurred in Octowaffle and page loading has been halted.

***Error information***:

 - {ex.Message}");
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

    public enum StatusCode
    {
        DocumentRequest = 102,
    }
}