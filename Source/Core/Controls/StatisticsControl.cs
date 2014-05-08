using System.Windows.Forms;

namespace CodeImp.DoomBuilder.Controls
{
	public partial class StatisticsControl : UserControl
	{
		public StatisticsControl() {
			InitializeComponent();
			this.Visible = false;
		}

		public void UpdateStatistics() {
			verticescount.Text = General.Map.Map.Vertices.Count.ToString();
			linedefscount.Text = General.Map.Map.Linedefs.Count.ToString();
			sidedefscount.Text = General.Map.Map.Sidedefs.Count.ToString();
			sectorscount.Text = General.Map.Map.Sectors.Count.ToString();
			thingscount.Text = General.Map.Map.Things.Count.ToString();
		}
	}
}
