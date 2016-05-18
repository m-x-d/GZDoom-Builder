
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	// No action or button for this mode, it is automatic.
	// The EditMode attribute does not have to be specified unless the
	// mode must be activated by class name rather than direct instance.
	// In that case, just specifying the attribute like this is enough:
	// [EditMode]

	[EditMode(DisplayName = "Vertices",
			  AllowCopyPaste = false,
			  Volatile = true)]

	public sealed class DragVerticesMode : DragGeometryMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor to start dragging immediately
		public DragVerticesMode(Vector2D dragstartmappos)
		{
			// Mark what we are dragging
			General.Map.Map.ClearAllMarks(false);
			General.Map.Map.MarkSelectedVertices(true, true);

			// Initialize
			base.StartDrag(dragstartmappos);
			undodescription = (selectedverts.Count == 1 ? "Drag vertex" : "Drag " + selectedverts.Count + " vertices"); //mxd
			
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
		
		// Disenagaging
		public override void OnDisengage()
		{
			// Select vertices from marks
			General.Map.Map.ClearSelectedVertices();
			General.Map.Map.SelectMarkedVertices(true, true);

			//mxd. Mark stable lines now (marks will be carried to split lines by MapSet.StitchGeometry())
			HashSet<Linedef> stablelines = (!cancelled ? new HashSet<Linedef>(General.Map.Map.LinedefsFromMarkedVertices(false, true, false)) : new HashSet<Linedef>());
			foreach(Linedef l in stablelines) l.Marked = true;

			//mxd. Mark moved sectors (used in Linedef.Join())
			HashSet<Sector> draggeddsectors = (!cancelled ? General.Map.Map.GetUnselectedSectorsFromLinedefs(stablelines) : new HashSet<Sector>());
			foreach(Sector s in draggeddsectors) s.Marked = true;

			// Perform normal disengage
			base.OnDisengage();
			
			// When not cancelled
			if(!cancelled)
			{
				//mxd. Reattach/add/remove sidedefs only when there are no unstable lines in selection
				if(stablelines.Count > 0 && unstablelines.Count == 0)
				{
					// Get new lines from linedef marks...
					HashSet<Linedef> newlines = new HashSet<Linedef>(General.Map.Map.GetMarkedLinedefs(true));

					// Marked lines were created during linedef splitting
					HashSet<Linedef> changedlines = new HashSet<Linedef>(stablelines);
					changedlines.UnionWith(newlines);

					// Get sectors, which have all their linedefs selected (otherwise those would be destroyed after moving the selection)
					HashSet<Sector> toadjust = General.Map.Map.GetUnselectedSectorsFromLinedefs(changedlines);

					if(changedlines.Count > 0)
					{
						// Process outer sidedefs
						Tools.AdjustOuterSidedefs(toadjust, changedlines);

						// Split outer sectors
						Tools.SplitOuterSectors(changedlines);

						// Additional verts may've been created
						if(selectedverts.Count > 1)
						{
							foreach(Linedef l in changedlines)
							{
								l.Start.Selected = true;
								l.End.Selected = true;
							}
						}
					}
				}
				
				// If only a single vertex was selected, deselect it now
				if(selectedverts.Count == 1) General.Map.Map.ClearSelectedVertices();
			}
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			bool viewchanged = CheckViewChanged();

			renderer.RedrawSurface();

			UpdateRedraw();
			
			// Redraw things when view changed
			if(viewchanged)
			{
				if(renderer.StartThings(true))
				{
					renderer.RenderThingSet(General.Map.Map.Things, General.Settings.ActiveThingsAlpha);
					renderer.Finish();
				}
			}

			renderer.Present();
		}
		
		// This redraws only the required things
		protected override void UpdateRedraw()
		{
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

			//mxd. Render things
			if(renderer.StartThings(true)) 
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, General.Settings.HiddenThingsAlpha);
				renderer.RenderThingSet(unselectedthings, General.Settings.ActiveThingsAlpha);
				renderer.RenderThingSet(selectedthings, General.Settings.ActiveThingsAlpha);
				renderer.Finish();
			}

			// Redraw overlay
			if(renderer.StartOverlay(true))
			{
				renderer.RenderText(labels);
				renderer.Finish();
			}
		}
		
		#endregion
	}
}
