
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
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public abstract class DataReader : IDisposable
	{
		#region ================== Variables

		protected DataLocation location;
		protected bool issuspended = false;
		protected bool isdisposed = false;

		#endregion

		#region ================== Properties

		public DataLocation Location { get { return location; } }
		public bool IsDisposed { get { return isdisposed; } }
		public bool IsSuspended { get { return issuspended; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DataReader(DataLocation dl)
		{
			// Keep information
			location = dl;
		}

		// Disposer
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Management

		// This suspends use of this resource
		public virtual void Suspend()
		{
			issuspended = true;
		}

		// This resumes use of this resource
		public virtual void Resume()
		{
			issuspended = false;
		}

		#endregion

		#region ================== Palette

		// When implemented, this should find and load a PLAYPAL palette
		public virtual Playpal LoadPalette() { return null; }
		
		#endregion

		#region ================== Textures

		// When implemented, this should read the patch names
		public virtual PatchNames LoadPatchNames() { return null; }

		// When implemented, this returns the patch lump
		public virtual Stream GetPatchData(string pname) { return null; }

		// When implemented, this loads the textures
		public virtual ICollection<ImageData> LoadTextures(PatchNames pnames) { return null; }

		#endregion
	}
}
