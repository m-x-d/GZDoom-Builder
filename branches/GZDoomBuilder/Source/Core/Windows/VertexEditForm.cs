
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.GZBuilder.Tools;

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
		private bool blockUpdate; //mxd
		private List<VertexProperties> vertexProps; //mxd

		private struct VertexProperties //mxd
		{
			public float X;
			public float Y;
			public float ZCeiling;
			public float ZFloor;

			public VertexProperties(Vertex v) {
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

			// Fill universal fields list
			fieldslist.ListFixedFields(General.Map.Config.VertexFields);

			// Custom fields?
			if(!General.Map.FormatInterface.HasCustomFields)
				tabs.TabPages.Remove(tabcustom);
			
			// Decimals allowed?
			if(General.Map.FormatInterface.VertexDecimals > 0)
			{
				positionx.AllowDecimal = true;
				positiony.AllowDecimal = true;

				//mxd
				zceiling.AllowDecimal = true;
				zfloor.AllowDecimal = true;
			}

			if(!General.Map.UDMF) panelHeightControls.Visible = false;

			// Initialize custom fields editor
			fieldslist.Setup("vertex");
		}

		#endregion
		
		#region ================== Methods

		// This sets up the form to edit the given vertices
		public void Setup(ICollection<Vertex> vertices, bool allowPositionChange)
		{
			blockUpdate = true; //mxd
			
			// Keep this list
			this.vertices = vertices;
			if(vertices.Count > 1) this.Text = "Edit Vertices (" + vertices.Count + ")";
			vertexProps = new List<VertexProperties>(); //mxd

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
				v.Fields.BeforeFieldsChange();//mxd
				fieldslist.SetValues(v.Fields, false);

				//mxd. Store initial properties
				vertexProps.Add(new VertexProperties(v));
			}

			//mxd. Height offsets
			if(General.Map.UDMF) {
				zceiling.Text = (float.IsNaN(vc.ZCeiling) ? CLEAR_VALUE : vc.ZCeiling.ToString());
				zfloor.Text = (float.IsNaN(vc.ZFloor) ? CLEAR_VALUE : vc.ZFloor.ToString());

				foreach(Vertex v in vertices) {
					string zc = (float.IsNaN(v.ZCeiling) ? CLEAR_VALUE : v.ZCeiling.ToString());
					string zf = (float.IsNaN(v.ZFloor) ? CLEAR_VALUE : v.ZFloor.ToString());

					if(zceiling.Text != zc)	zceiling.Text = "";
					if(zfloor.Text != zf) zfloor.Text = "";
				}
			}

			//mxd. Make undo
			string undodesc = "vertex";
			if(vertices.Count > 1) undodesc = vertices.Count + " vertices";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc);

			blockUpdate = false; //mxd
		}
		
		#endregion

		#region ================== mxd. Control Events

		private void positionx_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;

			//restore values
			if(string.IsNullOrEmpty(positionx.Text)) {
				int i = 0;

				// Apply position
				foreach(Vertex v in vertices)
					v.Move(new Vector2D(vertexProps[i++].X, v.Position.y));
			} else { //update values
				// Verify the coordinates
				float px = positionx.GetResultFloat(0.0f);
				if(px < General.Map.FormatInterface.MinCoordinate) {
					positionx.Text = General.Map.FormatInterface.MinCoordinate.ToString();
					return;
				} else if(px > General.Map.FormatInterface.MaxCoordinate) {
					positionx.Text = General.Map.FormatInterface.MaxCoordinate.ToString();
					return;
				}

				// Apply position
				foreach(Vertex v in vertices)
					v.Move(new Vector2D(px, v.Position.y));
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void positiony_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;

			//restore values
			if(string.IsNullOrEmpty(positiony.Text)) {
				int i = 0;

				// Apply position
				foreach(Vertex v in vertices)
					v.Move(new Vector2D(v.Position.x, vertexProps[i++].Y));
			} else { //update values
				// Verify the coordinates
				float py = positiony.GetResultFloat(0.0f);
				if(py < General.Map.FormatInterface.MinCoordinate) {
					positiony.Text = General.Map.FormatInterface.MinCoordinate.ToString();
					return;
				} else if(py > General.Map.FormatInterface.MaxCoordinate) {
					positiony.Text = General.Map.FormatInterface.MaxCoordinate.ToString();
					return;
				}

				// Apply position
				foreach(Vertex v in vertices)
					v.Move(new Vector2D(v.Position.x, py));
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void zceiling_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;

			//restore values
			if(string.IsNullOrEmpty(zceiling.Text)) {
				int i = 0;

				foreach(Vertex v in vertices)
					v.ZCeiling = vertexProps[i++].ZCeiling;
			//clear values
			} else if(zceiling.Text == CLEAR_VALUE) { 
				foreach(Vertex v in vertices)
					v.ZCeiling = float.NaN;
			//update values
			} else { 
				foreach(Vertex v in vertices)
					v.ZCeiling = zceiling.GetResultFloat(v.ZCeiling);
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void zfloor_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;

			//restore values
			if(string.IsNullOrEmpty(zfloor.Text)) {
				int i = 0;

				foreach(Vertex v in vertices)
					v.ZFloor = vertexProps[i++].ZFloor;
			//clear values
			}else if(zfloor.Text == CLEAR_VALUE){
				foreach(Vertex v in vertices)
					v.ZFloor = float.NaN;
			//update values
			} else { 
				foreach(Vertex v in vertices) 
					v.ZFloor = zfloor.GetResultFloat(v.ZFloor);
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void fieldslist_OnFieldValueChanged(string fieldname) {
			if(blockUpdate) return;

			foreach(Vertex v in vertices)
				fieldslist.Apply(v.Fields);

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		//mxd
		private void clearZFloor_Click(object sender, EventArgs e) {
			zfloor.Text = CLEAR_VALUE;
		}

		//mxd
		private void clearZCeiling_Click(object sender, EventArgs e) {
			zceiling.Text = CLEAR_VALUE;
		}

		#endregion

		#region ================== Events

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			// Done
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			//mxd. perform undo
			General.Map.UndoRedo.PerformUndo();
			
			// And close
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		//mxd
		private void tabcustom_MouseEnter(object sender, EventArgs e) {
			fieldslist.Focus();
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