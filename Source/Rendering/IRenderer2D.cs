
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
		// Properties
		float OffsetX { get; }
		float OffsetY { get; }
		float Scale { get; }
		int VertexSize { get; }
		
		// Color methods
		PixelColor DetermineLinedefColor(Linedef l);
		PixelColor DetermineThingColor(Thing t);
		int DetermineVertexColor(Vertex v);

		// Rendering management methods
		bool StartPlotter(bool clear);
		bool StartThings(bool clear);
		bool StartOverlay(bool clear);
		void Finish();
		void SetThingsRenderOrder(bool front);
		void Present();

		// Drawing methods
		void PlotLine(Vector2D start, Vector2D end, PixelColor c);
		void PlotLinedef(Linedef l, PixelColor c);
		void PlotLinedefSet(ICollection<Linedef> linedefs);
		void PlotSector(Sector s);
		void PlotSector(Sector s, PixelColor c);
		void PlotVertex(Vertex v, int colorindex);
		void PlotVertexAt(Vector2D v, int colorindex);
		void PlotVerticesSet(ICollection<Vertex> vertices);
		void RenderThing(Thing t, PixelColor c);
		void RenderThingSet(ICollection<Thing> things);
		void RenderRectangle(RectangleF rect, float bordersize, PixelColor c, bool transformrect);
		void RenderRectangleFilled(RectangleF rect, PixelColor c, bool transformrect);
		void RenderLine(Vector2D start, Vector2D end, float thickness, PixelColor c, bool transformcoords);
		void RenderText(TextLabel text);
	}
}
