
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
using CodeImp.DoomBuilder.Rendering;
using SlimDX.Direct3D;
using System.Drawing;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Geometry
{
	/// <summary>
	/// Responsible for creating and caching sector polygons.
	/// </summary>
	public abstract class Triangulator
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Final polygons by sector reference
		private Dictionary<Sector, TriangleList> triangles;
		private Dictionary<Sector, ulong> checksums;
		
		// Disposing
		protected bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		/// <summary>
		/// Responsible for creating and caching sector polygons.
		/// </summary>
		public Triangulator()
		{
			// Initialize
			triangles = new Dictionary<Sector, TriangleList>(General.Map.Map.Sectors.Count);
			checksums = new Dictionary<Sector, ulong>(General.Map.Map.Sectors.Count);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				triangles = null;
				checksums = null;
				
				// Done
				isdisposed = true;
			}
		}

		#endregion
		
		#region ================== Checksums

		// This creates a checksum from a sector
		private ulong CreateSectorChecksum(Sector sector)
		{
			ulong checksum = (ulong)sector.Sidedefs.Count;
			int shift = 11;
			
			// Go for all sidedefs in the sector
			foreach(Sidedef sd in sector.Sidedefs)
			{
				// Add the information that is significant for the polygon to the bit value
				AddChecksumValue(ref checksum, unchecked((ulong)sd.Line.Start.X), ref shift);
				AddChecksumValue(ref checksum, unchecked((ulong)sd.Line.Start.Y), ref shift);
			}

			// Return checksum
			return checksum;
		}

		// This adds to a checksum
		private void AddChecksumValue(ref ulong checksum, ulong value, ref int shift)
		{
			checksum = ((checksum << shift) | (checksum >> (64 - shift))) ^ value;
			shift = (shift + 23) & 0x3F;
		}
		
		#endregion

		#region ================== Methods

		// This triangulates a sector or returns a cached result
		public TriangleList Triangulate(Sector sector)
		{
			// Check if we need to do full triangulation
			if(CheckSectorUpdateNeeded(sector))
			{
				// Triangulate now and store it in cache
				PerformTriangulation(sector);
			}
			
			// Return result
			return triangles[sector];
		}

		// This must be implemented by derived classes to do the triangulation
		protected abstract void PerformTriangulation(Sector sector);
		
		// This adds triangles for a specific sector
		protected void StoreTriangles(Sector sector, TriangleList tris)
		{
			ulong c = CreateSectorChecksum(sector);
			if(triangles.ContainsKey(sector)) triangles.Remove(sector);
			if(checksums.ContainsKey(sector)) checksums.Remove(sector);
			triangles.Add(sector, tris);
			checksums.Add(sector, c);
		}

		// This checks if a sector needs updating
		private bool CheckSectorUpdateNeeded(Sector sector)
		{
			// Sector already added?
			if(checksums.ContainsKey(sector))
			{
				// Compare checksum
				ulong c = CreateSectorChecksum(sector);
				return (checksums[sector] != c);
			}
			else
			{
				// We don't know this sector yet, needs updating!
				return true;
			}
		}
		
		// This clears the cache
		public void ClearCache()
		{
			triangles.Clear();
			checksums.Clear();
		}
		
		#endregion
	}
}
