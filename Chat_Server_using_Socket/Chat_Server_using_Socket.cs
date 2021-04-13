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
                
                sockServer.Close();
                sockServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            sbIPport.Text = IP+port_num.ToString();
            sbStatus.Text = "Running Server";
            sbStatus.BackColor = Color.Blue;
            // Initialize Thread
            if (threadServer == null)
            {
                threadServer = new Thread(ServerProcess);
                threadServer.Start();
            }

            if (threadRead == null)
                threadRead = new Thread(ReadProcess);



        }
        void ServerProcess()
        {
            IPAddress ip_p = new IPAddress(IP);
            IPEndPoint ep = new IPEndPoint(ip_p, port_num);
            sockServer.Bind(ep);
            sockServer.Listen(1024 * 50);
            //listener.AcceptTcpClient();
            while (true)
            {
                socket = sockServer.Accept();
                threadRead.Start();
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
                tbReceive.Text += $"[{DateTime.Now.ToString("mm:ss")}] : {str}\r\n";
        }

        void ReadProcess()
        {
            while (true)
            {
                if(socket!=null && socket.Available>0)
                {
                    byte[] receive = new byte[socket.Available];
                    socket.Receive(receive);
                    AddText(Encoding.Default.GetString(receive));
                }
            }

        }

        void SendText(string str)
        {
            socket.Send(Encoding.Default.GetBytes(str));
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
    }
}
