
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
using System.Drawing;
using System.ComponentModel;
using CodeImp.DoomBuilder.Map;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.VisualModes
{
	public abstract class VisualThing : IVisualPickable, ID3DResource, IComparable<VisualThing>
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		// Thing
		private Thing thing;
		
		// Texture
		private ImageData texture;
		
		// Geometry
		private WorldVertex[] spritevertices;
		private WorldVertex[] cagevertices;
		private VertexBuffer geobuffer;
		private bool updategeo;
		private int spritetriangles;
		private int cagetriangles;
		
		// Disposing
		private bool isdisposed = false;
		
		#endregion
		
		#region ================== Properties
		
		internal VertexBuffer GeometryBuffer { get { return geobuffer; } }
		internal bool NeedsUpdateGeo { get { return updategeo; } }
		internal int SpriteTriangles { get { return spritetriangles; } }
		internal int CageTriangles { get { return cagetriangles; } }
		internal int CageOffset { get { return spritevertices.Length; } }
		
		public Thing Thing { get { return thing; } }
		public bool IsDisposed { get { return isdisposed; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public VisualThing(Thing t)
		{
			// Initialize
			this.thing = t;
			
			// Register as resource
			General.Map.Graphics.RegisterResource(this);
		}

		// Disposer
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				if(geobuffer != null) geobuffer.Dispose();
				geobuffer = null;

				// Unregister resource
				General.Map.Graphics.UnregisterResource(this);

				// Done
				isdisposed = true;
			}
		}
		
		#endregion
		
		#region ================== Methods
		
		// This is called before a device is reset
		// (when resized or display adapter was changed)
		public void UnloadResource()
		{
			// Trash geometry buffer
			if(geobuffer != null) geobuffer.Dispose();
			geobuffer = null;
			updategeo = true;
		}
		
		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public void ReloadResource()
		{
			// Make new geometry
			//Update();
		}
		
		// This compares for sorting by sprite
		public int CompareTo(VisualThing other)
		{
			return Math.Sign(this.texture.LongName - other.texture.LongName);
		}
		
		// This sets the vertices for the thing sprite
		protected void SetSpriteVertices(ICollection<WorldVertex> verts)
		{
			// Copy vertices
			spritevertices = new WorldVertex[verts.Count];
			verts.CopyTo(spritevertices, 0);
			spritetriangles = spritevertices.Length / 3;
			updategeo = true;
		}
		
		// This sets the vertices for the thing cage
		protected void SetCageVertices(ICollection<WorldVertex> verts)
		{
			// Copy vertices
			cagevertices = new WorldVertex[verts.Count];
			verts.CopyTo(cagevertices, 0);
			cagetriangles = cagevertices.Length / 3;
			updategeo = true;
		}

		// This updates the visual sector
		public void Update()
		{
			// Trash geometry buffer
			if(geobuffer != null) geobuffer.Dispose();
			geobuffer = null;
			
			// Count the number of vertices there are
			int numverts = spritevertices.Length + cagevertices.Length;
			
			// Any vertics?
			if(numverts > 0)
			{
				// Make a new buffer
				geobuffer = new VertexBuffer(General.Map.Graphics.Device, WorldVertex.Stride * numverts,
											 Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);
				
				// Fill the buffer
				DataStream bufferstream = geobuffer.Lock(0, WorldVertex.Stride * numverts, LockFlags.Discard);
				bufferstream.WriteRange<WorldVertex>(spritevertices);
				bufferstream.WriteRange<WorldVertex>(cagevertices);
				geobuffer.Unlock();
				bufferstream.Dispose();
			}
			
			// Done
			updategeo = false;
		}
		
		/// <summary>
		/// This is called when the thing must be tested for line intersection. This should reject
		/// as fast as possible to rule out all geometry that certainly does not touch the line.
		/// </summary>
		public virtual bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir)
		{
			return false;
		}
		
		/// <summary>
		/// This is called when the thing must be tested for line intersection. This should perform
		/// accurate hit detection and set u_ray to the position on the ray where this hits the geometry.
		/// </summary>
		public virtual bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref float u_ray)
		{
			return false;
		}
		
		#endregion
	}
}
