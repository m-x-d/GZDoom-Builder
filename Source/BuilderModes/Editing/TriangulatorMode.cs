
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
using System.Threading;
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.Editing
{
	#if DEBUG
	
	[EditMode(SwitchAction = "triangulatormode",		// Action name used to switch to this mode
			  ButtonDesc = "Triangulator Mode",			// Description on the button in toolbar/menu
			  ButtonImage = "TriangulatorMode.png",		// Image resource name for the button
			  ButtonOrder = int.MaxValue)]				// Position of the button (lower is more to the left)

	public class TriangulatorMode : ClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Highlighted item
		private Sector highlighted;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public TriangulatorMode()
		{
		}

		// Disposer
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
			General.Map.ChangeMode(new SectorsMode());
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

			// Check which mode we are switching to
			if(General.Map.NewMode is VerticesMode)
			{
				// Convert selection to vertices

				// Clear selected sectors
				General.Map.Map.ClearSelectedSectors();
			}
			else if(General.Map.NewMode is LinedefsMode)
			{
				// Convert selection to linedefs

				// Clear selected sectors
				General.Map.Map.ClearSelectedSectors();
			}

			// Hide highlight info
			General.Interface.HideInfo();
		}

		// This redraws the display
		public unsafe override void RedrawDisplay()
		{
			// Start with a clear display
			if(renderer.Start(true, true))
			{
				// Do not show things
				renderer.SetThingsRenderOrder(false);

				// Render lines and vertices
				renderer.RenderLinedefSet(General.Map.Map.Linedefs);
				renderer.RenderVerticesSet(General.Map.Map.Vertices);

				// Render highlighted item
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.RenderSector(highlighted, General.Colors.Highlight);

				// Done
				renderer.Finish();
			}
		}

		// This highlights a new item
		protected void Highlight(Sector s)
		{
			// Update display
			if(renderer.Start(false, false))
			{
				// Undraw previous highlight
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.RenderSector(highlighted);

				// Set new highlight
				highlighted = s;

				// Render highlighted item
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.RenderSector(highlighted, General.Colors.Highlight);

				// Done
				renderer.Finish();
			}

			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowSectorInfo(highlighted);
			else
				General.Interface.HideInfo();
		}

		// Mouse moves
		public override void MouseMove(MouseEventArgs e)
		{
			base.MouseMove(e);

			// Find the nearest linedef within highlight range
			Linedef l = General.Map.Map.NearestLinedef(mousemappos);

			// Check on which side of the linedef the mouse is
			float side = l.SideOfLine(mousemappos);
			if(side > 0)
			{
				// Is there a sidedef here?
				if(l.Back != null)
				{
					// Highlight if not the same
					if(l.Back.Sector != highlighted) Highlight(l.Back.Sector);
				}
				else
				{
					// Highlight nothing
					if(highlighted != null) Highlight(null);
				}
			}
			else
			{
				// Is there a sidedef here?
				if(l.Front != null)
				{
					// Highlight if not the same
					if(l.Front.Sector != highlighted) Highlight(l.Front.Sector);
				}
				else
				{
					// Highlight nothing
					if(highlighted != null) Highlight(null);
				}
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
			bool front, back;

			// Which button is used?
			if(e.Button == EditMode.SELECT_BUTTON)
			{
				// Item highlighted?
				if((highlighted != null) && !highlighted.IsDisposed)
				{
					// Flip selection
					highlighted.Selected = !highlighted.Selected;

					// Make update lines selection
					foreach(Sidedef sd in highlighted.Sidedefs)
					{
						if(sd.Line.Front != null) front = sd.Line.Front.Sector.Selected; else front = false;
						if(sd.Line.Back != null) back = sd.Line.Back.Sector.Selected; else back = false;
						sd.Line.Selected = front | back;
					}

					// Update display
					if(renderer.Start(false, false))
					{
						// Redraw highlight to show selection
						renderer.RenderSector(highlighted);
						renderer.Finish();
					}
				}
			}
		}

		// Mouse released
		public override void MouseUp(MouseEventArgs e)
		{
			ICollection<Sector> selected;
			TriangleList triangles;
			
			base.MouseUp(e);
			
			// Item highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Anything selected?
				selected = General.Map.Map.GetSectorsSelection(true);
				if(selected.Count > 0)
				{
					// Remove highlight
					Highlight(null);

					// Clear selection
					General.Map.Map.ClearSelectedSectors();
					General.Map.Map.ClearSelectedLinedefs();
					General.Interface.RedrawDisplay();
					
					// Get a triangulator and bind events
					EarClipTriangulator t = new EarClipTriangulator();
					t.OnShowLine = new EarClipTriangulator.ShowLine(ShowLine);
					t.OnShowPolygon = new EarClipTriangulator.ShowPolygon(ShowPolygon);
					t.OnShowPoint = new EarClipTriangulator.ShowPoint(ShowPoint);
					t.OnShowEarClip = new EarClipTriangulator.ShowEarClip(ShowEarClip);
					
					// Triangulate this now!
					triangles = t.Triangulate(General.GetByIndex<Sector>(selected, 0));

					// Start with a clear display
					if(renderer.Start(true, true))
					{
						// Do not show things
						renderer.SetThingsRenderOrder(false);

						// Render lines and vertices
						renderer.RenderLinedefSet(General.Map.Map.Linedefs);
						renderer.RenderVerticesSet(General.Map.Map.Vertices);
						
						// Go for all triangle vertices
						for(int i = 0; i < triangles.Count; i += 3)
						{
							renderer.RenderLine(triangles[i + 0], triangles[i + 1], General.Colors.Selection);
							renderer.RenderLine(triangles[i + 1], triangles[i + 2], General.Colors.Selection);
							renderer.RenderLine(triangles[i + 2], triangles[i + 0], General.Colors.Selection);
						}

						// Done
						renderer.Finish();
						Thread.Sleep(200);
					}
				}
			}
		}

		// This shows a point
		private void ShowPoint(Vector2D v, int c)
		{
			for(int a = 0; a < 6; a++)
			{
				RedrawDisplay();
				Thread.Sleep(10);

				// Start with a clear display
				if(renderer.Start(true, true))
				{
					// Do not show things
					renderer.SetThingsRenderOrder(false);

					// Render lines and vertices
					renderer.RenderLinedefSet(General.Map.Map.Linedefs);
					renderer.RenderVerticesSet(General.Map.Map.Vertices);

					// Show the point
					renderer.RenderVertexAt(v, c);

					// Done
					renderer.Finish();
				}

				// Wait a bit
				Thread.Sleep(100);
			}
		}

		// This shows a line
		private void ShowLine(Vector2D v1, Vector2D v2, PixelColor c)
		{
			// Start with a clear display
			if(renderer.Start(true, true))
			{
				// Do not show things
				renderer.SetThingsRenderOrder(false);

				// Render lines and vertices
				renderer.RenderLinedefSet(General.Map.Map.Linedefs);
				renderer.RenderVerticesSet(General.Map.Map.Vertices);

				// Show the line
				renderer.RenderLine(v1, v2, c);

				// Done
				renderer.Finish();
			}

			// Wait a bit
			Thread.Sleep(200);
		}

		// This shows a polygon
		private void ShowPolygon(Polygon p, PixelColor c)
		{
			for(int a = 0; a < 6; a++)
			{
				RedrawDisplay();
				Thread.Sleep(10);
				
				// Start with a clear display
				if(renderer.Start(true, true))
				{
					// Do not show things
					renderer.SetThingsRenderOrder(false);

					// Render lines and vertices
					renderer.RenderLinedefSet(General.Map.Map.Linedefs);
					renderer.RenderVerticesSet(General.Map.Map.Vertices);

					// Go for all vertices in the polygon
					for(int i = 1; i < p.Count; i++)
					{
						// Show the line
						renderer.RenderLine(p[i - 1], p[i], c);
					}
					
					// Show last line as well
					renderer.RenderLine(p[p.Count - 1], p[0], c);
					
					// Done
					renderer.Finish();
				}

				// Wait a bit
				Thread.Sleep(100);
			}
		}

		// This shows a polygon
		private void ShowEarClip(EarClipVertex[] found, LinkedList<EarClipVertex> remains)
		{
			EarClipVertex prev, first;
			
			for(int a = 0; a < 5; a++)
			{
				// Start with a clear display
				if(renderer.Start(true, true))
				{
					// Do not show things
					renderer.SetThingsRenderOrder(false);

					// Render lines and vertices
					renderer.RenderLinedefSet(General.Map.Map.Linedefs);
					renderer.RenderVerticesSet(General.Map.Map.Vertices);

					// Go for all remaining vertices
					prev = null; first = null;
					foreach(EarClipVertex v in remains)
					{
						// Show the line
						if(prev != null) renderer.RenderLine(v.Position, prev.Position, PixelColor.FromColor(Color.OrangeRed));
						if(prev == null) first = v;
						prev = v;
						
						if(v.IsReflex)
							renderer.RenderVertexAt(v.Position, ColorCollection.SELECTION);
						else
							renderer.RenderVertexAt(v.Position, ColorCollection.VERTICES);
					}
					if(first != null) renderer.RenderLine(first.Position, prev.Position, PixelColor.FromColor(Color.OrangeRed));

					if(found != null)
					{
						renderer.RenderLine(found[0].Position, found[1].Position, PixelColor.FromColor(Color.SkyBlue));
						renderer.RenderLine(found[1].Position, found[2].Position, PixelColor.FromColor(Color.SkyBlue));
						renderer.RenderLine(found[2].Position, found[0].Position, PixelColor.FromColor(Color.SkyBlue));
						renderer.RenderVertexAt(found[1].Position, ColorCollection.ASSOCIATION);
					}
					
					// Done
					renderer.Finish();
				}
				Thread.Sleep(10);

				// Start with a clear display
				if(renderer.Start(true, true))
				{
					// Do not show things
					renderer.SetThingsRenderOrder(false);

					// Render lines and vertices
					renderer.RenderLinedefSet(General.Map.Map.Linedefs);
					renderer.RenderVerticesSet(General.Map.Map.Vertices);

					// Go for all remaining vertices
					prev = null; first = null;
					foreach(EarClipVertex v in remains)
					{
						// Show the line
						if(prev != null) renderer.RenderLine(v.Position, prev.Position, PixelColor.FromColor(Color.OrangeRed));
						if(prev == null) first = v;
						prev = v;

						if(v.IsReflex)
							renderer.RenderVertexAt(v.Position, ColorCollection.SELECTION);
						else
							renderer.RenderVertexAt(v.Position, ColorCollection.VERTICES);
					}
					if(first != null) renderer.RenderLine(first.Position, prev.Position, PixelColor.FromColor(Color.OrangeRed));

					// Done
					renderer.Finish();
				}
				Thread.Sleep(20);
			}
		}
		
		#endregion
	}

	#endif
}
