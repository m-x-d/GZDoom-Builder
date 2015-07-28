
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

using System.Collections.Generic;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public struct Association
	{
		private HashSet<int> tags;
		private Vector2D center;
		private UniversalType type;

		public HashSet<int> Tags { get { return tags; } }
		public Vector2D Center { get { return center; } }
		public UniversalType Type { get { return type; } }

		// This sets up the association
		public Association(Vector2D center, int tag, int type)
		{
			this.tags = new HashSet<int>(); //mxd
			tags.Add(tag); //mxd
			this.type = (UniversalType)type;
			this.center = center;
		}

		// This sets up the association
		public Association(Vector2D center, int tag, UniversalType type)
		{
			this.tags = new HashSet<int>(); //mxd
			tags.Add(tag); //mxd
			this.type = type;
			this.center = center;
		}

		//mxd. This also sets up the association
		public Association(Vector2D center, IEnumerable<int> tags, int type)
		{
			this.tags = new HashSet<int>(tags); //mxd
			this.type = (UniversalType)type;
			this.center = center;
		}

		//mxd. This also sets up the association
		public Association(Vector2D center, IEnumerable<int> tags, UniversalType type)
		{
			this.tags = new HashSet<int>(tags); //mxd
			this.type = type;
			this.center = center;
		}

		// This sets up the association
		public void Set(Vector2D center, int tag, int type)
		{
			this.tags = new HashSet<int>(); //mxd
			tags.Add(tag); //mxd
			this.type = (UniversalType)type;
			this.center = center;
		}

		// This sets up the association
		public void Set(Vector2D center, int tag, UniversalType type)
		{
			this.tags = new HashSet<int>(); //mxd
			tags.Add(tag); //mxd
			this.type = type;
			this.center = center;
		}

		//mxd. This also sets up the association
		public void Set(Vector2D center, IEnumerable<int> tags, int type)
		{
			this.tags = new HashSet<int>(tags); //mxd
			this.type = (UniversalType)type;
			this.center = center;
		}

		//mxd. This also sets up the association
		public void Set(Vector2D center, IEnumerable<int> tags, UniversalType type)
		{
			this.tags = new HashSet<int>(tags); //mxd
			this.type = type;
			this.center = center;
		}

		// This compares an association
		public static bool operator ==(Association a, Association b)
		{
			return (a.type == b.type) && a.tags.SetEquals(b.tags);
		}

		// This compares an association
		public static bool operator !=(Association a, Association b)
		{
			return (a.type != b.type) || !a.tags.SetEquals(b.tags);
		}

		//mxd 
		public override int GetHashCode() 
		{
			return base.GetHashCode();
		}

		//mxd
		public override bool Equals(object obj) 
		{
			if(!(obj is Association)) return false;

			Association b = (Association)obj;
			return (type == b.type) && tags.SetEquals(b.tags);
		}
	}
}
