using System;
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
			if(hintsText.Length == 0) return;

			//convert to rtf markup
			hintsText[0] = "{\\rtf1" + hintsText[0];
			hintsText[hintsText.Length - 1] += "}";

			hints.SelectedRtf = string.Join("\\par\\par ", hintsText).Replace("<b>", "{\\b ").Replace("</b>", "}");
		}

		internal void ClearHints() {
			hints.Clear();
		}

		// Fight TextBoxes habit of not releasing the focus by using a carefully placed label
		private void hints_Enter(object sender, EventArgs e) {
			label1.Focus();
		}
	}
}
