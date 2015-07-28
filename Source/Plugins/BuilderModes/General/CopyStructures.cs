
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
using System.Linq;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.GZBuilder.Tools;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	//mxd
	public class FieldDescription : Attribute
	{
		public string Description { get; private set; }

		public FieldDescription(string description) 
		{
			this.Description = description;
		}
	}
	
	//mxd
	public class VertexPropertiesCopySettings
	{
		[FieldDescription("Vertex Floor Height")]
		public bool ZFloor = true;
		[FieldDescription("Vertex Ceiling Height")]
		public bool ZCeiling = true;
		[FieldDescription("Custom Fields")]
		public bool Fields = true;
	}
	
	// Vertex
	public class VertexProperties
	{
		public static VertexPropertiesCopySettings CopySettings = new VertexPropertiesCopySettings();
		
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
			if(CopySettings.ZCeiling) v.ZCeiling = zceiling; //mxd
			if(CopySettings.ZFloor) v.ZFloor = zfloor; //mxd
			if(CopySettings.Fields) 
			{
				v.Fields.BeforeFieldsChange();
				v.Fields.Clear();
				foreach (KeyValuePair<string, UniValue> uv in fields)
					v.Fields.Add(uv.Key, new UniValue(uv.Value));
			}
		}
	}

	//mxd
	public class SectorPropertiesCopySettings
	{
		[FieldDescription("Floor Height")]
		public bool FloorHeight = true;
		[FieldDescription("Ceiling Height")]
		public bool CeilingHeight = true;
		[FieldDescription("Floor Texture")]
		public bool FloorTexture = true;
		[FieldDescription("Ceiling Texture")]
		public bool CeilingTexture = true;
		[FieldDescription("Brightness")]
		public bool Brightness = true;
		[FieldDescription("Ceiling Slope")]
		public bool CeilingSlope = true;
		[FieldDescription("Floor Slope")]
		public bool FloorSlope = true;
		[FieldDescription("Tags")]
		public bool Tag = true;
		[FieldDescription("Effect")]
		public bool Special = true;
		[FieldDescription("Flags")]
		public bool Flags = true;
		[FieldDescription("Custom Fields")]
		public bool Fields = true;
	}

	// Sector
	public class SectorProperties
	{
		//mxd
		public static SectorPropertiesCopySettings CopySettings = new SectorPropertiesCopySettings();
		
		private readonly int floorheight;
		private readonly int ceilheight;
		private readonly string floortexture;
		private readonly string ceilingtexture;
		private readonly int effect;
		private readonly int brightness;
		private readonly float ceilslopeoffset;
		private readonly float floorslopeoffset;
		private readonly Vector3D ceilslope;
		private readonly Vector3D floorslope;
		private readonly List<int> tags;
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
			ceilslopeoffset = s.CeilSlopeOffset;
			floorslopeoffset = s.FloorSlopeOffset;
			ceilslope = s.CeilSlope;
			floorslope = s.FloorSlope;
			tags = new List<int>(s.Tags); //mxd
			fields = new UniFields(s.Fields);
			flags = s.GetFlags(); //mxd
		}
		
		public void Apply(Sector s)
		{
			if(CopySettings.FloorHeight) s.FloorHeight = floorheight;
			if(CopySettings.CeilingHeight) s.CeilHeight = ceilheight;
			if(CopySettings.FloorTexture) s.SetFloorTexture(floortexture);
			if(CopySettings.CeilingTexture) s.SetCeilTexture(ceilingtexture);
			if(CopySettings.Brightness) s.Brightness = brightness;
			if(CopySettings.Tag) s.Tags = new List<int>(tags); //mxd
			if(CopySettings.Special) s.Effect = effect;
			if (CopySettings.CeilingSlope) 
			{
				s.CeilSlopeOffset = ceilslopeoffset;
				s.CeilSlope = ceilslope;
			}
			if(CopySettings.FloorSlope) 
			{
				s.FloorSlopeOffset = floorslopeoffset;
				s.FloorSlope = floorslope;
			}
			if(CopySettings.Flags) 
			{
				s.ClearFlags(); //mxd
				foreach (KeyValuePair<string, bool> f in flags) //mxd
					s.SetFlag(f.Key, f.Value);
			}
			if(CopySettings.Fields) 
			{
				s.Fields.BeforeFieldsChange();
				s.Fields.Clear();
				foreach (KeyValuePair<string, UniValue> v in fields)
					s.Fields.Add(v.Key, new UniValue(v.Value));
			}
		}
	}

	//mxd
	public class SidedefPropertiesCopySettings
	{
		[FieldDescription("Upper Texture")]
		public bool UpperTexture = true;
		[FieldDescription("Middle Texture")]
		public bool MiddleTexture = true;
		[FieldDescription("Lower Texture")]
		public bool LowerTexture = true;
		[FieldDescription("Offset X")]
		public bool OffsetX = true;
		[FieldDescription("Offset Y")]
		public bool OffsetY = true;
		[FieldDescription("Flags")]
		public bool Flags = true;
		[FieldDescription("Custom Fields")]
		public bool Fields = true;
	}

	// Sidedef
	public class SidedefProperties
	{
		//mxd
		public static SidedefPropertiesCopySettings CopySettings = new SidedefPropertiesCopySettings();

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
			if(CopySettings.UpperTexture) s.SetTextureHigh(hightexture);
			if(CopySettings.MiddleTexture) s.SetTextureMid(middletexture);
			if(CopySettings.LowerTexture) s.SetTextureLow(lowtexture);
			if(CopySettings.OffsetX) s.OffsetX = offsetx;
			if(CopySettings.OffsetY) s.OffsetY = offsety;
			if(CopySettings.Flags) 
			{
				s.ClearFlags(); //mxd
				foreach (KeyValuePair<string, bool> f in flags) //mxd
					s.SetFlag(f.Key, f.Value);
			}
			if(CopySettings.Fields) 
			{
				s.Fields.BeforeFieldsChange();
				s.Fields.Clear();
				foreach (KeyValuePair<string, UniValue> v in fields)
					s.Fields.Add(v.Key, new UniValue(v.Value));
			}
		}
	}

	//mxd
	public class LinedefPropertiesCopySettings
	{
		[FieldDescription("Sidedef Properties")]
		public bool SidedefProperties = true;
		[FieldDescription("Action")]
		public bool Action = true;
		[FieldDescription("Action Arguments")]
		public bool Arguments = true;
		[FieldDescription("Activation")]
		public bool Activation = true;
		[FieldDescription("Tags")]
		public bool Tag = true;
		[FieldDescription("Flags")]
		public bool Flags = true;
		[FieldDescription("Custom Fields")]
		public bool Fields = true;
	}

	// Linedef
	public class LinedefProperties
	{
		//mxd
		public static LinedefPropertiesCopySettings CopySettings = new LinedefPropertiesCopySettings();
		
		private readonly SidedefProperties front;
		private readonly SidedefProperties back;
		private readonly Dictionary<string, bool> flags;
		private readonly int action;
		private readonly int activate;
		private readonly List<int> tags;
		private readonly int[] args;
		private readonly UniFields fields;

		public LinedefProperties(Linedef l)
		{
			front = (l.Front != null ? new SidedefProperties(l.Front) : null);
			back = (l.Back != null ? new SidedefProperties(l.Back) : null);

			flags = l.GetFlags();
			action = l.Action;
			activate = l.Activate;
			tags = new List<int>(l.Tags); //mxd
			args = (int[])(l.Args.Clone());
			fields = new UniFields(l.Fields);
		}
		
		public void Apply(Linedef l)
		{
			if(CopySettings.SidedefProperties) 
			{
				if ((front != null) && (l.Front != null)) front.Apply(l.Front);
				if ((back != null) && (l.Back != null)) back.Apply(l.Back);
			}
			if(CopySettings.Flags) 
			{
				l.ClearFlags();
				foreach (KeyValuePair<string, bool> f in flags)
					l.SetFlag(f.Key, f.Value);
			}
			if(CopySettings.Activation) l.Activate = activate;
			if(CopySettings.Tag) l.Tags = new List<int>(tags); //mxd
			if(CopySettings.Action) l.Action = action;
			if(CopySettings.Arguments) 
			{
				for(int i = 0; i < l.Args.Length; i++)
					l.Args[i] = args[i];
			}
			if(CopySettings.Fields) 
			{
				l.Fields.BeforeFieldsChange();
				l.Fields.Clear();
				foreach (KeyValuePair<string, UniValue> v in fields)
					l.Fields.Add(v.Key, new UniValue(v.Value));
			}
		}
	}

	//mxd
	public class ThingPropertiesCopySettings
	{
		[FieldDescription("Type")]
		public bool Type = true;
		[FieldDescription("Angle")]
		public bool Angle = true;
		[FieldDescription("Pitch")]
		public bool Pitch = true;
		[FieldDescription("Roll")]
		public bool Roll = true;
		[FieldDescription("Scale")]
		public bool Scale = true;
		[FieldDescription("Action")]
		public bool Action = true;
		[FieldDescription("Action Arguments")]
		public bool Arguments = true;
		[FieldDescription("Tag")]
		public bool Tag = true;
		[FieldDescription("Flags")]
		public bool Flags = true;
		[FieldDescription("Custom Fields")]
		public bool Fields = true;
	}
	
	// Thing
	public class ThingProperties
	{
		//mxd
		public static ThingPropertiesCopySettings CopySettings = new ThingPropertiesCopySettings();
		
		private readonly int type;
		private readonly float angle;
		private readonly int pitch; //mxd
		private readonly int roll; //mxd
		private readonly float scalex; //mxd
		private readonly float scaley; //mxd
		private readonly Dictionary<string, bool> flags;
		private readonly int tag;
		private readonly int action;
		private readonly int[] args;
		private readonly UniFields fields;
		
		public ThingProperties(Thing t)
		{
			type = t.Type;
			angle = t.Angle;
			pitch = t.Pitch;
			roll = t.Roll;
			scalex = t.ScaleX;
			scaley = t.ScaleY;
			flags = t.GetFlags();
			tag = t.Tag;
			action = t.Action;
			args = (int[])(t.Args.Clone());
			fields = new UniFields(t.Fields);
		}
		
		public void Apply(Thing t)
		{
			if(CopySettings.Type) t.Type = type;
			if(CopySettings.Angle) t.Rotate(angle);
			if(CopySettings.Pitch) t.SetPitch(pitch);
			if(CopySettings.Roll) t.SetRoll(roll);
			if(CopySettings.Scale) t.SetScale(scalex, scaley);
			if(CopySettings.Flags) 
			{
				t.ClearFlags();
				foreach (KeyValuePair<string, bool> f in flags)
					t.SetFlag(f.Key, f.Value);
			}
			if(CopySettings.Tag) t.Tag = tag;
			if(CopySettings.Action) t.Action = action;
			if(CopySettings.Arguments) 
			{
				for(int i = 0; i < t.Args.Length; i++)
					t.Args[i] = args[i];
			}
			if(CopySettings.Fields) 
			{
				t.Fields.BeforeFieldsChange();
				t.Fields.Clear();
				foreach (KeyValuePair<string, UniValue> v in fields)
					t.Fields.Add(v.Key, new UniValue(v.Value));
			}
		}
	}

	//mxd. A class, which checks whether source and target map element's properties match
	public static class PropertiesComparer
	{

		#region Vertex

		public static bool PropertiesMatch(VertexPropertiesCopySettings flags, Vertex source, Vertex target) 
		{
			if(flags.ZCeiling && source.ZCeiling != target.ZCeiling) return false;
			if(flags.ZFloor && source.ZFloor != target.ZFloor) return false;
			return !flags.Fields || UDMFTools.FieldsMatch(source.Fields, target.Fields);
		}

		#endregion

		#region Sector

		public static bool PropertiesMatch(SectorPropertiesCopySettings flags, Sector source, Sector target) 
		{
			if(flags.FloorHeight && source.FloorHeight != target.FloorHeight) return false;
			if(flags.CeilingHeight && source.CeilHeight != target.CeilHeight) return false;
			if(flags.FloorTexture && source.FloorTexture != target.FloorTexture) return false;
			if(flags.CeilingTexture && source.CeilTexture != target.CeilTexture) return false;
			if(flags.Brightness && source.Brightness != target.Brightness) return false;
			if(flags.Tag && TagsMatch(source.Tags, target.Tags)) return false; //mxd
			if(flags.Special && source.Effect != target.Effect) return false;
			if(flags.Flags && !FlagsMatch(source.GetFlags(), target.GetFlags())) return false;
			return !flags.Fields || UDMFTools.FieldsMatch(source.Fields, target.Fields);
		}

		#endregion

		#region Linedef

		public static bool PropertiesMatch(LinedefPropertiesCopySettings linedefflags, SidedefPropertiesCopySettings sideflags, Linedef source, Linedef target) 
		{
			if(linedefflags.Action && source.Action != target.Action) return false;
			if(linedefflags.Activation && source.Activate != target.Activate) return false;
			if(linedefflags.Tag && TagsMatch(source.Tags, target.Tags)) return false; //mxd
			if(linedefflags.Arguments) 
			{
				for(int i = 0; i < source.Args.Length; i++)
					if(source.Args[i] != target.Args[i]) return false;
			}
			if(linedefflags.Flags && !FlagsMatch(source.GetFlags(), target.GetFlags())) return false;
			if(linedefflags.SidedefProperties) 
			{
				if ((source.Front == null && target.Front != null) || (source.Front != null && target.Front == null) ||
					(source.Back == null && target.Back != null) || (source.Back != null && target.Back == null)) 
					return false;

				if(source.Front != null && !PropertiesMatch(sideflags, source.Front, target.Front)) return false;
				if(source.Back != null && !PropertiesMatch(sideflags, source.Back, target.Back)) return false;
			}
			return !linedefflags.Fields || UDMFTools.FieldsMatch(source.Fields, target.Fields);
		}

		#endregion

		#region Sidedef

		public static bool PropertiesMatch(SidedefPropertiesCopySettings flags, Sidedef source, Sidedef target) 
		{
			if(flags.OffsetX && source.OffsetX != target.OffsetX) return false;
			if(flags.OffsetY && source.OffsetY != target.OffsetY) return false;
			if(flags.UpperTexture && source.HighTexture != target.HighTexture) return false;
			if(flags.MiddleTexture && source.MiddleTexture != target.MiddleTexture) return false;
			if(flags.LowerTexture && source.LowTexture != target.LowTexture) return false;
			if(flags.Flags && !FlagsMatch(source.GetFlags(), target.GetFlags())) return false;
			return !flags.Fields || UDMFTools.FieldsMatch(source.Fields, target.Fields);
		}

		#endregion

		#region Thing

		public static bool PropertiesMatch(ThingPropertiesCopySettings flags, Thing source, Thing target) 
		{
			if(flags.Type && source.Type != target.Type) return false;
			if(flags.Angle && source.AngleDoom != target.AngleDoom) return false;
			if(flags.Action && source.Action != target.Action) return false;
			if (flags.Arguments) 
			{
				for(int i = 0; i < source.Args.Length; i++)
					if (source.Args[i] != target.Args[i]) return false;
			}
			if(flags.Tag && source.Tag != target.Tag) return false;
			if(flags.Pitch && source.Pitch != target.Pitch) return false;
			if(flags.Roll && source.Roll != target.Roll) return false;
			if(flags.Scale && (source.ScaleX != target.ScaleX) || (source.ScaleY != target.ScaleY))
				return false;
			if(flags.Flags && !FlagsMatch(source.GetFlags(), target.GetFlags())) return false;
			return !flags.Fields || UDMFTools.FieldsMatch(source.Fields, target.Fields);
		}

		#endregion

		private static bool FlagsMatch(Dictionary<string, bool> flags1, Dictionary<string, bool> flags2) 
		{
			if (flags1.Count != flags2.Count) return false;
			foreach (KeyValuePair<string, bool> group in flags1)
				if (!flags2.ContainsKey(group.Key) || flags2[group.Key] != flags1[group.Key]) return false;
			return true;
		}

		//mxd
		private static bool TagsMatch(List<int> tags1, List<int> tags2)
		{
			if(tags1.Count != tags2.Count) return false;
			Dictionary<int, int> count = new Dictionary<int, int>();

			foreach(int s in tags1)
			{
				if(count.ContainsKey(s)) count[s]++;
				else count.Add(s, 1);
			}

			foreach(int s in tags2)
			{
				if(count.ContainsKey(s)) count[s]--;
				else return false;
			}

			return count.Values.All(c => c == 0);
		}
	}
}
