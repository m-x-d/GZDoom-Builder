
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
	internal class WADReader : IDataReader
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Source
		private WAD file;
		private string location;
		private bool managefile;

		private bool issuspended = false;
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public string Location { get { return location; } }
		public bool IsDisposed { get { return isdisposed; } }
		public bool IsSuspended { get { return issuspended; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public WADReader(DataLocation dl)
		{
			// Initialize
			this.location = dl.location;
			file = new WAD(location, true);
			managefile = true;

			General.WriteLogLine("Opening WAD resource '" + file.Filename + "'");

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		public WADReader(WAD wadfile)
		{
			// Initialize
			file = wadfile;
			managefile = false;

			General.WriteLogLine("Opening WAD resource '" + file.Filename + "' (file already open)");

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				General.WriteLogLine("Closing WAD resource '" + file.Filename + "'");

				// Clean up
				if(managefile) file.Dispose();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Management

		// This suspends use of this resource
		public void Suspend()
		{
			issuspended = true;
			if(managefile) file.Dispose();
		}

		// This resumes use of this resource
		public void Resume()
		{
			issuspended = false;
			if(managefile) file = new WAD(location, true);
		}
		
		#endregion

		#region ================== Textures

		#endregion
	}
}
