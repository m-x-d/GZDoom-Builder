
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

namespace CodeImp.DoomBuilder.Rendering
{
	public abstract class Renderer : IDisposable, ID3DResource
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Graphics
		protected D3DGraphics graphics;

		// Disposing
		protected bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Renderer(D3DGraphics g)
		{
			// Initialize
			this.graphics = g;

			// Register as resource
			g.RegisterResource(this);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Unregister resource
				graphics.UnregisterResource(this);
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// This is called when the graphics need to be reset
		public virtual void Reset() { }

		// For DirectX resources
		public virtual void UnloadResource() { }
		public virtual void ReloadResource() { }

		#endregion
	}
}
