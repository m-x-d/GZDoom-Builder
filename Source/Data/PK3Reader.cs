
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
using ICSharpCode.SharpZipLib.Zip;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal sealed class PK3Reader : DataReader
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public PK3Reader(DataLocation dl) : base(dl)
		{
			// Initialize
			General.WriteLogLine("Opening PK3 resource '" + location.location + "'");

			//TEST
			/*
			ZipInputStream z = new ZipInputStream(File.Open(dl.location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
			ZipEntry ze;
			while((ze = z.GetNextEntry()) != null)
			{
				General.WriteLogLine(ze.Name);
				
			}
			z.Dispose();
			*/
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				General.WriteLogLine("Closing PK3 resource '" + location.location + "'");

				// Clean up

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Palette

		// This loads the PLAYPAL palette
		public override Playpal LoadPalette()
		{
			// Error when suspended
			if(issuspended) throw new Exception("Data reader is suspended");

			// Not yet implemented
			return null;
		}

		#endregion

		#region ================== Textures

		#endregion
	}
}
