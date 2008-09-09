
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
			Resizing,
			Rotating
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
		private readonly Cursor[] RESIZE_CURSORS = { Cursors.SizeNS, Cursors.SizeNESW, Cursors.SizeWE, Cursors.SizeNWSE };
		
		#endregion

		#region ================== Variables

		// Mode to return to
		private EditMode basemode;
		private bool modealreadyswitching = false;
		
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
		private int stickcorner;
		private float rotategripangle;
		
		// Rectangle components
		private Vector2D[] originalcorners;
		private Vector2D[] corners;
		private FlatVertex[] cornerverts;
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
			basemode = General.Map.Mode;
			mode = ModifyMode.None;
			
			// TEST:
			rotation = Angle2D.PI2 * 0;// 0.02f;
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

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			// Reset geometry in original position
			int index = 0;
			foreach(Vertex v in selectedvertices)
				v.Move(vertexpos[index++]);

			index = 0;
			foreach(Thing t in selectedthings)
				t.Move(thingpos[index++]);
			
			General.Map.Map.Update(true, true);
			
			// Return to original mode
			Type mt = basemode.GetType();
			basemode = (EditMode)Activator.CreateInstance(mt);
			General.Map.ChangeMode(basemode);
		}

		// When accepted
		public override void OnAccept()
		{
			base.OnAccept();
			
			// Reset geometry in original position
			int index = 0;
			foreach(Vertex v in selectedvertices)
				v.Move(vertexpos[index++]);

			index = 0;
			foreach(Thing t in selectedthings)
				t.Move(thingpos[index++]);

			// Make undo
			General.Map.UndoRedo.CreateUndo("Edit selection", UndoGroup.None, 0);

			// Move geometry to new position
			UpdateGeometry();
			General.Map.Map.Update(true, true);

			if(!modealreadyswitching)
			{
				// Return to original mode
				Type mt = basemode.GetType();
				basemode = (EditMode)Activator.CreateInstance(mt);
				General.Map.ChangeMode(basemode);
			}
		}
		
		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			// When not cancelled manually, we assume it is accepted
			if(!cancelled)
			{
				modealreadyswitching = true;
				this.OnAccept();
			}
			
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
				renderer.RenderGeometry(cornerverts, null, true);
				renderer.RenderLine(corners[0], corners[1], 4, General.Colors.Highlight.WithAlpha(100), true);
				renderer.RenderLine(corners[1], corners[2], 4, General.Colors.Highlight.WithAlpha(100), true);
				renderer.RenderLine(corners[2], corners[3], 4, General.Colors.Highlight.WithAlpha(100), true);
				renderer.RenderLine(corners[3], corners[0], 4, General.Colors.Highlight.WithAlpha(100), true);
				for(int i = 0; i < 4; i++)
				{
					renderer.RenderRectangleFilled(resizegrips[i], General.Colors.Background, true);
					renderer.RenderRectangle(resizegrips[i], 2, General.Colors.Highlight, true);
					renderer.RenderRectangleFilled(rotategrips[i], General.Colors.Background, true);
					renderer.RenderRectangle(rotategrips[i], 2, General.Colors.Indication, true);
				}
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

						// Keep corner position
						Vector2D oldcorner = corners[stickcorner];
						
						// Change size
						float scale = resizeaxis.GetNearestOnLine(mousemappos);
						size = (basesize * resizefilter) * scale + size * (1.0f - resizefilter);

						// Adjust corner position
						Vector2D newcorner = TransformedPoint(originalcorners[stickcorner]);
						offset -= newcorner - oldcorner;
						
						// Update
						UpdateGeometry();
						UpdateRectangleComponents();
						General.Interface.RedrawDisplay();
						break;

					// Rotating
					case ModifyMode.Rotating:

						// Get angle from mouse to center
						Vector2D center = offset + size * 0.5f;
						Vector2D delta = mousemappos - center;
						rotation = delta.GetAngle() - rotategripangle;

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

			// Used in many cases:
			Vector2D center = offset + size * 0.5f;
			Vector2D delta;

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

					// The resize vector is a unit vector in the direction of the resize.
					// We multiply this with the sign of the current size, because the
					// corners may be reversed when the selection is flipped.
					resizevector = corners[1] - corners[2];
					resizevector = resizevector.GetNormal() * Math.Sign(size.y);
					
					// Make the resize axis. This is a line with the length and direction
					// of basesize used to calculate the resize percentage.
					resizeaxis = new Line2D(corners[2], corners[2] + resizevector * basesize.y);

					// Original axis filter
					resizefilter = new Vector2D(0.0f, 1.0f);

					// This is the corner that must stay in the same position
					stickcorner = 2;

					mode = ModifyMode.Resizing;
					break;

				// Resize
				case Grip.SizeE:
					// See description above
					resizevector = corners[1] - corners[0];
					resizevector = resizevector.GetNormal() * Math.Sign(size.x);
					resizeaxis = new Line2D(corners[0], corners[0] + resizevector * basesize.x);
					resizefilter = new Vector2D(1.0f, 0.0f);
					stickcorner = 0;
					mode = ModifyMode.Resizing;
					break;

				// Resize
				case Grip.SizeS:
					// See description above
					resizevector = corners[2] - corners[1];
					resizevector = resizevector.GetNormal() * Math.Sign(size.y);
					resizeaxis = new Line2D(corners[1], corners[1] + resizevector * basesize.y);
					resizefilter = new Vector2D(0.0f, 1.0f);
					stickcorner = 0;
					mode = ModifyMode.Resizing;
					break;

				// Resize
				case Grip.SizeW:
					// See description above
					resizevector = corners[0] - corners[1];
					resizevector = resizevector.GetNormal() * Math.Sign(size.x);
					resizeaxis = new Line2D(corners[1], corners[1] + resizevector * basesize.x);
					resizefilter = new Vector2D(1.0f, 0.0f);
					stickcorner = 1;
					mode = ModifyMode.Resizing;
					break;

				// Rotate
				case Grip.RotateLB:
					delta = corners[3] - center;
					rotategripangle = delta.GetAngle() - rotation;
					mode = ModifyMode.Rotating;
					break;

				// Rotate
				case Grip.RotateLT:
					delta = corners[0] - center;
					rotategripangle = delta.GetAngle() - rotation;
					mode = ModifyMode.Rotating;
					break;

				// Rotate
				case Grip.RotateRB:
					delta = corners[2] - center;
					rotategripangle = delta.GetAngle() - rotation;
					mode = ModifyMode.Rotating;
					break;

				// Rotate
				case Grip.RotateRT:
					delta = corners[1] - center;
					rotategripangle = delta.GetAngle() - rotation;
					mode = ModifyMode.Rotating;
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
			
			// Rotate
			Vector2D center = baseoffset + size * 0.5f;
			Vector2D po = p - center;
			p = po.GetRotated(rotation);
			p += center;
			
			// Translate
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
			float gripsize = GRIP_SIZE / renderer.Scale;

			// Original (untransformed) corners
			originalcorners = new Vector2D[4];
			originalcorners[0] = new Vector2D(baseoffset.x, baseoffset.y);
			originalcorners[1] = new Vector2D(baseoffset.x + basesize.x, baseoffset.y);
			originalcorners[2] = new Vector2D(baseoffset.x + basesize.x, baseoffset.y + basesize.y);
			originalcorners[3] = new Vector2D(baseoffset.x, baseoffset.y + basesize.y);

			// Corners
			corners = new Vector2D[4];
			for(int i = 0; i < 4; i++)
				corners[i] = TransformedPoint(originalcorners[i]);

			// Vertices
			cornerverts = new FlatVertex[6];
			for(int i = 0; i < 6; i++)
			{
				cornerverts[i] = new FlatVertex();
				cornerverts[i].z = 1.0f;
				cornerverts[i].c = General.Colors.Highlight.WithAlpha(100).ToInt();
			}
			cornerverts[0].x = corners[0].x;
			cornerverts[0].y = corners[0].y;
			cornerverts[1].x = corners[1].x;
			cornerverts[1].y = corners[1].y;
			cornerverts[2].x = corners[2].x;
			cornerverts[2].y = corners[2].y;
			cornerverts[3].x = corners[0].x;
			cornerverts[3].y = corners[0].y;
			cornerverts[4].x = corners[2].x;
			cornerverts[4].y = corners[2].y;
			cornerverts[5].x = corners[3].x;
			cornerverts[5].y = corners[3].y;
			
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
