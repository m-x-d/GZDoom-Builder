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
		public const int STAT_VOID = 254;
		public const int STAT_OVERFLOW = 255;
		public static readonly int[] STATS_COMPRESSOR = new int[] { 1, 2, 1, 160 };
		public static readonly int[] STATS_LIMITS = new int[] { 128, 256, 32, 320 * 64 };
		
		// Members
		private Point position;
		private TileData[][] points;
		private int nextindex;
		private int pointsreceived;
		
		// Properties
		public Point Position { get { return position; } }
		public bool IsComplete { get { return nextindex == (TILE_SIZE * TILE_SIZE); } }

		// Constructor
		public Tile(Point lefttoppos)
		{
			position = lefttoppos;
		}

		// This receives a processed point
		public unsafe void StorePointData(PointData pd)
		{
			pointsreceived++;
			
			if(points == null)
			{
				// Don't allocate memory for void tiles
				if(pd.result == PointResult.Void) return;
				
				// Make the jagged array
				// I use a jagged array because, allegedly, it performs better than a multidimensional array.
				points = new TileData[TILE_SIZE][];
				for(int y = 0; y < TILE_SIZE; y++)
					points[y] = new TileData[TILE_SIZE];
				
				// Fill previously received points with void
				for(int i = 0; i < pointsreceived - 1; i++)
				{
					Point p = PointByIndex(i);
					points[p.Y][p.X] = TileData.VoidTile;
				}

				// We have to get all points for this tile again,
				// this causes a bit of overhead, but not really noticable.
				nextindex = 0;
			}
			
			TileData t;
			switch(pd.result)
			{
				case PointResult.OK:
					t.stats[(int)ViewStats.Visplanes] = (byte)Math.Min((pd.visplanes + (STATS_COMPRESSOR[(int)ViewStats.Visplanes] - 1)) / STATS_COMPRESSOR[(int)ViewStats.Visplanes], 255);
					t.stats[(int)ViewStats.Drawsegs] = (byte)Math.Min((pd.drawsegs + (STATS_COMPRESSOR[(int)ViewStats.Drawsegs] - 1)) / STATS_COMPRESSOR[(int)ViewStats.Drawsegs], 255);
					t.stats[(int)ViewStats.Solidsegs] = (byte)Math.Min((pd.solidsegs + (STATS_COMPRESSOR[(int)ViewStats.Solidsegs] - 1)) / STATS_COMPRESSOR[(int)ViewStats.Solidsegs], 255);
					t.stats[(int)ViewStats.Openings] = (byte)Math.Min((pd.openings + (STATS_COMPRESSOR[(int)ViewStats.Openings] - 1)) / STATS_COMPRESSOR[(int)ViewStats.Openings], 255);
					break;

				case PointResult.BadZ:
					t.stats[(int)ViewStats.Visplanes] = 1;
					t.stats[(int)ViewStats.Drawsegs] = 0;
					t.stats[(int)ViewStats.Solidsegs] = 0;
					t.stats[(int)ViewStats.Openings] = 0;
					break;
					
				case PointResult.Void:
					t.stats[(int)ViewStats.Visplanes] = STAT_VOID;
					t.stats[(int)ViewStats.Drawsegs] = STAT_VOID;
					t.stats[(int)ViewStats.Solidsegs] = STAT_VOID;
					t.stats[(int)ViewStats.Openings] = STAT_VOID;
					break;

				case PointResult.Overflow:
					t.stats[(int)ViewStats.Visplanes] = STAT_OVERFLOW;
					t.stats[(int)ViewStats.Drawsegs] = STAT_OVERFLOW;
					t.stats[(int)ViewStats.Solidsegs] = STAT_OVERFLOW;
					t.stats[(int)ViewStats.Openings] = STAT_OVERFLOW;
					break;

				default:
					throw new NotImplementedException();
			}

			points[pd.y - position.Y][pd.x - position.X] = t;
		}

		// This returns the next point to process
		public Point GetNextPoint()
		{
			Point p = PointByIndex(nextindex++);
			p.X += position.X;
			p.Y += position.Y;
			return p;
		}

		// Returns a position by index
		public Point PointByIndex(int index)
		{
			#if DEBUG
			if(index > (TILE_SIZE * TILE_SIZE))
				throw new IndexOutOfRangeException();
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
		// This method is actually slowing down the rendering quite a lot
		// (about 75% of the time the display is drawn is in this function!!!)
		// We should really think about a faster algorithm for this!
		public unsafe TileData GetNearestPoint(int x, int y)
		{
			if(points == null) return TileData.VoidTile;
			
			#if DEBUG
			if((x < 0) || (x > TILE_SIZE - 1) || (y < 0) || (y > TILE_SIZE - 1))
				throw new IndexOutOfRangeException();
			#endif
			
			while(true)
			{
				TileData p = points[y][x];
				if(p.stats[(int)ViewStats.Visplanes] > 0) return p;

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
