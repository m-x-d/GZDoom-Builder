
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
using System.Collections.Generic;
using System.Text;
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal class WAD
	{
		#region ================== Constants

		// WAD types
		public const string TYPE_IWAD = "IWAD";
		public const string TYPE_PWAD = "PWAD";
		
		// Encoder
		public static readonly Encoding ENCODING = Encoding.ASCII;
		
		#endregion

		#region ================== Variables

		// File objects
		private string filename;
		private FileStream file;
		private BinaryReader reader;
		private BinaryWriter writer;
		
		// Header
		private string type;
		private int numlumps;
		private int lumpsoffset;
		
		// Lumps
		private List<Lump> lumps;
		
		// Status
		private readonly bool isreadonly;
		private bool isdisposed;

		#endregion

		#region ================== Properties

		public string Filename { get { return filename; } }
		public string Type { get { return type; } }
		public Encoding Encoding { get { return ENCODING; } }
		public bool IsReadOnly { get { return isreadonly; } }
		public bool IsDisposed { get { return isdisposed; } }
		public List<Lump> Lumps { get { return lumps; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor to open or create a WAD file
		public WAD(string pathfilename)
		{
			// Initialize
			this.isreadonly = false;
			this.Open(pathfilename);
		}

		// Constructor to open or create a WAD file
		public WAD(string pathfilename, bool openreadonly)
		{
			// Initialize
			this.isreadonly = openreadonly;
			this.Open(pathfilename);
		}

		// Destructor
		/*~WAD()
		{
			// Make sure everything is disposed
			this.Dispose();
		}*/
		
		// Disposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Only possible when not read-only
				if(!isreadonly)
				{
					// Flush writing changes
					if(writer != null) writer.Flush();
					if(file != null) file.Flush();
				}
				
				// Clean up
				if(lumps != null) foreach(Lump l in lumps) l.Dispose();
				if(writer != null) writer.Close();
				if(reader != null) reader.Close();
				if(file != null) file.Dispose();
				
				// Done
				isdisposed = true;
				GC.SuppressFinalize(this); //mxd
			}
		}

		#endregion

		#region ================== IO

		// Open a WAD file
		private void Open(string pathfilename)
		{
			FileAccess access;
			FileShare share;
			
			// Determine if opening for read only
			if(isreadonly)
			{
				// Read only
				access = FileAccess.Read;
				share = FileShare.ReadWrite;
			}
			else
			{
				// Private access
				access = FileAccess.ReadWrite;
				share = FileShare.Read;
			}
			
			// Keep filename
			filename = pathfilename;
			
			// Open the file stream
			file = File.Open(pathfilename, FileMode.OpenOrCreate, access, share);

			// Create file handling tools
			reader = new BinaryReader(file, ENCODING);
			if(!isreadonly) writer = new BinaryWriter(file, ENCODING);

			// Is the WAD file zero length?
			if(file.Length == 0)
			{
				// Create the headers in file
				CreateHeaders();
			}
			else
			{
				// Read information from file
				ReadHeaders();
			}
		}

		// This creates new file headers
		private void CreateHeaders()
		{
			// Default settings
			type = TYPE_PWAD;
			lumpsoffset = 12;

			// New lumps array
			lumps = new List<Lump>(numlumps);
			
			// Write the headers
			WriteHeaders();
		}
		
		// This reads the WAD header and lumps table
		private void ReadHeaders()
		{
			int offset, length;
			byte[] fixedname;
			
			// Make sure the write is finished writing
			if(!isreadonly) writer.Flush();

			// Seek to beginning
			file.Seek(0, SeekOrigin.Begin);

			// Read WAD type
			type = ENCODING.GetString(reader.ReadBytes(4));
			
			// Number of lumps
			numlumps = reader.ReadInt32();
			if(numlumps < 0) throw new IOException("Invalid number of lumps in wad file.");

			// Lumps table offset
			lumpsoffset = reader.ReadInt32();
			if(lumpsoffset < 0) throw new IOException("Invalid lumps offset in wad file.");

			// Seek to the lumps table
			file.Seek(lumpsoffset, SeekOrigin.Begin);
			
			// Dispose old lumps and create new list
			if(lumps != null) foreach(Lump l in lumps) l.Dispose();
			lumps = new List<Lump>(numlumps);

			// Go for all lumps
			for(int i = 0; i < numlumps; i++)
			{
				// Read lump information
				offset = reader.ReadInt32();
				length = reader.ReadInt32();
				fixedname = reader.ReadBytes(8);

				// Create the lump
				lumps.Add(new Lump(file, this, fixedname, offset, length));
			}
		}

		// This reads the WAD header and lumps table
		public void WriteHeaders()
		{
			// Seek to beginning
			file.Seek(0, SeekOrigin.Begin);

			// Write WAD type
			writer.Write(ENCODING.GetBytes(type));

			// Number of lumps
			writer.Write(numlumps);

			// Lumps table offset
			writer.Write(lumpsoffset);

			// Seek to the lumps table
			file.Seek(lumpsoffset, SeekOrigin.Begin);

			// Go for all lumps
			for(int i = 0; i < lumps.Count; i++)
			{
				// Write lump information
				writer.Write(lumps[i].Offset);
				writer.Write(lumps[i].Length);
				writer.Write(lumps[i].FixedName);
			}
		}
		
		// This flushes writing changes
		/*public void Flush()
		{
			// Only possible when not read-only
			if(!isreadonly)
			{
				// Flush writing changes
				if(writer != null) writer.Flush();
				if(file != null) file.Flush();
			}
		}*/
		
		#endregion
		
		#region ================== Lumps

		// This creates a new lump in the WAD file
		public Lump Insert(string name, int position, int datalength)
		{
			// We will be adding a lump
			numlumps++;
			
			// Extend the file
			file.SetLength(file.Length + datalength + 16);
			
			// Create the lump
			Lump lump = new Lump(file, this, Lump.MakeFixedName(name, ENCODING), lumpsoffset, datalength);
			lumps.Insert(position, lump);
			
			// Advance lumps table offset
			lumpsoffset += datalength;

			// Write the new headers
			WriteHeaders();

			// Return the new lump
			return lump;
		}

		// This removes a lump from the WAD file by index
		public void RemoveAt(int index)
		{
			// Remove from list
			Lump l = lumps[index];
			lumps.RemoveAt(index);
			l.Dispose();
			numlumps--;
			
			// Write the new headers
			WriteHeaders();
		}
		
		// This removes a lump from the WAD file
		public void Remove(Lump lump)
		{
			// Remove from list
			lumps.Remove(lump);
			lump.Dispose();
			numlumps--;
			
			// Write the new headers
			WriteHeaders();
		}
		
		// This finds a lump by name, returns null when not found
		public Lump FindLump(string name)
		{
			int index = FindLumpIndex(name);
			return (index == -1 ? null : lumps[index]);
		}

		// This finds a lump by name, returns null when not found
		public Lump FindLump(string name, int start)
		{
			int index = FindLumpIndex(name, start);
			return (index == -1 ? null : lumps[index]);
		}

		// This finds a lump by name, returns null when not found
		public Lump FindLump(string name, int start, int end)
		{
			int index = FindLumpIndex(name, start, end);
			return (index == -1 ? null : lumps[index]);
		}

		// This finds a lump by name, returns -1 when not found
		public int FindLumpIndex(string name)
		{
			// Do search
			return FindLumpIndex(name, 0, lumps.Count - 1);
		}

		// This finds a lump by name, returns -1 when not found
		public int FindLumpIndex(string name, int start)
		{
			// Do search
			return FindLumpIndex(name, start, lumps.Count - 1);
		}
		
		// This finds a lump by name, returns -1 when not found
		public int FindLumpIndex(string name, int start, int end)
		{
			if(name.Length > 8)	return -1;//mxd. Can't be here. Go away!
			
			long longname = Lump.MakeLongName(name);
			
			// Fix end when it exceeds length
			if(end > (lumps.Count - 1)) end = lumps.Count - 1;

			// Loop through the lumps
			for(int i = start; i <= end; i++)
			{
				// Check if the lump name matches
				if(lumps[i].LongName == longname)
				{
					// Found the lump!
					return i;
				}
			}

			// Nothing found
			return -1;
		}

		#endregion
	}
}
