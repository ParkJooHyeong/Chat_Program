using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Clibrary;


namespace Chat_Server
{
    public partial class Chat_Server : Form
    {
        public Chat_Server()
        {
            InitializeComponent();
        }
        TcpListener listener = null;
        Thread threadServer = null;
        Thread threadRead = null;
        TcpClient tcp = null;
        string mainMess;

        delegate void CV(string str);

       
        private void AddText(string str)
        {
            if (tbReceive.InvokeRequired)
            {
                CV cb = new CV(AddText);
                
                Invoke(cb, new object[] { str});
            }
            else
            {
                tbReceive.Text+=str+"\r\n";
            }

        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (listener == null)
            {
                listener = new TcpListener(int.Parse(tbPort.Text));
                listener.Start();
            }

            if (threadServer == null)
            {
                threadServer = new Thread(ServerProcess);
                threadServer.Start();
                tbConnect.Text = "Connect Client";
                tbConnect.BackColor = Color.Blue;
                threadRead = new Thread(Readprocess);
            }

            timer1.Start();
        }
        private void ServerProcess()
        {
            
            while (true)
            {
                if (listener.Pending())
                {

                    tcp = listener.AcceptTcpClient();
                    IPEndPoint ep = (IPEndPoint)tcp.Client.RemoteEndPoint;
                    sbIPPORT.Text = $"{myLib.GetToken(ep.ToString(), ':', 0)}";

                    threadRead.Start();
                   
                }
                Thread.Sleep(100);

            }

        }
        void Readprocess()
        {
            if (tcp != null)
            {
                NetworkStream ns = tcp.GetStream();
                int n;
                byte[] barr = new byte[1024];
                while (true)
                {
                    while (ns.DataAvailable)
                    {
                        while((n = ns.Read(barr, 0, barr.Length)) > 0)
                        {
                            string timer = System.DateTime.Now.ToString("HH:mm:ss");
                            AddText($"[{timer}] {Encoding.Default.GetString(barr, 0, n)}");
                        }

                    }
                }
            }
        }

        private void Chat_Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (threadServer != null){
                threadServer.Abort();
                threadRead.Abort();
            }
            
            timer1.Stop();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            tbReceive.Text += mainMess;
            mainMess = "";
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (tcp != null)
            {
                NetworkStream ns = tcp.GetStream();
                if (tbSend.Text != "")
                {
                    string str = tbSend.Text;
                    string[] sArr = str.Split('\r');
                    string sLast = sArr.Last();
                    byte[] bArr = Encoding.Default.GetBytes(sLast);
                    ns.Write(bArr, 0, bArr.Length);
                }


            }
        }

        

        private void tbSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend_Click(sender, e);
            }

        }
    }
}
