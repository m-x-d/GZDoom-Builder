using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CodeImp.DoomBuilder;

namespace CodeImp.DoomBuilder.ColorPicker
{
    public partial class ToolsForm : Form
    {
        public ToolsForm() {
            InitializeComponent();

            General.Interface.AddButton(separator1);
            General.Interface.AddButton(cpButton);
            General.Interface.AddButton(separator2);
        }

        private void InvokeTaggedAction(object sender, EventArgs e) {
            General.Interface.InvokeTaggedAction(sender, e);
        }
    }
}
