
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
	internal class DirectoryReader : IDataReader
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Source
		private string path;
		private bool readtextures;
		private bool readflats;

		private bool issuspended = false;
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public string Location { get { return path; } }
		public bool IsDisposed { get { return isdisposed; } }
		public bool IsSuspended { get { return issuspended; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DirectoryReader(DataLocation dl)
		{
			// Initialize
			this.path = dl.location;
			this.readtextures = dl.textures;
			this.readflats = dl.flats;

			General.WriteLogLine("Opening directory resource '" + dl.location + "'");

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				General.WriteLogLine("Closing directory resource '" + path + "'");

				// Clean up

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
		}

		// This resumes use of this resource
		public void Resume()
		{
			issuspended = false;
		}

		#endregion

		#region ================== Textures

		#endregion
	}
}
