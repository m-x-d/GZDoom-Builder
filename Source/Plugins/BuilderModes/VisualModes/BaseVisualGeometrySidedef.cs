
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.VisualModes;
using System.Drawing;
using CodeImp.DoomBuilder.GZBuilder.Tools;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal abstract class BaseVisualGeometrySidedef : VisualGeometry, IVisualEventReceiver
	{
		#region ================== Constants
		
		private const float DRAG_ANGLE_TOLERANCE = 0.06f;
		
		#endregion

		#region ================== Variables

		protected BaseVisualMode mode;

		protected Plane top;
		protected Plane bottom;
		protected long setuponloadedtexture;
		
		// UV dragging
		private float dragstartanglexy;
		private float dragstartanglez;
		private Vector3D dragorigin;
		private Vector3D deltaxy;
		private Vector3D deltaz;
		private int startoffsetx;
		private int startoffsety;
		protected bool uvdragging;
		private int prevoffsetx;		// We have to provide delta offsets, but I don't
		private int prevoffsety;		// want to calculate with delta offsets to prevent
										// inaccuracy in the dragging.
		private static List<BaseVisualSector> updateList; //mxd

		// Undo/redo
		private int undoticket;
		
		#endregion
		
		#region ================== Properties
		
		public bool IsDraggingUV { get { return uvdragging; } }
		new public BaseVisualSector Sector { get { return (BaseVisualSector)base.Sector; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor for sidedefs
		public BaseVisualGeometrySidedef(BaseVisualMode mode, VisualSector vs, Sidedef sd) : base(vs, sd)
		{
			this.mode = mode;
			this.deltaz = new Vector3D(0.0f, 0.0f, 1.0f);
			this.deltaxy = (sd.Line.End.Position - sd.Line.Start.Position) * sd.Line.LengthInv;
			if(!sd.IsFront) this.deltaxy = -this.deltaxy;

			//mxd
			if(mode.UseSelectionFromClassicMode && sd.Line.Selected) {
				this.selected = true;
				mode.AddSelectedObject(this);
			}
		}
		
		#endregion

		#region ================== Methods

		// This sets the renderstyle from linedef information and returns the alpha value or the vertices
		protected byte SetLinedefRenderstyle(bool solidasmask)
		{
			byte alpha;

			// From TranslucentLine action
			if(Sidedef.Line.Action == 208)
			{
				alpha = (byte)General.Clamp(Sidedef.Line.Args[1], 0, 255);
				
				if(Sidedef.Line.Args[2] == 1)
					this.RenderPass = RenderPass.Additive;
				else if(alpha < 255)
					this.RenderPass = RenderPass.Alpha;
				else if(solidasmask)
					this.RenderPass = RenderPass.Mask;
				else
					this.RenderPass = RenderPass.Solid;
			}
			else
			{
				// From UDMF field
				string field = Sidedef.Line.Fields.GetValue("renderstyle", "translucent");
				alpha = (byte)(Sidedef.Line.Fields.GetValue("alpha", 1.0f) * 255.0f);
				if(alpha == 255 && Sidedef.Line.IsFlagSet("transparent")) { //mxd
					alpha = 64;
				}

				if(field == "add")
					this.RenderPass = RenderPass.Additive;
				else if(alpha < 255)
					this.RenderPass = RenderPass.Alpha;
				else if(solidasmask)
					this.RenderPass = RenderPass.Mask;
				else
					this.RenderPass = RenderPass.Solid;
			}
			
			return alpha;
		}

		// This performs a fast test in object picking
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			// Check if intersection point is between top and bottom
			return (pickintersect.z >= bottom.GetZ(pickintersect)) && (pickintersect.z <= top.GetZ(pickintersect));
		}
		
		
		// This performs an accurate test for object picking
		public override bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref float u_ray)
		{
			// The fast reject pass is already as accurate as it gets,
			// so we just return the intersection distance here
			u_ray = pickrayu;
			return true;
		}
		
		
		// This creates vertices from a wall polygon and applies lighting
		protected List<WorldVertex> CreatePolygonVertices(WallPolygon poly, TexturePlane tp, SectorData sd, int lightvalue, bool lightabsolute)
		{
			List<WallPolygon> polylist = new List<WallPolygon>(1);
			polylist.Add(poly);
			return CreatePolygonVertices(polylist, tp, sd, lightvalue, lightabsolute);
		}

		// This creates vertices from a wall polygon and applies lighting
		protected List<WorldVertex> CreatePolygonVertices(List<WallPolygon> poly, TexturePlane tp, SectorData sd, int lightvalue, bool lightabsolute)
		{
			List<WallPolygon> polygons = new List<WallPolygon>(poly);
			List<WorldVertex> verts = new List<WorldVertex>();

			// Go for all levels to build geometry
			for(int i = sd.LightLevels.Count - 1; i >= 0; i--)
			{
				SectorLevel l = sd.LightLevels[i];

				if((l != sd.Floor) && (l != sd.Ceiling) && (l.type != SectorLevelType.Floor))
				{
					// Go for all polygons
					int num = polygons.Count;
					for(int pi = 0; pi < num; pi++)
					{
						// Split by plane
						WallPolygon p = polygons[pi];
						WallPolygon np = SplitPoly(ref p, l.plane, false);
						if(np.Count > 0)
						{
							// Determine color
							int lightlevel = lightabsolute ? lightvalue : l.brightnessbelow + lightvalue;
							PixelColor wallbrightness = PixelColor.FromInt(mode.CalculateBrightness(lightlevel, Sidedef)); //mxd
							PixelColor wallcolor = PixelColor.Modulate(l.colorbelow, wallbrightness);
							np.color = wallcolor.WithAlpha(255).ToInt();
							
							if(p.Count == 0)
							{
								polygons[pi] = np;
							}
							else
							{
								polygons[pi] = p;
								polygons.Add(np);
							}
						}
						else
						{
							polygons[pi] = p;
						}
					}
				}
			}
			
			// Go for all polygons to make geometry
			foreach(WallPolygon p in polygons)
			{
				// Find texture coordinates for each vertex in the polygon
				List<Vector2D> texc = new List<Vector2D>(p.Count);
				foreach(Vector3D v in p)
					texc.Add(tp.GetTextureCoordsAt(v));
				
				// Now we create triangles from the polygon.
				// The polygon is convex and clockwise, so this is a piece of cake.
				if(p.Count >= 3)
				{
					for(int k = 1; k < (p.Count - 1); k++)
					{
						verts.Add(new WorldVertex(p[0], p.color, texc[0]));
						verts.Add(new WorldVertex(p[k], p.color, texc[k]));
						verts.Add(new WorldVertex(p[k + 1], p.color, texc[k + 1]));
					}
				}
			}
			
			return verts;
		}
		
		// This splits a polygon with a plane and returns the other part as a new polygon
		// The polygon is expected to be convex and clockwise
		protected WallPolygon SplitPoly(ref WallPolygon poly, Plane p, bool keepfront)
		{
			const float NEAR_ZERO = 0.01f;
			WallPolygon front = new WallPolygon(poly.Count);
			WallPolygon back = new WallPolygon(poly.Count);
			poly.CopyProperties(front);
			poly.CopyProperties(back);
			
			if(poly.Count > 0)
			{
				// Go for all vertices to see which side they have to be on
				Vector3D v1 = poly[poly.Count - 1];
				float side1 = p.Distance(v1);
				for(int i = 0; i < poly.Count; i++)
				{
					// Fetch vertex and determine side
					Vector3D v2 = poly[i];
					float side2 = p.Distance(v2);
					
					// Front?
					if(side2 > NEAR_ZERO)
					{
						if(side1 < -NEAR_ZERO)
						{
							// Split line with plane and insert the vertex
							float u = 0.0f;
							p.GetIntersection(v1, v2, ref u);
							Vector3D v3 = v1 + (v2 - v1) * u;
							front.Add(v3);
							back.Add(v3);
						}
						
						front.Add(v2);
					}
					// Back?
					else if(side2 < -NEAR_ZERO)
					{
						if(side1 > NEAR_ZERO)
						{
							// Split line with plane and insert the vertex
							float u = 0.0f;
							p.GetIntersection(v1, v2, ref u);
							Vector3D v3 = v1 + (v2 - v1) * u;
							front.Add(v3);
							back.Add(v3);
						}
						
						back.Add(v2);
					}
					else
					{
						// On the plane, add to both polygons
						front.Add(v2);
						back.Add(v2);
					}
					
					// Next
					v1 = v2;
					side1 = side2;
				}
			}
			
			if(keepfront)
			{
				poly = front;
				return back;
			}
			else
			{
				poly = back;
				return front;
			}
		}
		
		
		// This crops a polygon with a plane and keeps only a certain part of the polygon
		protected void CropPoly(ref WallPolygon poly, Plane p, bool keepfront)
		{
			const float NEAR_ZERO = 0.01f;
			float sideswitch = keepfront ? 1 : -1;
			WallPolygon newp = new WallPolygon(poly.Count);
			poly.CopyProperties(newp);
			
			if(poly.Count > 0)
			{
				// First split lines that cross the plane so that we have vertices on the plane where the lines cross
				Vector3D v1 = poly[poly.Count - 1];
				float side1 = p.Distance(v1) * sideswitch;
				for(int i = 0; i < poly.Count; i++)
				{
					// Fetch vertex and determine side
					Vector3D v2 = poly[i];
					float side2 = p.Distance(v2) * sideswitch;
					
					// Front?
					if(side2 > NEAR_ZERO)
					{
						if(side1 < -NEAR_ZERO)
						{
							// Split line with plane and insert the vertex
							float u = 0.0f;
							p.GetIntersection(v1, v2, ref u);
							Vector3D v3 = v1 + (v2 - v1) * u;
							newp.Add(v3);
						}
						
						newp.Add(v2);
					}
					// Back?
					else if(side2 < -NEAR_ZERO)
					{
						if(side1 > NEAR_ZERO)
						{
							// Split line with plane and insert the vertex
							float u = 0.0f;
							p.GetIntersection(v1, v2, ref u);
							Vector3D v3 = v1 + (v2 - v1) * u;
							newp.Add(v3);
						}
					}
					else
					{
						// On the plane
						newp.Add(v2);
					}
					
					// Next
					v1 = v2;
					side1 = side2;
				}
			}
			
			poly = newp;
		}

		//mxd
		protected float getRoundedTextureOffset(float oldValue, float offset, float scale, float textureSize) {
			if(offset == 0f) return oldValue;
			float scaledOffset = offset * scale;
			float result = (float)Math.Round(oldValue + scaledOffset);
			if (textureSize > 0) result %= textureSize;
			if(result == oldValue) result += (scaledOffset < 0 ? -1 : 1);
			return result;
		}

		//mxd
		protected void onTextureChanged() {
			//check for 3d floors
			if(Sidedef.Line.Action == 160) {
				int sectortag = Sidedef.Line.Args[0] + (Sidedef.Line.Args[4] << 8);
				if(sectortag == 0) return;

				foreach(Sector sector in General.Map.Map.Sectors) {
					if(sector.Tag == sectortag) {
						BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(sector);
						vs.UpdateSectorGeometry(true);
					}
				}
			}
				
		}

		//mxd
		protected void selectNeighbours(string texture, bool select, bool withSameTexture, bool withSameHeight) 
		{
			if(Sidedef.Sector == null || (!withSameTexture && !withSameHeight)) return;

			Rectangle rect = BuilderModesTools.GetSidedefPartSize(this, geoType);
			if(rect.Height == 0) return;

			if(select && !selected) 
			{
				selected = true;
				mode.AddSelectedObject(this);
			} 
			else if(!select && selected) 
			{
				selected = false;
				mode.RemoveSelectedObject(this);
			}

			//select
			List<Linedef> connectedLines = new List<Linedef>();

			foreach(Linedef line in Sidedef.Line.Start.Linedefs) 
			{
				if(line.Index == Sidedef.Line.Index) continue;
				connectedLines.Add(line);
			}
			foreach(Linedef line in Sidedef.Line.End.Linedefs) 
			{
				if(line.Index == Sidedef.Line.Index) continue;
				if(!connectedLines.Contains(line)) connectedLines.Add(line);
			}

			// Check connected lines
			foreach(Linedef line in connectedLines) 
			{
				bool addFrontTop = false;
				bool addFrontMiddle = false;
				bool addFrontBottom = false;
				bool addBackTop = false;
				bool addBackMiddle = false;
				bool addBackBottom = false;

				bool lineHasFrontSector = (line.Front != null && line.Front.Sector != null);
				bool lineHasBackSector = (line.Back != null && line.Back.Sector != null);
				bool doublesided = (lineHasFrontSector && lineHasBackSector);

				List<VisualMiddle3D> extrasides = new List<VisualMiddle3D>();

				// Gather 3d floor sides
				if (doublesided) 
				{
					BaseVisualSector s = mode.GetVisualSector(line.Front.Sector) as BaseVisualSector;
					if (s != null) {
						extrasides.AddRange(s.GetSidedefParts(line.Front).middle3d.ToArray());
					}

					s = mode.GetVisualSector(line.Back.Sector) as BaseVisualSector;
					if(s != null) {
						extrasides.AddRange(s.GetSidedefParts(line.Back).middle3d.ToArray());
					}
				}

				// Add regular sides
				if(withSameTexture) 
				{
					if(line.Front != null) 
					{
						addFrontTop = (line.Front.HighTexture == texture 
							&& line.Front.HighRequired()
							&& BuilderModesTools.GetSidedefPartSize(line.Front, VisualGeometryType.WALL_UPPER).IntersectsWith(rect));
						
						addFrontMiddle = (line.Front.MiddleTexture == texture 
							&& line.Front.MiddleRequired() 
							&& line.Front.GetMiddleHeight() > 0
							&& BuilderModesTools.GetSidedefPartSize(line.Front, VisualGeometryType.WALL_MIDDLE).IntersectsWith(rect));
						
						addFrontBottom = (line.Front.LowTexture == texture 
							&& line.Front.LowRequired()
							&& BuilderModesTools.GetSidedefPartSize(line.Front, VisualGeometryType.WALL_LOWER).IntersectsWith(rect));

					}

					if(line.Back != null) 
					{
						addBackTop = (line.Back.HighTexture == texture 
							&& line.Back.HighRequired()
							&& BuilderModesTools.GetSidedefPartSize(line.Back, VisualGeometryType.WALL_UPPER).IntersectsWith(rect));
						
						addBackMiddle = (line.Back.MiddleTexture == texture 
							&& line.Back.MiddleRequired() 
							&& line.Back.GetMiddleHeight() > 0
							&& BuilderModesTools.GetSidedefPartSize(line.Back, VisualGeometryType.WALL_MIDDLE).IntersectsWith(rect));

						addBackBottom = (line.Back.LowTexture == texture 
							&& line.Back.LowRequired()
							&& BuilderModesTools.GetSidedefPartSize(line.Back, VisualGeometryType.WALL_LOWER).IntersectsWith(rect));
					}

					// Add 3d floor sides
					List<VisualMiddle3D> filtered = new List<VisualMiddle3D>();
					foreach(VisualMiddle3D side3d in extrasides) 
					{
						Sidedef controlside = side3d.GetControlLinedef().Front;
						if (controlside.MiddleTexture == texture && BuilderModesTools.GetSidedefPartSize(controlside, VisualGeometryType.WALL_MIDDLE).IntersectsWith(rect)) 
						{
							filtered.Add(side3d);
						}
					}

					extrasides = filtered;
				}

				if(withSameHeight && rect.Height > 0) 
				{
					// Upper parts match?
					if((!withSameTexture || addFrontTop) && doublesided && line.Front.HighRequired())
					{
						Rectangle r = BuilderModesTools.GetSidedefPartSize(line.Front, VisualGeometryType.WALL_UPPER);
						addFrontTop = (rect.Height == r.Height && rect.Y == r.Y);
					}

					if((!withSameTexture || addBackTop) && doublesided && line.Back.HighRequired()) 
					{
						Rectangle r = BuilderModesTools.GetSidedefPartSize(line.Back, VisualGeometryType.WALL_UPPER);
						addBackTop = (rect.Height == r.Height && rect.Y == r.Y);
					}

					// Middle parts match?
					if((!withSameTexture || addFrontMiddle) 
						&& lineHasFrontSector 
						&& (line.Front.MiddleRequired() || line.Front.LongMiddleTexture != MapSet.EmptyLongName) ) 
					{
						Rectangle r = BuilderModesTools.GetSidedefPartSize(line.Front, VisualGeometryType.WALL_MIDDLE);
						addFrontMiddle = (rect.Height == r.Height && rect.Y == r.Y);
					}

					if((!withSameTexture || addBackMiddle) 
						&& lineHasBackSector 
						&& (line.Back.MiddleRequired() || line.Back.LongMiddleTexture != MapSet.EmptyLongName)) 
					{
						Rectangle r = BuilderModesTools.GetSidedefPartSize(line.Back, VisualGeometryType.WALL_MIDDLE);
						addBackMiddle = (rect.Height == r.Height && rect.Y == r.Y);
					}

					// Lower parts match?
					if((!withSameTexture || addFrontBottom) && doublesided && line.Front.LowRequired()) 
					{
						Rectangle r = BuilderModesTools.GetSidedefPartSize(line.Front, VisualGeometryType.WALL_LOWER);
						addFrontBottom = (rect.Height == r.Height && rect.Y == r.Y);
					}

					if((!withSameTexture || addBackBottom) && doublesided && line.Back.LowRequired()) 
					{
						Rectangle r = BuilderModesTools.GetSidedefPartSize(line.Back, VisualGeometryType.WALL_LOWER);
						addBackBottom = (rect.Height == r.Height && rect.Y == r.Y);
					}

					// 3d floor parts match?
					List<VisualMiddle3D> filtered = new List<VisualMiddle3D>();
					foreach(VisualMiddle3D side3d in extrasides) 
					{
						Sidedef controlside = side3d.GetControlLinedef().Front;
						Rectangle r = BuilderModesTools.GetSidedefPartSize(controlside, VisualGeometryType.WALL_MIDDLE);
						if(rect.Height == r.Height && rect.Y == r.Y) 
						{
							filtered.Add(side3d);
						}
					}

					extrasides = filtered;
				}

				// Select front?
				if(addFrontTop || addFrontMiddle || addFrontBottom)
					mode.SelectSideParts(line.Front, addFrontTop, addFrontMiddle, addFrontBottom, select, withSameTexture, withSameHeight);

				// Select back?
				if(addBackTop || addBackMiddle || addBackBottom)
					mode.SelectSideParts(line.Back, addBackTop, addBackMiddle, addBackBottom, select, withSameTexture, withSameHeight);

				// Select 3d floor sides?
				foreach (VisualMiddle3D side3d in extrasides) 
				{
					if( (select && !side3d.Selected) || (!select && side3d.Selected) )
						side3d.SelectNeighbours(select, withSameTexture, withSameHeight);
				}
			}
		}

		//mxd
		public virtual bool IsSelected() {
			return selected;
		}
		
		#endregion

		#region ================== Events
		
		// Unused
		public virtual void OnEditBegin() { }
		protected virtual void SetTexture(string texturename) { }
		public abstract bool Setup();
		protected abstract void SetTextureOffsetX(int x);
		protected abstract void SetTextureOffsetY(int y);
		protected virtual void ResetTextureScale() { } //mxd
		protected abstract void MoveTextureOffset(Point xy);
		protected abstract Point GetTextureOffset();
		public virtual void SelectNeighbours(bool select, bool withSameTexture, bool withSameHeight) { } //mxd
		public virtual void OnTextureFit(bool fitWidth, bool fitHeight) { } //mxd
		
		// Insert middle texture
		public virtual void OnInsert()
		{
			// No middle texture yet?
			if(!Sidedef.MiddleRequired() && (string.IsNullOrEmpty(Sidedef.MiddleTexture) || (Sidedef.MiddleTexture == "-")))
			{
				// Make it now
				mode.CreateUndo("Create middle texture");
				mode.SetActionResult("Created middle texture.");
				General.Settings.FindDefaultDrawSettings();
				Sidedef.SetTextureMid(General.Map.Options.DefaultWallTexture);

				// Update
				Sector.Changed = true;
				
				// Other side as well
				if(string.IsNullOrEmpty(Sidedef.Other.MiddleTexture) || (Sidedef.Other.MiddleTexture == "-"))
				{
					Sidedef.Other.SetTextureMid(General.Map.Options.DefaultWallTexture);

					// Update
					VisualSector othersector = mode.GetVisualSector(Sidedef.Other.Sector);
					if(othersector is BaseVisualSector) (othersector as BaseVisualSector).Changed = true;
				}
			}
		}

		// Delete texture
		public virtual void OnDelete()
		{
			// Remove texture
			mode.CreateUndo("Delete texture");
			mode.SetActionResult("Deleted a texture.");
			SetTexture("-");

			// Update
			Sector.Changed = true;
		}
		
		// Processing
		public virtual void OnProcess(float deltatime)
		{
			// If the texture was not loaded, but is loaded now, then re-setup geometry
			if(setuponloadedtexture != 0)
			{
				ImageData t = General.Map.Data.GetTextureImage(setuponloadedtexture);
				if(t != null)
				{
					if(t.IsImageLoaded)
					{
						setuponloadedtexture = 0;
						Setup();
					}
				}
			}
		}
		
		// Change target height
		public virtual void OnChangeTargetHeight(int amount)
		{
			switch(BuilderPlug.Me.ChangeHeightBySidedef)
			{
				// Change ceiling
				case 1:
					if(!this.Sector.Ceiling.Changed)
						this.Sector.Ceiling.OnChangeTargetHeight(amount);
					break;

				// Change floor
				case 2:
					if(!this.Sector.Floor.Changed)
						this.Sector.Floor.OnChangeTargetHeight(amount);
					break;

				// Change both
				case 3:
					if(!this.Sector.Floor.Changed)
						this.Sector.Floor.OnChangeTargetHeight(amount);
					if(!this.Sector.Ceiling.Changed)
						this.Sector.Ceiling.OnChangeTargetHeight(amount);
					break;
			}
		}
		
		// Reset texture offsets
		public virtual void OnResetTextureOffset()
		{
			mode.CreateUndo("Reset texture offsets");
			mode.SetActionResult("Texture offsets reset.");

			// Apply offsets
			Sidedef.OffsetX = 0;
			Sidedef.OffsetY = 0;
			
			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			parts.SetupAllParts();
		}

		//mxd
		public virtual void OnResetLocalTextureOffset() {
			if (!General.Map.UDMF) {
				OnResetTextureOffset();
				return;
			}

			mode.CreateUndo("Reset local texture offsets");
			mode.SetActionResult("Local texture offsets reset.");

			// Apply offsets
			SetTextureOffsetX(0);
			SetTextureOffsetY(0);
			ResetTextureScale();

			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			parts.SetupAllParts();
		}
		
		// Toggle upper-unpegged
		public virtual void OnToggleUpperUnpegged()
		{
			if(this.Sidedef.Line.IsFlagSet(General.Map.Config.UpperUnpeggedFlag))
			{
				// Remove flag
				mode.ApplyUpperUnpegged(false);
			}
			else
			{
				// Add flag
				mode.ApplyUpperUnpegged(true);
			}
		}

		// Toggle lower-unpegged
		public virtual void OnToggleLowerUnpegged()
		{
			if(this.Sidedef.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag))
			{
				// Remove flag
				mode.ApplyLowerUnpegged(false);
			}
			else
			{
				// Add flag
				mode.ApplyLowerUnpegged(true);
			}
		}

		
		// This sets the Upper Unpegged flag
		public virtual void ApplyUpperUnpegged(bool set)
		{
			if(!set)
			{
				// Remove flag
				mode.CreateUndo("Remove upper-unpegged setting");
				mode.SetActionResult("Removed upper-unpegged setting.");
				this.Sidedef.Line.SetFlag(General.Map.Config.UpperUnpeggedFlag, false);
			}
			else
			{
				// Add flag
				mode.CreateUndo("Set upper-unpegged setting");
				mode.SetActionResult("Set upper-unpegged setting.");
				this.Sidedef.Line.SetFlag(General.Map.Config.UpperUnpeggedFlag, true);
			}

			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			parts.SetupAllParts();
			
			// Update other sidedef geometry
			if(Sidedef.Other != null)
			{
				BaseVisualSector othersector = (BaseVisualSector)mode.GetVisualSector(Sidedef.Other.Sector);
				parts = othersector.GetSidedefParts(Sidedef.Other);
				parts.SetupAllParts();
			}
		}
		
		
		// This sets the Lower Unpegged flag
		public virtual void ApplyLowerUnpegged(bool set)
		{
			if(!set)
			{
				// Remove flag
				mode.CreateUndo("Remove lower-unpegged setting");
				mode.SetActionResult("Removed lower-unpegged setting.");
				this.Sidedef.Line.SetFlag(General.Map.Config.LowerUnpeggedFlag, false);
			}
			else
			{
				// Add flag
				mode.CreateUndo("Set lower-unpegged setting");
				mode.SetActionResult("Set lower-unpegged setting.");
				this.Sidedef.Line.SetFlag(General.Map.Config.LowerUnpeggedFlag, true);
			}

			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			parts.SetupAllParts();

			// Update other sidedef geometry
			if(Sidedef.Other != null)
			{
				BaseVisualSector othersector = (BaseVisualSector)mode.GetVisualSector(Sidedef.Other.Sector);
				parts = othersector.GetSidedefParts(Sidedef.Other);
				parts.SetupAllParts();
			}
		}


		// Flood-fill textures
		public virtual void OnTextureFloodfill()
		{
			if(BuilderPlug.Me.CopiedTexture != null)
			{
				string oldtexture = GetTextureName();
				long oldtexturelong = Lump.MakeLongName(oldtexture);
				string newtexture = BuilderPlug.Me.CopiedTexture;
				if(newtexture != oldtexture)
				{
					mode.CreateUndo("Flood-fill textures with " + newtexture);
					mode.SetActionResult("Flood-filled textures with " + newtexture + ".");
					
					mode.Renderer.SetCrosshairBusy(true);
					General.Interface.RedrawDisplay();

					// Get the texture
					ImageData newtextureimage = General.Map.Data.GetTextureImage(newtexture);
					if(newtextureimage != null)
					{
						if(mode.IsSingleSelection)
						{
							// Clear all marks, this will align everything it can
							General.Map.Map.ClearMarkedSidedefs(false);
						}
						else
						{
							// Limit the alignment to selection only
							General.Map.Map.ClearMarkedSidedefs(true);
							List<Sidedef> sides = mode.GetSelectedSidedefs();
							foreach(Sidedef sd in sides) sd.Marked = false;
						}
						
						// Do the alignment
						Tools.FloodfillTextures(this.Sidedef, oldtexturelong, newtextureimage, false);

						// Get the changed sidedefs
						List<Sidedef> changes = General.Map.Map.GetMarkedSidedefs(true);
						foreach(Sidedef sd in changes)
						{
							// Update the parts for this sidedef!
							if(mode.VisualSectorExists(sd.Sector))
							{
								BaseVisualSector vs = (mode.GetVisualSector(sd.Sector) as BaseVisualSector);
								VisualSidedefParts parts = vs.GetSidedefParts(sd);
								parts.SetupAllParts();
							}
						}

						General.Map.Data.UpdateUsedTextures();
						mode.Renderer.SetCrosshairBusy(false);
						mode.ShowTargetInfo();
					}
				}
			}
		}
		
		// Auto-align texture offsets
		public virtual void OnTextureAlign(bool alignx, bool aligny)
		{
			//mxd
			string rest;
			if(alignx && aligny) rest = "(X and Y)";
			else if(alignx)	rest = "(X)";
			else rest = "(Y)";

			mode.CreateUndo("Auto-align textures " + rest);
			mode.SetActionResult("Auto-aligned textures " + rest + ".");
			
			// Make sure the texture is loaded (we need the texture size)
			if(!base.Texture.IsImageLoaded) base.Texture.LoadImage();
			
			if(mode.IsSingleSelection)
			{
				// Clear all marks, this will align everything it can
				General.Map.Map.ClearMarkedSidedefs(false);
			}
			else
			{
				// Limit the alignment to selection only
				General.Map.Map.ClearMarkedSidedefs(true);
				List<Sidedef> sides = mode.GetSelectedSidedefs();
				foreach(Sidedef sd in sides) sd.Marked = false;
			}
			
			// Do the alignment
			mode.AutoAlignTextures(this, base.Texture, alignx, aligny, false, true);

			// Get the changed sidedefs
			List<Sidedef> changes = General.Map.Map.GetMarkedSidedefs(true);
			foreach(Sidedef sd in changes)
			{
				// Update the parts for this sidedef!
				if(mode.VisualSectorExists(sd.Sector))
				{
					BaseVisualSector vs = (mode.GetVisualSector(sd.Sector) as BaseVisualSector);
					VisualSidedefParts parts = vs.GetSidedefParts(sd);
					parts.SetupAllParts();
				}
			}
		}
		
		// Select texture
		public virtual void OnSelectTexture()
		{
			if(General.Interface.IsActiveWindow)
			{
				string oldtexture = GetTextureName();
				string newtexture = General.Interface.BrowseTexture(General.Interface, oldtexture);
				if(newtexture != oldtexture)
				{
					mode.ApplySelectTexture(newtexture, false);
				}
			}
		}

		// Apply Texture
		public virtual void ApplyTexture(string texture)
		{
			mode.CreateUndo("Change texture " + texture);
			SetTexture(texture);
			onTextureChanged();//mxd
		}
		
		// Paste texture
		public virtual void OnPasteTexture()
		{
			if(BuilderPlug.Me.CopiedTexture != null)
			{
				mode.CreateUndo("Paste texture '" + BuilderPlug.Me.CopiedTexture + "'");
				mode.SetActionResult("Pasted texture '" + BuilderPlug.Me.CopiedTexture + "'.");
				SetTexture(BuilderPlug.Me.CopiedTexture);
				onTextureChanged();//mxd
			}
		}
		
		// Paste texture offsets
		public virtual void OnPasteTextureOffsets()
		{
			mode.CreateUndo("Paste texture offsets");
			if (General.Map.UDMF) {
				SetTextureOffsetX(BuilderPlug.Me.CopiedOffsets.X);
				SetTextureOffsetY(BuilderPlug.Me.CopiedOffsets.Y);
			} else {
				Sidedef.OffsetX = BuilderPlug.Me.CopiedOffsets.X;
				Sidedef.OffsetY = BuilderPlug.Me.CopiedOffsets.Y;
			}
			mode.SetActionResult("Pasted texture offsets " + BuilderPlug.Me.CopiedOffsets.X + ", " + BuilderPlug.Me.CopiedOffsets.Y + ".");
			
			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			parts.SetupAllParts();
		}
		
		// Copy texture
		public virtual void OnCopyTexture()
		{
			BuilderPlug.Me.CopiedTexture = GetTextureName();
			if(General.Map.Config.MixTexturesFlats) BuilderPlug.Me.CopiedFlat = GetTextureName();
			mode.SetActionResult("Copied texture '" + GetTextureName() + "'.");
		}
		
		// Copy texture offsets
		public virtual void OnCopyTextureOffsets()
		{
			//mxd
			BuilderPlug.Me.CopiedOffsets = General.Map.UDMF ? GetTextureOffset() : new Point(Sidedef.OffsetX, Sidedef.OffsetY);
			mode.SetActionResult("Copied texture offsets " + BuilderPlug.Me.CopiedOffsets.X + ", " + BuilderPlug.Me.CopiedOffsets.Y + ".");
		}

		// Copy properties
		public virtual void OnCopyProperties()
		{
			BuilderPlug.Me.CopiedSidedefProps = new SidedefProperties(Sidedef);
			mode.SetActionResult("Copied sidedef properties.");
		}

		// Paste properties
		public virtual void OnPasteProperties()
		{
			if(BuilderPlug.Me.CopiedSidedefProps != null)
			{
				mode.CreateUndo("Paste sidedef properties");
				mode.SetActionResult("Pasted sidedef properties.");
				BuilderPlug.Me.CopiedSidedefProps.Apply(Sidedef);
				
				// Update sectors on both sides
				BaseVisualSector front = (BaseVisualSector)mode.GetVisualSector(Sidedef.Sector);
				if(front != null) front.Changed = true;
				if(Sidedef.Other != null)
				{
					BaseVisualSector back = (BaseVisualSector)mode.GetVisualSector(Sidedef.Other.Sector);
					if(back != null) back.Changed = true;
				}
				mode.ShowTargetInfo();
			}
		}
		
		// Return texture name
		public virtual string GetTextureName() { return ""; }
		
		// Select button pressed
		public virtual void OnSelectBegin()
		{
			mode.LockTarget();
			dragstartanglexy = General.Map.VisualCamera.AngleXY;
			dragstartanglez = General.Map.VisualCamera.AngleZ;
			dragorigin = pickintersect;
			startoffsetx = GetTextureOffset().X;
			startoffsety = GetTextureOffset().Y;
			prevoffsetx = GetTextureOffset().X;
			prevoffsety = GetTextureOffset().Y;
		}
		
		// Select button released
		public virtual void OnSelectEnd()
		{
			mode.UnlockTarget();
			
			// Was dragging?
			if(uvdragging)
			{
				// Dragging stops now
				uvdragging = false;
			}
			else
			{
				// Add/remove selection
				if(this.selected)
				{
					this.selected = false;
					mode.RemoveSelectedObject(this);
				}
				else
				{
					this.selected = true;
					mode.AddSelectedObject(this);
				}
			}
		}
		
		// Edit button released
		public virtual void OnEditEnd()
		{
			if(General.Interface.IsActiveWindow)
			{
				List<Linedef> linedefs = mode.GetSelectedLinedefs();
				updateList = new List<BaseVisualSector>(); //mxd
				foreach(Linedef l in linedefs) {
					if(l.Front != null && mode.VisualSectorExists(l.Front.Sector)) 
						updateList.Add((BaseVisualSector)mode.GetVisualSector(l.Front.Sector));
					if(l.Back != null && mode.VisualSectorExists(l.Back.Sector))
						updateList.Add((BaseVisualSector)mode.GetVisualSector(l.Back.Sector));
				}

				General.Interface.OnEditFormValuesChanged += Interface_OnEditFormValuesChanged;
				mode.StartRealtimeInterfaceUpdate(SelectionType.Linedefs);
				DialogResult result = General.Interface.ShowEditLinedefs(linedefs);
				mode.StopRealtimeInterfaceUpdate(SelectionType.Linedefs);
				General.Interface.OnEditFormValuesChanged -= Interface_OnEditFormValuesChanged;

				updateList.Clear();
				updateList = null;

				if(result == DialogResult.OK)
				{
					foreach(Linedef l in linedefs)
					{
						if(l.Front != null && mode.VisualSectorExists(l.Front.Sector))
						{
							BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(l.Front.Sector);
							vs.UpdateSectorGeometry(false);
						}
						
						if(l.Back != null && mode.VisualSectorExists(l.Back.Sector))
						{
							BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(l.Back.Sector);
							vs.UpdateSectorGeometry(false);
						}
					}
					
					mode.RebuildElementData();
				}
			}
		}

		//mxd
		private void Interface_OnEditFormValuesChanged(object sender, EventArgs e) {
			foreach(BaseVisualSector vs in updateList)
				vs.UpdateSectorGeometry(false);
		}
		
		// Mouse moves
		public virtual void OnMouseMove(MouseEventArgs e)
		{
			// Dragging UV?
			if(uvdragging)
			{
				UpdateDragUV();
			}
			else
			{
				// Select button pressed?
				if(General.Actions.CheckActionActive(General.ThisAssembly, "visualselect"))
				{
					// Check if tolerance is exceeded to start UV dragging
					float deltaxy = General.Map.VisualCamera.AngleXY - dragstartanglexy;
					float deltaz = General.Map.VisualCamera.AngleZ - dragstartanglez;
					if((Math.Abs(deltaxy) + Math.Abs(deltaz)) > DRAG_ANGLE_TOLERANCE)
					{
						mode.PreAction(UndoGroup.TextureOffsetChange);
						mode.CreateUndo("Change texture offsets");

						// Start drag now
						uvdragging = true;
						mode.Renderer.ShowSelection = false;
						mode.Renderer.ShowHighlight = false;
						UpdateDragUV();
					}
				}
			}
		}
		
		// This is called to update UV dragging
		protected virtual void UpdateDragUV()
		{
			float u_ray;
			
			// Calculate intersection position
			Line2D ray = new Line2D(General.Map.VisualCamera.Position, General.Map.VisualCamera.Target);
			Sidedef.Line.Line.GetIntersection(ray, out u_ray);
			Vector3D intersect = General.Map.VisualCamera.Position + (General.Map.VisualCamera.Target - General.Map.VisualCamera.Position) * u_ray;
			
			// Calculate offsets
			Vector3D dragdelta = intersect - dragorigin;
			Vector3D dragdeltaxy = dragdelta * deltaxy;
			Vector3D dragdeltaz = dragdelta * deltaz;
			float offsetx = dragdeltaxy.GetLength();
			float offsety = dragdeltaz.GetLength();
			if((Math.Sign(dragdeltaxy.x) < 0) || (Math.Sign(dragdeltaxy.y) < 0) || (Math.Sign(dragdeltaxy.z) < 0)) offsetx = -offsetx;
			if((Math.Sign(dragdeltaz.x) < 0) || (Math.Sign(dragdeltaz.y) < 0) || (Math.Sign(dragdeltaz.z) < 0)) offsety = -offsety;
			
			// Apply offsets
			if(General.Interface.CtrlState && General.Interface.ShiftState) { //mxd. Clamp to grid size?
				int newoffsetx = startoffsetx - (int)Math.Round(offsetx);
				int newoffsety = startoffsety + (int)Math.Round(offsety);
				int dx = prevoffsetx - newoffsetx;
				int dy = prevoffsety - newoffsety;

				if(Math.Abs(dx) >= General.Map.Grid.GridSize) {
					dx = General.Map.Grid.GridSize * Math.Sign(dx);
					prevoffsetx = newoffsetx;
				} else {
					dx = 0;
				}

				if(Math.Abs(dy) >= General.Map.Grid.GridSize) {
					dy = General.Map.Grid.GridSize * Math.Sign(dy);
					prevoffsety = newoffsety;
				} else {
					dy = 0;
				}

				if(dx != 0 || dy != 0) mode.ApplyTextureOffsetChange(dx, dy);
			} else { //mxd. Constraint to axis?
				int newoffsetx = (General.Interface.CtrlState ? startoffsetx : startoffsetx - (int)Math.Round(offsetx)); //mxd
				int newoffsety = (General.Interface.ShiftState ? startoffsety : startoffsety + (int)Math.Round(offsety)); //mxd
				mode.ApplyTextureOffsetChange(prevoffsetx - newoffsetx, prevoffsety - newoffsety);
				prevoffsetx = newoffsetx;
				prevoffsety = newoffsety;
			}
			
			mode.ShowTargetInfo();
		}
		
		// Sector brightness change
		public virtual void OnChangeTargetBrightness(bool up)
		{
			if(!Sector.Changed)
			{
				// Change brightness
				mode.CreateUndo("Change sector brightness", UndoGroup.SectorBrightnessChange, Sector.Sector.FixedIndex);

				if(up)
					Sector.Sector.Brightness = General.Map.Config.BrightnessLevels.GetNextHigher(Sector.Sector.Brightness);
				else
					Sector.Sector.Brightness = General.Map.Config.BrightnessLevels.GetNextLower(Sector.Sector.Brightness);
				
				mode.SetActionResult("Changed sector brightness to " + Sector.Sector.Brightness + ".");

				Sector.Sector.UpdateCache();
				
				// Rebuild sector
				Sector.UpdateSectorGeometry(false);

				// Go for all things in this sector
				foreach(Thing t in General.Map.Map.Things)
				{
					if(t.Sector == Sector.Sector)
					{
						if(mode.VisualThingExists(t))
						{
							// Update thing
							BaseVisualThing vt = (mode.GetVisualThing(t) as BaseVisualThing);
							vt.Changed = true;
						}
					}
				}
			}
		}
		
		// Texture offset change
		public virtual void OnChangeTextureOffset(int horizontal, int vertical, bool doSurfaceAngleCorrection)
		{
			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change texture offsets");
			
			//mxd
			if (General.Map.UDMF) {
				// Apply UDMF offsets
				MoveTextureOffset(new Point(-horizontal, -vertical));
				Point p = GetTextureOffset();
				mode.SetActionResult("Changed texture offsets to " + p.X + ", " + p.Y + ".");
			} else {
				//mxd. Apply classic offsets
				Sidedef.OffsetX = (Sidedef.OffsetX - horizontal);
				if (Texture != null) Sidedef.OffsetX %= Texture.Width;
				Sidedef.OffsetY = (Sidedef.OffsetY - vertical);
				if(geoType != VisualGeometryType.WALL_MIDDLE && Texture != null) Sidedef.OffsetY %= Texture.Height;

				mode.SetActionResult("Changed texture offsets to " + Sidedef.OffsetX + ", " + Sidedef.OffsetY + ".");
			}
			
			// Update sidedef geometry
			VisualSidedefParts parts = Sector.GetSidedefParts(Sidedef);
			parts.SetupAllParts();
		}

		//mxd
		public virtual void OnChangeTextureScale(float incrementX, float incrementY) {
			if(!General.Map.UDMF) return;

			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change wall scale");

			string keyX;
			string keyY;

			switch(GeometryType) {
				case VisualGeometryType.WALL_UPPER:
					keyX = "scalex_top";
					keyY = "scaley_top";
					break;

				case VisualGeometryType.WALL_MIDDLE:
				case VisualGeometryType.WALL_MIDDLE_3D:
					keyX = "scalex_mid";
					keyY = "scaley_mid";
					break;

				case VisualGeometryType.WALL_LOWER:
					keyX = "scalex_bottom";
					keyY = "scaley_bottom";
					break;

				default:
					throw new Exception("OnChangeTextureScale(): Got unknown GeometryType: " + GeometryType);
			}

			float scaleX = Sidedef.Fields.GetValue(keyX, 1.0f);
			float scaleY = Sidedef.Fields.GetValue(keyY, 1.0f);

			Sidedef.Fields.BeforeFieldsChange();

			if(incrementX != 0) {
				if(scaleX + incrementX == 0)
					scaleX *= -1;
				else
					scaleX += incrementX;
				UDMFTools.SetFloat(Sidedef.Fields, keyX, scaleX, 1.0f);
			}

			if(incrementY != 0) {
				if(scaleY + incrementY == 0)
					scaleY *= -1;
				else
					scaleY += incrementY;
				UDMFTools.SetFloat(Sidedef.Fields, keyY, scaleY, 1.0f);
			}

			//update geometry
			Setup();
			mode.SetActionResult("Wall scale changed to " + scaleX + ", " + scaleY);
		}

		#endregion
	}
}
