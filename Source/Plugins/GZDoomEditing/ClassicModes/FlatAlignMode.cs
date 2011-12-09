
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
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using System.Drawing;
using CodeImp.DoomBuilder.Actions;

#endregion

namespace CodeImp.DoomBuilder.GZDoomEditing
{
	public abstract class FlatAlignMode : BaseClassicMode
	{
		#region ================== Constants

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
			SizeV,
			SizeH,
			RotateRT,
			RotateLB
		}

		protected struct SectorInfo
		{
			public float rotation;
			public Vector2D scale;
			public Vector2D offset;
		}

		private const float GRIP_SIZE = 9.0f;
		private readonly Cursor[] RESIZE_CURSORS = { Cursors.SizeNS, Cursors.SizeNWSE, Cursors.SizeWE, Cursors.SizeNESW };
		
		#endregion

		#region ================== Variables

		private ICollection<Sector> selection;
		protected Sector editsector;
		protected IList<SectorInfo> sectorinfo;
		private ImageData texture;
		private Vector2D selectionoffset;
		private ModifyMode mode;
		private bool autopanning;
		
		// Modification
		private float rotation;
		private Vector2D scale;
		private Vector2D offset;

		// Rectangle components
		private Vector2D[] corners = new Vector2D[4]; // lefttop, righttop, rightbottom, leftbottom
		private Vector2D[] extends = new Vector2D[2]; // right, bottom
		private RectangleF[] resizegrips = new RectangleF[2];	// right, bottom
		private RectangleF[] rotategrips = new RectangleF[2];   // righttop, leftbottom
		
		// Aligning
		private RectangleF alignrect;
		private Vector2D dragalignoffset;
		private Vector2D dragoffset;
		
		#endregion

		#region ================== Properties

		public abstract string XScaleName { get; }
		public abstract string YScaleName { get; }
		public abstract string XOffsetName { get; }
		public abstract string YOffsetName { get; }
		public abstract string RotationName { get; }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		protected FlatAlignMode()
		{
		}

		#endregion

		#region ================== Methods

		protected abstract ImageData GetTexture(Sector editsector);

		// This checks if a point is in a rect
		private bool PointInRectF(RectangleF rect, Vector2D point)
		{
			return (point.x >= rect.Left) && (point.x <= rect.Right) && (point.y >= rect.Top) && (point.y <= rect.Bottom);
		}

		// Transforms p from Texture space into World space
		protected Vector2D TexToWorld(Vector2D p)
		{
			return TexToWorld(p, sectorinfo[0]);
		}
		
		// Transforms p from Texture space into World space
		protected Vector2D TexToWorld(Vector2D p, SectorInfo s)
		{
			p /= scale + s.scale;
			p -= offset + s.offset;
			p = p.GetRotated(-(rotation + s.rotation));
			return p;
		}

		// Transforms p from World space into Texture space
		protected Vector2D WorldToTex(Vector2D p)
		{
			return WorldToTex(p, sectorinfo[0]);
		}
		
		// Transforms p from World space into Texture space
		protected Vector2D WorldToTex(Vector2D p, SectorInfo s)
		{
			p = p.GetRotated(rotation + s.rotation);
			p += offset + s.offset;
			p *= scale + s.scale;
			return p;
		}

		// This updates all sectors
		private void UpdateSectors()
		{
			int index = 0;
			foreach(Sector s in selection)
			{
				SectorInfo si = sectorinfo[index];
				s.Fields.BeforeFieldsChange();
				s.Fields[RotationName] = new UniValue(UniversalType.AngleDegreesFloat, Angle2D.RadToDeg(si.rotation + rotation));
				s.Fields[XScaleName] = new UniValue(UniversalType.Float, si.scale.x + scale.x);
				s.Fields[YScaleName] = new UniValue(UniversalType.Float, si.scale.y + scale.y);
				s.Fields[XOffsetName] = new UniValue(UniversalType.Float, si.offset.x + offset.x);
				s.Fields[YOffsetName] = new UniValue(UniversalType.Float, -(si.offset.y + offset.y));
				index++;
				s.UpdateNeeded = true;
				s.UpdateCache();
			}
		}
		
		// This updates the selection
		private void Update()
		{
			// Not in any modifying mode?
			if(mode == ModifyMode.None)
			{
				Vector2D prevdragoffset = dragalignoffset;
				dragalignoffset = new Vector2D(-2f, -2f);
				
				// Check what grip the mouse is over
				// and change cursor accordingly
				Grip mousegrip = CheckMouseGrip();
				switch(mousegrip)
				{
					case Grip.Main:
						int closestcorner = -1;
						float cornerdist = float.MinValue;
						for(int i = 0; i < 4; i++)
						{
							Vector2D delta = corners[i] - mousemappos;
							float d = delta.GetLengthSq();
							if(d < cornerdist)
							{
								closestcorner = i;
								cornerdist = d;
							}
						}
						switch(closestcorner)
						{
							// TODO:
							case 0: dragalignoffset = new Vector2D(0f, 0f); break;
							case 1: dragalignoffset = new Vector2D(0f, 0f); break;
							case 2: dragalignoffset = new Vector2D(0f, 0f); break;
							case 3: dragalignoffset = new Vector2D(0f, 0f); break;
						}
						General.Interface.SetCursor(Cursors.Hand);
						break;

					case Grip.RotateLB:
					case Grip.RotateRT:
						General.Interface.SetCursor(Cursors.Cross);
						break;

					case Grip.SizeH:
					case Grip.SizeV:
						// Pick the best matching cursor depending on rotation and side
						float resizeangle = rotation;
						if(mousegrip == Grip.SizeH) resizeangle += Angle2D.PIHALF;
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

				if(prevdragoffset != dragalignoffset)
					General.Interface.RedrawDisplay();
			}
			else
			{
				// Change to crosshair cursor so we can clearly see around the mouse cursor
				General.Interface.SetCursor(Cursors.Cross);
				
				// Check what modifying mode we are in
				switch(mode)
				{
					case ModifyMode.Dragging:
						offset = new Vector2D();
						offset = WorldToTex(mousemappos) - WorldToTex(dragoffset);
						break;

					case ModifyMode.Resizing:

						break;

					case ModifyMode.Rotating:

						break;
				}
				
				UpdateSectors();
				General.Interface.RedrawDisplay();
			}
		}
		
		// This updates the selection rectangle components
		private void UpdateRectangleComponents()
		{
			float gripsize = GRIP_SIZE / renderer.Scale;

			// Corners in world space
			corners[0] = TexToWorld(selectionoffset + new Vector2D(0f, 0f));
			corners[1] = TexToWorld(selectionoffset + new Vector2D(texture.ScaledWidth, 0f));
			corners[2] = TexToWorld(selectionoffset + new Vector2D(texture.ScaledWidth, -texture.ScaledHeight));
			corners[3] = TexToWorld(selectionoffset + new Vector2D(0f, -texture.ScaledHeight));

			// Extended points for rotation corners
			extends[0] = TexToWorld(selectionoffset + new Vector2D(texture.ScaledWidth + 20f / renderer.Scale * (scale.x + sectorinfo[0].scale.x), 0f));
			extends[1] = TexToWorld(selectionoffset + new Vector2D(0f, -texture.ScaledHeight + -20f / renderer.Scale * (scale.y + sectorinfo[0].scale.y)));

			// Middle points between corners
			Vector2D middle12 = corners[1] + (corners[2] - corners[1]) * 0.5f;
			Vector2D middle23 = corners[2] + (corners[3] - corners[2]) * 0.5f;
			
			// Resize grips
			resizegrips[0] = new RectangleF(middle12.x - gripsize * 0.5f,
											middle12.y - gripsize * 0.5f,
											gripsize, gripsize);
			resizegrips[1] = new RectangleF(middle23.x - gripsize * 0.5f,
											middle23.y - gripsize * 0.5f,
											gripsize, gripsize);

			// Rotate grips
			rotategrips[0] = new RectangleF(extends[0].x - gripsize * 0.5f,
											extends[0].y - gripsize * 0.5f,
											gripsize, gripsize);
			rotategrips[1] = new RectangleF(extends[1].x - gripsize * 0.5f,
											extends[1].y - gripsize * 0.5f,
											gripsize, gripsize);
			
			Vector2D worldalignoffset = TexToWorld(selectionoffset + dragalignoffset);
			alignrect = new RectangleF(worldalignoffset.x - gripsize * 0.25f,
									   worldalignoffset.y - gripsize * 0.25f,
									   gripsize * 0.5f, gripsize * 0.5f);
		}

		// This checks and returns the grip the mouse pointer is in
		private Grip CheckMouseGrip()
		{
			if(PointInRectF(resizegrips[0], mousemappos))
				return Grip.SizeH;
			else if(PointInRectF(resizegrips[1], mousemappos))
				return Grip.SizeV;
			else if(PointInRectF(rotategrips[0], mousemappos))
				return Grip.RotateRT;
			else if(PointInRectF(rotategrips[1], mousemappos))
				return Grip.RotateLB;
			else if(Tools.PointInPolygon(corners, mousemappos))
				return Grip.Main;
			else
				return Grip.None;
		}
		
		#endregion

		#region ================== Events

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();

			// Presentation
			renderer.SetPresentation(Presentation.Standard);

			// Selection
			General.Map.Map.ConvertSelection(SelectionType.Sectors);
			General.Map.Map.SelectionType = SelectionType.Sectors;
			if(General.Map.Map.SelectedSectorsCount == 0)
			{
				// Find the nearest linedef within highlight range
				Linedef l = General.Map.Map.NearestLinedef(mousemappos);
				if(l != null)
				{
					Sector selectsector = null;
					
					// Check on which side of the linedef the mouse is and which sector there is
					float side = l.SideOfLine(mousemappos);
					if((side > 0) && (l.Back != null))
						selectsector = l.Back.Sector;
					else if((side <= 0) && (l.Front != null))
						selectsector = l.Front.Sector;

					// Select the sector!
					if(selectsector != null)
					{
						selectsector.Selected = true;
						foreach(Sidedef sd in selectsector.Sidedefs)
							sd.Line.Selected = true;
					}
				}
			}
			
			// Get sector selection
			selection = General.Map.Map.GetSelectedSectors(true);
			if(selection.Count == 0)
			{
				General.Interface.MessageBeep(MessageBeepType.Default);
				General.Interface.DisplayStatus(StatusType.Info, "A selected sector is required for this action.");
				General.Editing.CancelMode();
				return;
			}
			editsector = General.GetByIndex(selection, 0);

			// Get the texture
			texture = GetTexture(editsector);
			if((texture == null) || (texture == General.Map.Data.WhiteTexture) ||
			   (texture.Width <= 0) || (texture.Height <= 0) || !texture.IsImageLoaded)
			{
				General.Interface.MessageBeep(MessageBeepType.Default);
				General.Interface.DisplayStatus(StatusType.Info, "The selected sector must have a loaded texture to align.");
				General.Editing.CancelMode();
				return;
			}
			
			// Cache the transformation values
			sectorinfo = new List<SectorInfo>(selection.Count);
			foreach(Sector s in selection)
			{
				SectorInfo si;
				si.rotation = Angle2D.DegToRad(editsector.Fields.GetValue(RotationName, 0.0f));
				si.scale.x = editsector.Fields.GetValue(XScaleName, 1.0f);
				si.scale.y = editsector.Fields.GetValue(YScaleName, 1.0f);
				si.offset.x = editsector.Fields.GetValue(XOffsetName, 0.0f);
				si.offset.y = -editsector.Fields.GetValue(YOffsetName, 0.0f);
				sectorinfo.Add(si);
			}

			// We use the transformation of the first selected sector to work with
			rotation = sectorinfo[0].rotation;
			scale = sectorinfo[0].scale;
			offset = sectorinfo[0].offset;
			sectorinfo[0] = new SectorInfo();

			// We want the texture corner nearest to the center of the sector
			Vector2D fp;
			fp.x = (editsector.BBox.Left + editsector.BBox.Right) / 2;
			fp.y = (editsector.BBox.Top + editsector.BBox.Bottom) / 2;

			// Transform the point into texture space
			fp = WorldToTex(fp);
			
			// Snap to the nearest left-top corner
			fp.x = (float)Math.Round(fp.x / texture.ScaledWidth) * texture.ScaledWidth;
			fp.y = (float)Math.Round(fp.y / texture.ScaledHeight) * texture.ScaledHeight;
			selectionoffset = fp;

			UpdateRectangleComponents();
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Hide highlight info
			General.Interface.SetCursor(Cursors.Default);
			General.Interface.HideInfo();
		}

		// Cancel mode
		public override void OnCancel()
		{
			base.OnCancel();

			General.Map.Map.Update(true, true);

			// Return to previous stable mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			Update();
		}

		// Mouse leaves the display
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			// Reset cursor
			General.Interface.SetCursor(Cursors.Default);
		}


		// When edit button is pressed
		protected override void OnEditBegin()
		{
			base.OnEditBegin();
			OnSelectBegin();
		}

		// When edit button is released
		protected override void OnEditEnd()
		{
			base.OnEditEnd();
			OnSelectEnd();
		}

		// When select button is pressed
		protected override void OnSelectBegin()
		{
			base.OnSelectBegin();

			if(mode != ModifyMode.None) return;

			// Used in many cases
			Vector2D delta;

			// Check what grip the mouse is over
			switch(CheckMouseGrip())
			{
				// Drag main rectangle
				case Grip.Main:

					dragoffset = mousemappos - TexToWorld(offset);
					mode = ModifyMode.Dragging;

					EnableAutoPanning();
					autopanning = true;
					break;

				// Outside the selection?
				default:
					// Accept and be done with it
					General.Editing.AcceptMode();
					break;
			}
		}

		// When selected button is released
		protected override void OnSelectEnd()
		{
			base.OnSelectEnd();

			if(autopanning)
			{
				DisableAutoPanning();
				autopanning = false;
			}

			// No modifying mode
			mode = ModifyMode.None;

			// Redraw
			General.Map.Map.Update();
			General.Interface.RedrawDisplay();
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			UpdateRectangleComponents();

			renderer.RedrawSurface();

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

			// Render overlay
			if(renderer.StartOverlay(true))
			{
				renderer.RenderLine(corners[0], extends[0], 1f, General.Colors.Highlight, true);
				renderer.RenderLine(corners[0], extends[1], 1f, General.Colors.Highlight, true);
				renderer.RenderLine(corners[1], corners[2], 0.5f, General.Colors.Highlight, true);
				renderer.RenderLine(corners[2], corners[3], 0.5f, General.Colors.Highlight, true);
				renderer.RenderRectangleFilled(rotategrips[0], General.Colors.Background, true);
				renderer.RenderRectangleFilled(rotategrips[1], General.Colors.Background, true);
				renderer.RenderRectangle(rotategrips[0], 2f, General.Colors.Indication, true);
				renderer.RenderRectangle(rotategrips[1], 2f, General.Colors.Indication, true);
				renderer.RenderRectangleFilled(resizegrips[0], General.Colors.Background, true);
				renderer.RenderRectangleFilled(resizegrips[1], General.Colors.Background, true);
				renderer.RenderRectangle(resizegrips[0], 2f, General.Colors.Highlight, true);
				renderer.RenderRectangle(resizegrips[1], 2f, General.Colors.Highlight, true);
				renderer.RenderRectangleFilled(alignrect, General.Colors.Selection, true);
				renderer.Finish();
			}

			renderer.Present();
		}

		#endregion

		#region ================== Actions

		// This clears the selection
		[BeginAction("clearselection", BaseAction = true)]
		public void ClearSelection()
		{
			// Accept changes
			General.Editing.AcceptMode();
			General.Map.Map.ClearAllSelected();
			General.Interface.RedrawDisplay();
		}

		#endregion
	}
}
