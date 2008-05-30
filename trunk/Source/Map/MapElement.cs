
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

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public abstract class MapElement : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Univeral fields
		private UniFields fields;

		// Disposing
		protected bool isdisposed = false;
		
		#endregion

		#region ================== Properties

		public UniFields Fields { get { return fields; } }
		public bool IsDisposed { get { return isdisposed; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal MapElement()
		{
			// Initialize
			fields = new UniFields();
		}

		// Disposer
		public virtual void Dispose()
		{
			if(!isdisposed)
			{
				// Clean up
				fields = null;
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Fields
		
		// This copies fields to any other element
		protected void CopyFieldsTo(MapElement element)
		{
			element.fields = new UniFields(this.fields);
		}
		
		#endregion
	}
}
