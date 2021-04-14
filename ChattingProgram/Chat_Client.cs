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

        iniFile ini = new iniFile(".\\ChatClient.ini");
        Socket sock;
        Thread threadClient;
        string IP;
        string Port;
        int x1, x2, y1, y2;        

        delegate void cbAddText(string str);
        private void AddText(string str)
        {
            if (tbReceive.InvokeRequired)
            {
                cbAddText cb = new cbAddText(AddText);
                object[] oArr = { str };
                Invoke(cb, oArr);
            }
            else
            {
                tbReceive.Text += str + "\r\n";
            }
            
        }

        private void Chat_Client_Load(object sender, EventArgs e)
        {
           IP=ini.GetString("Server", "IP", "192.168.35.94");
           Port=ini.GetString("Server", "PORT", "8080");
            x1 = int.Parse(ini.GetString("Form", "LocationX", "0"));
            y1 = int.Parse(ini.GetString("Form", "LocationY", "0"));
            this.Location = new Point(x1, y1);
            x2 = int.Parse(ini.GetString("Form", "SizeX", "0"));
            y2 = int.Parse(ini.GetString("Form", "SizeY", "0"));
            this.Size = new Size(x2, y2);
        }

        void ConnectionServer()
        {
            if (sock == null)
            {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            }
            else
            {
                sock.Close();
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            }
            try
            {
                sock.Connect(IP, int.Parse(Port));
                tbReceive.Text += $"[{System.DateTime.Now.ToString("yyyy-MM-dd")}] Connected with Server\r\n";
                sbIPPORT.Text = $"{sock.RemoteEndPoint.ToString()}";
                tbExcep.Text = "Connect Success";
                tbExcep.BackColor = Color.Blue;
                if (threadClient == null)
                {
                    threadClient = new Thread(ClientProcess);
                    threadClient.IsBackground = true;
                    threadClient.Start();
                }
            }
            catch (Exception err)
            {
                tbExcep.Text = "Fail to connect";
                tbExcep.BackColor = Color.Red;
            }
        }

        private void btSend_Click(object sender, EventArgs e)
        {

            try
            {
                if (sock.Connected)
                {
                    string str = tbSend.Text;
                    string[] sArr = str.Split('\r');
                    string sLast = sArr.Last();
                    sock.Send(Encoding.Default.GetBytes(sLast));
                    string timer = System.DateTime.Now.ToString("HH:mm:ss");
                    AddText($"[발신][{timer}] {sLast}");
                }

            }catch(SocketException e1)
            {
                tbReceive.Text += "연결을 확인하세요.\r\n";
            }
            
        }
        
        private void ClientProcess()
        {
            if (sock != null)
            {
                while (true)
                {
                    int n = sock.Available;
                    if (sock != null && n > 0)
                    {
                        byte[] bArr = new byte[n];
                        sock.Receive(bArr);
                        string timer = System.DateTime.Now.ToString("HH:mm:ss");
                        AddText($"[수신][{timer}] {Encoding.Default.GetString(bArr)}");
                    }

                }
            }
            else
            {
                tbExcep.Text = "Check connection";
                tbExcep.BackColor = Color.Red;
                tbReceive.Text += "연결을 확인하세요.\r\n";
            }



        }

        private void menu_EndClient_Click(object sender, EventArgs e)
        {
            if (threadClient != null)
            {
                threadClient.Abort();
                sock.Shutdown(SocketShutdown.Both);
                sock.Close();
                tbReceive.Text += $"[{System.DateTime.Now.ToString("yyyy-MM-dd")}] Disconnected with Server\r\n";
                tbExcep.Text = "Disconnect Server";
                tbExcep.BackColor = Color.Red;
            }

        }

        private void connectionInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"[IP] {IP}\r\n[Port] {Port}","IP and Port info");
        }

        private void editConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting set = new Setting(IP, Port);
            DialogResult result = set.ShowDialog();
            if (result == DialogResult.OK)
            {
                IP = set.setIP();
                Port = set.setPort();
            }
            set.Close();

        }

        private void menu_Send_Click(object sender, EventArgs e)
        {
            btSend_Click(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Chat_Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            ini.WriteString("Server", "IP", IP);
            ini.WriteString("Server", "PORT", Port);
            ini.WriteString("Form", "LocationX", $"{Location.X}");
            ini.WriteString("Form", "LocationY", $"{Location.Y}");
            ini.WriteString("Form", "SizeX", $"{Size.Width}");
            ini.WriteString("Form", "SizeY", $"{Size.Height}");
            //if (threadClient != null) threadClient.Abort();

            
        }

        private void tbSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btSend_Click(sender, e);
            }
        }

        private void sendToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            ConnectionServer();
        }
    }
}
