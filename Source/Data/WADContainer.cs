
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal class WADContainer : IDataContainer
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Source
		private WAD file;
		private bool managefile;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public WADContainer(string filename)
		{
			// Initialize
			file = new WAD(filename, true);
			managefile = true;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		public WADContainer(WAD wadfile)
		{
			// Initialize
			file = wadfile;
			managefile = false;

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
				if(managefile) file.Dispose();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		#endregion
	}
}
