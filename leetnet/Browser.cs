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
        bool connecting = false;


        public void LoadPage(string text)
        {
            text = text.Replace("ltp://", "");
            var textsplit = text.Split('/');
            string ip_addr = textsplit[0];
            string fpath = text.Replace(ip_addr, "");
            if (clnt == null || clnt.RemoteHost != ip_addr)
            {
                ConnectToClient(ip_addr, fpath);
            }
            else
            {
                SendToServer(StatusCode.DocumentRequest, fpath);
            }
        }

        public void SetMD(string md)
        {
            this.Invoke(new Action(() =>
            {
                WebBrowser wbControl = tabControl1.SelectedTab.Controls.OfType<WebBrowser>().FirstOrDefault();
                wbControl.DocumentText = "<style> * {font-family: Arial;}</style>" + CommonMark.CommonMarkConverter.Convert(md);
                tabControl1.SelectedTab.Text = textBox1.Text;
            }));
        }

        public void SendToServer(StatusCode statusCode, object obj)
        {
            clnt.Send(new NetObject(this.thisID + " " + ((int)statusCode).ToString(), obj));
            string thing = (string)obj;
            thing.Replace("ltp://", "");
            //textBox1.Text = thing;
            tabControl1.SelectedTab.Text = thing;
        }

        public void ConnectToClient(string ip, string fpath)
        {
            try
            {
                connecting = true;
                clnt = new NetObjectClient();
                clnt.OnConnected += (o, a) =>
                {
                    connecting = false;
                };
            OnIDReceived = () =>
            {
                SendToServer(StatusCode.DocumentRequest, fpath);
            };
                clnt.OnDisconnected += (o, a) =>
                {
                    SetMD(@"Disconnected abrubtly.

You've been abrubtly disconnected from the server for no apparent reason.");
                };
                clnt.OnReceived += new NetReceivedEventHandler<NetObject>(this.OnReceived);
                clnt.Connect(ip, 13370);
           }
            catch (Exception ex)
            {
                SetMD($@"# Connection failure.

Could not connect to {ip}.

**Error**: {ex.Message}");
            }
       }

        Action OnIDReceived;

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
                    case 400:
                        thisID = Convert.ToInt32(e.Data.Object as string);
                        OnIDReceived?.Invoke();
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
                Dock = DockStyle.Fill,
                DocumentText = welcome_text
                
            };
            addedWebBrowser.Navigated += (o, a) =>
            {
                WebBrowser wbControl = tabControl1.SelectedTab.Controls.OfType<WebBrowser>().FirstOrDefault();
                String thing = wbControl.Url.ToString();
                if (thing.StartsWith("ltp://"))
                {
                    LoadPage(thing);
                }
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TabPage addedTabPage = new TabPage("about:blank"); //create the new tab
            tabControl1.TabPages.Add(addedTabPage); //add the tab to the TabControl

            WebBrowser addedWebBrowser = new WebBrowser()
            {
                Parent = addedTabPage, //add the new webBrowser to the new tab
                Dock = DockStyle.Fill,
                DocumentText = welcome_text
            };
            addedWebBrowser.Navigated += (o, a) =>
            {
                WebBrowser wbControl = tabControl1.SelectedTab.Controls.OfType<WebBrowser>().FirstOrDefault();
                String thing = wbControl.Url.ToString();
                if (thing.StartsWith("ltp://"))
                {
                    LoadPage(thing);
                }
            };
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void letsgetout(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public const string welcome_text = @"Welcome to the Leetnet.";
    }

    public enum StatusCode
    {
        DocumentRequest = 102,
    }
}