using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
    public partial class ConsoleDocker : UserControl
    {
#if DEBUG        
        public ConsoleDocker() {
            InitializeComponent();
        }

        public void TraceInHeader(string message) {
            label1.Text = message;
        }

        public void Trace(string message) {
            Trace(message, true);
        }

        public void Trace(string message, bool addLineBreak) {
            textBox.AppendText(message + (addLineBreak ? Environment.NewLine : ""));
        }

        public void Clear() {
            textBox.Clear();
        }

//events
        private void buttonClear_Click(object sender, EventArgs e) {
            textBox.Clear();
        }
#endif
    }
}
