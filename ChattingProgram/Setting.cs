using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChattingProgram
{
    public partial class Setting : Form
    {
        public Setting(string ip, string port)
        {
            InitializeComponent();
            tbIPnum.Text = ip;
            tbPortnum.Text = port;
        }

        

        public string setIP()
        {
            return tbIPnum.Text;
        }
        public string setPort()
        {
            return tbPortnum.Text;
        }
    }
}
