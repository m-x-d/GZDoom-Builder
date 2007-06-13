
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.Map
{
	internal class Vertex : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Position
		private Vector2D pos;

		// References
		private List<Linedef> linedefs;

		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public List<Linedef> Linedefs { get { return linedefs; } }
		public Vector2D Position { get { return pos; } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Vertex(Vector2D pos)
		{
			// Initialize
			this.pos = pos;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Dispose the lines that are attached to this vertex
				// because a linedef cannot exist without 2 vertices.
				foreach(Linedef l in linedefs) l.Dispose();
				
				// Clean up
				linedefs = null;

				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// This attaches a linedef
		public void Attach(Linedef l) { linedefs.Add(l); }

		// This detaches a linedef
		public void Detach(Linedef l) { linedefs.Remove(l); }
		
		// This rounds the coordinates to integrals
		public void Round()
		{
			// Round to integrals
			pos.x = (float)Math.Round(pos.x);
			pos.y = (float)Math.Round(pos.y);
		}
		
		#endregion
	}
}
