
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
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Plugins;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class PluginController : IPluginController
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public PluginController()
		{
			// Initialize
			
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
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		#endregion
	}
}
