
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

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(Volatile = true)]
	public sealed class CurveLinedefsMode : ClassicMode
	{
		#region ================== Constants

		private const float LINE_THICKNESS = 0.6f;

		#endregion

		#region ================== Variables

		// Mode to return to
		private EditMode basemode;

		// Collections
		private ICollection<Linedef> selectedlines;
		private ICollection<Linedef> unselectedlines;
		
		#endregion

		#region ================== Properties

		// Just keep the base mode button checked
		public override string EditModeButtonName { get { return basemode.GetType().Name; } }

		internal EditMode BaseMode { get { return basemode; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public CurveLinedefsMode(EditMode basemode)
		{
			this.basemode = basemode;

			// Make collections by selection
			selectedlines = General.Map.Map.GetSelectedLinedefs(true);
			unselectedlines = General.Map.Map.GetSelectedLinedefs(false);
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

		// Cancelled
		public override void OnCancel()
		{
			// Cancel base class
			base.OnCancel();

			// Return to base mode
			General.Map.ChangeMode(basemode);
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();

			// Show toolbox window
			BuilderPlug.Me.CurveLinedefsForm.Show((Form)General.Interface);
		}

		// Disenagaging
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Hide toolbox window
			BuilderPlug.Me.CurveLinedefsForm.Hide();
		}

		// This applies the curves and returns to the base mode
		public override void OnAccept()
		{
			// Create undo
			General.Map.UndoRedo.CreateUndo("Curve linedefs", UndoGroup.None, 0);
			
			// Go for all selected lines
			foreach(Linedef ld in selectedlines)
			{
				// Make curve for line
				List<Vector2D> points = GenerateCurve(ld);
				if(points.Count > 0)
				{
					// TODO: We may want some sector create/join code in here
					// to allow curves that overlap lines and some geometry merging

					// Go for all points to split the line
					Linedef splitline = ld;
					for(int i = 0; i < points.Count; i++)
					{
						// Make vertex
						Vertex v = General.Map.Map.CreateVertex(points[i]);

						// Split the line and move on with this line
						splitline = splitline.Split(v);
					}
				}
			}

			// Snap to map format accuracy
			General.Map.Map.SnapAllToAccuracy();
			
			// Update caches
			General.Map.Map.Update();
			
			// Return to base mode
			General.Map.ChangeMode(basemode);
		}
		
		// This generates the vertices to split the line with, from start to end
		private List<Vector2D> GenerateCurve(Linedef line)
		{
			// Fetch settings from window
			int vertices = BuilderPlug.Me.CurveLinedefsForm.Vertices;
			float distance = BuilderPlug.Me.CurveLinedefsForm.Distance;
			float angle = BuilderPlug.Me.CurveLinedefsForm.Angle;
			bool fixedcurve = BuilderPlug.Me.CurveLinedefsForm.FixedCurve;
			bool backwards = BuilderPlug.Me.CurveLinedefsForm.Backwards;

			// Make list
			List<Vector2D> points = new List<Vector2D>(vertices);
			
			// Anders Astrand, this is where your code goes.
			// "line" is the original linedef to create a curve from
			// "vertices" is the number of points to generate evenly along
			// the curve (excluding line start and end)
			// "distance" is the curve distance from the line (same as in Doom Builder 1)
			// "angle" is the delta angle (same as in Doom Builder 1)
			// "fixedcurve" is true when the curve must be forced circular (same as in Doom Builder 1)
			// "backwards" is true, then the curve should go towards the back side of the line
			// otherwise the curve goes to the front side of the line (like Doom Builder 1 did)
			// Return value should be a list of Vector2D points
			
			// TEST:
			points.Add(line.GetCenterPoint());

			// Done
			return points;
		}

		// Redrawing display
		public override void OnRedrawDisplay()
		{
			// Render lines
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(unselectedlines);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.SetThingsRenderOrder(false);
				renderer.RenderThingSet(General.Map.Map.Things);
				renderer.Finish();
			}

			// Render overlay
			if(renderer.StartOverlay(true))
			{
				// Go for all selected lines
				foreach(Linedef ld in selectedlines)
				{
					// Make curve for line
					List<Vector2D> points = GenerateCurve(ld);
					if(points.Count > 0)
					{
						Vector2D p1 = ld.Start.Position;
						Vector2D p2 = points[0];
						for(int i = 1; i <= points.Count; i++)
						{
							// Draw the line
							renderer.RenderLine(p1, p2, LINE_THICKNESS, General.Colors.Highlight, true);

							// Next points
							p1 = p2;
							if(i < points.Count) p2 = points[i];
						}

						// Draw last line
						renderer.RenderLine(p2, ld.End.Position, LINE_THICKNESS, General.Colors.Highlight, true);
					}
				}
				renderer.Finish();
			}

			renderer.Present();
		}
		
		#endregion
	}
}
