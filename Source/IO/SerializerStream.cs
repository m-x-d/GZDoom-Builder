
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
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using SlimDX.Direct3D9;
using System.Drawing;
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal sealed class SerializerStream : IReadWriteStream
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private Stream stream;
		private BinaryWriter writer;
		
		#endregion

		#region ================== Properties

		public bool IsWriting { get { return true; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public SerializerStream(Stream stream)
		{
			// Initialize
			this.stream = stream;
			this.writer = new BinaryWriter(stream);
		}

		#endregion

		#region ================== Methods

		// Bidirectional
		public void rwInt(ref int v) { writer.Write(v); }

		public void rwByte(ref byte v) { writer.Write(v); }

		public void rwShort(ref short v) { writer.Write(v); }

		public void rwString(ref string v) { writer.Write(v); }

		public void rwLong(ref long v) { writer.Write(v); }

		public void rwUInt(ref uint v) { writer.Write(v); }

		public void rwUShort(ref ushort v) { writer.Write(v); }

		public void rwULong(ref ulong v) { writer.Write(v); }

		public void rwFloat(ref float v) { writer.Write(v); }

		public void rwBool(ref bool v) { writer.Write(v); }

		public void rwVector2D(ref Vector2D v)
		{
			writer.Write(v.x);
			writer.Write(v.y);
		}

		public void rwVector3D(ref Vector3D v)
		{
			writer.Write(v.x);
			writer.Write(v.y);
			writer.Write(v.z);
		}

		// Write-only
		public void wInt(int v) { writer.Write(v); }

		public void wByte(byte v) { writer.Write(v); }

		public void wShort(short v) { writer.Write(v); }

		public void wString(string v) { writer.Write(v); }

		public void wLong(long v) { writer.Write(v); }

		public void wUInt(uint v) { writer.Write(v); }

		public void wUShort(ushort v) { writer.Write(v); }

		public void wULong(ulong v) { writer.Write(v); }

		public void wFloat(float v) { writer.Write(v); }

		public void wBool(bool v) { writer.Write(v); }

		public void wVector2D(Vector2D v)
		{
			writer.Write(v.x);
			writer.Write(v.y);
		}

		public void wVector3D(Vector3D v)
		{
			writer.Write(v.x);
			writer.Write(v.y);
			writer.Write(v.z);
		}

		// Read-only is not supported
		public void rInt(out int v) { v = 0; General.Fail("Read-only is not supported on serialization stream. Consider passing the element by reference for bidirectional support."); }

		public void rByte(out byte v) { v = 0; General.Fail("Read-only is not supported on serialization stream. Consider passing the element by reference for bidirectional support."); }

		public void rShort(out short v) { v = 0; General.Fail("Read-only is not supported on serialization stream. Consider passing the element by reference for bidirectional support."); }

		public void rString(out string v) { v = ""; General.Fail("Read-only is not supported on serialization stream. Consider passing the element by reference for bidirectional support."); }

		public void rLong(out long v) { v = 0; General.Fail("Read-only is not supported on serialization stream. Consider passing the element by reference for bidirectional support."); }

		public void rUInt(out uint v) { v = 0; General.Fail("Read-only is not supported on serialization stream. Consider passing the element by reference for bidirectional support."); }

		public void rUShort(out ushort v) { v = 0; General.Fail("Read-only is not supported on serialization stream. Consider passing the element by reference for bidirectional support."); }

		public void rULong(out ulong v) { v = 0; General.Fail("Read-only is not supported on serialization stream. Consider passing the element by reference for bidirectional support."); }

		public void rFloat(out float v) { v = 0; General.Fail("Read-only is not supported on serialization stream. Consider passing the element by reference for bidirectional support."); }

		public void rBool(out bool v) { v = false; General.Fail("Read-only is not supported on serialization stream. Consider passing the element by reference for bidirectional support."); }

		public void rVector2D(out Vector2D v)
		{
			v = new Vector2D();
			General.Fail("Read-only is not supported on serialization stream. Consider passing the element by reference for bidirectional support.");
		}

		public void rVector3D(out Vector3D v)
		{
			v = new Vector3D();
			General.Fail("Read-only is not supported on serialization stream. Consider passing the element by reference for bidirectional support.");
		}
		
		#endregion
	}
}
