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
using myLibrary;

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


        delegate void CV();

       
        private void AddText()
        {
            if (tbReceive.InvokeRequired)
            {
                CV cb = new CV(AddText);
                
                Invoke(cb);
            }
            else
            {
                tbReceive.Text+=mainMess;
            }

        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            
            if (threadServer == null)
            {
                threadServer = new Thread(ServerProcess);
                threadServer.Start();
            }
            else
            {
                threadServer.Resume();
            }

           
            timer1.Start();
        }

        private void ServerProcess()
        {
            
            while (true)
            {
                if (listener == null)
                {
                    listener = new TcpListener(int.Parse(tbPort.Text));
                    listener.Start();
                }
                if (listener.Pending())
                {
                    Socket sock = listener.AcceptSocket();
                    byte[] bArr = new byte[sock.Available];
                    int n = sock.Receive(bArr, 0, sock.Available, SocketFlags.None);
                    mainMess = Encoding.Default.GetString(bArr, 0, n);
                    AddText();
                   

                    IPEndPoint ep = (IPEndPoint)sock.RemoteEndPoint;
                    sbIPPORT.Text = $"{myLib.GetToken(sock.RemoteEndPoint.ToString(), ':', 0)}";


                }

            }

        }
        //void ReadProcess()
        //{
        //    if(tcp!=null)
        //    {
        //        NetworkStream ns = tcp.GetStream();
        //        byte[] bArr = 
        //    }        
        //}
        string mainMess;

        private void Chat_Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (threadServer != null){
                threadServer.Abort();
            }
            
            timer1.Stop();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            tbReceive.Text += mainMess;
            mainMess = "";
        }
    }
}
