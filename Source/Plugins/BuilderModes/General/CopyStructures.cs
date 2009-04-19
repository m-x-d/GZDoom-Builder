
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
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	// Sector
	public class SectorProperties
	{
		private int floorheight;
		private int ceilheight;
		private string floortexture;
		private string ceilingtexture;
		private int brightness;
		private int tag;
		
		public SectorProperties(Sector s)
		{
			floorheight = s.FloorHeight;
			ceilheight = s.CeilHeight;
			floortexture = s.FloorTexture;
			ceilingtexture = s.CeilTexture;
			brightness = s.Brightness;
			tag = s.Tag;
		}
		
		public void Apply(Sector s)
		{
			s.FloorHeight = floorheight;
			s.CeilHeight = ceilheight;
			s.SetFloorTexture(floortexture);
			s.SetCeilTexture(ceilingtexture);
			s.Brightness = brightness;
			s.Tag = s.Tag;
		}
	}
	
	// Sidedef
	public class SidedefProperties
	{
		private string hightexture;
		private string middletexture;
		private string lowtexture;
		private int offsetx;
		private int offsety;

		public SidedefProperties(Sidedef s)
		{
			hightexture = s.HighTexture;
			middletexture = s.MiddleTexture;
			lowtexture = s.LowTexture;
			offsetx = s.OffsetX;
			offsety = s.OffsetY;
		}
		
		public void Apply(Sidedef s)
		{
			s.SetTextureHigh(hightexture);
			s.SetTextureMid(middletexture);
			s.SetTextureLow(lowtexture);
			s.OffsetX = offsetx;
			s.OffsetY = offsety;
		}
	}
	
	// Thing
	public class ThingProperties
	{
		private int type;
		private float angle;
		private Dictionary<string, bool> flags;
		private int tag;
		private int action;
		private int[] args;
		
		public ThingProperties(Thing t)
		{
			type = t.Type;
			angle = t.Angle;
			flags = new Dictionary<string, bool>(t.Flags);
			tag = t.Tag;
			action = t.Action;
			args = (int[])(t.Args.Clone());
		}
		
		public void Apply(Thing t)
		{
			t.Type = type;
			t.Rotate(angle);
			t.Flags.Clear();
			foreach(KeyValuePair<string, bool> f in flags)
				t.Flags.Add(f.Key, f.Value);
			t.Tag = tag;
			t.Action = action;
			for(int i = 0; i < t.Args.Length; i++)
				t.Args[i] = args[i];
		}
	}
}
