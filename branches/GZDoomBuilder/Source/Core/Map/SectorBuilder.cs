#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	//mxd. Shameless Slade 3 SectorBuilder::SectorBuilder ripoff...
	//TODO: There are lots of overlaps with already existing code.
	//TODO: Replace with existing implementations if results are the same & existing code performs faster
	internal sealed class SectorBuilder
	{
		#region ================== Variables

		private List<LinedefSide> sector_edges;
		private HashSet<Vertex> vertex_valid;

		// Current outline
		private List<LinedefSide> o_edges;
		private bool o_clockwise;
		private RectangleF o_bbox;
		private Vertex vertex_right;

		#endregion

		#region ================== Properties

		public List<LinedefSide> SectorEdges { get { return sector_edges; } }

		#endregion

		#region ================== Constructor

		public SectorBuilder()
		{
			sector_edges = new List<LinedefSide>();
			vertex_valid = new HashSet<Vertex>();
			o_edges = new List<LinedefSide>();
		}

		#endregion

		#region ================== Methods

		///<summary>Traces all edges to build a closed sector starting from [line]</summary>
		internal bool TraceSector(Linedef line, bool front)
		{
			if(line == null) return false;

			//DebugConsole.WriteLine(" ");
			//DebugConsole.WriteLine("TraceSector for line " + line.Index + (front ? " (front)" : " (back)"));

			// Init
			sector_edges.Clear();

			// Create valid vertices list
			vertex_valid = new HashSet<Vertex>(General.Map.Map.Vertices);

			// Find outmost outline
			for(int a = 0; a < 10000; a++)
			{
				// Trace outline
				if(!TraceOutline(line, front))
				{
					//DebugConsole.WriteLine("TraceSector: find outmost outline failed");
					break;
				}

				// Discard any vertices outside the traced outline
				vertex_valid.RemoveWhere(PointOutsideOutline);
				//DebugConsole.WriteLine("vertex_valid: " + vertex_valid.Count + " verts after RemoveWhere");

				// If it is clockwise, we've found the outmost outline
				if(o_clockwise)
				{
					//DebugConsole.WriteLine("TraceSector: found outmost outline");
					break;
				}

				// Otherwise, find the next edge outside the outline
				LinedefSide next = FindOuterEdge();

				// If none was found, we're outside the map
				if(next == null)
				{
					//DebugConsole.WriteLine("TraceSector aborted: no outer edge");
					return false;
				}

				// Repeat with this edge
				line = next.Line;
				front = next.Front;
			}

			//DebugConsole.WriteLine("FindOuterEdge: " + o_edges.Count + " lines");

			// Trace all inner outlines, by tracing from the rightmost vertex
			// until all vertices have been discarded
			for(int a = 0; a < 10000; a++)
			{
				// Get inner edge
				LinedefSide edge = FindInnerEdge();

				// Check if we're done
				if(edge == null)
				{
					//DebugConsole.WriteLine("No inner edge found (no edge)");
					break;
				}

				// Trace outline from edge
				if(!TraceOutline(edge.Line, edge.Front))
				{
					//DebugConsole.WriteLine("No inner edge found (TraceOutline failed)");
					break;
				}

				// Discard any vertices outside the traced outline
				vertex_valid.RemoveWhere(PointOutsideOutline);
				//DebugConsole.WriteLine("vertex_valid: " + vertex_valid.Count + " verts after RemoveWhere");
			}

			//DebugConsole.WriteLine("FindInnerEdge: " + o_edges.Count + " lines");
			return true;
		}

		///<summary>Traces the sector outline from lines beginning at [line],
		/// on either the front or back side ([front])</summary>
		private bool TraceOutline(Linedef line, bool front)
		{
			// Check line was given
			if(line == null) return false;

			//DebugConsole.WriteLine(" ");
			//DebugConsole.WriteLine("Tracing line " + line.Index + (front ? " (front)" : " (back)"));

			// Init outline
			o_edges.Clear();
			o_bbox = RectangleF.Empty;
			LinedefSide start = new LinedefSide(line, front);
			o_edges.Add(start);
			int edge_sum = 0;
			Dictionary<Linedef, int> visited_lines = new Dictionary<Linedef, int>();

			// Begin tracing
			LinedefSide edge = new LinedefSide(line, front);
			vertex_right = edge.Line.Start;
			for(int a = 0; a < 10000; a++)
			{
				// Update edge sum (for clockwise detection)
				if(edge.Front)
					edge_sum += (int)(edge.Line.Start.Position.x * edge.Line.End.Position.y - edge.Line.End.Position.x * edge.Line.Start.Position.y);
				else
					edge_sum += (int)(edge.Line.End.Position.x * edge.Line.Start.Position.y - edge.Line.Start.Position.x * edge.Line.End.Position.y);

				// Update rightmost vertex
				if(edge.Line.Start.Position.x > vertex_right.Position.x)
					vertex_right = edge.Line.Start;
				if(edge.Line.End.Position.x > vertex_right.Position.x)
					vertex_right = edge.Line.End;

				// Get next edge. If no valid next edge was found, go back along the current line
				LinedefSide edge_next = (NextEdge(edge, visited_lines) ?? new LinedefSide(edge.Line, !edge.Front));

				//DebugConsole.WriteLine("Next line for " + edge.Line.Index + (edge.Front ? " (front)" : " (back)") + ": " + edge_next.Line.Index + (edge_next.Front ? " (front)" : " (back)"));

				// Discard edge vertices
				vertex_valid.Remove(edge_next.Line.Start);
				vertex_valid.Remove(edge_next.Line.End);

				//DebugConsole.WriteLine("vertex_valid: " + vertex_valid.Count + " verts after Remove (in TraceOutline)");

				// Check if we're back to the start
				if(edge_next.Line == start.Line && edge_next.Front == start.Front)
					break;

				// Add edge to outline
				o_edges.Add(edge_next);
				edge.Line = edge_next.Line;
				edge.Front = edge_next.Front;

				// Update bounding box
				RectangleF l_bbox = RectangleF.FromLTRB(
					Math.Min(edge.Line.Start.Position.x, edge.Line.End.Position.x),  // left
					Math.Min(edge.Line.Start.Position.y, edge.Line.End.Position.y),  // top
					Math.Max(edge.Line.Start.Position.x, edge.Line.End.Position.x),  // right
					Math.Max(edge.Line.Start.Position.y, edge.Line.End.Position.y)); // bottom

				//mxd. As it turned out, o_bbox.IsEmpty was not what we needed...
				o_bbox = (o_bbox == RectangleF.Empty ? l_bbox : RectangleF.Union(o_bbox, l_bbox));
			}

			// Check if outline is clockwise
			o_clockwise = (edge_sum < 0);

			//DebugConsole.WriteLine("TraceOutline for line " + line.Index + " (" + (front ? "front":"back") + ") found " + o_edges.Count + " edges; o_clockwise=" + o_clockwise);

			// Add outline edges to sector edge list
			sector_edges.AddRange(o_edges);

			// Trace complete
			return true;
		}

		/// <summary>Finds the next closest edge outside of the current outline (that isn't part of the current outline)</summary>
		private LinedefSide FindOuterEdge()
		{
			// Check we have a rightmost vertex
			if(vertex_right == null) return null;

			// Init
			float vr_x = vertex_right.Position.x;
			float vr_y = vertex_right.Position.y;
			float min_dist = float.MaxValue;
			Linedef nearest = null;

			// Fire a ray east from the vertex and find the first line it crosses
			foreach(Linedef line in General.Map.Map.Linedefs)
			{
				// Ignore if the line is completely left of the vertex
				if(line.Start.Position.x <= vr_x && line.End.Position.x <= vr_x) continue;

				// Ignore horizontal lines
				if(line.Start.Position.y == line.End.Position.y) continue;

				// Ignore if the line doesn't intersect the y value
				if((line.Start.Position.y < vr_y && line.End.Position.y < vr_y) ||
				   (line.Start.Position.y > vr_y && line.End.Position.y > vr_y))
					continue;

				// Get x intercept
				float int_frac = (vr_y - line.Start.Position.y) / (line.End.Position.y - line.Start.Position.y);
				float int_x = line.Start.Position.x + ((line.End.Position.x - line.Start.Position.x) * int_frac);
				float dist = Math.Abs(int_x - vr_x);

				// Check if closest
				if(nearest == null || dist < min_dist)
				{
					min_dist = dist;
					nearest = line;
				}
				else if(Math.Abs(dist - min_dist) < 0.001f)
				{
					// In the case of a tie, use the distance to each line as a tiebreaker - this fixes cases where the ray hits a vertex
					// shared by two lines.  Choosing the further line would mean choosing an inner edge, which is clearly wrong.
					float line_dist = line.SafeDistanceToSq(vertex_right.Position, true);
					float nearest_dist = nearest.SafeDistanceToSq(vertex_right.Position, true);
					if(line_dist < nearest_dist)
					{
						min_dist = dist;
						nearest = line;
					}
				}
			}

			// Check for valid line
			if(nearest == null) return null;

			// Determine the edge side
			float side = -nearest.SideOfLine(vertex_right.Position); //mxd. SideOfLine logic is inverted in Slade 3
			return new LinedefSide(nearest, side > 0);				 //mxd. The meaning of 0.0 is also inverted!!! (I've spent 2 days figuring this out...)
		}

		/// <summary>Find the closest edge within the current outline (that isn't part of the current outline)</summary>
		private LinedefSide FindInnerEdge()
		{
			//DebugConsole.WriteLine("FindInnerEdge: processing " + vertex_valid.Count + " verts");
			
			// Find rightmost non-discarded vertex
			vertex_right = null;
			foreach(Vertex v in vertex_valid)
			{
				// Set rightmost if no current rightmost vertex
				if(vertex_right == null)
				{
					vertex_right = v;
					continue;
				}

				// Check if the vertex is rightmost
				if(v.Position.x > vertex_right.Position.x)
					vertex_right = v;
			}

			// If no vertex was found, we're done
			if(vertex_right == null)
			{
				//DebugConsole.WriteLine("FindInnerEdge: no vertex_right");
				return null;
			}

			// Go through vertex's connected lines, to find
			// the line with the smallest angle parallel with
			// the right side of the bbox
			Linedef eline = null;
			float min_angle = float.MaxValue;
			foreach(Linedef line in vertex_right.Linedefs)
			{
				// Ignore if zero-length
				if(line.Start == line.End) continue;

				// Get opposite vertex
				Vertex opposite = (line.Start == vertex_right ? line.End : line.Start);

				// Determine angle
				float angle = Angle2D.GetAngle(new Vector2D(vertex_right.Position.x + 32, vertex_right.Position.y),
											   new Vector2D(vertex_right.Position.x, vertex_right.Position.y),
											   new Vector2D(opposite.Position.x, opposite.Position.y));

				// Check if minimum
				if(angle < min_angle)
				{
					min_angle = angle;
					eline = line;
				}
			}

			// If no line was found, something is wrong (the vertex may have no attached lines)
			if(eline == null)
			{
				// Discard vertex and try again
				vertex_valid.Remove(vertex_right);
				//DebugConsole.WriteLine("vertex_valid: " + vertex_valid.Count + " verts after Remove (in FindInnerEdge)");
				
				return FindInnerEdge();
			}

			// Determine appropriate side
			return new LinedefSide(eline, (vertex_right == eline.Start));
		}

		///<summary>Finds the next adjacent edge to [edge], ie the adjacent edge that creates the smallest angle</summary>
		private static LinedefSide NextEdge(LinedefSide edge, Dictionary<Linedef, int> visited_lines)
		{
			// Get relevant vertices
			Vertex vertex;		// Vertex to be tested
			Vertex vertex_prev;	// 'Previous' vertex
			if(edge.Front)
			{
				vertex = edge.Line.End;
				vertex_prev = edge.Line.Start;
			}
			else
			{
				vertex = edge.Line.Start;
				vertex_prev = edge.Line.End;
			}

			// Find next connected line with the lowest angle
			float min_angle = Angle2D.PI2;
			LinedefSide next = null;
			foreach(Linedef line in vertex.Linedefs)
			{
				// Ignore original line
				if(line == edge.Line) continue;

				// Ignore if zero-length
				if(line.Start.Position == line.End.Position) continue;

				// Get next vertex
				Vertex vertex_next;
				bool front = true;
				if(line.Start == vertex)
				{
					vertex_next = line.End;
				}
				else
				{
					vertex_next = line.Start;
					front = false;
				}

				// Ignore already-traversed lines
				int side = (front ? 1 : 2);
				if(visited_lines.ContainsKey(line) && (visited_lines[line] & side) == side) continue;


				// Determine angle between lines
				float angle = Angle2D.GetAngle(new Vector2D(vertex_prev.Position.x, vertex_prev.Position.y),
											   new Vector2D(vertex.Position.x, vertex.Position.y),
											   new Vector2D(vertex_next.Position.x, vertex_next.Position.y));

				// Check if minimum angle
				if(angle < min_angle)
				{
					min_angle = angle;

					if(next == null)
					{
						next = new LinedefSide(line, front);
					}
					else
					{
						next.Line = line;
						next.Front = front;
					}
				}
			}

			// Return the next edge found
			if(next == null) return null;
			if(!visited_lines.ContainsKey(next.Line)) visited_lines.Add(next.Line, 0);
			visited_lines[next.Line] |= (next.Front ? 1 : 2);
			return next;
		}

		/// <summary>Returns true if the vertex is outside the current outline</summary>
		private bool PointOutsideOutline(Vertex v)
		{
			// Check with bounding box
			Vector2D point = v.Position;
			bool pointwithin = (point.x >= o_bbox.Left && point.x <= o_bbox.Right && point.y >= o_bbox.Top && point.y <= o_bbox.Bottom);
			if(!pointwithin)
			{
				// If the point is not within the bbox and the outline is clockwise, it can't be within the outline
				// On the other hand, if the outline is anticlockwise, the point *must* be 'within' the outline
				return o_clockwise;
			}

			// Find nearest edge
			int nearest = NearestEdge(point);
			if(nearest >= 0)
			{
				// Check what side of the edge the point is on
				float side = -o_edges[nearest].Line.SideOfLine(point); //mxd. SideOfLine logic is inverted in Slade 3
																	   //mxd. The meaning of 0.0 is also inverted!!!

				// Return false if it is on the correct side
				if(side > 0 && o_edges[nearest].Front)
				{
					//DebugConsole.WriteLine("Point " + point + " is within outline (infront of line) " + nearest);
					return false;
				}
				if(side <= 0 && !o_edges[nearest].Front)
				{
					//DebugConsole.WriteLine("Point " + point + " is within outline (at the back of line) " + nearest);
					return false;
				}
			}

			// Not within the outline
			//DebugConsole.WriteLine("Point " + point + " is outside outline");
			return true;
		}

		private int NearestEdge(Vector2D point)
		{
			// Init variables
			float min_dist = float.MaxValue;
			int nearest = -1;

			// Go through edges
			for(int i = 0; i < o_edges.Count; i++)
			{
				// Get distance to edge
				float dist = o_edges[i].Line.SafeDistanceToSq(point, true);

				// Check if minimum
				if(dist < min_dist)
				{
					min_dist = dist;
					nearest = i;
				}
			}

			// Return nearest edge index
			return nearest;
		}

		/// <summary>Checks if the traced sector is valid (ie. all edges are currently referencing the same (existing) sector)</summary>
		public bool IsValidSector()
		{
			if(sector_edges.Count == 0) return false;

			// Get first edge's sector
			Sector sector = (sector_edges[0].Front ?
				(sector_edges[0].Line.Front != null ? sector_edges[0].Line.Front.Sector : null) :
				(sector_edges[0].Line.Back != null ? sector_edges[0].Line.Back.Sector : null));

			// Sector is invalid if any edge has no current sector
			if(sector == null) return false; 

			// Go through subsequent edges
			for(int a = 1; a < sector_edges.Count; a++)
			{
				// Get edge sector
				Sector ssector = (sector_edges[a].Front ? 
					(sector_edges[a].Line.Front != null ? sector_edges[a].Line.Front.Sector : null) :
					(sector_edges[a].Line.Back != null ? sector_edges[a].Line.Back.Sector : null));

				// Check if different
				if(sector != ssector) return false;
			}

			// Return true if the entire sector was traced
			return (sector.Sidedefs.Count == sector_edges.Count);
		}

		/// <summary>Finds any existing sector that is already part of the traced new sector</summary>
		internal Sector FindExistingSector(HashSet<Sidedef> sides_ignore)
		{
			// Go through new sector edges
			Sector sector = null;
			Sector sector_priority = null;
			foreach(LinedefSide edge in sector_edges)
			{
				// Check if the edge's corresponding MapSide has a front sector
				if(edge.Front && edge.Line.Front != null && edge.Line.Front.Sector != null)
				{
					if(sides_ignore.Contains(edge.Line.Front))
						sector = edge.Line.Front.Sector;
					else
						sector_priority = edge.Line.Front.Sector;
				}

				// Check if the edge's corresponding MapSide has a back sector
				if(!edge.Front && edge.Line.Back != null && edge.Line.Back.Sector != null)
				{
					if(sides_ignore.Contains(edge.Line.Back))
						sector = edge.Line.Back.Sector;
					else
						sector_priority = edge.Line.Back.Sector;
				}
			}

			return (sector_priority ?? sector);
		}

		/// <summary>Sets all traced edges to [sector], or creates a new sector using properties
		/// from [sector_copy] if none given</summary>
		internal void CreateSector(Sector sector, Sector sector_copy)
		{
			// Create the sector if needed
			if(sector == null)
			{
				sector = General.Map.Map.CreateSector();
				if(sector == null) return;
				sector.Marked = true; //mxd

				// Find potential sector to copy if none specified
				if(sector_copy == null) sector_copy = FindCopySector();
				if(sector_copy != null) sector_copy.CopyPropertiesTo(sector);
			}

			//DebugConsole.WriteLine(" ");
			//DebugConsole.WriteLine("Creating sector " + sector.Index + " from " + sector_edges.Count + " lines");
			//DebugConsole.WriteLine("*************************************************************");
			//DebugConsole.WriteLine(" ");

			// Set sides to new sector
			foreach(LinedefSide edge in sector_edges)
			{
				Sidedef target = (edge.Front ? edge.Line.Front : edge.Line.Back);
				if(target != null)
				{
					if(target.Sector != sector)
					{
						bool targetwas2s = (target.Other != null);
						target.SetSector(sector); //mxd. Reattach side

						//mxd. Mark for texture adjustments if sidedness was changed.
						//mxd. Also keep existing mark if the side was already marked.
						target.Marked |= ((targetwas2s && target.Other == null) || (!targetwas2s && target.Other != null));
					}
				}
				else
				{
					target = General.Map.Map.CreateSidedef(edge.Line, edge.Front, sector); //mxd. Create new side
					target.Marked = true; //mxd. Mark it for texture adjustments
					if(target.Other != null)
					{
						//mxd. Better than nothing
						target.Other.CopyPropertiesTo(target);

						//mxd. Other was singlesided. We'll need to adjust it's textures as well
						target.Other.Marked = true; 
					}
				}
			}
		}

		/// <summary>Finds an appropriate existing sector to copy properties from, for the new sector being built</summary>
		private Sector FindCopySector()
		{
			// Go through new sector edges
			Sector sector_copy = null;
			foreach(LinedefSide edge in sector_edges)
			{
				// Check if the edge's corresponding MapSide has a front sector
				if(edge.Line.Front != null && edge.Line.Front.Sector != null)
				{
					// Set sector to copy
					sector_copy = edge.Line.Front.Sector;

					// If the edge is a front edge, use this sector and ignore all else
					if(edge.Front) break;
				}

				// Check if the edge's corresponding MapSide has a back sector
				if(edge.Line.Back != null && edge.Line.Back.Sector != null)
				{
					// Set sector to copy
					sector_copy = edge.Line.Back.Sector;

					// If the edge is a back edge, use this sector and ignore all else
					if(!edge.Front) break;
				}
			}

			return sector_copy;
		}

		#endregion
	}
}
