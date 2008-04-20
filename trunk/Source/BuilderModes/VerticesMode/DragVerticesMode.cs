
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
using System.Drawing;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.Editing
{
	// No action or button for this mode, it is automatic.
	// The EditMode attribute does not have to be specified unless the
	// mode must be activated by class name rather than direct instance.
	// In that case, just specifying the attribute like this is enough:
	[EditMode]

	public sealed class DragVerticesMode : DragGeometryMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private ICollection<Vertex> selectedverts;
		private ICollection<Vertex> unselectedverts;
		
		#endregion

		#region ================== Properties
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor to start dragging immediately
		public DragVerticesMode(EditMode basemode, Vertex dragitem, Vector2D dragstartmappos)
		{
			// Get selected vertices
			selectedverts = General.Map.Map.GetVerticesSelection(true);
			unselectedverts = General.Map.Map.GetVerticesSelection(false);
			
			// Initialize
			base.StartDrag(basemode, dragitem, dragstartmappos,
						   General.Map.Map.GetVerticesSelection(true),
						   General.Map.Map.GetVerticesSelection(false));
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// Mode engages
		public override void Engage()
		{
			base.Engage();
		}
		
		// Disenagaging
		public override void Disengage()
		{
			base.Disengage();
			
			// When not cancelled
			if(!cancelled)
			{
				// If only a single vertex was selected, deselect it now
				if(selectedverts.Count == 1) General.Map.Map.ClearSelectedVertices();
			}
		}

		// This redraws the display
		public override void RedrawDisplay()
		{
			bool viewchanged = CheckViewChanged();
			
			// Start rendering
			if(renderer.StartPlotter(true))
			{
				// Render lines and vertices
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(unselectedverts);
				renderer.PlotVerticesSet(selectedverts);

				// Draw the dragged item highlighted
				// This is important to know, because this item is used
				// for snapping to the grid and snapping to nearest items
				renderer.PlotVertex(dragitem, ColorCollection.HIGHLIGHT);
				
				// Done
				renderer.Finish();
			}

			// Redraw things when view changed
			if(viewchanged)
			{
				if(renderer.StartThings(true))
				{
					renderer.SetThingsRenderOrder(false);
					renderer.RenderThingSet(General.Map.Map.Things);
					renderer.Finish();
				}
			}

			if(renderer.StartOverlay(true))
			{
				foreach(Linedef l in unstablelines)
				{
					Vector2D delta = new Vector2D(l.End.X - l.Start.X, l.End.Y - l.Start.Y);
					Vector2D textpos = l.Start.Position + (delta * 0.5f);
					int length = (int)Math.Round(l.Length);
					renderer.RenderTextCentered(length.ToString(), textpos, General.Colors.Highlight, true);
				}

				renderer.Finish();
			}


			renderer.Present();
		}
		
		#endregion
	}
}
