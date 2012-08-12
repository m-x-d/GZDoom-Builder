using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;

namespace CodeImp.DoomBuilder.Windows
{
	public partial class LinedefFlagsForm : DelayedForm
	{
		#region ================== Variables

		private bool setup;
		private string value;

		#endregion

		#region ================== Properties

		public string Value { get { return value; } }

		#endregion

		#region ================== Methods

		public LinedefFlagsForm()
		{
			InitializeComponent();

			// Fill flags list
			foreach (KeyValuePair<string, string> tf in General.Map.Config.LinedefFlags)
				flags.Add(tf.Value, tf.Key);
		}

		// Setup from EnumList
		public void Setup(string value)
		{
			setup = true;
			this.value = value;

			// Parse the value string and check the boxes if necessary
			if (value.Trim() != "")
			{
				foreach (string s in value.Split(','))
				{
					string str = s.Trim();

					// Make sure the given flag actually exists
					if(!General.Map.Config.LinedefFlags.ContainsKey(str))
						continue;

					foreach (CheckBox c in flags.Checkboxes)
					{
						if (c.Text == General.Map.Config.LinedefFlags[str])
							c.Checked = true;
					}
				}
			}

			setup = false;
		}

		// This shows the dialog
		// Returns the flags or the same flags when cancelled
		public static string ShowDialog(IWin32Window owner, string value)
		{
			string result = value;
			LinedefFlagsForm f = new LinedefFlagsForm();
			f.Setup(value);
			if (f.ShowDialog(owner) == DialogResult.OK) result = f.Value;
			f.Dispose();
			return result;
		}

		#endregion

		private void LinedefFlagsForm_Load(object sender, EventArgs e)
		{

		}

		private void apply_Click(object sender, EventArgs e)
		{
			value = "";

			foreach (CheckBox c in flags.Checkboxes)
			{
				if(c.Checked == false) continue;

				foreach (KeyValuePair<string, string> lf in General.Map.Config.LinedefFlags)
				{
					if (lf.Value == c.Text)
					{
						if (value != "") value += ",";
						value += lf.Key.ToString();
					}
				}
			}

			DialogResult = DialogResult.OK;
			this.Close();
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			// Close
			DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}
