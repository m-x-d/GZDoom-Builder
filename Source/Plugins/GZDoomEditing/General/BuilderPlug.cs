
#region ================== Copyright (c) 2010 Pascal vd Heiden

/*
 * Copyright (c) 2010 Pascal vd Heiden
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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.GZDoomEditing
{
	public class BuilderPlug : Plug
	{
		#region ================== Variables

		// Static instance
		private static BuilderPlug me;

		#endregion

		#region ================== Properties

		// Static property to access the BuilderPlug
		public static BuilderPlug Me { get { return me; } }

		#endregion

		#region ================== Initialize / Dispose
		
		// This event is called when the plugin is initialized
		public override void OnInitialize()
		{
			base.OnInitialize();

			// Keep a static reference
            me = this;
		}
		
		// This is called when the plugin is terminated
		public override void Dispose()
		{
			base.Dispose();
		}

		#endregion

		#region ================== Classic Mode Surfaces
		
		// This is called when the vertices are created for the classic mode surfaces
		public override void OnSectorFloorSurfaceUpdate(Sector s, ref FlatVertex[] vertices)
		{
			ImageData img = General.Map.Data.GetFlatImage(s.LongFloorTexture);
			if((img != null) && img.IsImageLoaded)
			{
				float xpan, ypan, xscale, yscale, rotate;
				int color, light;
				bool absolute;
				
				try
				{
					// Fetch ZDoom fields
					xpan = s.Fields.ContainsKey("xpanningfloor") ? (float)s.Fields["xpanningfloor"].Value : 0.0f;
					ypan = s.Fields.ContainsKey("ypanningfloor") ? (float)s.Fields["ypanningfloor"].Value : 0.0f;
					xscale = s.Fields.ContainsKey("xscalefloor") ? (float)s.Fields["xscalefloor"].Value : 1.0f;
					yscale = s.Fields.ContainsKey("yscalefloor") ? (float)s.Fields["yscalefloor"].Value : 1.0f;
					rotate = s.Fields.ContainsKey("rotationfloor") ? (float)s.Fields["rotationfloor"].Value : 0.0f;
					color = s.Fields.ContainsKey("lightcolor") ? (int)s.Fields["lightcolor"].Value : -1;
					light = s.Fields.ContainsKey("lightfloor") ? (int)s.Fields["lightfloor"].Value : 0;
					absolute = s.Fields.ContainsKey("lightfloorabsolute") ? (bool)s.Fields["lightfloorabsolute"].Value : false;
				}
				catch(Exception) { return; }
				
				// Setup the vertices with the given settings
				SetupSurfaceVertices(vertices, s, img, xpan, ypan, xscale, yscale, rotate, color, light, absolute);
			}
		}

		// This is called when the vertices are created for the classic mode surfaces
		public override void OnSectorCeilingSurfaceUpdate(Sector s, ref FlatVertex[] vertices)
		{
			ImageData img = General.Map.Data.GetFlatImage(s.LongFloorTexture);
			if((img != null) && img.IsImageLoaded)
			{
				float xpan, ypan, xscale, yscale, rotate;
				int color, light;
				bool absolute;
				
				try
				{
					// Fetch ZDoom fields
					xpan = s.Fields.ContainsKey("xpanningceiling") ? (float)s.Fields["xpanningceiling"].Value : 0.0f;
					ypan = s.Fields.ContainsKey("ypanningceiling") ? (float)s.Fields["ypanningceiling"].Value : 0.0f;
					xscale = s.Fields.ContainsKey("xscaleceiling") ? (float)s.Fields["xscaleceiling"].Value : 1.0f;
					yscale = s.Fields.ContainsKey("yscaleceiling") ? (float)s.Fields["yscaleceiling"].Value : 1.0f;
					rotate = s.Fields.ContainsKey("rotationceiling") ? (float)s.Fields["rotationceiling"].Value : 0.0f;
					color = s.Fields.ContainsKey("lightcolor") ? (int)s.Fields["lightcolor"].Value : -1;
					light = s.Fields.ContainsKey("lightceiling") ? (int)s.Fields["lightceiling"].Value : 0;
					absolute = s.Fields.ContainsKey("lightceilingabsolute") ? (bool)s.Fields["lightceilingabsolute"].Value : false;
				}
				catch(Exception) { return; }

				// Setup the vertices with the given settings
				SetupSurfaceVertices(vertices, s, img, xpan, ypan, xscale, yscale, rotate, color, light, absolute);
			}
		}

		// This applies the given values on the vertices
		private void SetupSurfaceVertices(FlatVertex[] vertices, Sector s, ImageData img, float xpan, float ypan,
			                              float xscale, float yscale, float rotate, int color, int light, bool absolute)
		{
			// Prepare for math!
			rotate = Angle2D.DegToRad(rotate);
			Vector2D scale = new Vector2D(xscale, yscale);
			Vector2D texscale = new Vector2D(1.0f / img.ScaledWidth, 1.0f / img.ScaledHeight);
			Vector2D offset = new Vector2D(xpan, ypan);
			if(!absolute) light = s.Brightness + light;
			PixelColor lightcolor = PixelColor.FromInt(color);
			PixelColor brightness = PixelColor.FromInt(General.Map.Renderer2D.CalculateBrightness(light));
			PixelColor finalcolor = PixelColor.Modulate(lightcolor, brightness);
			color = finalcolor.WithAlpha(255).ToInt();
			
			// Do the math for all vertices
			for(int i = 0; i < vertices.Length; i++)
			{
				Vector2D pos = new Vector2D(vertices[i].x, vertices[i].y);
				pos = pos.GetRotated(rotate);
				pos.y = -pos.y;
				pos = (pos + offset) * scale * texscale;
				vertices[i].u = pos.x;
				vertices[i].v = pos.y;
				vertices[i].c = color;
			}
		}

		#endregion
	}
}
