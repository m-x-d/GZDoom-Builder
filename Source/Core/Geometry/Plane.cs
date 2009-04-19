
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

#endregion

namespace CodeImp.DoomBuilder.Geometry
{
	public struct Plane
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		//
		// Plane definition:
		// A * x + B * y + C * z + D = 0
		//
		// A, B, C is the normal
		// D is the offset along the normal (negative)
		//
		private Vector3D normal;
		private float offset;
		
		#endregion
		
		#region ================== Properties
		
		public Vector3D Normal { get { return normal; } }
		public float Offset { get { return offset; } set { offset = value; } }
		public float a { get { return normal.x; } }
		public float b { get { return normal.y; } }
		public float c { get { return normal.z; } }
		public float d { get { return offset; } set { offset = value; } }
		
		#endregion
		
		#region ================== Constructors
		
		// Constructor
		public Plane(Vector3D normal, float offset)
		{
			#if DEBUG
				if(!normal.IsNormalized())
					General.Fail("Attempt to create a plane with a vector that is not normalized!");
			#endif
			this.normal = normal;
			this.offset = offset;
		}
		
		// Constructor
		public Plane(Vector3D normal, Vector3D position)
		{
			#if DEBUG
				if(!normal.IsNormalized())
					General.Fail("Attempt to create a plane with a vector that is not normalized!");
			#endif
			this.normal = normal;
			this.offset = -Vector3D.DotProduct(normal, position);
		}
		
		// Constructor
		public Plane(Vector3D p1, Vector3D p2, Vector3D p3)
		{
			this.normal = Vector3D.CrossProduct(p1 - p2, p3 - p2).GetNormal();
			this.offset = -Vector3D.DotProduct(normal, p2);
		}
		
		#endregion
		
		#region ================== Methods
		
		// This tests for intersection using a position and direction
		public bool GetIntersection(Vector3D position, Vector3D direction, ref float u_ray)
		{
			float a = Vector3D.DotProduct(normal, direction);
			if(a != 0.0f)
			{
				float b = Vector3D.DotProduct(normal, position);
				u_ray = (offset - b) / a;
				return true;
			}
			else
			{
				return false;
			}
		}
		
		// This returns the smallest distance to the plane and the side on which the point lies.
		// > 0 means the point lies on the front of the plane
		// < 0 means the point lies behind the plane
		public float Distance(Vector3D p)
		{
			return Vector3D.DotProduct(p, normal) + offset;
		}
		
		// This returns a point on the plane closest to the given point
		public Vector3D ClosestOnPlane(Vector3D p)
		{
			float d = Vector3D.DotProduct(p, normal) + offset;
			return p - normal * d;
		}
		
		// This inverts the plane
		public Plane GetInverted()
		{
			return new Plane(-normal, -offset);
		}
		
		#endregion
	}
}
