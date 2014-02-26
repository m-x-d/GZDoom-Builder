using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder.BuilderEffects
{
	public partial class MenusForm : Form
	{
		public MenusForm() {
			InitializeComponent();
		}

		// This invokes an action from control event
		private void InvokeTaggedAction(object sender, EventArgs e) {
			General.Interface.InvokeTaggedAction(sender, e);
		}

		// This registers with the core
		public void Register() {
			// Add the menus to the core
			General.Interface.AddModesMenu(jitterItem, "002_modify");
			General.Interface.AddModesButton(jitterButton, "002_modify");
			General.Interface.AddMenu(importStripMenuItem, MenuSection.FileNewOpenClose);
			General.Interface.AddMenu(exportStripMenuItem, MenuSection.FileNewOpenClose);
		}

		// This unregisters from the core
		public void Unregister() {
			// Remove the menus from the core
			General.Interface.RemoveMenu(jitterItem);
			General.Interface.RemoveButton(jitterButton);
			General.Interface.RemoveMenu(importStripMenuItem);
			General.Interface.RemoveMenu(exportStripMenuItem);
		}
	}
}
