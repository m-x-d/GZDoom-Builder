
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
using System.Runtime.InteropServices;
using System.Diagnostics;

#endregion

namespace CodeImp.DoomBuilder
{
	public class CRC
	{
		#region ================== Constants

        private static readonly uint[] Table;

		#endregion

		#region ================== Variables

		private uint _value = 0xFFFFFFFF;

		#endregion

		#region ================== Properties

		public long Value { get { return _value ^ 0xFFFFFFFF; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public CRC()
		{
			// We have no destructor
			GC.SuppressFinalize(this);
		}

        static CRC()
        {
            Table = new uint[256];
            const uint kPoly = 0xEDB88320;
            for (uint i = 0; i < 256; i++)
            {
                uint r = i;
                for (int j = 0; j < 8; j++)
                    if ((r & 1) != 0)
                        r = (r >> 1) ^ kPoly;
                    else
                        r >>= 1;
                Table[i] = r;
            }
        }

		#endregion

		#region ================== Methods

		public void Add(long value)
		{
			uint lo = (uint)((ulong)value & 0x00000000FFFFFFFF);
			uint hi = (uint)(((ulong)value & 0xFFFFFFFF00000000) >> 32);
			Add(unchecked((int)lo));
			Add(unchecked((int)hi));
		}

		public void Add(int value)
		{
            _value = Table[(((byte)(_value)) ^ (value & 0xFF))] ^ (_value >> 8);
            _value = Table[(((byte)(_value)) ^ ((value >> 8) & 0xFF))] ^ (_value >> 8);
            _value = Table[(((byte)(_value)) ^ ((value >> 16) & 0xFF))] ^ (_value >> 8);
            _value = Table[(((byte)(_value)) ^ ((value >> 24) & 0xFF))] ^ (_value >> 8);
		}

		public void Add(byte[] data)
		{
            for (uint i = 0; i < data.Length; i++)
                _value = Table[(((byte)(_value)) ^ data[i])] ^ (_value >> 8);
        }

		public void Reset()
		{
            _value = 0xFFFFFFFF;
		}

        #endregion
    }
}
