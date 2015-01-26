
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class VertexEditForm : DelayedForm
	{
		#region ================== Constants

		private const string CLEAR_VALUE = "Unused"; //mxd

		#endregion

		#region ================== Events

		public event EventHandler OnValuesChanged; //mxd

		#endregion
		
		#region ================== Variables

		private ICollection<Vertex> vertices;
		private bool preventchanges; //mxd
		private bool undocreated; //mxd
		private List<VertexProperties> vertexprops; //mxd

		//mxd. Window setup stuff
		private static Point location = Point.Empty;
		private static int activeTab;

		private struct VertexProperties //mxd
		{
			public readonly float X;
			public readonly float Y;
			public readonly float ZCeiling;
			public readonly float ZFloor;

			public VertexProperties(Vertex v) 
			{
				X = v.Position.x;
				Y = v.Position.y;
				ZCeiling = v.ZCeiling;
				ZFloor = v.ZFloor;
			}
		}

		#endregion

		#region ================== Constructor

		// Constructor
		public VertexEditForm()
		{
			InitializeComponent();

			//mxd. Widow setup
			if(location != Point.Empty) 
			{
				this.StartPosition = FormStartPosition.Manual;
				this.Location = location;
				if(activeTab > 0 && activeTab < tabs.TabCount) 
				{
					tabs.SelectTab(activeTab);
				} 
				else 
				{
					activeTab = 0;
				}
			}

			if(General.Map.FormatInterface.HasCustomFields) //mxd
			{ 
				// Initialize custom fields editor
				fieldslist.Setup("vertex");

				// Fill universal fields list
				fieldslist.ListFixedFields(General.Map.Config.VertexFields);
			} 
			else 
			{
				tabs.TabPages.Remove(tabcustom);
				panelHeightControls.Visible = false;
			}
			
			// Decimals allowed?
			if(General.Map.FormatInterface.VertexDecimals > 0)
			{
				positionx.AllowDecimal = true;
				positiony.AllowDecimal = true;

				//mxd
				zceiling.AllowDecimal = true;
				zfloor.AllowDecimal = true;
			}
		}

		#endregion
		
		#region ================== Methods

		// This sets up the form to edit the given vertices
		public void Setup(ICollection<Vertex> vertices, bool allowPositionChange)
		{
			preventchanges = true; //mxd
			
			// Keep this list
			this.vertices = vertices;
			if(vertices.Count > 1) this.Text = "Edit Vertices (" + vertices.Count + ")";
			vertexprops = new List<VertexProperties>(); //mxd

			////////////////////////////////////////////////////////////////////////
			// Set all options to the first vertex properties
			////////////////////////////////////////////////////////////////////////

			// Get first vertex
			Vertex vc = General.GetByIndex(vertices, 0);

			// Position
			positionx.Text = vc.Position.x.ToString();
			positiony.Text = vc.Position.y.ToString();

			//mxd
			positionx.Enabled = allowPositionChange;
			positiony.Enabled = allowPositionChange;
			
			// Custom fields
			if(General.Map.FormatInterface.HasCustomFields) //mxd
				fieldslist.SetValues(vc.Fields, true);
			
			////////////////////////////////////////////////////////////////////////
			// Now go for all sectors and change the options when a setting is different
			////////////////////////////////////////////////////////////////////////

			// Go for all vertices
			foreach(Vertex v in vertices)
			{
				// Position
				if(positionx.Text != v.Position.x.ToString()) positionx.Text = "";
				if(positiony.Text != v.Position.y.ToString()) positiony.Text = "";

				// Custom fields
				if(General.Map.FormatInterface.HasCustomFields) 
				{
					fieldslist.SetValues(v.Fields, false);
				}

				//mxd. Store initial properties
				vertexprops.Add(new VertexProperties(v));
			}

			//mxd. Height offsets
			if(General.Map.UDMF) 
			{
				zceiling.Text = (float.IsNaN(vc.ZCeiling) ? CLEAR_VALUE : vc.ZCeiling.ToString());
				zfloor.Text = (float.IsNaN(vc.ZFloor) ? CLEAR_VALUE : vc.ZFloor.ToString());

				foreach(Vertex v in vertices) 
				{
					string zc = (float.IsNaN(v.ZCeiling) ? CLEAR_VALUE : v.ZCeiling.ToString());
					string zf = (float.IsNaN(v.ZFloor) ? CLEAR_VALUE : v.ZFloor.ToString());

					if(zceiling.Text != zc)	zceiling.Text = "";
					if(zfloor.Text != zf) zfloor.Text = "";
				}
			}

			preventchanges = false; //mxd
		}

		//mxd
		private void MakeUndo()
		{
			if(undocreated) return;
			undocreated = true;

			//mxd. Make undo
			General.Map.UndoRedo.CreateUndo("Edit " + (vertices.Count > 1 ? vertices.Count + " vertices" : "vertex"));

			if(General.Map.FormatInterface.HasCustomFields)
			{
				foreach(Vertex v in vertices) v.Fields.BeforeFieldsChange();
			}
		}
		
		#endregion

		#region ================== mxd. Realtime Events

		private void positionx_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo();
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(positionx.Text)) 
			{
				// Apply position
				foreach(Vertex v in vertices) v.Move(new Vector2D(vertexprops[i++].X, v.Position.y));
			} 
			else //update values
			{ 
				// Verify the coordinates
				float px = positionx.GetResultFloat(vertexprops[i].X);
				if(px < General.Map.FormatInterface.MinCoordinate) 
				{
					positionx.Text = General.Map.FormatInterface.MinCoordinate.ToString();
					return;
				} 
				
				if(px > General.Map.FormatInterface.MaxCoordinate) 
				{
					positionx.Text = General.Map.FormatInterface.MaxCoordinate.ToString();
					return;
				}

				// Apply position
				foreach(Vertex v in vertices) v.Move(new Vector2D(px, v.Position.y));
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void positiony_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo();
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(positiony.Text)) 
			{
				// Apply position
				foreach(Vertex v in vertices) v.Move(new Vector2D(v.Position.x, vertexprops[i++].Y));
			} 
			else //update values
			{ 
				// Verify the coordinates
				float py = positiony.GetResultFloat(vertexprops[i].Y);
				if(py < General.Map.FormatInterface.MinCoordinate) 
				{
					positiony.Text = General.Map.FormatInterface.MinCoordinate.ToString();
					return;
				} 
				
				if(py > General.Map.FormatInterface.MaxCoordinate) 
				{
					positiony.Text = General.Map.FormatInterface.MaxCoordinate.ToString();
					return;
				}

				// Apply position
				foreach(Vertex v in vertices) v.Move(new Vector2D(v.Position.x, py));
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void zceiling_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo();
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(zceiling.Text)) 
			{
				foreach(Vertex v in vertices) v.ZCeiling = vertexprops[i++].ZCeiling;

			} 
			else if(zceiling.Text == CLEAR_VALUE) //clear values
			{ 
				foreach(Vertex v in vertices) v.ZCeiling = float.NaN;

			} 
			else //update values
			{ 
				foreach(Vertex v in vertices)
					v.ZCeiling = zceiling.GetResultFloat(vertexprops[i++].ZCeiling);
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void zfloor_WhenTextChanged(object sender, EventArgs e) 
		{
			if(preventchanges) return;
			MakeUndo();
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(zfloor.Text)) 
			{
				foreach(Vertex v in vertices)
					v.ZFloor = vertexprops[i++].ZFloor;
			
			} 
			else if(zfloor.Text == CLEAR_VALUE) //clear values
			{
				foreach(Vertex v in vertices) v.ZFloor = float.NaN;
			} 
			else //update values
			{ 
				foreach(Vertex v in vertices)
					v.ZFloor = zfloor.GetResultFloat(vertexprops[i++].ZFloor);
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		//mxd
		private void clearZFloor_Click(object sender, EventArgs e) 
		{
			zfloor.Text = CLEAR_VALUE;
		}

		//mxd
		private void clearZCeiling_Click(object sender, EventArgs e) 
		{
			zceiling.Text = CLEAR_VALUE;
		}

		#endregion

		#region ================== Events

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			//mxd. Make undo if required
			MakeUndo();
			
			// Apply custom fields
			if(General.Map.FormatInterface.HasCustomFields) 
			{
				foreach(Vertex v in vertices) fieldslist.Apply(v.Fields); //mxd
			}
			
			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
			
			// Done
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			//mxd. Perform undo if required
			if(undocreated) General.Map.UndoRedo.WithdrawUndo();
			
			// And close
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		//mxd
		private void tabcustom_MouseEnter(object sender, EventArgs e) 
		{
			fieldslist.Focus();
		}

		//mxd
		private void VertexEditForm_FormClosing(object sender, FormClosingEventArgs e) 
		{
			location = this.Location;
			activeTab = tabs.SelectedIndex;
		}

		// Help requested
		private void VertexEditForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_vertexeditor.html");
			hlpevent.Handled = true;
		}

		#endregion
	}
}