
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

using System.Collections.Generic;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	// Vertex
	public class VertexProperties
	{
		public static bool ZCeiling = true; //mxd
		public static bool ZFloor = true; //mxd
		public static bool Universal_Fields = true; //mxd
		
		private readonly UniFields fields;
		private readonly float zceiling; //mxd
		private readonly float zfloor; //mxd

		public VertexProperties(Vertex v)
		{
			fields = new UniFields(v.Fields);
			zceiling = v.ZCeiling; //mxd
			zfloor = v.ZFloor; //mxd
		}

		public void Apply(Vertex v)
		{
			if (ZCeiling) v.ZCeiling = zceiling; //mxd
			if (ZFloor) v.ZFloor = zfloor; //mxd
			if (Universal_Fields) {
				v.Fields.BeforeFieldsChange();
				v.Fields.Clear();
				foreach (KeyValuePair<string, UniValue> uv in fields)
					v.Fields.Add(uv.Key, new UniValue(uv.Value));
			}
		}
	}

	// Sector
	public class SectorProperties
	{
		public static bool Floor_Height = true; //mxd
		public static bool Ceiling_Height = true; //mxd
		public static bool Floor_Texture = true; //mxd
		public static bool Ceiling_Texture = true; //mxd
		public static bool Brightness = true; //mxd
		public static bool Tag = true; //mxd
		public static bool Special = true; //mxd
		public static bool Flags = true; //mxd
		public static bool Universal_Fields = true; //mxd
		
		private readonly int floorheight;
		private readonly int ceilheight;
		private readonly string floortexture;
		private readonly string ceilingtexture;
		private readonly int effect;
		private readonly int brightness;
		private readonly int tag;
		private readonly UniFields fields;
		private readonly Dictionary<string, bool> flags; //mxd
		
		public SectorProperties(Sector s)
		{
			floorheight = s.FloorHeight;
			ceilheight = s.CeilHeight;
			floortexture = s.FloorTexture;
			ceilingtexture = s.CeilTexture;
			brightness = s.Brightness;
			effect = s.Effect;
			tag = s.Tag;
			fields = new UniFields(s.Fields);
			flags = s.GetFlags(); //mxd
		}
		
		public void Apply(Sector s)
		{
			if (Floor_Height) s.FloorHeight = floorheight;
			if (Ceiling_Height) s.CeilHeight = ceilheight;
			if (Floor_Texture) s.SetFloorTexture(floortexture);
			if (Ceiling_Texture) s.SetCeilTexture(ceilingtexture);
			if (Brightness) s.Brightness = brightness;
			if (Tag) s.Tag = tag;
			if (Special) s.Effect = effect;
			if (Flags) {
				s.ClearFlags(); //mxd
				foreach (KeyValuePair<string, bool> f in flags) //mxd
					s.SetFlag(f.Key, f.Value);
			}
			if (Universal_Fields) {
				s.Fields.BeforeFieldsChange();
				s.Fields.Clear();
				foreach (KeyValuePair<string, UniValue> v in fields)
					s.Fields.Add(v.Key, new UniValue(v.Value));
			}
		}
	}

	// Sidedef
	public class SidedefProperties
	{
		public static bool Upper_Texture = true; //mxd
		public static bool Middle_Texture = true; //mxd
		public static bool Lower_Texture = true; //mxd
		public static bool OffsetX = true; //mxd
		public static bool OffsetY = true; //mxd
		public static bool Flags = true; //mxd
		public static bool Universal_Fields = true; //mxd
		
		private readonly string hightexture;
		private readonly string middletexture;
		private readonly string lowtexture;
		private readonly int offsetx;
		private readonly int offsety;
		private readonly UniFields fields;
		private readonly Dictionary<string, bool> flags; //mxd

		public SidedefProperties(Sidedef s)
		{
			hightexture = s.HighTexture;
			middletexture = s.MiddleTexture;
			lowtexture = s.LowTexture;
			offsetx = s.OffsetX;
			offsety = s.OffsetY;
			fields = new UniFields(s.Fields);
			flags = s.GetFlags(); //mxd
		}
		
		public void Apply(Sidedef s)
		{
			if (Upper_Texture) s.SetTextureHigh(hightexture);
			if (Middle_Texture) s.SetTextureMid(middletexture);
			if (Lower_Texture) s.SetTextureLow(lowtexture);
			if (OffsetX) s.OffsetX = offsetx;
			if (OffsetY) s.OffsetY = offsety;
			if (Flags) {
				s.ClearFlags(); //mxd
				foreach (KeyValuePair<string, bool> f in flags) //mxd
					s.SetFlag(f.Key, f.Value);
			}
			if (Universal_Fields) {
				s.Fields.BeforeFieldsChange();
				s.Fields.Clear();
				foreach (KeyValuePair<string, UniValue> v in fields)
					s.Fields.Add(v.Key, new UniValue(v.Value));
			}
		}
	}

	// Linedef
	public class LinedefProperties
	{
		public static bool Sidedef_Properties = true; //mxd
		public static bool Action = true; //mxd
		public static bool Activation = true; //mxd
		public static bool Tag = true; //mxd
		public static bool Flags = true; //mxd
		public static bool Universal_Fields = true; //mxd
		
		private readonly SidedefProperties front;
		private readonly SidedefProperties back;
		private readonly Dictionary<string, bool> flags;
		private readonly int action;
		private readonly int activate;
		private readonly int tag;
		private readonly int[] args;
		private readonly UniFields fields;

		public LinedefProperties(Linedef l)
		{
			front = (l.Front != null ? new SidedefProperties(l.Front) : null);
			back = (l.Back != null ? new SidedefProperties(l.Back) : null);

			flags = l.GetFlags();
			action = l.Action;
			activate = l.Activate;
			tag = l.Tag;
			args = (int[])(l.Args.Clone());
			fields = new UniFields(l.Fields);
		}
		
		public void Apply(Linedef l)
		{
			if (Sidedef_Properties) {
				if ((front != null) && (l.Front != null)) front.Apply(l.Front);
				if ((back != null) && (l.Back != null)) back.Apply(l.Back);
			}
			if (Flags) {
				l.ClearFlags();
				foreach (KeyValuePair<string, bool> f in flags)
					l.SetFlag(f.Key, f.Value);
			}
			if (Activation) l.Activate = activate;
			if (Tag)l.Tag = tag;
			if (Action) {
				l.Action = action;
				for (int i = 0; i < l.Args.Length; i++)
					l.Args[i] = args[i];
			}
			if (Universal_Fields) {
				l.Fields.BeforeFieldsChange();
				l.Fields.Clear();
				foreach (KeyValuePair<string, UniValue> v in fields)
					l.Fields.Add(v.Key, new UniValue(v.Value));
			}
		}
	}
	
	// Thing
	public class ThingProperties
	{
		public static bool Type = true; //mxd
		public static bool Angle = true; //mxd
		public static bool Action = true; //mxd
		public static bool Tag = true; //mxd
		public static bool Flags = true; //mxd
		public static bool Universal_Fields = true; //mxd
		
		private readonly int type;
		private readonly float angle;
		private readonly Dictionary<string, bool> flags;
		private readonly int tag;
		private readonly int action;
		private readonly int[] args;
		private readonly UniFields fields;
		
		public ThingProperties(Thing t)
		{
			type = t.Type;
			angle = t.Angle;
			flags = t.GetFlags();
			tag = t.Tag;
			action = t.Action;       
			args = (int[])(t.Args.Clone());
			fields = new UniFields(t.Fields);
		}
		
		public void Apply(Thing t)
		{
			if (Type) t.Type = type;
			if (Angle) t.Rotate(angle);
			if (Flags) {
				t.ClearFlags();
				foreach (KeyValuePair<string, bool> f in flags)
					t.SetFlag(f.Key, f.Value);
			}
			if (Tag) t.Tag = tag;
			if (Action) {
				t.Action = action;
				for (int i = 0; i < t.Args.Length; i++)
					t.Args[i] = args[i];
			}
			if (Universal_Fields) {
				t.Fields.BeforeFieldsChange();
				t.Fields.Clear();
				foreach (KeyValuePair<string, UniValue> v in fields)
					t.Fields.Add(v.Key, new UniValue(v.Value));
			}
		}
	}
}
