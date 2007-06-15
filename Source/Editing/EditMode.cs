
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
using CodeImp.DoomBuilder.Interface;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	internal class EditMode : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Disposing
		protected bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public EditMode()
		{
			// Initialize

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

				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// Optional interface methods
		public virtual void MouseClick(MouseEventArgs e) { }
		public virtual void MouseDoubleClick(MouseEventArgs e) { }
		public virtual void MouseDown(MouseEventArgs e) { }
		public virtual void MouseEnter(EventArgs e) { }
		public virtual void MouseLeave(EventArgs e) { }
		public virtual void MouseMove(MouseEventArgs e) { }
		public virtual void MouseUp(MouseEventArgs e) { }
		public virtual void Cancel() { }
		
		#endregion
	}
}
