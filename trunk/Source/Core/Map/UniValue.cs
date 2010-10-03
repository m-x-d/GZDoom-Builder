
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
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	public class UniValue
	{
		#region ================== Constants

		private const string NAME_CHARS = "abcdefghijklmnopqrstuvwxyz0123456789_";
		private const string START_CHARS = "abcdefghijklmnopqrstuvwxyz_";
		
		#endregion

		#region ================== Variables
		
		private object value;
		private int type;

		#endregion

		#region ================== Properties

		public object Value
		{
			get
			{
				return this.value;
			}
			
			set
			{
				// Value may only be a primitive type
				if((!(value is int) && !(value is float) && !(value is string) && !(value is bool)) || (value == null))
					throw new ArgumentException("Universal field values can only be of type int, float, string or bool.");
				
				this.value = value;
			}
		}
		
		public int Type { get { return this.type; } set { this.type = value; } }

		#endregion

		#region ================== Constructor

		// Constructor
		public UniValue(int type, object value)
		{
			this.type = type;
			this.value = value;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		public UniValue(UniversalType type, object value)
		{
			this.type = (int)type;
			this.value = value;

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		public UniValue(UniValue v)
		{
			this.type = v.type;
			this.value = v.value;

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor
		public UniValue()
		{
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		#endregion

		#region ================== Methods

		// Serialize / deserialize
		internal void ReadWrite(IReadWriteStream s)
		{
			s.rwInt(ref type);
			switch((UniversalType)type)
			{
				case UniversalType.AngleRadians:
				case UniversalType.AngleDegreesFloat:
				case UniversalType.Float:
				{
					float v = 0.0f;
					try { v = (float)value; } catch(NullReferenceException e) { }
					s.rwFloat(ref v);
					value = v;
					break;
				}
				
				case UniversalType.AngleDegrees:
				case UniversalType.Color:
				case UniversalType.EnumBits:
				case UniversalType.EnumOption:
				case UniversalType.Integer:
				case UniversalType.LinedefTag:
				case UniversalType.LinedefType:
				case UniversalType.SectorEffect:
				case UniversalType.SectorTag:
				case UniversalType.ThingTag:
				{
					int v = 0;
					try { v = (int)value; } catch(NullReferenceException e) { }
					s.rwInt(ref v);
					value = v;
					break;
				}

				case UniversalType.Boolean:
				{
					bool v = false;
					try { v = (bool)value; } catch(NullReferenceException e) { }
					s.rwBool(ref v);
					value = v;
					break;
				}
				
				case UniversalType.Flat:
				case UniversalType.String:
				case UniversalType.Texture:
				case UniversalType.EnumStrings:
				{
					string v = (string)value;
					s.rwString(ref v);
					value = v;
					break;
				}
				
				default:
					General.Fail("Unknown field type to read/write!");
					break;
			}
		}
		
		// This validates a UDMF field name and returns the valid part
		public static string ValidateName(string name)
		{
			// Keep only valid characters
			string fieldname = name.Trim().ToLowerInvariant();
			string validname = "";
			for(int c = 0; c < fieldname.Length; c++)
			{
				string valid_chars = (validname.Length > 0) ? NAME_CHARS : START_CHARS;
				if(valid_chars.IndexOf(fieldname[c]) > -1) validname += fieldname[c];
			}
			return validname;
		}

		#endregion
	}
}
