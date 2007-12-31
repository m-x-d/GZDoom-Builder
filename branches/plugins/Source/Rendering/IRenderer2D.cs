
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
using System.Drawing;
using System.ComponentModel;
using CodeImp.DoomBuilder.Map;
using SlimDX.Direct3D;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	public interface IRenderer2D
	{
		float OffsetX { get; }
		float OffsetY { get; }
		float Scale { get; }
		PixelColor DetermineLinedefColor(CodeImp.DoomBuilder.Map.Linedef l);
		PixelColor DetermineThingColor(CodeImp.DoomBuilder.Map.Thing t);
		int DetermineVertexColor(CodeImp.DoomBuilder.Map.Vertex v);
		void Finish();
		void RenderLinedef(CodeImp.DoomBuilder.Map.Linedef l, PixelColor c);
		void RenderLinedefSet(System.Collections.Generic.ICollection<CodeImp.DoomBuilder.Map.Linedef> linedefs);
		void RenderSector(CodeImp.DoomBuilder.Map.Sector s);
		void RenderSector(CodeImp.DoomBuilder.Map.Sector s, PixelColor c);
		void RenderThing(CodeImp.DoomBuilder.Map.Thing t, PixelColor c);
		void RenderThingSet(System.Collections.Generic.ICollection<CodeImp.DoomBuilder.Map.Thing> things);
		void RenderVertex(CodeImp.DoomBuilder.Map.Vertex v, int colorindex);
		void RenderVerticesSet(System.Collections.Generic.ICollection<CodeImp.DoomBuilder.Map.Vertex> vertices);
		void SetThingsRenderOrder(bool front);
		bool Start(bool clearstructs, bool clearthings);
	}
}
