using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.Data;
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.Geometry;

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
		public string BackgroundName { get { return background; } }
		public int BackgroundSource { get { return backsource; } }
		public ImageData Background { get { return backimage; } }
		public int BackgroundX { get { return backoffsetx; } }
		public int BackgroundY { get { return backoffsety; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public GridSetup()
		{
			// Initialize
			SetGridSize(DEFAULT_GRID_SIZE);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// This sets the grid size
		public void SetGridSize(int size)
		{
			// Change grid
			this.gridsize = size;
			this.gridsizef = (float)gridsize;
			this.gridsizefinv = 1f / gridsizef;

			// Update in main window
			General.MainWindow.UpdateGrid(gridsize);
		}

		// This sets the background
		public void SetBackground(string name, int source)
		{
			// Set background
			if(name == null) name = "";
			this.backsource = source;
			this.background = name;

			// Find this image
			LinkBackground();
		}

		// This sets the background offset
		public void SetBackgroundOffset(int offsetx, int offsety)
		{
			// Set background offset
			this.backoffsetx = offsetx;
			this.backoffsety = offsety;
		}
		
		// This finds and links the background image
		public void LinkBackground()
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
