using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.GZBuilder.Windows
{
	public partial class TagStatisticsForm : Form
	{
		private static Size size = Size.Empty;
		private static Point location = Point.Empty;
		
		public TagStatisticsForm() {
			InitializeComponent();

			//apply window size and location
			if(!size.IsEmpty && !location.IsEmpty){
				this.StartPosition = FormStartPosition.Manual;
				this.Size = size;
				this.Location = location;
			}

			//collect all tags
			List<int> tags = new List<int>();
			Dictionary<int, int> sectorsCountByTag = new Dictionary<int, int>();
			Dictionary<int, int> linedefsCountByTag = new Dictionary<int, int>();
			Dictionary<int, int> thingsCountByTag = new Dictionary<int, int>();

			//collect used tags from sectors...
			foreach(Sector s in General.Map.Map.Sectors) {
				if(s.Tag == 0) continue;
				if(!tags.Contains(s.Tag)) tags.Add(s.Tag);
					
				if(!sectorsCountByTag.ContainsKey(s.Tag))
					sectorsCountByTag.Add(s.Tag, 1);
				else
					sectorsCountByTag[s.Tag] += 1;
			}

			//...and linedefs...
			if(General.Map.FormatInterface.HasLinedefTag) {
				foreach(Linedef l in General.Map.Map.Linedefs) {
					if(l.Tag == 0) continue;
					if(!tags.Contains(l.Tag)) tags.Add(l.Tag);
						
					if(!linedefsCountByTag.ContainsKey(l.Tag))
						linedefsCountByTag.Add(l.Tag, 1);
					else
						linedefsCountByTag[l.Tag] += 1;
				}
			}else{
				Linedefs.Visible = false;
			}

			//...and things...
			if(General.Map.FormatInterface.HasThingTag) {
				foreach(Thing t in General.Map.Map.Things) {
					if(t.Tag == 0) continue;
					if(!tags.Contains(t.Tag)) tags.Add(t.Tag);
						
					if(!thingsCountByTag.ContainsKey(t.Tag))
						thingsCountByTag.Add(t.Tag, 1);
					else
						thingsCountByTag[t.Tag] += 1;
				}
			}else{
				Things.Visible = false;
			}

			//create rows
			foreach(int tag in tags){
				addRow(tag, 
					General.Map.Options.TagLabels.ContainsKey(tag) ? General.Map.Options.TagLabels[tag] : string.Empty,
					sectorsCountByTag.ContainsKey(tag) ? sectorsCountByTag[tag] : 0,
					linedefsCountByTag.ContainsKey(tag) ? linedefsCountByTag[tag] : 0,
					thingsCountByTag.ContainsKey(tag) ? thingsCountByTag[tag] : 0);
			}

			dataGridView.Sort(TagColumn, ListSortDirection.Ascending);
		}

		private void addRow(int tag, string label, int sectorsCount, int linesCount, int thingsCount) {
			DataGridViewRow row = new DataGridViewRow();

			DataGridViewTextBoxCell cTag = new DataGridViewTextBoxCell();
			cTag.Value = tag;

			DataGridViewTextBoxCell cLabel = new DataGridViewTextBoxCell();
			cLabel.Value = label;

			DataGridViewTextBoxCell cSectors = new DataGridViewTextBoxCell();
			cSectors.Value = sectorsCount;

			DataGridViewTextBoxCell cLines = new DataGridViewTextBoxCell();
			cLines.Value = linesCount;

			DataGridViewTextBoxCell cThings = new DataGridViewTextBoxCell();
			cThings.Value = thingsCount;

			row.Cells.Add(cTag);
			row.Cells.Add(cLabel);
			row.Cells.Add(cSectors);
			row.Cells.Add(cLines);
			row.Cells.Add(cThings);

			dataGridView.Rows.Add(row);
		}

//events
		private void apply_Click(object sender, EventArgs e) {
			//refill TagLabels with table data
			dataGridView.Sort(TagColumn, ListSortDirection.Ascending);
			General.Map.Options.TagLabels.Clear();

			foreach(DataGridViewRow row in dataGridView.Rows) {
				string label = row.Cells[1].Value.ToString();
				if(!string.IsNullOrEmpty(label))
					General.Map.Options.TagLabels.Add((int)row.Cells[0].Value, label);
			}
			
			this.Close();
		}

		private void cancel_Click(object sender, EventArgs e) {
			this.Close();
		}

		private void TagStatisticsForm_FormClosing(object sender, FormClosingEventArgs e) {
			size = this.Size;
			location = this.Location;
		}
	}
}
