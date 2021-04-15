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

namespace Chat_Server_using_Socket
{
    public partial class Chat_Server_using_Socket : Form
    {
        public Chat_Server_using_Socket()
        {
            InitializeComponent();
        }
        Socket sockServer = null;
        Socket socket = null;
        TcpListener listener = null;
        Thread threadServer = null;
        Thread threadRead = null;
        byte[] IP={192,168,35,94};
        int port_num=8080;
       // byte[] sAddress = { 0, 0, 0, 0 }; // {127,0,0,1 XXX} :서버에서 Bind하기 위한 포트 어드레스

        private void menu_Start_Click(object sender, EventArgs e)
        {
            // Initialize Socket
            if (sockServer == null)
            {
                sockServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                if (threadServer != null)
                    threadServer.Abort();
                if (threadRead != null)
                    threadRead.Abort();
                if (thsession != null)
                    thsession.Abort();
                
                sockServer.Close();
                sockServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            tbReceive.Text += $"Open Server {new IPAddress(IP)}::{port_num}\r\n";
            sbIPport.Text = port_num.ToString();
            sbStatus.Text = "Running Server";
            sbStatus.BackColor = Color.Blue;
            // Initialize Thread
            if (threadServer == null)
            {
                threadServer = new Thread(ServerProcess);
                threadServer.IsBackground = true;
                threadServer.Start();
            }

            if (threadRead == null)
            {
                threadRead = new Thread(ReadProcess);
                threadRead.IsBackground = true;
            }
            if (thsession == null)
            {
                thsession = new Thread(SessionProcess);
                thsession.IsBackground = true;
            }
        }
        bool Pending = false; // 외부로부터의 서버요청 수신.
        IAsyncResult ar;

        private void onAccept(IAsyncResult iar)
        {
            Pending = true;
            ar = iar;
        }
        private Socket accpetSocket()
        {
            Socket sock1 = sockServer.EndAccept(ar);
            sockServer.BeginAccept(new AsyncCallback(onAccept), null);
            Pending = false;
            return sock1;
        }

        Thread thsession = null;
        void ServerProcess()
        {
            IPAddress ip_p = new IPAddress(IP);
            IPEndPoint ep = new IPEndPoint(ip_p, port_num);
            sockServer.Bind(ep);
            sockServer.Listen(10);
            IAsyncResult result = sockServer.BeginAccept(new AsyncCallback(onAccept), null);


            while (true)
            {
                if (Pending)
                {
                    socket = accpetSocket();
                   // threadRead.Start();
                    if (threadRead != null)
                    {
                        threadRead.Abort(); threadRead = null;
                    }
                    threadRead = new Thread(ReadProcess);
                    threadRead.Start();

                }
                Thread.Sleep(100);
                //socket = sockServer.BeginAccept();   //Create channel(session)
                //threadRead.Start();
            }

        }

        delegate void CallBack(string str);
        void AddText(string str)
        {
            if (tbReceive.InvokeRequired)
            {
                CallBack cb = new CallBack(AddText);
                Invoke(cb, new object[]{ str });
            }
            else 
                tbReceive.Text += $"{str}\r\n";
        }

        void ReadProcess()
        {
            while (true)
            {
                if(socket!=null && socket.Available>0)
                {
                    byte[] receive = new byte[socket.Available];
                    socket.Receive(receive);
                    string timer = System.DateTime.Now.ToString("HH:mm:ss");
                    AddText($"[수신][{timer}] : {Encoding.Default.GetString(receive)}");
                }
            }

        }

        void SendText(string str)
        {
            try
            {

                string strt = tbSend.Text;
                string[] sArr = strt.Split('\r');
                string sLast = sArr.Last();
                socket.Send(Encoding.Default.GetBytes(sLast));

            }
            catch(NullReferenceException e1)
            {
                MessageBox.Show(e1.Message);
            }
            catch (Exception )
            {
                MessageBox.Show("Client 연결을 확인하세요.");
                sbStatus.Text = "Check connection";
                sbStatus.BackColor = Color.Red;
            }


        }
        // Send button in Pop-up menu 
        private void sendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendText(tbSend.SelectedText);

        }

        private void menu_Stop_Click(object sender, EventArgs e)
        {
            sockServer.Close();
            socket.Close();
        }

        private void menu_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendText(tbSend.Text);
                string timer = System.DateTime.Now.ToString("HH:mm:ss");
                AddText($"[발신][{timer}] : {tbSend.Text}");
                tbSend.Clear();
            }
        }

        private void Chat_Server_using_Socket_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void menu_Config_Click(object sender, EventArgs e)
        {
            PortSet ps = new PortSet(port_num.ToString());
            DialogResult result = ps.ShowDialog();
            if (result == DialogResult.OK)
            {
                string pp = ps.setPort();
                if (pp != "")
                {
                    try
                    {
                        int pn = int.Parse(pp);
                        port_num = pn;
                    }
                    catch(Exception e1)
                    {
                        MessageBox.Show(e1.ToString());

                    }
                }
            }
            ps.Close();
        }
        private byte[] GetIPBytes(string str)
        {
            string[] sa = str.Split('.');
            byte[] ba = new byte[4];
            if (sa.Length != 4) return null;
            for (int i = 0; i < 4; i++){
                ba[i] = byte.Parse(sa[i]);
            }
            return ba;
        }

        void SessionProcess()
        {
            while (true)
            {
                if( socket!=null&&socket.Connected)
                {
                    sbStatus.BackColor = Color.GreenYellow;
                }
                else
                {
                    sbStatus.BackColor = Color.PaleVioletRed;
                    socket = null;
                }
                Thread.Sleep(100);
            }

        }
    }
}
