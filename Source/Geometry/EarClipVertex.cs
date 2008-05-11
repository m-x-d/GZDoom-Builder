
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
using SlimDX.Direct3D9;
using System.Drawing;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Geometry
{
	public class EarClipVertex
	{
		#region ================== Variables
		
		// Position
		private Vector2D pos;
		
		// Lists
		private LinkedListNode<EarClipVertex> vertslink;
		private LinkedListNode<EarClipVertex> reflexlink;
		private LinkedListNode<EarClipVertex> eartiplink;
		
		#endregion

		#region ================== Properties

		public Vector2D Position { get { return pos; } }
		internal LinkedListNode<EarClipVertex> MainListNode { get { return vertslink; } }
		public bool IsReflex { get { return (reflexlink != null); } }
		public bool IsEarTip { get { return (eartiplink != null); } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal EarClipVertex(EarClipVertex v)
		{
			// Initialize
			this.pos = v.pos;

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		internal EarClipVertex(Vector2D v)
		{
			// Initialize
			this.pos = v;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal void Dispose()
		{
			reflexlink = null;
			eartiplink = null;
			vertslink = null;
		}
		
		#endregion

		#region ================== Methods

		// This sets the main linked list node
		internal void SetVertsLink(LinkedListNode<EarClipVertex> link)
		{
			this.vertslink = link;
		}
		
		// This removes the item from all lists
		internal void Remove()
		{
			vertslink.List.Remove(vertslink);
			if(reflexlink != null) reflexlink.List.Remove(reflexlink);
			if(eartiplink != null) eartiplink.List.Remove(eartiplink);
			reflexlink = null;
			eartiplink = null;
			vertslink = null;
		}

		// This adds to reflexes list
		public void AddReflex(LinkedList<EarClipVertex> reflexes)
		{
			if(vertslink == null) throw new Exception();
			if(reflexlink == null) reflexlink = reflexes.AddLast(this);
		}

		// This removes from reflexes list
		internal void RemoveReflex()
		{
			if(reflexlink != null) reflexlink.List.Remove(reflexlink);
			reflexlink = null;
		}

		// This adds to eartips list
		internal void AddEarTip(LinkedList<EarClipVertex> eartips)
		{
			if(vertslink == null) throw new Exception();
			if(eartiplink == null) eartiplink = eartips.AddLast(this);
		}

		// This removes from eartips list
		internal void RemoveEarTip()
		{
			if(eartiplink != null) eartiplink.List.Remove(eartiplink);
			eartiplink = null;
		}
		
		#endregion
	}
}
