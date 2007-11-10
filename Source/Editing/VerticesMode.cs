
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Interface;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	public class VerticesMode : ClassicMode
	{
		#region ================== Constants

		protected const float VERTEX_HIGHLIGHT_RANGE = 20f;

		#endregion

		#region ================== Variables

		// Highlighted item
		protected Vertex highlighted;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public VerticesMode()
		{
		}

		// Diposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// Cancel mode
		public override void Cancel()
		{
			base.Cancel();
			
			// Return to this mode
			General.Map.ChangeMode(new VerticesMode());
		}

		// Mode engages
		public override void Engage()
		{
			base.Engage();
			
			// Check vertices button on main window
			General.MainWindow.SetVerticesChecked(true);
		}

		// Mode disengages
		public override void Disengage()
		{
			base.Disengage();

			// Hide highlight info
			General.MainWindow.HideInfo();
			
			// Uncheck vertices button on main window
			General.MainWindow.SetVerticesChecked(false);
		}

		// This redraws the display
		public unsafe override void RedrawDisplay()
		{
			// Start with a clear display
			if(renderer.StartRendering(true, true))
			{
				// Render things
				renderer.SetThingsRenderOrder(false);
				renderer.RenderThingSet(General.Map.Map.Things);
				
				// Render lines and vertices
				renderer.RenderLinedefSet(General.Map.Map.Linedefs);
				renderer.RenderVerticesSet(General.Map.Map.Vertices);

				// Render highlighted item
				if(highlighted != null)
					renderer.RenderVertex(highlighted, ColorCollection.HIGHLIGHT);
				
				// Done
				renderer.FinishRendering();
			}
		}
		
		// This highlights a new item
		protected void Highlight(Vertex v)
		{
			// Update display
			if(renderer.StartRendering(false, false))
			{
				// Undraw previous highlight
				if(highlighted != null)
					renderer.RenderVertex(highlighted, renderer.DetermineVertexColor(highlighted));

				// Set new highlight
				highlighted = v;

				// Render highlighted item
				if(highlighted != null)
					renderer.RenderVertex(highlighted, ColorCollection.HIGHLIGHT);
				
				// Done
				renderer.FinishRendering();
			}

			// Show highlight info
			if(highlighted != null) General.MainWindow.ShowVertexInfo(highlighted);
				else General.MainWindow.HideInfo();
		}
		
		// Mouse moves
		public override void MouseMove(MouseEventArgs e)
		{
			base.MouseMove(e);

			// Not holding any buttons?
			if(e.Button == MouseButtons.None)
			{
				// Find the nearest vertex within highlight range
				Vertex v = General.Map.Map.NearestVertexSquareRange(mousemappos, VERTEX_HIGHLIGHT_RANGE / renderer.Scale);

				// Highlight if not the same
				if(v != highlighted) Highlight(v);
			}
		}

		// Mouse leaves
		public override void MouseLeave(EventArgs e)
		{
			base.MouseLeave(e);
			
			// Highlight nothing
			Highlight(null);
		}
		
		// Mouse button pressed
		public override void MouseDown(MouseEventArgs e)
		{
			base.MouseDown(e);

			// Which button is used?
			if(e.Button == EditMode.SELECT_BUTTON)
			{
				// Item highlighted?
				if(highlighted != null)
				{
					// Item already selected?
					if(General.Map.Selection.Vertices.Contains(highlighted))
					{
						// Deselect
						General.Map.Selection.RemoveVertex(highlighted);
					}
					else
					{
						// Select
						General.Map.Selection.AddVertex(highlighted);
					}

					// Update display
					if(renderer.StartRendering(false, false))
					{
						// Undraw highlight to show selection
						renderer.RenderVertex(highlighted, renderer.DetermineVertexColor(highlighted));
						renderer.FinishRendering();
					}
				}
			}
		}
		
		// Mouse released
		public override void MouseUp(MouseEventArgs e)
		{
			base.MouseUp(e);

			// Item highlighted?
			if(highlighted != null)
			{
				// Update display
				if(renderer.StartRendering(false, false))
				{
					// Render highlighted item
					renderer.RenderVertex(highlighted, ColorCollection.HIGHLIGHT);
					renderer.FinishRendering();
				}
			}
		}

		// Mouse wants to drag
		protected override void DragStart(MouseEventArgs e)
		{
			base.DragStart(e);

			// Which button is used?
			if(e.Button == EditMode.SELECT_BUTTON)
			{
				// Make selection

			}
			else if(e.Button == EditMode.EDIT_BUTTON)
			{
				// Anything highlighted?
				if(highlighted != null)
				{
					// Highlighted item not selected?
					if(!General.Map.Selection.Vertices.Contains(highlighted))
					{
						// Select only this vertex for dragging
						General.Map.Selection.ClearVertices();
						General.Map.Selection.AddVertex(highlighted);
					}

					// Start dragging the selection
					General.Map.ChangeMode(new DragVerticesMode(highlighted, mousedownmappos - highlighted.Position));
				}
			}
		}
		#endregion
	}
}
