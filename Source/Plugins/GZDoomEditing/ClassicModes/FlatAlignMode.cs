
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
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using System.Drawing;
using CodeImp.DoomBuilder.Actions;

#endregion

namespace CodeImp.DoomBuilder.GZDoomEditing
{
	public abstract class FlatAlignMode : BaseClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private ICollection<Sector> selection;
		protected Sector editsector;
		private ImageData texture;
		private Vector2D texturegraboffset;
		private float rotation;
		private Vector2D scale;
		private Vector2D offset;
		
		#endregion

		#region ================== Properties

		public abstract string XScaleName { get; }
		public abstract string YScaleName { get; }
		public abstract string XOffsetName { get; }
		public abstract string YOffsetName { get; }
		public abstract string RotationName { get; }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		protected FlatAlignMode()
		{
		}

		#endregion

		#region ================== Methods

		protected abstract ImageData GetTexture(Sector editsector);

		// Transforms p from Texture space into World space
		protected Vector2D TexToWorld(Vector2D p)
		{
			p /= scale;
			p -= offset;
			p = p.GetRotated(-rotation);
			return p;
		}

		// Transforms p from World space into Texture space
		protected Vector2D WorldToTex(Vector2D p)
		{
			p = p.GetRotated(rotation);
			p += offset;
			p *= scale;
			return p;
		}

		#endregion

		#region ================== Events

		// Mode engages
		public override void OnEngage()
		{
			base.OnEngage();

			// Presentation
			renderer.SetPresentation(Presentation.Standard);

			// Selection
			General.Map.Map.ConvertSelection(SelectionType.Sectors);
			General.Map.Map.SelectionType = SelectionType.Sectors;
			if(General.Map.Map.SelectedSectorsCount == 0)
			{
				// Find the nearest linedef within highlight range
				Linedef l = General.Map.Map.NearestLinedef(mousemappos);
				if(l != null)
				{
					Sector selectsector = null;
					
					// Check on which side of the linedef the mouse is and which sector there is
					float side = l.SideOfLine(mousemappos);
					if((side > 0) && (l.Back != null))
						selectsector = l.Back.Sector;
					else if((side <= 0) && (l.Front != null))
						selectsector = l.Front.Sector;

					// Select the sector!
					if(selectsector != null)
					{
						selectsector.Selected = true;
						foreach(Sidedef sd in selectsector.Sidedefs)
							sd.Line.Selected = true;
					}
				}
			}
			
			// Get sector selection
			selection = General.Map.Map.GetSelectedSectors(true);
			if(selection.Count == 0)
			{
				General.Interface.MessageBeep(MessageBeepType.Default);
				General.Interface.DisplayStatus(StatusType.Info, "A selected sector is required for this action.");
				General.Editing.CancelMode();
				return;
			}
			editsector = General.GetByIndex(selection, 0);

			// Get the texture
			texture = GetTexture(editsector);
			if((texture == null) || (texture == General.Map.Data.WhiteTexture) ||
			   (texture.Width <= 0) || (texture.Height <= 0) || !texture.IsImageLoaded)
			{
				General.Interface.MessageBeep(MessageBeepType.Default);
				General.Interface.DisplayStatus(StatusType.Info, "The selected sector must have a loaded texture to align.");
				General.Editing.CancelMode();
				return;
			}
			
			// Cache the transformation values
			rotation = Angle2D.DegToRad(editsector.Fields.GetValue(RotationName, 0.0f));
			scale.x = editsector.Fields.GetValue(XScaleName, 1.0f);
			scale.y = editsector.Fields.GetValue(YScaleName, 1.0f);
			offset.x = editsector.Fields.GetValue(XOffsetName, 0.0f);
			offset.y = -editsector.Fields.GetValue(YOffsetName, 0.0f);
			
			// We want the texture corner nearest to the center of the sector
			Vector2D fp;
			fp.x = (editsector.BBox.Left + editsector.BBox.Right) / 2;
			fp.y = (editsector.BBox.Top + editsector.BBox.Bottom) / 2;

			// Transform the point into texture space
			fp = WorldToTex(fp);
			
			// Snap to the nearest left-top corner
			fp.x = (float)Math.Round(fp.x / texture.ScaledWidth) * texture.ScaledWidth;
			fp.y = (float)Math.Round(fp.y / texture.ScaledHeight) * texture.ScaledHeight;
			texturegraboffset = fp;

			// Transorm the point into world space
			// fp = fp.GetRotated(rotation);
			// fp = (fp / scale) + offset;
		}

		// Mode disengages
		public override void OnDisengage()
		{
			base.OnDisengage();

			// Hide highlight info
			General.Interface.HideInfo();
		}

		// This redraws the display
		public override void OnRedrawDisplay()
		{
			renderer.RedrawSurface();

			// Render lines
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
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
			if(renderer.StartOverlay(true))
			{
				Vector2D rightpoint = texturegraboffset + new Vector2D(texture.ScaledWidth, 0f);

				Vector2D p1world = TexToWorld(texturegraboffset);
				Vector2D p2world = TexToWorld(rightpoint);

				renderer.RenderLine(p1world, p2world, 1f, General.Colors.Highlight, true);

				renderer.Finish();
			}

			renderer.Present();
		}

		#endregion
	}
}
