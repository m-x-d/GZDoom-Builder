
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

using Configuration = CodeImp.DoomBuilder.IO.Configuration;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	// This is an entry is the surface manager and contains the information
	// needed for a sector to place it's ceiling and floor surface geometry
	// in a vertexbuffer. Sectors keep a reference to this entry to tell the
	// surface manager to remove them if needed.
	internal class SurfaceEntry
	{
		// Number of vertices in the geometry and index of the buffer
		// This tells the surface manager which vertexbuffer this is in.
		public int numvertices;
		public int bufferindex;
		
		// Offset in the buffer (in number of vertices)
		public int vertexoffset;
		
		// Sector geometry (local copy used to quickly refill buffers)
		// The sector must set these!
		public FlatVertex[] floorvertices;
		public FlatVertex[] ceilvertices;
		
		// Sector images
		// The sector must set these!
		public long floortexture;
		public long ceiltexture;
		
		// Constructor
		internal SurfaceEntry(int numvertices, int bufferindex, int vertexoffset)
		{
			this.numvertices = numvertices;
			this.bufferindex = bufferindex;
			this.vertexoffset = vertexoffset;
		}

		// Constructor that copies the entry, but does not copy the vertices
		internal SurfaceEntry(SurfaceEntry oldentry)
		{
			this.numvertices = oldentry.numvertices;
			this.bufferindex = oldentry.bufferindex;
			this.vertexoffset = oldentry.vertexoffset;
		}
	}
}
