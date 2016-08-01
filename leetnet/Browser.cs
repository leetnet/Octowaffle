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
using CefSharp.WinForms;
using CefSharp;

namespace leetnet
{
    public partial class Browser : Form
    {
        

        public Browser()
        {
            Cef.Initialize(new CefSettings());
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
            this.Invoke(new Action(() =>
            {
                var wbControl = tabControl1.SelectedTab.Controls.OfType<ChromiumWebBrowser>().FirstOrDefault();
                wbControl.Tag = text;
            }));

        }

        public void SetMD(string md)
        {
            this.Invoke(new Action(() =>
            {
                var wbControl = tabControl1.SelectedTab.Controls.OfType<ChromiumWebBrowser>().FirstOrDefault();
                wbControl.Load("data:text/html,<style> * {font-family: Arial;}</style>" + CommonMark.CommonMarkConverter.Convert(md));
            }));
        }

        public void SendToServer(StatusCode statusCode, object obj)
        {
            clnt.Send(new NetObject(this.thisID + " " + ((int)statusCode).ToString(), obj));
            string thing = (string)obj;
            this.Invoke(new Action(() =>
            {
                thing.Replace("ltp://", "");
                //textBox1.Text = thing;
                tabControl1.SelectedTab.Text = thing;
            }));
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

            ChromiumWebBrowser addedWebBrowser = new ChromiumWebBrowser("about:blank")
            {
                Parent = addedTabPage, //add the new webBrowser to the new tab
                Dock = DockStyle.Fill,
                
            };
            SetMD(welcome_text);
            addedWebBrowser.TitleChanged += (o, a) =>
            {
                try
                {
                    this.Invoke(new Action(() =>
                    {
                        tabControl1.SelectedTab.Text = a.Title;
                    }));
                }
                catch
                {

                }
            };
            addedWebBrowser.AddressChanged += (o, a) =>
            {
                try
                {
                    this.Invoke(new Action(() =>
                    {
                        ChromiumWebBrowser wbControl = tabControl1.SelectedTab.Controls.OfType<ChromiumWebBrowser>().FirstOrDefault();
                        String thing = a.Address.ToString();
                        if (thing.StartsWith("ltp://"))
                        {
                            LoadPage(thing);
                        }
                    }));
                }
                catch
                {

                }
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TabPage addedTabPage = new TabPage("about:blank"); //create the new tab
            tabControl1.TabPages.Add(addedTabPage); //add the tab to the TabControl
            tabControl1.SelectTab(addedTabPage);
            ChromiumWebBrowser addedWebBrowser = new ChromiumWebBrowser("about:blank")
            {
                Parent = addedTabPage, //add the new webBrowser to the new tab
                Dock = DockStyle.Fill,
                
            };
            SetMD(welcome_text);
            addedWebBrowser.TitleChanged += (o, a) =>
            {
                this.Invoke(new Action(() =>
                {
                    tabControl1.SelectedTab.Text = a.Title;
                }));
            };
            addedWebBrowser.AddressChanged += (o, a) =>
            {
                try
                {
                    this.Invoke(new Action(() =>
                    {
                        ChromiumWebBrowser wbControl = tabControl1.SelectedTab.Controls.OfType<ChromiumWebBrowser>().FirstOrDefault();
                        String thing = a.Address.ToString();
                        if (thing.StartsWith("ltp://"))
                        {
                            LoadPage(thing);
                        }
                    }));
                }
                catch
                {

                }
            };
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                var wbControl = tabControl1.SelectedTab.Controls.OfType<ChromiumWebBrowser>().FirstOrDefault();
                LoadPage(wbControl.Tag as string);
            }));

        }

        private void letsgetout(object sender, EventArgs e)
        {
            if(clnt != null && clnt.IsConnected)
                clnt.Disconnect();
            Application.Exit();
        }

        public const string welcome_text = "Welcome to the Leetnet.";

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                goButton_Click(this, EventArgs.Empty);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                ChromiumWebBrowser wbControl = tabControl1.SelectedTab.Controls.OfType<ChromiumWebBrowser>().FirstOrDefault();
                wbControl.Back();
                wbControl.Back();
            }));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                ChromiumWebBrowser wbControl = tabControl1.SelectedTab.Controls.OfType<ChromiumWebBrowser>().FirstOrDefault();
                wbControl.Forward();
                wbControl.Forward();
            }));
        }
    }

    public enum StatusCode
    {
        DocumentRequest = 102,
    }
}