
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
using CodeImp.DoomBuilder.Interface;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using System.Drawing;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(SwitchAction = "brightnessmode",
			  ButtonDesc = "Brightness Mode",
			  ButtonImage = "BrightnessMode.png",
			  ButtonOrder = int.MinValue + 3)]

	public class BrightnessMode : SectorsMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public BrightnessMode()
		{
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();

			// Make customized presentation
			CustomPresentation p = new CustomPresentation();
			p.AddLayer(new PresentLayer(RendererLayer.Background, BlendingMode.Mask));
			p.AddLayer(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			p.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.Alpha, 1f, true));
			p.AddLayer(new PresentLayer(RendererLayer.Things, BlendingMode.Alpha, 0.3f, false));
			p.AddLayer(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1f, true));
			renderer.SetPresentation(p);
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			// Render lines and vertices
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.PlotSector(highlighted, General.Colors.Highlight);
				renderer.Finish();
			}

			// Render things
			if(renderer.StartThings(true))
			{
				renderer.RenderThingSet(General.Map.Map.Things);
				renderer.Finish();
			}

			// Render overlay
			UpdateOverlay();

			renderer.Present();
		}

		// This updates the overlay
		private void UpdateOverlay()
		{
			if(renderer.StartOverlay(true))
			{
				// Go for all sectors
				foreach(Sector s in General.Map.Map.Sectors)
				{
					// Determine color by brightness
					PixelColor brightnesscolor = new PixelColor(255, (byte)s.Brightness, (byte)s.Brightness, (byte)s.Brightness);
					int brightnessint = brightnesscolor.ToInt();

					/*
					// Load texture image
					ImageData texture = General.Map.Data.GetFlatImage(s.LongFloorTexture);
					if(!texture.IsLoaded) texture.LoadImage();

					// Make vertices
					FlatVertex[] verts = new FlatVertex[s.Vertices.Length];
					s.Vertices.CopyTo(verts, 0);
					for(int i = 0; i < verts.Length; i++)
					{
						verts[i].u = verts[i].x / texture.ScaledWidth;
						verts[i].v = verts[i].y / texture.ScaledHeight;
					}

					// Render the geometry
					renderer.RenderGeometry(verts, texture, true);
					*/
					
					// Render the geometry
					renderer.RenderGeometry(s.Vertices, null, true);
				}

				if(selecting) RenderMultiSelection();

				renderer.Finish();
			}
		}
		
		#endregion

		#region ================== Actions

		#endregion
	}
}
