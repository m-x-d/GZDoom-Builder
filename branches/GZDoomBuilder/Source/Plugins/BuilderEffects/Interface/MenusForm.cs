using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.BuilderEffects
{
	public partial class MenusForm : Form
	{
		// Menus list
		private ToolStripItem[] menus;
		
		public MenusForm() {
			InitializeComponent();

			// List all menus
			menus = new ToolStripItem[menuStrip.Items.Count];
			for(int i = 0; i < menuStrip.Items.Count; i++) menus[i] = menuStrip.Items[i];
		}

		// This invokes an action from control event
		private void InvokeTaggedAction(object sender, EventArgs e) {
			General.Interface.InvokeTaggedAction(sender, e);
		}

		// This registers with the core
		public void Register() {
			// Add the menus to the core
			foreach(ToolStripMenuItem m in menus)
				General.Interface.AddMenu(m);
		}

		// This unregisters from the core
		public void Unregister() {
			// Remove the menus from the core
			foreach(ToolStripMenuItem m in menus)
				General.Interface.RemoveMenu(m);
		}
	}
}
