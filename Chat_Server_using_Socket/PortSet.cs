using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chat_Server_using_Socket
{
    public partial class PortSet : Form
    {
        public PortSet(string port)
        {
            InitializeComponent();
            textBox1.Text = port;
        }

        public string setPort()
        {
            return textBox1.Text;
        }
    }
}
