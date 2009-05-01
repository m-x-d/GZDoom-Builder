
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
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using System.Diagnostics;
using CodeImp.DoomBuilder.Actions;
using ICSharpCode.SharpZipLib.BZip2;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	public class UndoSnapshot
	{
		#region ================== Variables

		private MemoryStream mapdata;
		private string filename;
		private string description;
		private int ticketid;			// For safe withdrawing
		private volatile bool storeondisk;
		private volatile bool isondisk;
		private bool isdisposed;
		
		#endregion

		#region ================== Properties

		public string Description { get { return description; } set { description = value; } }
		public int TicketID { get { return ticketid; } }
		internal bool StoreOnDisk { get { return storeondisk; } set { storeondisk = value; } }
		public bool IsOnDisk { get { return isondisk; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal UndoSnapshot(string description, MemoryStream mapdata, int ticketid)
		{
			if(mapdata == null) General.Fail("Argument cannot be null!");
			this.ticketid = ticketid;
			this.description = description;
			this.mapdata = mapdata;
			this.filename = null;
		}

		// Constructor
		internal UndoSnapshot(UndoSnapshot info, MemoryStream mapdata)
		{
			if(mapdata == null) General.Fail("Argument cannot be null!");
			this.ticketid = info.ticketid;
			this.description = info.description;
			this.mapdata = mapdata;
			this.filename = null;
		}

		// Disposer
		internal void Dispose()
		{
			lock(this)
			{
				isdisposed = true;
				if(mapdata != null) mapdata.Dispose();
				mapdata = null;
				if(isondisk) File.Delete(filename);
				isondisk = false;
			}
		}

		#endregion
		
		#region ================== Methods

		// This returns the map data
		internal MemoryStream GetMapData()
		{
			lock(this)
			{
				// Restore into memory if needed
				if(isondisk) RestoreFromFile();
				
				// Return a copy of the buffer
				return new MemoryStream(mapdata.ToArray());
			}
		}
		
		// This moves the snapshot from memory to harddisk
		internal void WriteToFile()
		{
			lock(this)
			{
				if(isdisposed) return;
				if(isondisk) return;
				isondisk = true;
				
				// Compress data
				mapdata.Seek(0, SeekOrigin.Begin);
				MemoryStream outstream = new MemoryStream((int)mapdata.Length);
				BZip2.Compress(mapdata, outstream, 300000);

				// Make temporary file
				filename = General.MakeTempFilename(General.Map.TempPath, "snapshot");

				// Write data to file
				File.WriteAllBytes(filename, outstream.ToArray());

				// Remove data from memory
				mapdata.Dispose();
				mapdata = null;
				outstream.Dispose();
			}
		}

		// This loads the snapshot from harddisk into memory
		internal void RestoreFromFile()
		{
			lock(this)
			{
				if(isdisposed) return;
				if(!isondisk) return;
				isondisk = false;

				// Read the file data
				MemoryStream instream = new MemoryStream(File.ReadAllBytes(filename));
				
				// Decompress data
				MemoryStream outstream = new MemoryStream((int)instream.Length * 4);
				instream.Seek(0, SeekOrigin.Begin);
				BZip2.Decompress(instream, outstream);
				mapdata = new MemoryStream(outstream.ToArray());
				
				// Clean up
				instream.Dispose();
				File.Delete(filename);
				filename = null;
			}
		}
		
		#endregion
	}
}
