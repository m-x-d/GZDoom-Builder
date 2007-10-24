
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
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public class MapSelection : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Selected items
		private List<Vertex> vertices;
		private List<Linedef> linedefs;
		private List<Sector> sectors;
		private List<Thing> things;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public ICollection<Vertex> Vertices { get { return vertices; } }
		public ICollection<Linedef> Linedefs { get { return linedefs; } }
		public ICollection<Sector> Sectors { get { return sectors; } }
		public ICollection<Thing> Things { get { return things; } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public MapSelection()
		{
			// Initialize
			vertices = new List<Vertex>();
			linedefs = new List<Linedef>();
			sectors = new List<Sector>();
			things = new List<Thing>();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				ClearThings();
				ClearSectors();
				ClearLinedefs();
				ClearVertices();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Management

		// This adds a vertex
		public void AddVertex(Vertex v)
		{
			// Select it
			v.Selected++;
			vertices.Add(v);
		}

		// This adds a linedef
		public void AddLinedef(Linedef l)
		{
			// Select it
			l.Selected++;
			linedefs.Add(l);
		}

		// This adds a sector
		public void AddSector(Sector s)
		{
			// Select it
			s.Selected++;
			sectors.Add(s);
		}

		// This adds a thing
		public void AddThing(Thing t)
		{
			// Select it
			t.Selected++;
			things.Add(t);
		}

		// This removes a vertex
		public void RemoveVertex(Vertex v)
		{
			// Remove it
			v.Selected--;
			vertices.Remove(v);
		}

		// This adds a linedef
		public void RemoveLinedef(Linedef l)
		{
			// Remove it
			l.Selected--;
			linedefs.Remove(l);
		}

		// This adds a sector
		public void RemoveSector(Sector s)
		{
			// Remove it
			s.Selected--;
			sectors.Remove(s);
		}

		// This adds a thing
		public void RemoveThing(Thing t)
		{
			// Remove it
			t.Selected--;
			things.Remove(t);
		}

		// This clears vertices
		public void ClearVertices()
		{
			// Remove it
			foreach(Vertex v in vertices) v.Selected--;
			vertices.Clear();
		}

		// This clears linedefs
		public void ClearLinedefs()
		{
			// Remove it
			foreach(Linedef l in linedefs) l.Selected--;
			linedefs.Clear();
		}

		// This clears sectors
		public void ClearSectors()
		{
			// Remove it
			foreach(Sector s in sectors) s.Selected--;
			sectors.Clear();
		}

		// This clears things
		public void ClearThings()
		{
			// Remove it
			foreach(Thing t in things) t.Selected--;
			things.Clear();
		}
		
		#endregion
	}
}
