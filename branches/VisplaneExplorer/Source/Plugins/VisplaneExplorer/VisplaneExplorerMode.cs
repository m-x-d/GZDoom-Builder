#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Plugins.VisplaneExplorer
{
	[EditMode(DisplayName = "Visplane Explorer",
			  SwitchAction = "visplaneexplorermode",
			  ButtonImage = "Gauge.png",
			  ButtonOrder = 300,
			  ButtonGroup = "002_tools",
			  UseByDefault = true,
			  AllowCopyPaste = false)]
	public class VisplaneExplorerMode : ClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// The image is the ImageData resource for Doom Builder to work with
		private BitmapImage image;
		
		// This is the bitmap that we will be drawing on
		private Bitmap canvas;
		private Graphics graphics;

		// Temporary WAD file written for the vpo.dll library
		private string tempfile;

		// Rectangle around the map
		private Rectangle mapbounds;

		// 64x64 tiles in map space. These are discarded when outside view.
		private Dictionary<Point, Tile> tiles = new Dictionary<Point, Tile>();

		// Time when to do another update
		private DateTime nextupdate;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public VisplaneExplorerMode()
		{
			// Initialize
		}

		// Disposer
		public override void Dispose()
		{
			CleanUp();
			base.Dispose();
		}

		#endregion

		#region ================== Methods

		// This cleans up anything we used for this mode
		private void CleanUp()
		{
			BuilderPlug.VPO.Stop();
			
			if(!string.IsNullOrEmpty(tempfile))
			{
				File.Delete(tempfile);
				tempfile = null;
			}

			if(graphics != null)
			{
				graphics.Dispose();
				graphics = null;
			}

			if(image != null)
			{
				image.Dispose();
				image = null;
			}

			if(canvas != null)
			{
				canvas.Dispose();
				canvas = null;
			}

			tiles.Clear();
		}

		// This returns the tile position for the given map coordinate
		private Point TileForPoint(int x, int y)
		{
			return new Point(x / Tile.TILE_SIZE * Tile.TILE_SIZE, y / Tile.TILE_SIZE * Tile.TILE_SIZE);
		}

		// This draws all tiles on the image
		private void RedrawAllTiles()
		{
			graphics.Clear(Color.Black);
			foreach(Tile t in tiles.Values)
			{
				Vector2D lb = Renderer.MapToDisplay(new Vector2D(t.Position.X, t.Position.Y - 0.5f));
				Vector2D rt = Renderer.MapToDisplay(new Vector2D(t.Position.X + Tile.TILE_SIZE + 0.5f, t.Position.Y + Tile.TILE_SIZE));
				Point plb = new Point((int)lb.x, (int)lb.y);
				Point prt = new Point((int)rt.x, (int)rt.y);
				graphics.DrawImage(t.Bitmap, plb.X, prt.Y, prt.X - plb.X, plb.Y - prt.Y);
			}
			graphics.Flush();
			image.ReleaseTexture();
			image.CreateTexture();
		}

		#endregion

		#region ================== Events

		// Mode starts
		public override void OnEngage()
		{
			base.OnEngage();

			CleanUp();

			// Determine map boundary
			mapbounds = Rectangle.Round(MapSet.CreateArea(General.Map.Map.Vertices));

			// Make an image to draw on.
			// The BitmapImage for Doom Builder's resources must be Format32bppArgb and NOT using color correction,
			// otherwise DB will make a copy of the bitmap when LoadImage() is called! This is normally not a problem,
			// but we want to keep drawing to the same bitmap.
			canvas = new Bitmap(General.Interface.Display.ClientSize.Width, General.Interface.Display.ClientSize.Height, PixelFormat.Format32bppArgb);
			graphics = Graphics.FromImage(canvas);
			graphics.CompositingQuality = CompositingQuality.AssumeLinear;
			graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			graphics.SmoothingMode = SmoothingMode.None;
			graphics.PixelOffsetMode = PixelOffsetMode.None;
			graphics.Clear(Color.Black);
			image = new BitmapImage(canvas, "_CANVAS_");
			image.UseColorCorrection = false;
			image.LoadImage();
			image.CreateTexture();

			// Make custom presentation
			CustomPresentation p = new CustomPresentation();
			p.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.None, 1f, false));
			p.AddLayer(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			p.AddLayer(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1f, true));
			renderer.SetPresentation(p);

			// Export the current map to a temporary WAD file
			tempfile = BuilderPlug.MakeTempFilename(".wad");
			General.Map.ExportToFile(tempfile);
			
			// Load the map in VPO_DLL
			BuilderPlug.VPO.Start(tempfile, General.Map.Options.LevelName);

			// Make sure tiles are created for the current view.
			// This also queues new list of points.
			OnViewChanged();

			// Setup processing
			nextupdate = DateTime.Now + new TimeSpan(0, 0, 1);
			General.Interface.EnableProcessing();
		}

		// Mode ends
		public override void OnDisengage()
		{
			CleanUp();
			General.Interface.DisableProcessing();
			base.OnDisengage();
		}

		// View position/scale changed!
		protected override void OnViewChanged()
		{
			base.OnViewChanged();

			// Determine viewport rectangle in map space
			Vector2D mapleftbot = Renderer.DisplayToMap(new Vector2D(0f, 0f));
			Vector2D maprighttop = Renderer.DisplayToMap(new Vector2D(General.Interface.Display.ClientSize.Width, General.Interface.Display.ClientSize.Height));
			Rectangle mapviewrect = new Rectangle((int)mapleftbot.x, (int)maprighttop.y, (int)maprighttop.x - (int)mapleftbot.x, (int)mapleftbot.y - (int)maprighttop.y);
			
			// Forget tiles that are outside viewport rectangle
			List<Point> tilepoints = new List<Point>(tiles.Keys);
			foreach(Point p in tilepoints)
			{
				Rectangle prect = new Rectangle(p, new Size(Tile.TILE_SIZE, Tile.TILE_SIZE));
				if(!mapviewrect.IntersectsWith(prect)) tiles.Remove(p);
			}

			// Create tiles for all points inside the viewport rectangle
			Point lt = TileForPoint(mapviewrect.Left - Tile.TILE_SIZE, mapviewrect.Top - Tile.TILE_SIZE);
			Point rb = TileForPoint(mapviewrect.Right + Tile.TILE_SIZE, mapviewrect.Bottom + Tile.TILE_SIZE);
			Rectangle tilesrect = new Rectangle(lt.X, lt.Y, rb.X - lt.X, rb.Y - lt.Y);
			tilesrect.Intersect(mapbounds);
			for(int x = tilesrect.X; x <= tilesrect.Right; x += Tile.TILE_SIZE)
			{
				for(int y = tilesrect.Y; y <= tilesrect.Bottom; y += Tile.TILE_SIZE)
				{
					Point p = new Point(x, y);
					if(!tiles.ContainsKey(p)) tiles.Add(p, new Tile(p));
				}
			}

			RedrawAllTiles();
		}

		// Draw the display
		public override void OnRedrawDisplay()
		{
			base.OnRedrawDisplay();

			// Clear the overlay and begin rendering to it.
			if(renderer.StartOverlay(true))
			{
				// Rectangle of coordinates where to draw the image.
				// We use untranslated coordinates: this means the coordinates here
				// are already in screen space.
				RectangleF r = General.Interface.Display.ClientRectangle;

				// Show the picture!
				renderer.RenderRectangleFilled(r, PixelColor.FromColor(Color.White), false, image);

				// Finish our rendering to this layer.
				renderer.Finish();
			}

			// Render lines and vertices
			if(renderer.StartPlotter(true))
			{
				renderer.PlotLinedefSet(General.Map.Map.Linedefs);
				renderer.PlotVerticesSet(General.Map.Map.Vertices);
				renderer.Finish();
			}
			renderer.Present();
		}

		// Processing
		public override void OnProcess(double deltatime)
		{
			base.OnProcess(deltatime);
			if(DateTime.Now <= nextupdate)
			{
				// Get the processed points from the VPO manager
				List<PointData> points = new List<PointData>();
				int pointsleft = BuilderPlug.VPO.DequeueResults(points);
				if(pointsleft < (VPOManager.POINTS_PER_ITERATION * BuilderPlug.VPO.NumThreads) * 100)
				{
				}
				
				nextupdate = DateTime.Now + new TimeSpan(0, 0, 1);
			}
		}

		#endregion
	}
}
