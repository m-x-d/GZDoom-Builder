using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;

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

			setup();
		}

		private void setup() {
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
			} else {
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
			} else {
				Things.Visible = false;
			}

			//create rows
			dataGridView.Rows.Clear();
			foreach(int tag in tags) {
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

		private List<Sector> getSectorsWithTag(int tag, int count) {
			List<Sector> list = new List<Sector>();
			foreach(Sector s in General.Map.Map.Sectors) {
				if(s.Tag == tag) {
					list.Add(s);
					if(list.Count == count) break;
				}
			}

			return list;
		}

		private List<Linedef> getLinedefsWithTag(int tag, int count) {
			List<Linedef> list = new List<Linedef>();
			foreach(Linedef l in General.Map.Map.Linedefs) {
				if(l.Tag == tag) {
					list.Add(l);
					if(list.Count == count) break;
				}
			}

			return list;
		}

		private List<Thing> getThingsWithTag(int tag, int count) {
			List<Thing> list = new List<Thing>();
			foreach(Thing t in General.Map.Map.Things) {
				if(t.Tag == tag) {
					list.Add(t);
					if(list.Count == count) break;
				}
			}

			return list;
		}

		private void showSelection(List<Vector2D> points) {
			RectangleF area = MapSet.CreateEmptyArea();
			
			// Make a view area from the points
			foreach(Vector2D p in points) area = MapSet.IncreaseArea(area, p);

			// Make the area square, using the largest side
			if(area.Width > area.Height) {
				float delta = area.Width - area.Height;
				area.Y -= delta * 0.5f;
				area.Height += delta;
			} else {
				float delta = area.Height - area.Width;
				area.X -= delta * 0.5f;
				area.Width += delta;
			}

			// Add padding
			area.Inflate(100f, 100f);

			// Zoom to area
			ClassicMode editmode = (General.Editing.Mode as ClassicMode);
			editmode.CenterOnArea(area, 0.6f);
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

		private void dataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
			if(e.ColumnIndex < 2) return;
			
			//select 
			if (e.Button == MouseButtons.Left) {
				int tag = (int)dataGridView.Rows[e.RowIndex].Cells[0].Value;

				if(e.ColumnIndex == 2) { //sectors
					List<Sector> list = getSectorsWithTag(tag, (int)dataGridView.Rows[e.RowIndex].Cells[2].Value);
					if(list.Count > 0) {
						General.Map.Map.ClearSelectedSectors();
						General.Map.Map.ClearSelectedLinedefs();
						
						List<Vector2D> points = new List<Vector2D>();
						General.Editing.ChangeMode("SectorsMode");
						ClassicMode mode = (ClassicMode)General.Editing.Mode;

						foreach(Sector s in list) {
							//s.Selected = true;
							mode.SelectMapElement(s);

							foreach(Sidedef sd in s.Sidedefs) {
								points.Add(sd.Line.Start.Position);
								points.Add(sd.Line.End.Position);
							}
						}

						//General.Map.Map.Update();
						showSelection(points);
					}
				} else if(e.ColumnIndex == 3) { //linedefs
					List<Linedef> list = getLinedefsWithTag(tag, (int)dataGridView.Rows[e.RowIndex].Cells[3].Value);
					if(list.Count > 0) {
						General.Map.Map.ClearSelectedSectors();
						General.Map.Map.ClearSelectedLinedefs();

						List<Vector2D> points = new List<Vector2D>();
						foreach(Linedef l in list) {
							l.Selected = true;
							points.Add(l.Start.Position);
							points.Add(l.End.Position);
						}

						General.Map.Map.Update();
						General.Editing.ChangeMode("LinedefsMode");
						showSelection(points);
					}
				} else if(e.ColumnIndex == 4) { //things
					List<Thing> list = getThingsWithTag(tag, (int)dataGridView.Rows[e.RowIndex].Cells[4].Value);
					if(list.Count > 0) {
						General.Map.Map.ClearSelectedThings();

						List<Vector2D> points = new List<Vector2D>();
						foreach(Thing t in list) {
							t.Selected = true;

							Vector2D p = t.Position;
							points.Add(p);
							points.Add(p + new Vector2D(t.Size * 2.0f, t.Size * 2.0f));
							points.Add(p + new Vector2D(t.Size * 2.0f, -t.Size * 2.0f));
							points.Add(p + new Vector2D(-t.Size * 2.0f, t.Size * 2.0f));
							points.Add(p + new Vector2D(-t.Size * 2.0f, -t.Size * 2.0f));
						}

						General.Map.Map.Update();
						General.Editing.ChangeMode("ThingsMode");
						showSelection(points);
					}
				}

			//open properties window
			} else if(e.Button == MouseButtons.Right) {
				dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
				int tag = (int)dataGridView.Rows[e.RowIndex].Cells[0].Value;

				if(e.ColumnIndex == 2) { //sectors
					List<Sector> list = getSectorsWithTag(tag, (int)dataGridView.Rows[e.RowIndex].Cells[2].Value);
					if(list.Count > 0) {
						General.MainWindow.ShowEditSectors(list);
						General.Map.Map.Update();
						setup();
					}
				} else if(e.ColumnIndex == 3) { //linedefs
					List<Linedef> list = getLinedefsWithTag(tag, (int)dataGridView.Rows[e.RowIndex].Cells[3].Value);
					if(list.Count > 0) {
						General.MainWindow.ShowEditLinedefs(list);
						General.Map.Map.Update();
						setup();
					}
				} else if(e.ColumnIndex == 4) { //things
					List<Thing> list = getThingsWithTag(tag, (int)dataGridView.Rows[e.RowIndex].Cells[4].Value);
					if(list.Count > 0) {
						General.MainWindow.ShowEditThings(list);
						General.Map.Map.Update();
						setup();
					}
				}
			}
		}

		private void dataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
			if(e.ColumnIndex < 2) return;
			int tag = (int)dataGridView.Rows[e.RowIndex].Cells[0].Value;

			if(e.ColumnIndex == 2) { //sectors
				List<Sector> list = getSectorsWithTag(tag, (int)dataGridView.Rows[e.RowIndex].Cells[2].Value);
				if(list.Count > 0) {
					General.Map.Map.ClearSelectedSectors();
					General.Map.Map.ClearSelectedLinedefs();

					List<Vector2D> points = new List<Vector2D>();
					foreach(Sector s in list) {
						s.Selected = true;

						foreach(Sidedef sd in s.Sidedefs) {
							points.Add(sd.Line.Start.Position);
							points.Add(sd.Line.End.Position);
						}
					}
					
					General.Map.Map.Update();
					General.Editing.ChangeMode("SectorsMode");
					showSelection(points);
				}
			} else if(e.ColumnIndex == 3) { //linedefs
				List<Linedef> list = getLinedefsWithTag(tag, (int)dataGridView.Rows[e.RowIndex].Cells[3].Value);
				if(list.Count > 0) {
					General.Map.Map.ClearSelectedSectors();
					General.Map.Map.ClearSelectedLinedefs();

					List<Vector2D> points = new List<Vector2D>();
					foreach(Linedef l in list) {
						l.Selected = true;
						points.Add(l.Start.Position);
						points.Add(l.End.Position);
					}

					General.Map.Map.Update();
					General.Editing.ChangeMode("LinedefsMode");
					showSelection(points);
				}
			} else if(e.ColumnIndex == 4) { //things
				List<Thing> list = getThingsWithTag(tag, (int)dataGridView.Rows[e.RowIndex].Cells[4].Value);
				if(list.Count > 0) {
					General.Map.Map.ClearSelectedThings();

					List<Vector2D> points = new List<Vector2D>();
					foreach(Thing t in list) {
						t.Selected = true;
						
						Vector2D p = t.Position;
						points.Add(p);
						points.Add(p + new Vector2D(t.Size * 2.0f, t.Size * 2.0f));
						points.Add(p + new Vector2D(t.Size * 2.0f, -t.Size * 2.0f));
						points.Add(p + new Vector2D(-t.Size * 2.0f, t.Size * 2.0f));
						points.Add(p + new Vector2D(-t.Size * 2.0f, -t.Size * 2.0f));
					}

					General.Map.Map.Update();
					General.Editing.ChangeMode("ThingsMode");
					showSelection(points);
				}
			}
		}

		private void TagStatisticsForm_FormClosing(object sender, FormClosingEventArgs e) {
			size = this.Size;
			location = this.Location;
		}
	}
}
