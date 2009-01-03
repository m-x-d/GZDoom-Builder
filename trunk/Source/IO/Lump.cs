
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

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal class Lump
	{
		#region ================== Methods

		// Allowed characters in a map lump name
		public const string MAP_LUMP_NAME_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_";

		#endregion

		#region ================== Variables

		// Owner
		private WAD owner;
		
		// Data stream
		private ClippedStream stream;
		
		// Data info
		private string name;
		private long longname;
		private byte[] fixedname;
		private int offset;
		private int length;

		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public WAD Owner { get { return owner; } }
		public string Name { get { return name; } }
		public long LongName { get { return longname; } }
		public byte[] FixedName { get { return fixedname; } }
		public int Offset { get { return offset; } }
		public int Length { get { return length; } }
		public ClippedStream Stream { get { return stream; } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Lump(Stream data, WAD owner, byte[] fixedname, int offset, int length)
		{
			// Initialize
			this.stream = new ClippedStream(data, offset, length);
			this.owner = owner;
			this.fixedname = fixedname;
			this.offset = offset;
			this.length = length;

			// Make name
			this.name = MakeNormalName(fixedname, WAD.ENCODING).ToUpperInvariant();
			this.fixedname = MakeFixedName(name, WAD.ENCODING);
			this.longname = MakeLongName(name);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				stream.Dispose();
				owner = null;

				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// This returns the long value for a 8 byte texture name
		public static unsafe long MakeLongName(string name)
		{
			long value = 0;
			byte[] namebytes = Encoding.ASCII.GetBytes(name.Trim().ToUpper());
			uint bytes = (uint)namebytes.Length;
			if(bytes > 8) bytes = 8;

			fixed(void* bp = namebytes)
			{
				General.CopyMemory(&value, bp, bytes);
			}

			return value;
		}
		
		// This makes the normal name from fixed name
		public static string MakeNormalName(byte[] fixedname, Encoding encoding)
		{
			int length = 0;
			
			// Figure out the length of the lump name
			while((length < fixedname.Length) && (fixedname[length] != 0)) length++;
			
			// Make normal name
			return encoding.GetString(fixedname, 0, length).Trim().ToUpper();
		}

		// This makes the fixed name from normal name
		public static byte[] MakeFixedName(string name, Encoding encoding)
		{
			// Make 8 bytes, all zeros
			byte[] fixedname = new byte[8];

			// Write the name in bytes
			encoding.GetBytes(name.Trim().ToUpper(), 0, name.Length, fixedname, 0);

			// Return result
			return fixedname;
		}

		// This copies lump data to another lump
		public void CopyTo(Lump lump)
		{
			BinaryReader reader;

			// Create a reader
			reader = new BinaryReader(stream);

			// Copy bytes over
			stream.Seek(0, SeekOrigin.Begin);
			lump.Stream.Write(reader.ReadBytes((int)stream.Length), 0, (int)stream.Length);
		}
		
		// String representation
		public override string ToString()
		{
			return name;
		}
		
		// This renames the lump
		public void Rename(string newname)
		{
			// Make name
			this.fixedname = MakeFixedName(newname, WAD.ENCODING);
			this.name = MakeNormalName(this.fixedname, WAD.ENCODING).ToUpperInvariant();
			this.longname = MakeLongName(newname);

			// Write changes
			owner.WriteHeaders();
		}
		
		#endregion
	}
}
