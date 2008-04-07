
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

	public sealed class DragSectorsMode : DragGeometryMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private ICollection<Linedef> selectedlines;
		private ICollection<Linedef> unselectedlines;
		private ICollection<Sector> selectedsectors;

		#endregion

		#region ================== Properties
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor to start dragging immediately
		public DragSectorsMode(EditMode basemode, Vector2D dragstartmappos)
		{
			// Get the nearest vertex for snapping
			Vertex nearest = MapSet.NearestVertex(General.Map.Map.GetVerticesFromLinesSelection(true), dragstartmappos);
			
			// Get selected lines
			selectedlines = General.Map.Map.GetLinedefsSelection(true);
			unselectedlines = General.Map.Map.GetLinedefsSelection(false);
			selectedsectors = General.Map.Map.GetSectorsSelection(true);
			
			// Initialize
			base.StartDrag(basemode, nearest, dragstartmappos,
						   General.Map.Map.GetVerticesFromLinesSelection(true),
						   General.Map.Map.GetVerticesFromLinesSelectionEx(false));
			
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
				// If only a single sector was selected, deselect it now
				if(selectedsectors.Count == 1)
				{
					General.Map.Map.ClearSelectedSectors();
					General.Map.Map.ClearSelectedLinedefs();
				}
			}
		}

		// This redraws the display
		public unsafe override void RedrawDisplay()
		{
			bool viewchanged = CheckViewChanged();
			
			// Start rendering
			if(renderer.Start(true, viewchanged))
			{
				// Uncomment this to see triangulation
				/*
				foreach(Sector s in General.Map.Map.Sectors)
				{
					for(int i = 0; i < s.Triangles.Count; i += 3)
					{
						renderer.RenderLine(s.Triangles[i + 0], s.Triangles[i + 1], General.Colors.Selection);
						renderer.RenderLine(s.Triangles[i + 1], s.Triangles[i + 2], General.Colors.Selection);
						renderer.RenderLine(s.Triangles[i + 2], s.Triangles[i + 0], General.Colors.Selection);
					}
				}
				*/

				// Redraw things when view changed
				if(viewchanged)
				{
					renderer.SetThingsRenderOrder(false);
					renderer.RenderThingSet(General.Map.Map.Things);
				}
				
				// Render lines and vertices
				renderer.RenderLinedefSet(unselectedlines);
				renderer.RenderLinedefSet(selectedlines);
				renderer.RenderVerticesSet(General.Map.Map.Vertices);

				// Draw the dragged item highlighted
				// This is important to know, because this item is used
				// for snapping to the grid and snapping to nearest items
				renderer.RenderVertex(dragitem, ColorCollection.HIGHLIGHT);
				
				// Done
				renderer.Finish();
			}
		}
		
		#endregion
	}
}
