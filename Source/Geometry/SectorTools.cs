
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
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using SlimDX.Direct3D;
using System.Drawing;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Geometry
{
	/// <summary>
	/// This makes a sector from all surrounding lines from a given coordinate.
	/// Automatically finds the sidedef/sector properties from surrounding sectors/sidedefs.
	/// </summary>
	public static class SectorTools
	{
		#region ================== Constants

		#endregion

		#region ================== Pathfinding

		/// <summary>
		/// This finds a potential sector at the given coordinates,
		/// or returns null when a sector is not possible there.
		/// </summary>
		public static List<LinedefSide> FindPotentialSectorAt(Vector2D pos)
		{
			// Find the nearest line and determine side, then use the other method to create the sector
			Linedef l = General.Map.Map.NearestLinedef(pos);
			return FindPotentialSectorAt(l, (l.SideOfLine(pos) <= 0));
		}

		/// <summary>
		/// This finds a potential sector starting at the given line and side,
		/// or returns null when sector is not possible.
		/// </summary>
		public static List<LinedefSide> FindPotentialSectorAt(Linedef line, bool front)
		{
			List<LinedefSide> alllines = new List<LinedefSide>();
			
			// Find the outer lines
			Polygon p = FindOuterLines(line, front, alllines);
			if(p != null)
			{
				// Find the inner lines
				FindInnerLines(p, alllines);
				return alllines;
			}
			else
				return null;
		}

		// This finds the inner lines of the sector and adds them to the sector polygon
		private static void FindInnerLines(Polygon p, List<LinedefSide> alllines)
		{
			Vertex foundv;
			bool vvalid, findmore;
			Linedef foundline;
			float foundangle = 0f;
			bool foundlinefront;
			
			do
			{
				findmore = false;

				// Go for all vertices to find the right-most vertex inside the polygon
				foundv = null;
				foreach(Vertex v in General.Map.Map.Vertices)
				{
					// More to the right?
					if((foundv == null) || (v.Position.x >= foundv.Position.x))
					{
						// Vertex is inside the polygon?
						if(p.Intersect(v.Position))
						{
							// Vertex has lines attached?
							if(v.Linedefs.Count > 0)
							{
								// Go for all lines to see if the vertex is not of the polygon itsself
								vvalid = true;
								foreach(LinedefSide ls in alllines)
								{
									if((ls.Line.Start == v) || (ls.Line.End == v))
									{
										vvalid = false;
										break;
									}
								}

								// Valid vertex?
								if(vvalid) foundv = v;
							}
						}
					}
				}

				// Found a vertex inside the polygon?
				if(foundv != null)
				{
					// Find the attached linedef with the smallest angle to the right
					float targetangle = Angle2D.PIHALF;
					foundline = null;
					foreach(Linedef l in foundv.Linedefs)
					{
						// We need an angle unrelated to line direction, so correct for that
						float lineangle = l.Angle;
						if(l.End == foundv) lineangle += Angle2D.PI;

						// Better result?
						float deltaangle = Angle2D.Difference(targetangle, lineangle);
						if((foundline == null) || (deltaangle < foundangle))
						{
							foundline = l;
							foundangle = deltaangle;
						}
					}

					// We already know that each linedef will go from this vertex
					// to the left, because this is the right-most vertex in this area.
					// If the line would go to the right, that means the other vertex of
					// that line must lie outside this area and the mapper made an error.
					// Should I check for this error and fail to create a sector in
					// that case or ignore it and create a malformed sector (possibly
					// breaking another sector also)?

					// Find the side at which to start pathfinding
					Vector2D testpos = new Vector2D(100.0f, 0.0f);
					foundlinefront = (foundline.SideOfLine(foundv.Position + testpos) < 0.0f);

					// Find inner path
					List<LinedefSide> innerlines = FindClosestPath(foundline, foundlinefront, true);
					if(innerlines != null)
					{
						// Make polygon
						LinedefTracePath tracepath = new LinedefTracePath(innerlines);
						Polygon innerpoly = tracepath.MakePolygon();

						// Check if the front of the line is outside the polygon
						if(!innerpoly.Intersect(foundline.GetSidePoint(foundlinefront)))
						{
							// Valid hole found!
							alllines.AddRange(innerlines);
							p.InsertChild(innerpoly);
							findmore = true;
						}
					}
				}
			}
			// Continue until no more holes found
			while(findmore);
		}

		// This finds the outer lines of the sector as a polygon
		// Returns null when no valid outer polygon can be found
		private static Polygon FindOuterLines(Linedef line, bool front, List<LinedefSide> alllines)
		{
			Linedef scanline = line;
			bool scanfront = front;

			do
			{
				// Find closest path
				List<LinedefSide> pathlines = FindClosestPath(scanline, scanfront, true);
				if(pathlines != null)
				{
					// Make polygon
					LinedefTracePath tracepath = new LinedefTracePath(pathlines);
					Polygon poly = tracepath.MakePolygon();

					// Check if the front of the line is inside the polygon
					if(poly.Intersect(line.GetSidePoint(front)))
					{
						// Outer lines found!
						alllines.AddRange(pathlines);
						return poly;
					}
					else
					{
						// Inner lines found. This is not what we need, we want the outer lines.
						// Find the right-most vertex to start a scan from there towards the outer lines.
						Vertex foundv = null;
						foreach(LinedefSide ls in pathlines)
						{
							if((foundv == null) || (ls.Line.Start.Position.x > foundv.Position.x))
								foundv = ls.Line.Start;
							
							if((foundv == null) || (ls.Line.End.Position.x > foundv.Position.x))
								foundv = ls.Line.End;
						}

						// If foundv is null then something is horribly wrong with the
						// path we received from FindClosestPath!
						if(foundv == null) throw new Exception("FAIL!");
						
						// From the right-most vertex trace outward to the right to
						// find the next closest linedef, this is based on the idea that
						// all sectors are closed.
						Vector2D lineoffset = new Vector2D(100.0f, 0.0f);
						Line2D testline = new Line2D(foundv.Position, foundv.Position + lineoffset);
						scanline = null;
						float foundu = float.MaxValue;
						foreach(Linedef ld in General.Map.Map.Linedefs)
						{
							// Line to the right of start point?
							if((ld.Start.Position.x > foundv.Position.x) ||
							   (ld.End.Position.x > foundv.Position.x))
							{
								// Line intersecting the y axis?
								if( !((ld.Start.Position.y > foundv.Position.y) &&
									  (ld.End.Position.y > foundv.Position.y)) &&
								    !((ld.Start.Position.y < foundv.Position.y) &&
									  (ld.End.Position.y < foundv.Position.y)))
								{
									// Check if this linedef intersects our test line at a closer range
									float thisu;
									ld.Line.GetIntersection(testline, out thisu);
									if((thisu > 0.00001f) && (thisu < foundu) && !float.IsNaN(thisu))
									{
										scanline = ld;
										foundu = thisu;
									}
								}
							}
						}

						// Did we meet another line?
						if(scanline != null)
						{
							// Determine on which side we should start the next pathfind
							scanfront = (scanline.SideOfLine(foundv.Position) < 0.0f);
						}
						else
						{
							// Appearently we reached the end of the map, no sector possible here
							return null;
						}
					}
				}
				else
				{
					// Can't find a path
					return null;
				}
			}
			while(true);
		}
		
		/// <summary>
		/// This finds the closest path from the beginning of a line to the end of the line.
		/// When turnatends is true, the algorithm will continue at the other side of the
		/// line when a dead end has been reached. Returns null when no path could be found.
		/// </summary>
		public static List<LinedefSide> FindClosestPath(Linedef start, bool front, bool turnatends)
		{
			List<LinedefSide> path = new List<LinedefSide>();
			Dictionary<Linedef, int> tracecount = new Dictionary<Linedef, int>();
			Linedef nextline = start;
			bool nextfront = front;

			do
			{
				// Add line to path
				path.Add(new LinedefSide(nextline, nextfront));
				if(!tracecount.ContainsKey(nextline)) tracecount.Add(nextline, 1); else tracecount[nextline]++;

				// Determine next vertex to use
				Vertex v = nextfront ? nextline.End : nextline.Start;

				// Get list of linedefs and sort by angle
				List<Linedef> lines = new List<Linedef>(v.Linedefs);
				LinedefAngleSorter sorter = new LinedefAngleSorter(nextline, nextfront, v);
				lines.Sort(sorter);

				// Source line is the only one?
				if(lines.Count == 1)
				{
					// Are we allowed to trace along this line again?
					if(turnatends && (!tracecount.ContainsKey(nextline) || (tracecount[nextline] < 3)))
					{
						// Turn around and go back along the other side of the line
						nextfront = !nextfront;
					}
					else
					{
						// No more lines, trace ends here
						path = null;
					}
				}
				else
				{
					// Trace along the next line
					Linedef prevline = nextline;
					if(lines[0] == nextline) nextline = lines[1]; else nextline = lines[0];

					// Are we allowed to trace this line again?
					if(!tracecount.ContainsKey(nextline) || (tracecount[nextline] < 3))
					{
						// Check if front side changes
						if((prevline.Start == nextline.Start) ||
						   (prevline.End == nextline.End)) nextfront = !nextfront;
					}
					else
					{
						// No more lines, trace ends here
						path = null;
					}
				}
			}
			// Continue as long as we have not reached the start yet
			// or we have no next line to trace
			while((path != null) && ((nextline != start) || (nextfront != front)));

			// Return path (null when trace failed)
			return path;
		}

		#endregion
		
		#region ================== Sector Making

		// This makes the sector from the given lines and sides
		public static Sector MakeSector(List<LinedefSide> alllines)
		{
			Sidedef source = null;
			Sector newsector = General.Map.Map.CreateSector();

			// Check if any of the sides already has a sidedef
			// Then we use information from that sidedef to make the others
			foreach(LinedefSide ls in alllines)
			{
				if(ls.Front)
				{
					if(ls.Line.Front != null)
					{
						source = ls.Line.Front;
						source.Sector.CopyPropertiesTo(newsector);
						break;
					}
				}
				else
				{
					if(ls.Line.Back != null)
					{
						source = ls.Line.Back;
						source.Sector.CopyPropertiesTo(newsector);
						break;
					}
				}
			}

			// If we couldn't find anything, try the other sides
			if(source == null)
			{
				foreach(LinedefSide ls in alllines)
				{
					if(ls.Front)
					{
						if(ls.Line.Back != null)
						{
							source = ls.Line.Back;
							source.Sector.CopyPropertiesTo(newsector);
							break;
						}
					}
					else
					{
						if(ls.Line.Front != null)
						{
							source = ls.Line.Front;
							source.Sector.CopyPropertiesTo(newsector);
							break;
						}
					}
				}
			}

			// Go for all sides to make sidedefs
			foreach(LinedefSide ls in alllines)
			{
				if(ls.Front)
				{
					// Create sidedef is needed and ensure it points to the new sector
					if(ls.Line.Front == null) General.Map.Map.CreateSidedef(ls.Line, true, newsector);
					if(ls.Line.Front.Sector != newsector) ls.Line.Front.ChangeSector(newsector);
					if(source != null) source.CopyPropertiesTo(ls.Line.Front); else source = ls.Line.Front;
				}
				else
				{
					// Create sidedef is needed and ensure it points to the new sector
					if(ls.Line.Back == null) General.Map.Map.CreateSidedef(ls.Line, false, newsector);
					if(ls.Line.Back.Sector != newsector) ls.Line.Back.ChangeSector(newsector);
					if(source != null) source.CopyPropertiesTo(ls.Line.Back); else source = ls.Line.Back;
				}

				// Update line
				ls.Line.ApplySidedFlags();
			}

			// Return the new sector
			return newsector;
		}


		// This joins a sector with the given lines and sides
		public static Sector JoinSector(List<LinedefSide> alllines, Sidedef original)
		{
			// Go for all sides to make sidedefs
			foreach(LinedefSide ls in alllines)
			{
				if(ls.Front)
				{
					// Create sidedef is needed and ensure it points to the new sector
					if(ls.Line.Front == null)
					{
						General.Map.Map.CreateSidedef(ls.Line, true, original.Sector);
						ls.Line.ApplySidedFlags();
					}
					original.CopyPropertiesTo(ls.Line.Front);
				}
				else
				{
					// Create sidedef is needed and ensure it points to the new sector
					if(ls.Line.Back == null)
					{
						General.Map.Map.CreateSidedef(ls.Line, false, original.Sector);
						ls.Line.ApplySidedFlags();
					}
					original.CopyPropertiesTo(ls.Line.Back);
				}
			}

			// Return the new sector
			return original.Sector;
		}

		#endregion
	}
}
