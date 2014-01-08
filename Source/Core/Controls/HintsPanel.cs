using System;
using System.Drawing;
using System.Windows.Forms;

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class HintsPanel : UserControl
	{
		public HintsPanel() {
			InitializeComponent();
			hints.Clear();
		}

		internal void SetHints(string[] hintsText) {
			hints.Clear();

			foreach (string s in hintsText) {
				hints.AppendText(s + Environment.NewLine + Environment.NewLine);
			}

			//apply <b> tags
			int start = hints.Text.IndexOf("<b>");
			int end = hints.Text.IndexOf("</b>");
			Font regular = hints.Font;
			Font bold = new Font(hints.SelectionFont, FontStyle.Bold);

			while(start != -1 && end != -1) {
				hints.Select(start, end + 4 - start);
				hints.SelectionFont = bold;
				hints.SelectedText = hints.SelectedText.Replace("<b>", "").Replace("</b>", "");

				start = hints.Text.IndexOf("<b>");
				end = hints.Text.IndexOf("</b>");
			}

			hints.SelectionFont = regular;
		}

		internal void ClearHints() {
			hints.Clear();
		}
	}
}
