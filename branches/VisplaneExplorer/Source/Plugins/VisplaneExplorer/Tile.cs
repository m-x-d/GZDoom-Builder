#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;

#endregion

namespace CodeImp.DoomBuilder.Plugins.VisplaneExplorer
{
	// This is a 64x64 tile in map space which holds the point data results.
	internal class Tile
	{
		// Constants
		public const int TILE_SIZE = 64;
		
		// Members
		private Point position;
		private TileData[][] points;
		private Bitmap bmp;
		
		// Properties
		public Point Position { get { return position; } }
		public Bitmap Bitmap { get { return bmp; } }

		// Constructor
		public Tile(Point lefttoppos)
		{
			position = lefttoppos;
			
			// Make the jagged array
			// I use a jagged array because, allegedly, it performs better than a multidimensional array.
			points = new TileData[TILE_SIZE][];
			for(int y = 0; y < TILE_SIZE; y++)
				points[y] = new TileData[TILE_SIZE];

			bmp = new Bitmap(TILE_SIZE, TILE_SIZE, PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(bmp);
			Random rnd = new Random(lefttoppos.X ^ lefttoppos.Y ^ DateTime.Now.Millisecond);
			Color testcolor = Color.FromArgb(255, rnd.Next(25, 255), rnd.Next(25, 255), rnd.Next(25, 255));
			g.Clear(testcolor);
			g.DrawLine(Pens.White, new Point(0, 60), new Point(60, 60));
			g.Flush();
			g.Dispose();
		}

		// This receives a processed point
		public void StorePointData(PointData pd)
		{
			TileData t;
			t.visplanes = (byte)Math.Min(pd.visplanes, 255);
			t.drawsegs = (byte)Math.Min(pd.drawsegs, 255);
			t.solidsegs = (byte)Math.Min(pd.solidsegs, 255);
			t.openings = (byte)Math.Min(pd.openings, 255);
			int x = pd.x - position.X;
			int y = pd.y - position.Y;
			points[y][x] = t;
			Color c = Color.FromArgb(255, Math.Min(t.visplanes * 5, 255), Math.Min(t.visplanes * 2, 255), 0);
			bmp.SetPixel(x, y, c);
		}

		// Returns a position by index
		public Point PointByIndex(int index)
		{
			#if DEBUG
			if(index > (TILE_SIZE * TILE_SIZE)) throw new ArgumentOutOfRangeException("index");
			#endif

			Point p = new Point();

			// this is a "butterfly" style sequence, which begins like:
			//    ( 0  0)  (32 32)  ( 0 32)  (32  0)
			//    (16 16)  (48 48)  (16 48)  (48 16)
			//    ( 0 16)  (32 48)  ( 0 48)  (32 16)
			//    (16  0)  (48 32)  (16 32)  (48  0)
			//    ( 8  8)  (40 40)  ( 8 40)  (40  8)
			//    etc....

			p.X = (index & 1) << 5;
			p.Y = (((index >> 1) ^ index) & 1) << 5;

			index >>= 2;
			p.X += (index & 1) << 4;
			p.Y += (((index >> 1) ^ index) & 1) << 4;

			index >>= 2;
			p.X |= (index & 1) << 3;
			p.Y |= (((index >> 1) ^ index) & 1) << 3;

			index >>= 2;
			p.X |= (index & 1) << 2;
			p.Y |= (((index >> 1) ^ index) & 1) << 2;

			index >>= 2;
			p.X |= (index & 1) << 1;
			p.Y |= (((index >> 1) ^ index) & 1) << 1;

			index >>= 2;
			p.X |= index & 1;
			p.Y |= ((index >> 1) ^ index) & 1;

			return p;
		}

		// Return the tile data nearest to x/y
		public TileData GetNearestPoint(int x, int y)
		{
			while(true)
			{
				TileData p = points[x][y];
				if(p.visplanes > 0) return p;

				// Move coordinate a step closer to (0,0)
				// NOTE: if the 64x64 size is changes, this will need more/less stages

				int xy = x | y;

				if((xy & 1) != 0)
				{
					x &= ~1; y &= ~1;
				}
				else if((xy & 2) != 0)
				{
					x &= ~2; y &= ~2;
				}
				else if((xy & 4) != 0)
				{
					x &= ~4; y &= ~4;
				}
				else if((xy & 8) != 0)
				{
					x &= ~8; y &= ~8;
				}
				else if((xy & 16) != 0)
				{
					x &= ~16; y &= ~16;
				}
				else
				{
					return points[0][0];
				}
			}
		}
	}
}
