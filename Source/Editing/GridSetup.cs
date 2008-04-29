
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
using System.Diagnostics;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	public class GridSetup
	{
		#region ================== Constants

		private const int DEFAULT_GRID_SIZE = 32;

		public const int SOURCE_TEXTURES = 0;
		public const int SOURCE_FLATS = 1;

		#endregion

		#region ================== Variables

		// Grid
		private int gridsize;
		private float gridsizef;
		private float gridsizefinv;

		// Background
		private string background = "";
		private int backsource;
		private ImageData backimage = new NullImage();
		private int backoffsetx, backoffsety;

		#endregion

		#region ================== Properties

		public int GridSize { get { return gridsize; } }
		public float GridSizeF { get { return gridsizef; } }
		internal string BackgroundName { get { return background; } }
		internal int BackgroundSource { get { return backsource; } }
		internal ImageData Background { get { return backimage; } }
		internal int BackgroundX { get { return backoffsetx; } }
		internal int BackgroundY { get { return backoffsety; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal GridSetup()
		{
			// Initialize
			SetGridSize(DEFAULT_GRID_SIZE);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This sets the grid size
		internal void SetGridSize(int size)
		{
			// Change grid
			this.gridsize = size;
			this.gridsizef = (float)gridsize;
			this.gridsizefinv = 1f / gridsizef;

			// Update in main window
			General.MainWindow.UpdateGrid(gridsize);
		}

		// This sets the background
		internal void SetBackground(string name, int source)
		{
			// Set background
			if(name == null) name = "";
			this.backsource = source;
			this.background = name;

			// Find this image
			LinkBackground();
		}

		// This sets the background offset
		internal void SetBackgroundOffset(int offsetx, int offsety)
		{
			// Set background offset
			this.backoffsetx = offsetx;
			this.backoffsety = offsety;
		}
		
		// This finds and links the background image
		internal void LinkBackground()
		{
			// From textures?
			if(backsource == SOURCE_TEXTURES)
			{
				// Get this texture
				backimage = General.Map.Data.GetTextureImage(background);
			}
			// From flats?
			else if(backsource == SOURCE_FLATS)
			{
				// Get this flat
				backimage = General.Map.Data.GetFlatImage(background);
			}
			
			// Make sure it is loaded
			backimage.LoadImage();
			backimage.CreateTexture();
		}
		
		// This returns the next higher coordinate
		public float GetHigher(float offset)
		{
			return (float)Math.Round((offset + (gridsizef * 0.5f)) * gridsizefinv) * gridsizef;
		}

		// This returns the next lower coordinate
		public float GetLower(float offset)
		{
			return (float)Math.Round((offset - (gridsizef * 0.5f)) * gridsizefinv) * gridsizef;
		}
		
		// This snaps to the nearest grid coordinate
		public Vector2D SnappedToGrid(Vector2D v)
		{
			return GridSetup.SnappedToGrid(v, gridsizef, gridsizefinv);
		}

		// This snaps to the nearest grid coordinate
		public static Vector2D SnappedToGrid(Vector2D v, float gridsize, float gridsizeinv)
		{
			return new Vector2D((float)Math.Round(v.x * gridsizeinv) * gridsize,
								(float)Math.Round(v.y * gridsizeinv) * gridsize);
		}

		#endregion
	}
}
