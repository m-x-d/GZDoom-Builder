using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.GZBuilder.Windows
{
    public partial class DebugForm : Form
    {
        public TextBox TextPannel;
        
        public DebugForm() {
            InitializeComponent();
            TextPannel = textBox1;
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {

        }
    }
}
