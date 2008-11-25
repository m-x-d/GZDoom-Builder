
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

#endregion

namespace CodeImp.DoomBuilder.VisualModes
{
	internal class Clipper
	{
		#region ================== Constants

		private const float MULTIPLIER = 10000f;
		private const int MAXRANGE = (int)(Angle2D.PI2 * MULTIPLIER);
		private const int HALFRANGE = (int)(Angle2D.PI * MULTIPLIER);
		private const int MINRANGE = 0;

		#endregion
		
		#region ================== ClipRange struct

		// ClipRange structure
		private struct ClipRange
		{
			// Variables
			public int low;
			public int high;

			// Constructor
			public ClipRange(int low, int high)
			{
				this.low = low;
				this.high = high;
			}
		}

		#endregion
		
		#region ================== Variables
		
		// Position where we are viewing from
		private Vector2D position;
		
		// Clipping ranges
		private LinkedList<ClipRange> ranges;

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Clipper(Vector2D pos)
		{
			// Initialize
			this.position = pos;
			this.ranges = new LinkedList<ClipRange>();
		}

		// Constructor
		public Clipper(Clipper copy)
		{
			// Initialize
			this.position = copy.position;
			this.ranges = new LinkedList<ClipRange>(copy.ranges);
		}

		// Disposer
		public void Dispose()
		{
			this.ranges.Clear();
		}
		
		#endregion

		#region ================== Testing methods
		
		// This tests a range to see if it is at least partially visible
		public bool TestRange(Vector2D a, Vector2D b)
		{
			int i1, i2, c1, c2, m;
			float a1, a2;
			
			// Get angles
			a1 = Angle2D.Normalized(Vector2D.GetAngle(position, a));
			a2 = Angle2D.Normalized(Vector2D.GetAngle(position, b));
			
			// Convert angles to ranges
			i1 = (int)(a1 * MULTIPLIER);
			i2 = (int)(a2 * MULTIPLIER);
			c1 = Math.Min(i1, i2);
			c2 = Math.Max(i1, i2);
			
			// Determine rotation direction
			m = c2 - c1;
			if(m < MINRANGE) m += MAXRANGE;
			if(m < HALFRANGE)
			{
				// Check if the range goes through zero point
				if(c2 < c1)
				{
					// Test two ranges
					return RangeVisible(new ClipRange(c1, MAXRANGE)) ||
						   RangeVisible(new ClipRange(MINRANGE, c2));
				}
				else
				{
					// Test a single range
					return RangeVisible(new ClipRange(c1, c2));
				}
			}
			else
			{
				// Check if the range goes through zero point
				if(c2 > c1)
				{
					// Test two ranges
					return RangeVisible(new ClipRange(MINRANGE, c1)) ||
						   RangeVisible(new ClipRange(c2, MAXRANGE));
				}
				else
				{
					// Test a single range
					return RangeVisible(new ClipRange(c2, c1));
				}
			}
		}
		
		// This tests a single range for visibility
		private bool RangeVisible(ClipRange r)
		{
			// Go for all clipped ranges
			foreach(ClipRange c in ranges)
			{
				// Does clipped range completely hide the given range?
				if((c.low <= r.low) && (c.high >= r.high))
				{
					// No further testing needed, range is clipped
					return false;
				}
			}

			// Not completely clipped
			return true;
		}
		
		#endregion

		#region ================== Clipping methods

		// This tests a range to see if it is at least partially visible
		public bool InsertRange(Vector2D a, Vector2D b)
		{
			int i1, i2, c1, c2, m;
			float a1, a2;

			// Get angles
			a1 = Angle2D.Normalized(Vector2D.GetAngle(position, a));
			a2 = Angle2D.Normalized(Vector2D.GetAngle(position, b));

			// Convert angles to ranges
			i1 = (int)(a1 * MULTIPLIER);
			i2 = (int)(a2 * MULTIPLIER);
			c1 = Math.Min(i1, i2);
			c2 = Math.Max(i1, i2);

			// Determine rotation direction
			m = c2 - c1;
			if(m < MINRANGE) m += MAXRANGE;
			if(m < HALFRANGE)
			{
				// Check if the range goes through zero point
				if(c2 < c1)
				{
					// Add two ranges
					return AddRange(new ClipRange(c1, MAXRANGE)) ||
						   AddRange(new ClipRange(MINRANGE, c2));
				}
				else
				{
					// Add a single range
					return AddRange(new ClipRange(c1, c2));
				}
			}
			else
			{
				// Check if the range goes through zero point
				if(c2 > c1)
				{
					// Add two ranges
					return AddRange(new ClipRange(MINRANGE, c1)) ||
						   AddRange(new ClipRange(c2, MAXRANGE));
				}
				else
				{
					// Add a single range
					return AddRange(new ClipRange(c2, c1));
				}
			}
		}

		// This tests a single range for visibility
		// Returns true when the entire range has been clipped
		private bool AddRange(ClipRange r)
		{
			LinkedListNode<ClipRange> current, next;
			ClipRange c;

			// Go for all ranges to find overlappings
			current = ranges.First;
			while(current != null)
			{
				// Keep reference to the next
				next = current.Next;
				c = current.Value;
				
				// Check if ranges overlap
				if((c.low <= (r.high + 1)) && (c.high >= (r.low - 1)))
				{
					// Remove old range from list
					ranges.Remove(current);

					// Extend range with overlapping range
					if(c.low < r.low) r.low = c.low;
					if(c.high > r.high) r.high = c.high;
				}

				// Move to the next
				current = next;
			}
			
			// Insert the new range
			ranges.AddLast(r);

			// Return true when entire range is now clipped
			return (r.low == MINRANGE) && (r.high == MAXRANGE);
		}

		#endregion
	}
}
