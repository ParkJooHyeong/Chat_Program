using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Clibrary;

namespace ChattingProgram
{
    public partial class Chat_Client : Form
    {
        public Chat_Client()
        {
            InitializeComponent();
        }

        delegate void cbAddText(string str);
        private void AddText(string str)
        {
            if (tbReceive.InvokeRequired)
            {
                cbAddText cb = new cbAddText(AddText);
                object[] oArr = { str };
                Invoke(cb);
            }
            else
            {
                tbReceive.AppendText(str);
            }
            
        }
        iniFile ini = new iniFile(".\\ChatClient.ini");
        private void Chat_Client_Load(object sender, EventArgs e)
        {
           tbIP.Text=ini.GetString("Server", "IP", "192.168.35.94");
           tbPort.Text=ini.GetString("Server", "PORT", "8080");
            int x1, x2, y1, y2;
            x1 = int.Parse(ini.GetString("Form", "LocationX", "0"));
            y1 = int.Parse(ini.GetString("Form", "LocationY", "0"));
            this.Location = new Point(x1, y1);
            x2 = int.Parse(ini.GetString("Form", "SizeX", "0"));
            y2 = int.Parse(ini.GetString("Form", "SizeY", "0"));
            this.Size = new Size(x2, y2);
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            int size1 = 200;
           // splitContainer1.SplitterDistance = splitContainer1.Size.Width - size1;

        }

        Socket sock;
        Thread threadClient;

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (sock == null)
            {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            }
            try
            {
                sock.Connect(tbIP.Text, int.Parse(tbPort.Text));
                sbIPPORT.Text = $"{sock.RemoteEndPoint.ToString()}";
                tbExcep.Text = "Connect Success";
                tbExcep.BackColor = Color.Blue;
            }
            catch(Exception err)
            {
                tbExcep.Text = "Connect Fail";
                tbExcep.BackColor = Color.Red;
            }
            
            
            if (threadClient == null)
            {
                threadClient = new Thread(ClientProcess);
                threadClient.Start();
            }
        }

        private void btSend_Click(object sender, EventArgs e)
        {

            string str = tbSend.Text;
            string[] sArr = str.Split('\r');
            string sLast = sArr.Last();
            sock.Send(Encoding.Default.GetBytes(sLast));
        }
        
        private void ClientProcess()
        {
            while (true)
            {
                int n = sock.Available;
                if (n > 0)
                {
                    byte[] bArr = new byte[n];
                    sock.Receive(bArr);
                    AddText(Encoding.Default.GetString(bArr));
                }

            }


        }

        private void Chat_Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            ini.WriteString("Server", "IP", tbIP.Text);
            ini.WriteString("Server", "PORT", tbPort.Text);
            ini.WriteString("Form", "LocationX", $"{Location.X}");
            ini.WriteString("Form", "LocationY", $"{Location.Y}");
            ini.WriteString("Form", "SizeX", $"{Size.Width}");
            ini.WriteString("Form", "SizeY", $"{Size.Height}");
        }
    }
}
