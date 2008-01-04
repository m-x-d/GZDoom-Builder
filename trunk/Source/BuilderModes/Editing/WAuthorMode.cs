
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
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.Editing
{
	[EditMode(SwitchAction = "wauthormode",
			  ButtonDesc = "WadAuthor Mode",
			  ButtonImage = "WAuthor.png",
			  ButtonOrder = int.MinValue + 4,
			  ConfigSpecific = true)]
	
	public class WAuthorMode : ClassicMode
	{
		#region ================== Constants

		protected const float LINEDEF_HIGHLIGHT_RANGE = 10f;
		protected const float VERTEX_HIGHLIGHT_RANGE = 8f;
		protected const float THING_HIGHLIGHT_RANGE = 2f;

		#endregion

		#region ================== Variables
		
		// Tools
		protected WAuthorTools tools;
		
		// Highlighted item
		protected object highlighted;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public WAuthorMode()
		{
			// Initialize
			tools = new WAuthorTools();

			// Enable this and you'll have a floating window
			//tools.Show(General.Interface);
			//General.Interface.Focus();
			
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
				tools.Dispose();
				
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
			General.Map.ChangeMode(new WAuthorMode());
		}

		// Mode engages
		public override void Engage()
		{
			base.Engage();
		}

		// Mode disengages
		public override void Disengage()
		{
			base.Disengage();

			// Clear selected vertices
			General.Map.Map.ClearAllSelected();

			// Hide highlight info
			General.Interface.HideInfo();
		}

		// This redraws the display
		public unsafe override void RedrawDisplay()
		{
			// Start with a clear display
			if(renderer.Start(true, true))
			{
				// Render things
				renderer.SetThingsRenderOrder(true);
				renderer.RenderThingSet(General.Map.Map.Things);

				// Render lines and vertices
				renderer.RenderLinedefSet(General.Map.Map.Linedefs);
				renderer.RenderVerticesSet(General.Map.Map.Vertices);

				// Render highlighted item
				if(highlighted != null) DrawHighlight(true);

				// Done
				renderer.Finish();
			}
		}

		// This draws the highlighted item
		protected void DrawHighlight(bool highlightcolor)
		{
			// With highlight color
			if(highlightcolor)
			{
				// Vertex
				if(highlighted is Vertex)
				{
					if((highlighted as Vertex).IsDisposed) return;
					renderer.RenderVertex(highlighted as Vertex, ColorCollection.HIGHLIGHT);
				}
				// Linedef
				else if(highlighted is Linedef)
				{
					if((highlighted as Linedef).IsDisposed) return;
					renderer.RenderLinedef((highlighted as Linedef), General.Colors.Highlight);
					renderer.RenderVertex((highlighted as Linedef).Start, renderer.DetermineVertexColor((highlighted as Linedef).Start));
					renderer.RenderVertex((highlighted as Linedef).End, renderer.DetermineVertexColor((highlighted as Linedef).End));
				}
				// Sector
				else if(highlighted is Sector)
				{
					if((highlighted as Sector).IsDisposed) return;
					renderer.RenderSector((highlighted as Sector), General.Colors.Highlight);
				}
				// Thing
				else if(highlighted is Thing)
				{
					if((highlighted as Thing).IsDisposed) return;
					renderer.RenderThing((highlighted as Thing), General.Colors.Highlight);
				}
			}
			// With original color
			else
			{
				// Vertex
				if(highlighted is Vertex)
				{
					if((highlighted as Vertex).IsDisposed) return;
					renderer.RenderVertex(highlighted as Vertex, renderer.DetermineVertexColor(highlighted as Vertex));
				}
				// Linedef
				else if(highlighted is Linedef)
				{
					if((highlighted as Linedef).IsDisposed) return;
					renderer.RenderLinedef((highlighted as Linedef), renderer.DetermineLinedefColor((highlighted as Linedef)));
					renderer.RenderVertex((highlighted as Linedef).Start, renderer.DetermineVertexColor((highlighted as Linedef).Start));
					renderer.RenderVertex((highlighted as Linedef).End, renderer.DetermineVertexColor((highlighted as Linedef).End));
				}
				// Sector
				else if(highlighted is Sector)
				{
					if((highlighted as Sector).IsDisposed) return;
					renderer.RenderSector((highlighted as Sector));
				}
				// Thing
				else if(highlighted is Thing)
				{
					if((highlighted as Thing).IsDisposed) return;
					renderer.RenderThing((highlighted as Thing), renderer.DetermineThingColor((highlighted as Thing)));
				}
			}
		}
		
		// This highlights a new item
		protected void Highlight(object h)
		{
			// Changes?
			if(highlighted != h)
			{
				// Update display
				if(renderer.Start(false, false))
				{
					// Undraw previous highlight
					if(highlighted != null) DrawHighlight(false);

					// Set new highlight
					highlighted = h;

					// Render highlighted item
					if(highlighted != null) DrawHighlight(true);

					// Done
					renderer.Finish();
				}

				// Hide info
				General.Interface.HideInfo();
				
				// Anything highlighted?
				if(highlighted != null)
				{
					// Show highlight info
					if(highlighted is Vertex)
						General.Interface.ShowVertexInfo(highlighted as Vertex);
					else if(highlighted is Linedef)
						General.Interface.ShowLinedefInfo(highlighted as Linedef);
					else if(highlighted is Sector)
						General.Interface.ShowSectorInfo(highlighted as Sector);
					else if(highlighted is Thing)
						General.Interface.ShowThingInfo(highlighted as Thing);
				}
			}
		}

		// Mouse moves
		public override void MouseMove(MouseEventArgs e)
		{
			base.MouseMove(e);

			// Not holding any buttons?
			if(e.Button == MouseButtons.None)
			{
				// Find the nearest items within highlight range
				Vertex v = General.Map.Map.NearestVertexSquareRange(mousemappos, VERTEX_HIGHLIGHT_RANGE / renderer.Scale);
				Thing t = General.Map.Map.NearestThingSquareRange(mousemappos, THING_HIGHLIGHT_RANGE / renderer.Scale);
				Linedef l = General.Map.Map.NearestLinedef(mousemappos);
				Sector s;
				
				// Check on which side of the linedef the mouse is
				float side = l.SideOfLine(mousemappos);
				if(side > 0)
				{
					// Is there a sidedef here?
					if(l.Back != null) s = l.Back.Sector;
					else s = null;
				}
				else
				{
					// Is there a sidedef here?
					if(l.Front != null) s = l.Front.Sector;
					else s = null;
				}

				// Both a vertex and thing in range?
				if((v != null) && (t != null))
				{
					// Highlight closest
					float vd = v.DistanceToSq(mousemappos);
					float td = t.DistanceToSq(mousemappos);
					if(vd < td) Highlight(v); else Highlight(t);
				}
				// Vertex in range?
				else if(v != null)
				{
					// Highlight vertex
					Highlight(v);
				}
				// Thing in range?
				else if(t != null)
				{
					// Highlight thing
					Highlight(t);
				}
				else
				{
					// Linedef within in range?
					float ld = l.DistanceTo(mousemappos, true);
					if(ld < (LINEDEF_HIGHLIGHT_RANGE / renderer.Scale))
					{
						// Highlight line
						Highlight(l);
					}
					// Mouse inside a sector?
					else if(s != null)
					{
						// Highlight sector
						Highlight(s);
					}
					else
					{
						// Highlight nothing
						Highlight(null);
					}
				}
			}
		}

		// Maybe i'll finish this later, or not even include this mode at all, not sure yet.
		
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
		}

		// Mouse released
		public override void MouseUp(MouseEventArgs e)
		{
			base.MouseUp(e);

			// This shows a popup menu
			tools.LinedefPopup.Show(Cursor.Position);
		}

		// Mouse wants to drag
		protected override void DragStart(MouseEventArgs e)
		{
			base.DragStart(e);
		}
		
		#endregion
	}
}
