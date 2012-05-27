#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
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
		
		#region ================== APIs

		[DllImport("kernel32.dll")]
		static extern void RtlZeroMemory(IntPtr dst, int length);

		#endregion

		#region ================== Variables

		// The image is the ImageData resource for Doom Builder to work with
		private BitmapImage image;
		
		// This is the bitmap that we will be drawing on
		private Bitmap canvas;

		// Temporary WAD file written for the vpo.dll library
		private string tempfile;

		// Rectangle around the map
		private Rectangle mapbounds;

		// 64x64 tiles in map space. These are discarded when outside view.
		private Dictionary<Point, Tile> tiles = new Dictionary<Point, Tile>();

		// Time when to do another update
		private DateTime nextupdate;

		// Are we processing?
		private bool processingenabled;
		
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
			base.Dispose();
		}

		#endregion

		#region ================== Methods

		// This cleans up anything we used for this mode
		private void CleanUp()
		{
			BuilderPlug.VPO.Stop();

			if(processingenabled)
			{
				General.Interface.DisableProcessing();
				processingenabled = false;
			}
			
			if(!string.IsNullOrEmpty(tempfile))
			{
				File.Delete(tempfile);
				tempfile = null;
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
		private Point TileForPoint(float x, float y)
		{
			return new Point((int)Math.Floor(x / (float)Tile.TILE_SIZE) * Tile.TILE_SIZE, (int)Math.Floor(y / (float)Tile.TILE_SIZE) * Tile.TILE_SIZE);
		}

		// This draws all tiles on the image
		private unsafe void RedrawAllTiles()
		{
			if(canvas == null) return;
			
			Size canvassize = canvas.Size;
			BitmapData bd = canvas.LockBits(new Rectangle(0, 0, canvassize.Width, canvassize.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			RtlZeroMemory(bd.Scan0, bd.Width * bd.Height * 4);
			int* p = (int*)bd.Scan0.ToInt32();

			foreach(Tile t in tiles.Values)
			{
				// Map this tile to screen space
				Vector2D lb = Renderer.MapToDisplay(new Vector2D(t.Position.X, t.Position.Y));
				Vector2D rt = Renderer.MapToDisplay(new Vector2D(t.Position.X + Tile.TILE_SIZE, t.Position.Y + Tile.TILE_SIZE));

				// Make sure the coordinates are aligned with pixels
				float x1 = (float)Math.Round(lb.x);
				float x2 = (float)Math.Round(rt.x);
				float y1 = (float)Math.Round(rt.y);
				float y2 = (float)Math.Round(lb.y);
				float w = x2 - x1;
				float h = y2 - y1;
				float winv = 1f / w;
				float hinv = 1f / h;
				
				// Draw all pixels within this tile
				int screenx = (int)x1;
				for(float x = 0; x < w; x++, screenx++)
				{
					int screeny = (int)y1;
					for(float y = 0; y < h; y++, screeny++)
					{
						// TODO: Clip before loop!
						if((screenx >= 0) && (screenx < bd.Width) && (screeny >= 0) && (screeny < bd.Height))
						{
							// Calculate the relative offset in map coordinates for this pixel
							float ux = x * winv * Tile.TILE_SIZE;
							float uy = y * hinv * Tile.TILE_SIZE;

							// Get the data and apply the color
							TileData td = t.GetNearestPoint((int)ux, Tile.TILE_SIZE - 1 - (int)uy);
							Color c = Color.FromArgb(255, Math.Min(td.visplanes * 5, 255), Math.Min(td.visplanes * 3, 255), 0);
							p[screeny * bd.Width + screenx] = c.ToArgb();
						}
					}
				}
			}
			
			canvas.UnlockBits(bd);
			image.ReleaseTexture();
			image.CreateTexture();
		}

		// This queues points for all current tiles
		private void QueuePoints(int pointsleft)
		{
			while(pointsleft < (VPOManager.POINTS_PER_ITERATION * BuilderPlug.VPO.NumThreads * 10))
			{
				List<Point> newpoints = new List<Point>(tiles.Count);
				foreach(Tile t in tiles.Values)
					if(!t.IsComplete) newpoints.Add(t.GetNextPoint());
				if(newpoints.Count == 0) break;
				pointsleft = BuilderPlug.VPO.EnqueuePoints(newpoints);
			}
		}

		#endregion

		#region ================== Events

		// Mode starts
		public override void OnEngage()
		{
			base.OnEngage();

			CleanUp();

			// Export the current map to a temporary WAD file
			tempfile = BuilderPlug.MakeTempFilename(".wad");
			General.Map.ExportToFile(tempfile);
			
			// Load the map in VPO_DLL
			BuilderPlug.VPO.Start(tempfile, General.Map.Options.LevelName);

			// Determine map boundary
			mapbounds = Rectangle.Round(MapSet.CreateArea(General.Map.Map.Vertices));

			// Create tiles for current view and queue points to process
			OnViewChanged();

			// Make an image to draw on.
			// The BitmapImage for Doom Builder's resources must be Format32bppArgb and NOT using color correction,
			// otherwise DB will make a copy of the bitmap when LoadImage() is called! This is normally not a problem,
			// but we want to keep drawing to the same bitmap.
			canvas = new Bitmap(General.Interface.Display.ClientSize.Width, General.Interface.Display.ClientSize.Height, PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(canvas);
			g.Clear(Color.Black);
			g.Dispose();
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

			// Setup processing
			nextupdate = DateTime.Now + new TimeSpan(0, 0, 0, 0, 100);
			General.Interface.EnableProcessing();
			processingenabled = true;
		}

		// Mode ends
		public override void OnDisengage()
		{
			CleanUp();
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
			QueuePoints(BuilderPlug.VPO.GetRemainingPoints());

			// Update the screen sooner
			nextupdate = DateTime.Now + new TimeSpan(0, 0, 0, 0, 100);
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
			if(DateTime.Now >= nextupdate)
			{
				// Get the processed points from the VPO manager
				List<PointData> points = new List<PointData>();
				int pointsleft = BuilderPlug.VPO.DequeueResults(points);

				// Queue more points if needed
				QueuePoints(pointsleft);

				// Apply the points to the tiles
				foreach(PointData pd in points)
				{
					Tile t;
					Point tp = TileForPoint(pd.x, pd.y);
					if(tiles.TryGetValue(tp, out t))
						t.StorePointData(pd);
				}

				// Redraw
				RedrawAllTiles();
				General.Interface.RedrawDisplay();

				nextupdate = DateTime.Now + new TimeSpan(0, 0, 0, 0, 600);
			}
			else
			{
				// Queue more points if needed
				QueuePoints(BuilderPlug.VPO.GetRemainingPoints());
			}
		}

		#endregion
	}
}
