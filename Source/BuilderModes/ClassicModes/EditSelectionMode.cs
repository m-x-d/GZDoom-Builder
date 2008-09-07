
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
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Edit Selection",
			  SwitchAction = "editselectionmode",	// Action name used to switch to this mode
			  ButtonDesc = "Edit Selection Mode",	// Description on the button in toolbar/menu
			  ButtonImage = "LinesMode.png",		// Image resource name for the button
			  Volatile = true,						
			  ButtonOrder = int.MinValue + 210)]	// Position of the button (lower is more to the left)

	public class EditSelectionMode : ClassicMode
	{
		#region ================== Enums

		private enum ModifyMode : int
		{
			None,
			Dragging,
			Resizing
		}

		private enum Grip : int
		{
			None,
			Main,
			SizeN,
			SizeS,
			SizeE,
			SizeW,
			RotateLT,
			RotateRT,
			RotateRB,
			RotateLB
		}

		#endregion

		#region ================== Constants

		private const float GRIP_SIZE = 11.0f;
		private const float BORDER_PADDING = 5.0f;
		private readonly Cursor[] RESIZE_CURSORS = { Cursors.SizeNS, Cursors.SizeNESW, Cursors.SizeWE, Cursors.SizeNWSE };
		
		#endregion

		#region ================== Variables

		// Selection
		private ICollection<Vertex> selectedvertices;
		private ICollection<Thing> selectedthings;
		private List<Vector2D> vertexpos;
		private List<Vector2D> thingpos;

		// Modification
		private float rotation;
		private Vector2D offset;
		private Vector2D size;
		private Vector2D baseoffset;
		private Vector2D basesize;
		
		// Modifying Modes
		private ModifyMode mode;
		private Vector2D dragposition;
		private Vector2D resizefilter;
		private Vector2D resizevector;
		private Line2D resizeaxis;
		private bool adjustoffset;
		
		// Rectangle components
		private Vector2D[] corners;
		private RectangleF[] resizegrips;	// top, right, bottom, left
		private RectangleF[] rotategrips;   // lefttop, righttop, rightbottom, leftbottom
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public EditSelectionMode()
		{
			// Initialize
			mode = ModifyMode.None;
			
			// TEST:
			rotation = Angle2D.PI2 * 0.01f;
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

		#region ================== Events

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Return to this mode
			General.Map.ChangeMode(new LinedefsMode());
		}

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();

			// Convert geometry selection into marked vertices
			General.Map.Map.ClearAllMarks();
			General.Map.Map.MarkSelectedVertices(true, true);
			General.Map.Map.MarkSelectedThings(true, true);
			General.Map.Map.MarkSelectedLinedefs(true, true);
			ICollection<Vertex> verts = General.Map.Map.GetVerticesFromLinesMarks(true);
			foreach(Vertex v in verts) v.Marked = true;
			selectedvertices = General.Map.Map.GetMarkedVertices(true);
			selectedthings = General.Map.Map.GetMarkedThings(true);

			// Make sure everything is selected so that it turns up red
			foreach(Vertex v in selectedvertices) v.Selected = true;
			ICollection<Linedef> markedlines = General.Map.Map.LinedefsFromMarkedVertices(false, true, false);
			foreach(Linedef l in markedlines) l.Selected = true;
			
			// Array to keep original coordinates
			vertexpos = new List<Vector2D>(selectedvertices.Count);
			thingpos = new List<Vector2D>(selectedthings.Count);

			// A selection must be made!
			if((selectedvertices.Count > 0) || (selectedthings.Count > 0))
			{
				// Initialize offset and size
				offset.x = float.MaxValue;
				offset.y = float.MaxValue;
				Vector2D right;
				right.x = float.MinValue;
				right.y = float.MinValue;
				
				foreach(Vertex v in selectedvertices)
				{
					// Calculate offset and size
					if(v.Position.x < offset.x) offset.x = v.Position.x;
					if(v.Position.y < offset.y) offset.y = v.Position.y;
					if(v.Position.x > right.x) right.x = v.Position.x;
					if(v.Position.y > right.y) right.y = v.Position.y;
					
					// Keep original coordinates
					vertexpos.Add(v.Position);
				}

				foreach(Thing t in selectedthings)
				{
					// Calculate offset and size
					if(t.Position.x < offset.x) offset.x = t.Position.x;
					if(t.Position.y < offset.y) offset.y = t.Position.y;
					if(t.Position.x > right.x) right.x = t.Position.x;
					if(t.Position.y > right.y) right.y = t.Position.y;

					// Keep original coordinates
					thingpos.Add(t.Position);
				}

				size = right - offset;
				basesize = size;
				baseoffset = offset;

				// Set presentation
				if(selectedthings.Count > 0)
					renderer.SetPresentation(Presentation.Things);
				else
					renderer.SetPresentation(Presentation.Standard);

				// Update
				UpdateRectangleComponents();
			}
			else
			{
				General.Interface.DisplayWarning("Please make a selection first!");

				// Cancel now
				General.Map.CancelMode();
			}
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();
			
			// Hide highlight info
			General.Interface.HideInfo();
			General.Interface.SetCursor(Cursors.Default);
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			UpdateRectangleComponents();
			
			// Render lines
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
				renderer.Finish();
			}

			// Render selection
			if(renderer.StartOverlay(true))
			{
				renderer.RenderLine(corners[0], corners[1], 1, General.Colors.Vertices, true);
				renderer.RenderLine(corners[1], corners[2], 1, General.Colors.Highlight, true);
				renderer.RenderLine(corners[2], corners[3], 1, General.Colors.Highlight, true);
				renderer.RenderLine(corners[3], corners[0], 1, General.Colors.Highlight, true);
				for(int i = 0; i < 4; i++)
				{
					renderer.RenderRectangleFilled(resizegrips[i], General.Colors.Background, true);
					renderer.RenderRectangle(resizegrips[i], 2, General.Colors.Highlight, true);
					renderer.RenderRectangleFilled(rotategrips[i], General.Colors.Background, true);
					renderer.RenderRectangle(rotategrips[i], 2, General.Colors.Indication, true);
				}
				renderer.RenderRectangle(rotategrips[0], 2, General.Colors.Vertices, true);
				renderer.Finish();
			}

			renderer.Present();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			
			// Not in any modifying mode?
			if(mode == ModifyMode.None)
			{
				// Check what grip the mouse is over
				// and change cursor accordingly
				Grip mousegrip = CheckMouseGrip();
				switch(mousegrip)
				{
					case Grip.Main:
						General.Interface.SetCursor(Cursors.Hand);
						break;

					case Grip.RotateLB:
					case Grip.RotateLT:
					case Grip.RotateRB:
					case Grip.RotateRT:
						General.Interface.SetCursor(Cursors.Cross);
						break;

					case Grip.SizeE:
					case Grip.SizeS:
					case Grip.SizeW:
					case Grip.SizeN:

						// Pick the best matching cursor depending on rotation and side
						float resizeangle = rotation;
						if((mousegrip == Grip.SizeE) || (mousegrip == Grip.SizeW)) resizeangle += Angle2D.PIHALF;
						resizeangle = Angle2D.Normalized(resizeangle);
						if(resizeangle > Angle2D.PI) resizeangle -= Angle2D.PI;
						resizeangle = Math.Abs(resizeangle + Angle2D.PI / 8.000001f);
						int cursorindex = (int)Math.Floor((resizeangle / Angle2D.PI) * 4.0f) % 4;
						General.Interface.SetCursor(RESIZE_CURSORS[cursorindex]);
						break;

					default:
						General.Interface.SetCursor(Cursors.Default);
						break;
				}
			}
			else
			{
				// Check what modifying mode we are in
				switch(mode)
				{
					// Dragging
					case ModifyMode.Dragging:
						
						// Change offset
						offset += mousemappos - dragposition;
						dragposition = mousemappos;

						// Update
						UpdateGeometry();
						UpdateRectangleComponents();
						General.Interface.RedrawDisplay();
						break;

					// Resizing
					case ModifyMode.Resizing:

						// Adjust offset if needed
						Vector2D oldsize = size;
						
						// Change size
						float scale = resizeaxis.GetNearestOnLine(mousemappos);
						size = (basesize * resizefilter) * scale + (1.0f - resizefilter) * basesize;

						// Adjust offset if needed
						if(adjustoffset) offset -= (size - oldsize) * resizevector / basesize.y;
						
						// Update
						UpdateGeometry();
						UpdateRectangleComponents();
						General.Interface.RedrawDisplay();
						break;
				}
			}
		}

		// Mouse leaves the display
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			
			// Reset cursor
			General.Interface.SetCursor(Cursors.Default);
		}

		// When select button is pressed
		protected override void OnSelect()
		{
			base.OnSelect();

			// Check what grip the mouse is over
			switch(CheckMouseGrip())
			{
				// Drag main rectangle
				case Grip.Main:
					dragposition = mousemappos;
					mode = ModifyMode.Dragging;
					break;

				// Resize
				case Grip.SizeN:

					// Make the resize axis. This is a line with the length and direction
					// of basesize used to calculate the resize percentage.
					resizevector = corners[2] - corners[1];
					resizevector = resizevector.GetNormal() * basesize.y;
					resizeaxis = new Line2D(corners[2], corners[2] + resizevector);
					resizefilter = new Vector2D(0.0f, 1.0f);
					adjustoffset = true;
					mode = ModifyMode.Resizing;
					break;

			}
		}

		// When selected button is released
		protected override void OnEndSelect()
		{
			base.OnEndSelect();

			// Check what modifying mode we are in
			switch(mode)
			{
				case ModifyMode.Dragging:
					General.Interface.RedrawDisplay();
					break;
			}
			
			mode = ModifyMode.None;
		}

		#endregion
		
		#region ================== Methods
		
		// This checks and returns the grip the mouse pointer is in
		private Grip CheckMouseGrip()
		{
			// Make polygon from corners
			Polygon rectpoly = new Polygon();
			rectpoly.AddRange(corners);

			if(PointInRectF(resizegrips[0], mousemappos))
				return Grip.SizeN;
			else if(PointInRectF(resizegrips[2], mousemappos))
				return Grip.SizeS;
			else if(PointInRectF(resizegrips[1], mousemappos))
				return Grip.SizeE;
			else if(PointInRectF(resizegrips[3], mousemappos))
				return Grip.SizeW;
			else if(PointInRectF(rotategrips[0], mousemappos))
				return Grip.RotateLT;
			else if(PointInRectF(rotategrips[1], mousemappos))
				return Grip.RotateRT;
			else if(PointInRectF(rotategrips[2], mousemappos))
				return Grip.RotateRB;
			else if(PointInRectF(rotategrips[3], mousemappos))
				return Grip.RotateLB;
			else if(rectpoly.Intersect(mousemappos))
				return Grip.Main;
			else
				return Grip.None;
		}
		
		// This applies the current rotation and resize to a point
		private Vector2D TransformedPoint(Vector2D p)
		{
			// Resize
			p = (p - baseoffset) * (size / basesize) + baseoffset;
			
			// Rotate around center
			Vector2D center = baseoffset + basesize * 0.5f;
			Vector2D po = p - center;
			p.x = (float)Math.Cos(rotation) * po.x + (float)Math.Sin(rotation) * po.y;
			p.y = (float)Math.Sin(rotation) * -po.x + (float)Math.Cos(rotation) * po.y;
			p += center;
			
			// Move
			p += offset - baseoffset;
			
			return p;
		}
		
		// This checks if a point is in a rect
		private bool PointInRectF(RectangleF rect, Vector2D point)
		{
			return (point.x >= rect.Left) && (point.x <= rect.Right) && (point.y >= rect.Top) && (point.y <= rect.Bottom);
		}

		// This moves all things and vertices to match the current transformation
		private void UpdateGeometry()
		{
			int index = 0;
			foreach(Vertex v in selectedvertices)
			{
				v.Move(TransformedPoint(vertexpos[index++]));
			}
			
			index = 0;
			foreach(Thing t in selectedthings)
			{
				t.Move(TransformedPoint(thingpos[index++]));
			}

			General.Map.Map.Update(true, false);
		}
		
		// This updates the selection rectangle components
		private void UpdateRectangleComponents()
		{
			float border = BORDER_PADDING / renderer.Scale;
			float gripsize = GRIP_SIZE / renderer.Scale;

			// Corners
			corners = new Vector2D[4];
			corners[0] = TransformedPoint(new Vector2D(baseoffset.x - border, baseoffset.y - border));
			corners[1] = TransformedPoint(new Vector2D(baseoffset.x - border + basesize.x + border * 2, baseoffset.y - border));
			corners[2] = TransformedPoint(new Vector2D(baseoffset.x - border + basesize.x + border * 2, baseoffset.y - border + basesize.y + border * 2));
			corners[3] = TransformedPoint(new Vector2D(baseoffset.x - border, baseoffset.y - border + basesize.y + border * 2));

			// Middle points between corners
			Vector2D middle01 = corners[0] + (corners[1] - corners[0]) * 0.5f;
			Vector2D middle12 = corners[1] + (corners[2] - corners[1]) * 0.5f;
			Vector2D middle23 = corners[2] + (corners[3] - corners[2]) * 0.5f;
			Vector2D middle30 = corners[3] + (corners[0] - corners[3]) * 0.5f;
			
			// Resize grips
			resizegrips = new RectangleF[4];
			resizegrips[0] = new RectangleF(middle01.x - gripsize * 0.5f,
											middle01.y - gripsize * 0.5f,
											gripsize, gripsize);
			resizegrips[1] = new RectangleF(middle12.x - gripsize * 0.5f,
											middle12.y - gripsize * 0.5f,
											gripsize, gripsize);
			resizegrips[2] = new RectangleF(middle23.x - gripsize * 0.5f,
											middle23.y - gripsize * 0.5f,
											gripsize, gripsize);
			resizegrips[3] = new RectangleF(middle30.x - gripsize * 0.5f,
											middle30.y - gripsize * 0.5f,
											gripsize, gripsize);

			// Rotate grips
			rotategrips = new RectangleF[4];
			rotategrips[0] = new RectangleF(corners[0].x - gripsize * 0.5f,
											corners[0].y - gripsize * 0.5f,
											gripsize, gripsize);
			rotategrips[1] = new RectangleF(corners[1].x - gripsize * 0.5f,
											corners[1].y - gripsize * 0.5f,
											gripsize, gripsize);
			rotategrips[2] = new RectangleF(corners[2].x - gripsize * 0.5f,
											corners[2].y - gripsize * 0.5f,
											gripsize, gripsize);
			rotategrips[3] = new RectangleF(corners[3].x - gripsize * 0.5f,
											corners[3].y - gripsize * 0.5f,
											gripsize, gripsize);
		}
		
		#endregion
	}
}
