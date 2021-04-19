using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatProgram_Socket
{
    
    public partial class Targets : Form
    {
        Control[] Rad;
        bool[] check_list;
        public Targets(List<Socket> sock_list)
        {
            InitializeComponent();
            check_list = new bool[sock_list.Count];
            Rad = new Control[sock_list.Count];
            for(int i=0;i<sock_list.Count;i++)
            {
                check_list[i] = true;
                Rad[i] = new RadioButton();
                Rad[i].Name = i.ToString();
                Rad[i].Text = $"[{i+1}번째]{sock_list[i].RemoteEndPoint}";
                Rad[i].Size = new Size(200, 20);
                
                fLPanel.Controls.Add(Rad[i]);
            }
        }
        public bool[] Rad_Status(object sender, EventArgs e)
        {

            return check_list;
        }

        private void btn_apply_Click(object sender, EventArgs e)
        {

        }
    }
   
}
