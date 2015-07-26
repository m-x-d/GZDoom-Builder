using System;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.Windows
{
	public partial class UpdateForm : Form
	{
		public UpdateForm(int remoterev, string changelog)
		{
			InitializeComponent();
			Setup(remoterev, changelog);
		}

		private void Setup(int remoterev, string changelog)
		{
			this.Text = this.Text.Replace("[rev]", remoterev.ToString());
			this.label.Text = label.Text.Replace("[rev]", remoterev.ToString());
			this.changelog.SelectedRtf = changelog;
		}

		private void UpdateForm_Shown(object sender, EventArgs e)
		{
			this.changelog.Focus();
		}

		private void downloadupdate_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}
