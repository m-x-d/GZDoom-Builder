
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
using System.Reflection;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	//mxd
	[AttributeUsage(AttributeTargets.Field)]
	public class FieldDescription : Attribute
	{
		private bool doom = true;
		private bool hexen = true;
		private bool udmf = true;
		private string description = "Unnamed field";

		public bool DOOM { get { return doom; } set { doom = value; } }
		public bool HEXEN { get { return hexen; } set { hexen = value; } }
		public bool UDMF { get { return udmf; } set { udmf = value; } }
		public string Description { get { return description; } set { description = value; } }

		public bool SupportsCurrentMapFormat { get { return General.Map != null && (General.Map.DOOM && doom || General.Map.HEXEN && hexen || General.Map.UDMF && udmf); } }
	}

	public abstract class MapElementPropertiesCopySettings
	{
		protected MapElementPropertiesCopySettings()
		{
			// Set all properties supported by currect map format to true
			FieldInfo[] props = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach(FieldInfo prop in props)
			{
				foreach(Attribute attr in Attribute.GetCustomAttributes(prop))
				{
					if(attr.GetType() == typeof(FieldDescription))
					{
						FieldDescription fd = (FieldDescription)attr;
						prop.SetValue(this, fd.SupportsCurrentMapFormat);
					}
				}
			}
		}
	}

	//mxd
	public abstract class MapElementProperties
	{
		private readonly UniFields fields;   // Only custom (e.g. not UI-managed) fields here
		private readonly UniFields uifields; // Only UI-managed fields here
		private readonly MapElementType elementtype;

		protected MapElementProperties(UniFields other, MapElementType type)
		{
			// Should we bother?
			if(!General.Map.UDMF) return;
			
			// Copy source fields except the ones controlled by the UI
			fields = new UniFields();
			uifields = new UniFields();
			elementtype = type;
			foreach(KeyValuePair<string, UniValue> group in other)
			{
				if(General.Map.FormatInterface.UIFields[elementtype].ContainsKey(group.Key))
					uifields.Add(group.Key, new UniValue(group.Value));
				else
					fields.Add(group.Key, new UniValue(group.Value));
			}
		}

		protected void Apply(UniFields other, string key)
		{
			if(uifields.ContainsKey(key)) other[key] = new UniValue(uifields[key]);
			else if(other.ContainsKey(key)) other.Remove(key);
		}

		protected void Apply(UniFields other, string key1, string key2)
		{
			Apply(other, key1);
			Apply(other, key2);
		}

		protected void ApplyCustomFields(UniFields other)
		{
			// Remove custom fields
			string[] keys = new string[other.Keys.Count];
			other.Keys.CopyTo(keys, 0);
			foreach(string key in keys)
			{
				if(!General.Map.FormatInterface.UIFields[elementtype].ContainsKey(key)) other.Remove(key);
			}

			// Replace with stored custom fields
			foreach(KeyValuePair<string, UniValue> group in fields)
			{
				other.Add(group.Key, new UniValue(group.Value));
			}
		}
	}

	#region ================== Vertex

	//mxd
	public class VertexPropertiesCopySettings : MapElementPropertiesCopySettings
	{
		[FieldDescription(Description = "Vertex Floor Height", DOOM = false, HEXEN = false)]
		public bool ZFloor = true;
		
		[FieldDescription(Description = "Vertex Ceiling Height", DOOM = false, HEXEN = false)]
		public bool ZCeiling = true;
		
		[FieldDescription(Description = "Custom Fields", DOOM = false, HEXEN = false)]
		public bool Fields = true;
	}
	
	// Vertex
	public class VertexProperties : MapElementProperties
	{
		private static VertexPropertiesCopySettings defaultsettings = new VertexPropertiesCopySettings();
		public static VertexPropertiesCopySettings CopySettings = new VertexPropertiesCopySettings();
		
		private readonly float zceiling; //mxd
		private readonly float zfloor; //mxd

		public VertexProperties(Vertex v) : base(v.Fields, MapElementType.VERTEX)
		{
			zceiling = v.ZCeiling; //mxd
			zfloor = v.ZFloor; //mxd
		}

		//mxd. Applies coped properties
		public void Apply(Vertex v, bool usecopysettings)
		{
			Apply(v, (usecopysettings ? CopySettings : defaultsettings));
		}

		//mxd. Applies coped properties using selected settings
		public void Apply(Vertex v, VertexPropertiesCopySettings settings)
		{
			if(settings.ZCeiling) v.ZCeiling = zceiling;
			if(settings.ZFloor) v.ZFloor = zfloor;
			if(settings.Fields)
			{
				v.Fields.BeforeFieldsChange();
				ApplyCustomFields(v.Fields);
			}
		}
	}

	#endregion

	#region ================== Sector

	//mxd
	public class SectorPropertiesCopySettings : MapElementPropertiesCopySettings
	{
		[FieldDescription(Description = "Floor Height")]
		public bool FloorHeight = true;
		
		[FieldDescription(Description = "Ceiling Height")]
		public bool CeilingHeight = true;
		
		[FieldDescription(Description = "Floor Texture")]
		public bool FloorTexture = true;
		
		[FieldDescription(Description = "Ceiling Texture")]
		public bool CeilingTexture = true;

		[FieldDescription(Description = "Floor Texture Offset", DOOM = false, HEXEN = false)]
		public bool FloorTextureOffset = true;

		[FieldDescription(Description = "Ceiling Texture Offset", DOOM = false, HEXEN = false)]
		public bool CeilingTextureOffset = true;

		[FieldDescription(Description = "Floor Texture Scale", DOOM = false, HEXEN = false)]
		public bool FloorTextureScale = true;

		[FieldDescription(Description = "Ceiling Texture Scale", DOOM = false, HEXEN = false)]
		public bool CeilingTextureScale = true;

		[FieldDescription(Description = "Floor Texture Rotation", DOOM = false, HEXEN = false)]
		public bool FloorTextureRotation = true;

		[FieldDescription(Description = "Ceiling Texture Rotation", DOOM = false, HEXEN = false)]
		public bool CeilingTextureRotation = true;

		[FieldDescription(Description = "Floor Alpha", DOOM = false, HEXEN = false)]
		public bool FloorAlpha = true;

		[FieldDescription(Description = "Ceiling Alpha", DOOM = false, HEXEN = false)]
		public bool CeilingAlpha = true;

		[FieldDescription(Description = "Sector Brightness")]
		public bool Brightness = true;

		[FieldDescription(Description = "Floor Brightness", DOOM = false, HEXEN = false)]
		public bool FloorBrightness = true;

		[FieldDescription(Description = "Ceiling Brightness", DOOM = false, HEXEN = false)]
		public bool CeilingBrightness = true;

		[FieldDescription(Description = "Floor Render Style", DOOM = false, HEXEN = false)]
		public bool FloorRenderStyle = true;

		[FieldDescription(Description = "Ceiling Render Style", DOOM = false, HEXEN = false)]
		public bool CeilingRenderStyle = true;
		
		[FieldDescription(Description = "Floor Slope", DOOM = false, HEXEN = false)]
		public bool FloorSlope = true;

		[FieldDescription(Description = "Ceiling Slope", DOOM = false, HEXEN = false)]
		public bool CeilingSlope = true;
		
		[FieldDescription(Description = "Tags")]
		public bool Tag = true;
		
		[FieldDescription(Description = "Effect")]
		public bool Special = true;
		
		[FieldDescription(Description = "Flags", DOOM = false, HEXEN = false)]
		public bool Flags = true;

		[FieldDescription(Description = "Light Color", DOOM = false, HEXEN = false)]
		public bool LightColor = true;

		[FieldDescription(Description = "Fade Color", DOOM = false, HEXEN = false)]
		public bool FadeColor = true;

		[FieldDescription(Description = "Desaturation", DOOM = false, HEXEN = false)]
		public bool Desaturation = true;

		[FieldDescription(Description = "Sound Sequence", DOOM = false, HEXEN = false)]
		public bool SoundSequence = true;

		[FieldDescription(Description = "Gravity", DOOM = false, HEXEN = false)]
		public bool Gravity = true;
		
		[FieldDescription(Description = "Custom Fields", DOOM = false, HEXEN = false)]
		public bool Fields = true;

		[FieldDescription(Description = "Comment", DOOM = false, HEXEN = false)]
		public bool Comment = true;
	}

	// Sector
	public class SectorProperties : MapElementProperties
	{
		//mxd
		private static SectorPropertiesCopySettings defaultsettings = new SectorPropertiesCopySettings();
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
		private readonly Dictionary<string, bool> flags; //mxd
		
		public SectorProperties(Sector s) : base(s.Fields, MapElementType.SECTOR)
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
			flags = s.GetFlags(); //mxd
		}
		
		//mxd. Applies coped properties
		public void Apply(Sector s, bool usecopysettings)
		{
			Apply(s, (usecopysettings ? CopySettings : defaultsettings));
		}

		//mxd. Applies coped properties using selected settings
		public void Apply(Sector s, SectorPropertiesCopySettings settings)
		{
			if(settings.FloorHeight) s.FloorHeight = floorheight;
			if(settings.CeilingHeight) s.CeilHeight = ceilheight;
			if(settings.FloorTexture) s.SetFloorTexture(floortexture);
			if(settings.CeilingTexture) s.SetCeilTexture(ceilingtexture);
			if(settings.Brightness) s.Brightness = brightness;
			if(settings.Tag) s.Tags = new List<int>(tags); //mxd
			if(settings.Special) s.Effect = effect;
			if(settings.CeilingSlope) 
			{
				s.CeilSlopeOffset = ceilslopeoffset;
				s.CeilSlope = ceilslope;
			}
			if(settings.FloorSlope) 
			{
				s.FloorSlopeOffset = floorslopeoffset;
				s.FloorSlope = floorslope;
			}
			if(settings.Flags) 
			{
				s.ClearFlags(); //mxd
				foreach(KeyValuePair<string, bool> f in flags) //mxd
					s.SetFlag(f.Key, f.Value);
			}

			// Should we bother?
			if(!General.Map.UDMF) return;

			// Apply fields
			s.Fields.BeforeFieldsChange();

			// Apply UI fields
			if(settings.FloorTextureOffset)     Apply(s.Fields, "xpanningfloor", "ypanningfloor");
			if(settings.CeilingTextureOffset)   Apply(s.Fields, "xpanningceiling", "ypanningceiling");
			if(settings.FloorTextureScale)      Apply(s.Fields, "xscalefloor", "yscalefloor");
			if(settings.CeilingTextureScale)    Apply(s.Fields, "xscaleceiling", "yscaleceiling");
			if(settings.FloorTextureRotation)   Apply(s.Fields, "rotationfloor");
			if(settings.CeilingTextureRotation) Apply(s.Fields, "rotationceiling");
			if(settings.FloorBrightness)        Apply(s.Fields, "lightfloor", "lightfloorabsolute");
			if(settings.CeilingBrightness)      Apply(s.Fields, "lightceiling", "lightceilingabsolute");
			if(settings.FloorAlpha)				Apply(s.Fields, "alphafloor");
			if(settings.CeilingAlpha)			Apply(s.Fields, "alphaceiling");
			if(settings.FloorRenderStyle)		Apply(s.Fields, "renderstylefloor");
			if(settings.CeilingRenderStyle)		Apply(s.Fields, "renderstyleceiling");
			if(settings.Gravity)				Apply(s.Fields, "gravity");
			if(settings.LightColor)				Apply(s.Fields, "lightcolor");
			if(settings.FadeColor)				Apply(s.Fields, "fadecolor");
			if(settings.Desaturation)			Apply(s.Fields, "desaturation");
			if(settings.SoundSequence)			Apply(s.Fields, "soundsequence");
			if(settings.Comment)				Apply(s.Fields, "comment");

			// Apply custom fields
			if(settings.Fields) ApplyCustomFields(s.Fields);
		}
	}

	#endregion

	#region ================== Sidedef

	//mxd
	public class SidedefPropertiesCopySettings : MapElementPropertiesCopySettings
	{
		[FieldDescription(Description = "Upper Texture")]
		public bool UpperTexture = true;
		
		[FieldDescription(Description = "Middle Texture")]
		public bool MiddleTexture = true;
		
		[FieldDescription(Description = "Lower Texture")]
		public bool LowerTexture = true;
		
		[FieldDescription(Description = "Texture Offset X")]
		public bool OffsetX = true;
		
		[FieldDescription(Description = "Texture Offset Y")]
		public bool OffsetY = true;

		[FieldDescription(Description = "Upper Texture Offset", DOOM = false, HEXEN = false)]
		public bool UpperTextureOffset = true;

		[FieldDescription(Description = "Middle Texture Offset", DOOM = false, HEXEN = false)]
		public bool MiddleTextureOffset = true;

		[FieldDescription(Description = "Lower Texture Offset", DOOM = false, HEXEN = false)]
		public bool LowerTextureOffset = true;

		[FieldDescription(Description = "Upper Texture Scale", DOOM = false, HEXEN = false)]
		public bool UpperTextureScale = true;

		[FieldDescription(Description = "Middle Texture Scale", DOOM = false, HEXEN = false)]
		public bool MiddleTextureScale = true;

		[FieldDescription(Description = "Lower Texture Scale", DOOM = false, HEXEN = false)]
		public bool LowerTextureScale = true;

		[FieldDescription(Description = "Brightness", DOOM = false, HEXEN = false)]
		public bool Brightness = true;
		
		[FieldDescription(Description = "Flags", DOOM = false, HEXEN = false)]
		public bool Flags = true;
		
		[FieldDescription(Description = "Custom Fields", DOOM = false, HEXEN = false)]
		public bool Fields = true;
	}

	// Sidedef
	public class SidedefProperties : MapElementProperties
	{
		//mxd
		internal static SidedefPropertiesCopySettings DefaultSettings = new SidedefPropertiesCopySettings();
		public static SidedefPropertiesCopySettings CopySettings = new SidedefPropertiesCopySettings();

		private readonly string hightexture;
		private readonly string middletexture;
		private readonly string lowtexture;
		private readonly int offsetx;
		private readonly int offsety;
		private readonly Dictionary<string, bool> flags; //mxd

		public SidedefProperties(Sidedef s) : base(s.Fields, MapElementType.SIDEDEF)
		{
			hightexture = s.HighTexture;
			middletexture = s.MiddleTexture;
			lowtexture = s.LowTexture;
			offsetx = s.OffsetX;
			offsety = s.OffsetY;
			flags = s.GetFlags(); //mxd
		}

		//mxd. Applies coped properties with all settings enabled
		public void Apply(Sidedef s, bool usecopysettings)
		{
			Apply(s, (usecopysettings ? CopySettings : DefaultSettings));
		}

		//mxd. Applies selected settings
		public void Apply(Sidedef s, SidedefPropertiesCopySettings settings)
		{
			if(settings.UpperTexture) s.SetTextureHigh(hightexture);
			if(settings.MiddleTexture) s.SetTextureMid(middletexture);
			if(settings.LowerTexture) s.SetTextureLow(lowtexture);
			if(settings.OffsetX) s.OffsetX = offsetx;
			if(settings.OffsetY) s.OffsetY = offsety;
			if(settings.Flags)
			{
				s.ClearFlags(); //mxd
				foreach(KeyValuePair<string, bool> f in flags) //mxd
					s.SetFlag(f.Key, f.Value);
			}

			// Should we bother?
			if(!General.Map.UDMF) return;

			// Apply fields
			s.Fields.BeforeFieldsChange();

			// Apply UI fields
			if(settings.UpperTextureOffset)  Apply(s.Fields, "offsetx_top", "offsety_top");
			if(settings.MiddleTextureOffset) Apply(s.Fields, "offsetx_mid", "offsety_mid");
			if(settings.LowerTextureOffset)  Apply(s.Fields, "offsetx_bottom", "offsety_bottom");
			if(settings.UpperTextureScale)   Apply(s.Fields, "scalex_top", "scaley_top");
			if(settings.MiddleTextureScale)  Apply(s.Fields, "scalex_mid", "scaley_mid");
			if(settings.LowerTextureScale)   Apply(s.Fields, "scalex_bottom", "scaley_bottom");
			if(settings.Brightness)			 Apply(s.Fields, "light", "lightabsolute");

			// Apply custom fields
			if(settings.Fields) ApplyCustomFields(s.Fields);
		}
	}

	#endregion

	#region ================== Linedef

	//mxd
	public class LinedefPropertiesCopySettings : MapElementPropertiesCopySettings
	{
		[FieldDescription(Description = "Sidedef Properties")]
		public bool SidedefProperties = true;
		
		[FieldDescription(Description = "Action")]
		public bool Action = true;
		
		[FieldDescription(Description = "Action Arguments", DOOM = false)]
		public bool Arguments = true;
		
		[FieldDescription(Description = "Activation", DOOM = false, UDMF = false)]
		public bool Activation = true;
		
		[FieldDescription(Description = "Tags", HEXEN = false)]
		public bool Tag = true;
		
		[FieldDescription(Description = "Flags")]
		public bool Flags = true;

		[FieldDescription(Description = "Alpha", DOOM = false, HEXEN = false)]
		public bool Alpha = true;

		[FieldDescription(Description = "Render Style", DOOM = false, HEXEN = false)]
		public bool RenderStyle = true;

		[FieldDescription(Description = "Lock Number", DOOM = false, HEXEN = false)]
		public bool LockNumber = true;
		
		[FieldDescription(Description = "Custom Fields", DOOM = false, HEXEN = false)]
		public bool Fields = true;

		[FieldDescription(Description = "Comment", DOOM = false, HEXEN = false)]
		public bool Comment = true;
	}

	// Linedef
	public class LinedefProperties : MapElementProperties
	{
		//mxd
		private static LinedefPropertiesCopySettings defaultsettings = new LinedefPropertiesCopySettings();
		public static LinedefPropertiesCopySettings CopySettings = new LinedefPropertiesCopySettings();
		
		private readonly SidedefProperties front;
		private readonly SidedefProperties back;
		private readonly Dictionary<string, bool> flags;
		private readonly int action;
		private readonly int activate;
		private readonly List<int> tags;
		private readonly int[] args;

		public LinedefProperties(Linedef l) : base(l.Fields, MapElementType.LINEDEF)
		{
			front = (l.Front != null ? new SidedefProperties(l.Front) : null);
			back = (l.Back != null ? new SidedefProperties(l.Back) : null);

			flags = l.GetFlags();
			action = l.Action;
			activate = l.Activate;
			tags = new List<int>(l.Tags); //mxd
			args = (int[])(l.Args.Clone());
		}

		//mxd. Applies coped properties with all settings enabled
		public void Apply(Linedef l, bool usecopysettings) { Apply(l, usecopysettings, true); }
		public void Apply(Linedef l, bool usecopysettings, bool applytosidedefs)
		{
			if(usecopysettings)
				Apply(l, CopySettings, (applytosidedefs ? SidedefProperties.CopySettings : null));
			else
				Apply(l, defaultsettings, (applytosidedefs ? SidedefProperties.DefaultSettings : null));
		}

		//mxd. Applies selected linededf and sidedef settings
		public void Apply(Linedef l, LinedefPropertiesCopySettings settings, SidedefPropertiesCopySettings sidesettings)
		{
			if(settings.SidedefProperties && sidesettings != null) 
			{
				if((front != null) && (l.Front != null)) front.Apply(l.Front, sidesettings);
				if((back != null) && (l.Back != null)) back.Apply(l.Back, sidesettings);
			}
			if(settings.Flags) 
			{
				l.ClearFlags();
				foreach(KeyValuePair<string, bool> f in flags)
					l.SetFlag(f.Key, f.Value);
			}
			if(settings.Activation) l.Activate = activate;
			if(settings.Tag) l.Tags = new List<int>(tags); //mxd
			if(settings.Action) l.Action = action;
			if(settings.Arguments)
			{
				for(int i = 0; i < l.Args.Length; i++)
					l.Args[i] = args[i];
			}

			// Should we bother?
			if(!General.Map.UDMF) return;

			// Apply fields
			l.Fields.BeforeFieldsChange();

			// Apply string arguments
			if(settings.Arguments)
			{
				Apply(l.Fields, "arg0str");

				//TODO: re-enable when UI part is ready
				//Apply(l.Fields, "arg1str");
				//Apply(l.Fields, "arg2str");
				//Apply(l.Fields, "arg3str");
				//Apply(l.Fields, "arg4str");
			}

			// Apply UI fields
			if(settings.Alpha)		 Apply(l.Fields, "alpha");
			if(settings.RenderStyle) Apply(l.Fields, "renderstyle");
			if(settings.LockNumber)  Apply(l.Fields, "locknumber");
			if(settings.Comment)	 Apply(l.Fields, "comment");

			// Apply custom fields
			if(settings.Fields) ApplyCustomFields(l.Fields);
		}
	}

	#endregion

	#region ================== Thing

	//mxd
	public class ThingPropertiesCopySettings : MapElementPropertiesCopySettings
	{
		[FieldDescription(Description = "Type")]
		public bool Type = true;
		
		[FieldDescription(Description = "Angle")]
		public bool Angle = true;

		[FieldDescription(Description = "Z Height", DOOM = false)]
		public bool ZHeight = true;
		
		[FieldDescription(Description = "Pitch", DOOM = false, HEXEN = false)]
		public bool Pitch = true;
		
		[FieldDescription(Description = "Roll", DOOM = false, HEXEN = false)]
		public bool Roll = true;
		
		[FieldDescription(Description = "Scale", DOOM = false, HEXEN = false)]
		public bool Scale = true;
		
		[FieldDescription(Description = "Action", DOOM = false)]
		public bool Action = true;
		
		[FieldDescription(Description = "Action Arguments", DOOM = false)]
		public bool Arguments = true;
		
		[FieldDescription(Description = "Tag", DOOM = false)]
		public bool Tag = true;
		
		[FieldDescription(Description = "Flags")]
		public bool Flags = true;

		[FieldDescription(Description = "Conversation ID", DOOM = false, HEXEN = false)]
		public bool Conversation = true;

		[FieldDescription(Description = "Gravity", DOOM = false, HEXEN = false)]
		public bool Gravity = true;

		[FieldDescription(Description = "Health Multiplier", DOOM = false, HEXEN = false)]
		public bool Health = true;

		[FieldDescription(Description = "Score", DOOM = false, HEXEN = false)]
		public bool Score = true;

		[FieldDescription(Description = "Alpha", DOOM = false, HEXEN = false)]
		public bool Alpha = true;

		[FieldDescription(Description = "Fill Color", DOOM = false, HEXEN = false)]
		public bool FillColor = true;

		[FieldDescription(Description = "Render Style", DOOM = false, HEXEN = false)]
		public bool RenderStyle = true;
		
		[FieldDescription(Description = "Custom Fields", DOOM = false, HEXEN = false)]
		public bool Fields = true;

		[FieldDescription(Description = "Comment", DOOM = false, HEXEN = false)]
		public bool Comment = true;
	}
	
	// Thing
	public class ThingProperties : MapElementProperties
	{
		//mxd
		private static ThingPropertiesCopySettings defaultsettings = new ThingPropertiesCopySettings();
		public static ThingPropertiesCopySettings CopySettings = new ThingPropertiesCopySettings();
		
		private readonly int type;
		private readonly float angle;
		private readonly float zheight; //mxd
		private readonly int pitch; //mxd
		private readonly int roll; //mxd
		private readonly float scalex; //mxd
		private readonly float scaley; //mxd
		private readonly Dictionary<string, bool> flags;
		private readonly int tag;
		private readonly int action;
		private readonly int[] args;
		
		public ThingProperties(Thing t) : base(t.Fields, MapElementType.THING)
		{
			type = t.Type;
			angle = t.Angle;
			zheight = t.Position.z;
			pitch = t.Pitch;
			roll = t.Roll;
			scalex = t.ScaleX;
			scaley = t.ScaleY;
			flags = t.GetFlags();
			tag = t.Tag;
			action = t.Action;
			args = (int[])(t.Args.Clone());
		}

		//mxd. Applies coped properties with all settings enabled
		public void Apply(Thing t, bool usecopysettings)
		{
			Apply(t, (usecopysettings ? CopySettings : defaultsettings));
		}
		
		//mxd. Applies selected settings
		public void Apply(Thing t, ThingPropertiesCopySettings settings)
		{
			if(settings.Type) t.Type = type;
			if(settings.Angle) t.Rotate(angle);
			if(settings.ZHeight) t.Move(t.Position.x, t.Position.y, zheight);
			if(settings.Pitch) t.SetPitch(pitch);
			if(settings.Roll) t.SetRoll(roll);
			if(settings.Scale) t.SetScale(scalex, scaley);
			if(settings.Flags) 
			{
				t.ClearFlags();
				foreach(KeyValuePair<string, bool> f in flags)
					t.SetFlag(f.Key, f.Value);
			}
			if(settings.Tag) t.Tag = tag;
			if(settings.Action) t.Action = action;
			if(settings.Arguments)
			{
				for(int i = 0; i < t.Args.Length; i++)
					t.Args[i] = args[i];
			}

			// Should we bother?
			if(!General.Map.UDMF) return;

			// Apply fields
			t.Fields.BeforeFieldsChange();

			// Apply string arguments
			if(settings.Arguments)
			{
				Apply(t.Fields, "arg0str");

				//TODO: re-enable when UI part is ready
				//Apply(t.Fields, "arg1str");
				//Apply(t.Fields, "arg2str");
				//Apply(t.Fields, "arg3str");
				//Apply(t.Fields, "arg4str");
			}

			// Apply UI fields
			if(settings.Conversation) Apply(t.Fields, "conversation");
			if(settings.Gravity)	  Apply(t.Fields, "gravity");
			if(settings.Health)		  Apply(t.Fields, "health");
			if(settings.FillColor)	  Apply(t.Fields, "fillcolor");
			if(settings.Alpha)		  Apply(t.Fields, "alpha");
			if(settings.Score)		  Apply(t.Fields, "score");
			if(settings.RenderStyle)  Apply(t.Fields, "renderstyle");
			if(settings.Comment)	  Apply(t.Fields, "comment");

			// Apply custom fields
			if(settings.Fields) ApplyCustomFields(t.Fields);
		}
	}

	#endregion

	#region ================== Properties Comparer

	//mxd. A class, which checks whether source and target map element's properties match
	public static class PropertiesComparer
	{

		#region Vertex

		public static bool PropertiesMatch(VertexPropertiesCopySettings flags, Vertex source, Vertex target) 
		{
			if(!General.Map.UDMF) return true;
			
			// Built-in properties
			if(flags.ZCeiling && source.ZCeiling != target.ZCeiling) return false;
			if(flags.ZFloor && source.ZFloor != target.ZFloor) return false;

			// Custom fields
			return !flags.Fields || UniFields.CustomFieldsMatch(source.Fields, target.Fields);
		}

		#endregion

		#region Sector

		public static bool PropertiesMatch(SectorPropertiesCopySettings flags, Sector source, Sector target) 
		{
			// Built-in properties
			if(flags.FloorHeight && source.FloorHeight != target.FloorHeight) return false;
			if(flags.CeilingHeight && source.CeilHeight != target.CeilHeight) return false;
			if(flags.FloorTexture && source.FloorTexture != target.FloorTexture) return false;
			if(flags.CeilingTexture && source.CeilTexture != target.CeilTexture) return false;
			if(flags.Brightness && source.Brightness != target.Brightness) return false;
			if(flags.Tag && TagsMatch(source.Tags, target.Tags)) return false;
			if(flags.Special && source.Effect != target.Effect) return false;
			if(flags.Flags && !FlagsMatch(source.GetFlags(), target.GetFlags())) return false;
			if(!General.Map.UDMF) return true;

			// UI fields
			if(flags.FloorTextureOffset && !UniFields.ValuesMatch("xpanningfloor", "ypanningfloor", source, target)) return false;
			if(flags.CeilingTextureOffset && !UniFields.ValuesMatch("xpanningceiling", "ypanningceiling", source, target)) return false;
			if(flags.FloorTextureScale && !UniFields.ValuesMatch("xscalefloor", "yscalefloor", source, target)) return false;
			if(flags.CeilingTextureScale && !UniFields.ValuesMatch("xscaleceiling", "yscaleceiling", source, target)) return false;
			if(flags.FloorTextureRotation && !UniFields.ValuesMatch("rotationfloor", source, target)) return false;
			if(flags.CeilingTextureRotation && !UniFields.ValuesMatch("rotationceiling", source, target)) return false;
			if(flags.FloorBrightness && !UniFields.ValuesMatch("lightfloor", "lightfloorabsolute", source, target)) return false;
			if(flags.CeilingBrightness && !UniFields.ValuesMatch("lightceiling", "lightceilingabsolute", source, target)) return false;
			if(flags.FloorAlpha && !UniFields.ValuesMatch("alphafloor", source, target)) return false;
			if(flags.CeilingAlpha && !UniFields.ValuesMatch("alphaceiling", source, target)) return false;
			if(flags.FloorRenderStyle && !UniFields.ValuesMatch("renderstylefloor", source, target)) return false;
			if(flags.CeilingRenderStyle && !UniFields.ValuesMatch("renderstyleceiling", source, target)) return false;
			if(flags.Gravity && !UniFields.ValuesMatch("gravity", source, target)) return false;
			if(flags.LightColor && !UniFields.ValuesMatch("lightcolor", source, target)) return false;
			if(flags.FadeColor && !UniFields.ValuesMatch("fadecolor", source, target)) return false;
			if(flags.Desaturation && !UniFields.ValuesMatch("desaturation", source, target)) return false;
			if(flags.SoundSequence && !UniFields.ValuesMatch("soundsequence", source, target)) return false;
			if(flags.Comment && !UniFields.ValuesMatch("comment", source, target)) return false;

			// Custom fields
			return !flags.Fields || UniFields.CustomFieldsMatch(source.Fields, target.Fields);
		}

		#endregion

		#region Linedef

		public static bool PropertiesMatch(LinedefPropertiesCopySettings linedefflags, SidedefPropertiesCopySettings sideflags, Linedef source, Linedef target) 
		{
			// Built-in properties
			if(linedefflags.Action && source.Action != target.Action) return false;
			if(linedefflags.Activation && source.Activate != target.Activate) return false;
			if(linedefflags.Tag && TagsMatch(source.Tags, target.Tags)) return false; //mxd
			if(linedefflags.Arguments) 
			{
				// Classic args
				for(int i = 0; i < source.Args.Length; i++)
					if(source.Args[i] != target.Args[i]) return false;

				// String args
				if(General.Map.UDMF)
				{
					if(!UniFields.ValuesMatch("arg0str", source, target)) return false;
					if(!UniFields.ValuesMatch("arg1str", source, target)) return false;
					if(!UniFields.ValuesMatch("arg2str", source, target)) return false;
					if(!UniFields.ValuesMatch("arg3str", source, target)) return false;
					if(!UniFields.ValuesMatch("arg4str", source, target)) return false;
				}
			}
			if(linedefflags.Flags && !FlagsMatch(source.GetFlags(), target.GetFlags())) return false;
			if(linedefflags.SidedefProperties) 
			{
				if((source.Front == null && target.Front != null) || (source.Front != null && target.Front == null) ||
					(source.Back == null && target.Back != null) || (source.Back != null && target.Back == null)) 
					return false;

				if(source.Front != null && !PropertiesMatch(sideflags, source.Front, target.Front)) return false;
				if(source.Back != null && !PropertiesMatch(sideflags, source.Back, target.Back)) return false;
			}
			if(!General.Map.UDMF) return true;

			// UI fields
			if(linedefflags.Alpha && !UniFields.ValuesMatch("alpha", source, target)) return false;
			if(linedefflags.RenderStyle && !UniFields.ValuesMatch("renderstyle", source, target)) return false;
			if(linedefflags.LockNumber && !UniFields.ValuesMatch("locknumber", source, target)) return false;
			if(linedefflags.Comment && !UniFields.ValuesMatch("comment", source, target)) return false;

			// Custom fields
			return !linedefflags.Fields || UniFields.CustomFieldsMatch(source.Fields, target.Fields);
		}

		#endregion

		#region Sidedef

		public static bool PropertiesMatch(SidedefPropertiesCopySettings flags, Sidedef source, Sidedef target) 
		{
			// Built-in properties
			if(flags.OffsetX && source.OffsetX != target.OffsetX) return false;
			if(flags.OffsetY && source.OffsetY != target.OffsetY) return false;
			if(flags.UpperTexture && source.HighTexture != target.HighTexture) return false;
			if(flags.MiddleTexture && source.MiddleTexture != target.MiddleTexture) return false;
			if(flags.LowerTexture && source.LowTexture != target.LowTexture) return false;
			if(!General.Map.UDMF) return true;

			// UDMF-specific properties
			if(flags.Flags && !FlagsMatch(source.GetFlags(), target.GetFlags())) return false;

			// UI fields
			if(flags.UpperTextureScale && !UniFields.ValuesMatch("scalex_top", "scaley_top", source, target)) return false;
			if(flags.MiddleTextureScale && !UniFields.ValuesMatch("scalex_mid", "scaley_mid", source, target)) return false;
			if(flags.LowerTextureScale && !UniFields.ValuesMatch("scalex_bottom", "scaley_bottom", source, target)) return false;
			if(flags.UpperTextureOffset && !UniFields.ValuesMatch("offsetx_top", "offsety_top", source, target)) return false;
			if(flags.MiddleTextureOffset && !UniFields.ValuesMatch("offsetx_mid", "offsety_mid", source, target)) return false;
			if(flags.LowerTextureOffset && !UniFields.ValuesMatch("offsetx_bottom", "offsety_bottom", source, target)) return false;
			if(flags.Brightness && !UniFields.ValuesMatch("light", "lightabsolute", source, target)) return false;

			// Custom fields
			return !flags.Fields || UniFields.CustomFieldsMatch(source.Fields, target.Fields);
		}

		#endregion

		#region Thing

		public static bool PropertiesMatch(ThingPropertiesCopySettings flags, Thing source, Thing target) 
		{
			// Built-in properties
			if(flags.Type && source.Type != target.Type) return false;
			if(flags.Angle && source.AngleDoom != target.AngleDoom) return false;
			if(flags.Action && source.Action != target.Action) return false;
			if(flags.Arguments) 
			{
				// Classic args
				for(int i = 0; i < source.Args.Length; i++)
					if(source.Args[i] != target.Args[i]) return false;

				// String args
				if(General.Map.UDMF)
				{
					if(!UniFields.ValuesMatch("arg0str", source, target)) return false;
					if(!UniFields.ValuesMatch("arg1str", source, target)) return false;
					if(!UniFields.ValuesMatch("arg2str", source, target)) return false;
					if(!UniFields.ValuesMatch("arg3str", source, target)) return false;
					if(!UniFields.ValuesMatch("arg4str", source, target)) return false;
				}
			}
			if(flags.Tag && source.Tag != target.Tag) return false;
			if(flags.Flags && !FlagsMatch(source.GetFlags(), target.GetFlags())) return false;
			if(!General.Map.UDMF) return true;

			// UDMF-specific properties
			if(flags.Pitch && source.Pitch != target.Pitch) return false;
			if(flags.Roll && source.Roll != target.Roll) return false;
			if(flags.Scale && (source.ScaleX != target.ScaleX) || (source.ScaleY != target.ScaleY)) return false;

			// UI fields
			if(flags.Conversation && !UniFields.ValuesMatch("conversation", source, target)) return false;
			if(flags.Gravity && !UniFields.ValuesMatch("gravity", source, target)) return false;
			if(flags.Health && !UniFields.ValuesMatch("health", source, target)) return false;
			if(flags.FillColor && !UniFields.ValuesMatch("fillcolor", source, target)) return false;
			if(flags.Alpha && !UniFields.ValuesMatch("alpha", source, target)) return false;
			if(flags.Score && !UniFields.ValuesMatch("score", source, target)) return false;
			if(flags.RenderStyle && !UniFields.ValuesMatch("renderstyle", source, target)) return false;
			if(flags.Comment && !UniFields.ValuesMatch("comment", source, target)) return false;

			// Custom fields
			return !flags.Fields || UniFields.CustomFieldsMatch(source.Fields, target.Fields);
		}

		#endregion

		private static bool FlagsMatch(Dictionary<string, bool> flags1, Dictionary<string, bool> flags2) 
		{
			if(flags1.Count != flags2.Count) return false;
			foreach(KeyValuePair<string, bool> group in flags1)
				if(!flags2.ContainsKey(group.Key) || flags2[group.Key] != flags1[group.Key]) return false;
			return true;
		}

		//mxd
		private static bool TagsMatch(List<int> tags1, List<int> tags2)
		{
			if(!General.Map.UDMF) return tags1[0] == tags2[0];
			
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

	#endregion

}
