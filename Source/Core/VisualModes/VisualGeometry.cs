
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
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.GZBuilder.Data; //mxd

#endregion

namespace CodeImp.DoomBuilder.VisualModes
{
	public abstract class VisualGeometry : IVisualPickable, IComparable<VisualGeometry>
	{
		#region ================== Variables

		// Texture
		private ImageData texture;
		
		// Vertices
		private WorldVertex[] vertices;
		private int triangles;

		// Desired modulate color
		private PixelColor modulatecolor;
		private Color4 modcolor4;
		
		// Selected?
		protected bool selected;
		
		// Elements that this geometry is bound to
		// Only the sector is required, sidedef is only for walls
		private VisualSector sector;
		private Sidedef sidedef;

		/// <summary>
		/// Absolute intersecting coordinates are set during object picking. This is not set if the geometry is not bound to a sidedef.
		/// </summary>
		protected Vector3D pickintersect;

		/// <summary>
		/// Distance unit along the object picking ray is set during object picking. (0.0 is at camera, 1.0f is at far plane) This is not set if the geometry is not bound to a sidedef.
		/// </summary>
		protected float pickrayu;
		
		// Rendering
		private int renderpass = (int)RenderPass.Solid;
		
		// Sector buffer info
		private int vertexoffset;

		//mxd
		private Vector3[] boundingBox;
		protected VisualGeometryType geoType;
		
		#endregion

		#region ================== Properties
		
		// Internal properties
		public WorldVertex[] Vertices { get { return vertices; } } //mxd
		internal int VertexOffset { get { return vertexoffset; } set { vertexoffset = value; } }
		internal int Triangles { get { return triangles; } }
		internal int RenderPassInt { get { return renderpass; } }
		internal Color4 ModColor4 { get { return modcolor4; } }

		//mxd
		public Vector3[] BoundingBox { get { return boundingBox; } }
		public VisualGeometryType GeometryType { get { return geoType; } }

		/// <summary>
		/// Render pass in which this geometry must be rendered. Default is Solid.
		/// </summary>
		public RenderPass RenderPass { get { return (RenderPass)renderpass; } set { renderpass = (int)value; } }

		/// <summary>
		/// Image to use as texture on this geometry.
		/// </summary>
		public ImageData Texture { get { return texture; } set { texture = value; } }

		/// <summary>
		/// Color to modulate the texture pixels with.
		/// </summary>
		public PixelColor ModulateColor { get { return modulatecolor; } set { modcolor4 = value.ToColorValue(); modulatecolor = value; } }

		/// <summary>
		/// Returns the VisualSector this geometry has been added to.
		/// </summary>
		public VisualSector Sector { get { return sector; } internal set { sector = value; } }
		
		/// <summary>
		/// Returns the Sidedef that this geometry is created for. Null for geometry that is sector-wide.
		/// </summary>
		public Sidedef Sidedef { get { return sidedef; } }

		/// <summary>
		/// Selected or not? This is only used by the core to determine what color to draw it with.
		/// </summary>
		public bool Selected { get { return selected; } set { selected = value; } }

		#endregion

		#region ================== Constructor / Destructor
		
		/// <summary>
		/// This creates sector-global visual geometry. This geometry is always visible when any of the sector is visible.
		/// </summary>
		public VisualGeometry(VisualSector vs)
		{
			this.sector = vs;
			this.ModulateColor = new PixelColor(255, 255, 255, 255);
			//mxd
			geoType = VisualGeometryType.UNKNOWN;
		}

		/// <summary>
		/// This creates visual geometry that is bound to a sidedef. This geometry is only visible when the sidedef is visible. It is automatically back-face culled during rendering and automatically XY intersection tested as well as back-face culled during object picking.
		/// </summary>
		/// <param name="sd"></param>
		public VisualGeometry(VisualSector vs, Sidedef sd)
		{
			this.sector = vs;
			this.sidedef = sd;
			this.ModulateColor = new PixelColor(255, 255, 255, 255);
			//mxd
			geoType = VisualGeometryType.UNKNOWN;
		}

		#endregion

		#region ================== Methods
		
		// This sets the vertices for this geometry
		protected void SetVertices(ICollection<WorldVertex> verts)
		{
			// Copy vertices
			if (verts != null) { //mxd
				vertices = new WorldVertex[verts.Count];
				verts.CopyTo(vertices, 0);
				triangles = vertices.Length / 3;

				//mxd
				CalculateNormals();
			} else {
				vertices = new WorldVertex[1];
				triangles = 0;
			}

			if(sector != null) sector.NeedsUpdateGeo = true;
		}

		//mxd. Normals calculation algorithm taken from OpenGl wiki 
		protected void CalculateNormals() {
			if (triangles > 0) {
				int startIndex;
				Vector3 U, V;

				BoundingBoxSizes bbs = new BoundingBoxSizes(vertices[0]);

				for (int i = 0; i < triangles; i++) {
					startIndex = i * 3;
					WorldVertex p1 = vertices[startIndex];
					WorldVertex p2 = vertices[startIndex + 1];
					WorldVertex p3 = vertices[startIndex + 2];

					U = new Vector3(p2.x - p1.x, p2.y - p1.y, p2.z - p1.z);
					V = new Vector3(p3.x - p1.x, p3.y - p1.y, p3.z - p1.z);

					p1.nx = p2.nx = p3.nx = -(U.Y * V.Z - U.Z * V.Y);
					p1.ny = p2.ny = p3.ny = -(U.Z * V.X - U.X * V.Z);
					p1.nz = p2.nz = p3.nz = -(U.X * V.Y - U.Y * V.X);

					vertices[startIndex] = p1;
					vertices[startIndex + 1] = p2;
					vertices[startIndex + 2] = p3;

					BoundingBoxTools.UpdateBoundingBoxSizes(ref bbs, p1);
					BoundingBoxTools.UpdateBoundingBoxSizes(ref bbs, p2);
					BoundingBoxTools.UpdateBoundingBoxSizes(ref bbs, p3);
				}

				boundingBox = BoundingBoxTools.CalculateBoundingPlane(bbs);
			}
		}

		//mxd. Used to get proper sector from 3d-floors
		public virtual Sector GetControlSector() {
			return sector.Sector;
		}

		//mxd. Used to get proper linedef from 3d-floors
		public virtual Linedef GetControlLinedef() {
			return sidedef.Line;
		}
		
		// This compares for sorting by sector
		public int CompareTo(VisualGeometry other)
		{
			// Compare sectors
			return this.sector.Sector.FixedIndex - other.sector.Sector.FixedIndex;
		}

		// This keeps the results for a sidedef intersection
		internal void SetPickResults(Vector3D intersect, float u)
		{
			this.pickintersect = intersect;
			this.pickrayu = u;
		}
		
		/// <summary>
		/// This is called when the geometry must be tested for line intersection. This should reject
		/// as fast as possible to rule out all geometry that certainly does not touch the line.
		/// </summary>
		public virtual bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			return false;
		}
		
		/// <summary>
		/// This is called when the geometry must be tested for line intersection. This should perform
		/// accurate hit detection and set u_ray to the position on the ray where this hits the geometry.
		/// </summary>
		public virtual bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref float u_ray)
		{
			return false;
		}

		#endregion
	}

	//mxd
	public enum VisualGeometryType{
		FLOOR,
		CEILING,
		WALL_UPPER,
		WALL_MIDDLE,
		WALL_MIDDLE_3D,
		WALL_LOWER,
		UNKNOWN,
	}
}
