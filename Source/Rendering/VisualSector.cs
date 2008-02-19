
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
using SlimDX.Direct3D;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	public class VisualSector : ID3DResource
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Geometry
		private List<VisualGeometry> geolist;
		private VertexBuffer geobuffer;
		private bool updategeo;
		
		// Original sector
		private Sector sector;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		internal List<VisualGeometry> GeometryList { get { return geolist; } }
		internal VertexBuffer GeometryBuffer { get { return geobuffer; } }
		internal bool NeedsUpdateGeo { get { return updategeo; } set { updategeo |= value; } }
		public bool IsDisposed { get { return isdisposed; } }
		public Sector Sector { get { return sector; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public VisualSector(Sector s)
		{
			// Initialize
			this.sector = s;
			geolist = new List<VisualGeometry>();

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
				foreach(VisualGeometry g in geolist) g.Sector = null;
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
		public virtual void UnloadResource()
		{
			// Trash geometry buffer
			if(geobuffer != null) geobuffer.Dispose();
			geobuffer = null;
			updategeo = true;
		}

		// This is called resets when the device is reset
		// (when resized or display adapter was changed)
		public virtual void ReloadResource()
		{
			// Make new geometry
			//Update();
		}
		
		// This updates the visual sector
		public void Update()
		{
			DataStream bufferstream;
			int numverts = 0;
			int v = 0;
			
			// Trash geometry buffer
			if(geobuffer != null) geobuffer.Dispose();
			geobuffer = null;
			
			// Sort the geo list by texture
			geolist.Sort();
			
			// Count the number of vertices there are
			foreach(VisualGeometry g in geolist) numverts += g.Vertices.Length;
			
			// Any vertics?
			if(numverts > 0)
			{
				// Make a new buffer
				geobuffer = new VertexBuffer(General.Map.Graphics.Device, WorldVertex.Stride * numverts,
											 Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);

				// Fill the buffer
				bufferstream = geobuffer.Lock(0, WorldVertex.Stride * numverts, LockFlags.Discard);
				foreach(VisualGeometry g in geolist)
				{
					if(g.Vertices.Length > 0)
					{
						bufferstream.WriteRange<WorldVertex>(g.Vertices);
						g.VertexOffset = v;
						v += g.Vertices.Length;
					}
				}
				geobuffer.Unlock();
				bufferstream.Dispose();
			}
			
			// Done
			updategeo = false;
		}

		// This adds geometry
		public void AddGeometry(VisualGeometry geo)
		{
			geo.Sector = this;
			geolist.Add(geo);
			updategeo = true;
		}

		// This removes all geometry
		public void ClearGeometry()
		{
			foreach(VisualGeometry g in geolist) g.Sector = null;
			geolist.Clear();
			updategeo = true;
		}
		
		#endregion
	}
}
