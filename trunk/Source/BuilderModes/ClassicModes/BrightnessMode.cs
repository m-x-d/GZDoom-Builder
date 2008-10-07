
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
using CodeImp.DoomBuilder.Editing;
using System.Drawing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Brightness Mode",
			  SwitchAction = "brightnessmode",
			  ButtonDesc = "Brightness Mode",
			  ButtonImage = "BrightnessMode.png",
			  ButtonOrder = int.MinValue + 201,
			  AllowCopyPaste = false)]
	
	public sealed class BrightnessMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables
		
		// Highlighted item
		private Sector highlighted;
		
		// Interface
		private bool editpressed;
		
		// Labels
		private Dictionary<Sector, TextLabel[]> labels;
		
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
					
					// This was only to test if it would work
					// It works, but it is very slow
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
					renderer.RenderGeometry(s.FlatVertices, null, true);
				}
				
				// Go for all sectors
				foreach(Sector s in General.Map.Map.Sectors)
				{
					// Render labels
					TextLabel[] labelarray = labels[s];
					foreach(TextLabel l in labelarray) renderer.RenderText(l);
				}
				
				renderer.Finish();
			}
		}
		
		// This highlights a new item
		protected void Highlight(Sector s)
		{
			// Update display
			if(renderer.StartPlotter(false))
			{
				// Undraw previous highlight
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.PlotSector(highlighted);
				
				// Set new highlight
				highlighted = s;
				
				// Render highlighted item
				if((highlighted != null) && !highlighted.IsDisposed)
					renderer.PlotSector(highlighted, General.Colors.Highlight);
				
				// Done
				renderer.Finish();
				renderer.Present();
			}
			
			// Show highlight info
			if((highlighted != null) && !highlighted.IsDisposed)
				General.Interface.ShowSectorInfo(highlighted);
			else
				General.Interface.HideInfo();
		}
		
		#endregion
		
		#region ================== Events
		
		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();
			
			// No selection
			General.Map.Map.ClearAllMarks(false);
			General.Map.Map.ClearAllSelected();
			
			// Make custom presentation
			CustomPresentation p = new CustomPresentation();
			p.AddLayer(new PresentLayer(RendererLayer.Background, BlendingMode.Mask));
			p.AddLayer(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			p.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.Alpha, 1f, true));
			//p.AddLayer(new PresentLayer(RendererLayer.Things, BlendingMode.Alpha, Presentation.THINGS_BACK_ALPHA, false));
			p.AddLayer(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1f, true));
			renderer.SetPresentation(p);
			
			// Make text labels for sectors
			labels = new Dictionary<Sector, TextLabel[]>(General.Map.Map.Sectors.Count);
			foreach(Sector s in General.Map.Map.Sectors)
			{
				// Setup labels
				TextLabel[] labelarray = new TextLabel[s.Triangles.IslandVertices.Count];
				for(int i = 0; i < s.Triangles.IslandVertices.Count; i++)
				{
					Vector2D v = s.Labels[i].position;
					labelarray[i] = new TextLabel(20);
					labelarray[i].TransformCoords = true;
					labelarray[i].Rectangle = new RectangleF(v.x, v.y, 0.0f, 0.0f);
					labelarray[i].AlignX = TextAlignmentX.Center;
					labelarray[i].AlignY = TextAlignmentY.Middle;
					labelarray[i].Scale = 14f;
					labelarray[i].Color = General.Colors.Highlight;
					labelarray[i].Backcolor = General.Colors.Background;
				}
				labels.Add(s, labelarray);
			}
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
				renderer.RenderThingSet(General.Map.ThingsFilter.HiddenThings, Presentation.THINGS_HIDDEN_ALPHA);
				renderer.RenderThingSet(General.Map.ThingsFilter.VisibleThings, 1.0f);
				renderer.Finish();
			}

			// Render overlay
			UpdateOverlay();

			renderer.Present();
		}
		
		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			// Not holding any buttons?
			if(e.Button == MouseButtons.None)
			{
				// Find the nearest linedef within highlight range
				Linedef l = General.Map.Map.NearestLinedef(mousemappos);
				if(l != null)
				{
					// Check on which side of the linedef the mouse is
					float side = l.SideOfLine(mousemappos);
					if(side > 0)
					{
						// Is there a sidedef here?
						if(l.Back != null)
						{
							// Highlight if not the same
							if(l.Back.Sector != highlighted) Highlight(l.Back.Sector);
						}
						else
						{
							// Highlight nothing
							if(highlighted != null) Highlight(null);
						}
					}
					else
					{
						// Is there a sidedef here?
						if(l.Front != null)
						{
							// Highlight if not the same
							if(l.Front.Sector != highlighted) Highlight(l.Front.Sector);
						}
						else
						{
							// Highlight nothing
							if(highlighted != null) Highlight(null);
						}
					}
				}
				else
				{
					// Highlight nothing
					if(highlighted != null) Highlight(null);
				}
			}
		}
		
		// Selecting with mouse
		protected override void OnSelect()
		{
			base.OnSelect();
			
			// Sector highlighted?
			if((highlighted != null) && !highlighted.IsDisposed)
			{
				// Show index on label
				for(int i = 0; i < highlighted.Triangles.IslandVertices.Count; i++)
				{
					labels[highlighted][i].Text = highlighted.Index.ToString();
				}
				
				UpdateOverlay();
				renderer.Present();
			}
		}
		
		#endregion
	}
}
