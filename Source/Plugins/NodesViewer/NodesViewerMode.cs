#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Plugins.NodesViewer
{
	[EditMode(DisplayName = "Nodes Viewer",
			  SwitchAction = "nodesviewermode",
			  ButtonImage = "NodesView.png",
			  ButtonOrder = 350,
			  ButtonGroup = "002_tools",
			  Volatile = true,
			  UseByDefault = true,
			  AllowCopyPaste = false)]
	public class NodesViewerMode : ClassicMode
	{
		#region ================== Constants

		private const float EPSILON = 0.00001f;
		
		#endregion
		
		#region ================== Variables

		private Seg[] segs;
		private Node[] nodes;
		private Vector2D[] verts;
		private Subsector[] ssectors;
		private List<PixelColor> distinctcolors;
		private NodesForm form;
		private int mouseinssector = -1;
		private string nodesformat = "Classic nodes";

		#endregion

		#region ================== Properties

		public Seg[] Segs { get { return segs; } }
		public Node[] Nodes { get { return nodes; } }
		public Vector2D[] Vertices { get { return verts; } }
		public Subsector[] Subsectors { get { return ssectors; } }
		public NodesForm Form { get { return form; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public NodesViewerMode()
		{
			// Make a list of distict colors we can use to
			// display multiple things on the screen
			// Note that black and white are not in this list, because
			// these are the most likely colors for the user's background
			distinctcolors = new List<PixelColor> {
				PixelColor.FromColor(Color.Blue), 
				PixelColor.FromColor(Color.Orange), 
				PixelColor.FromColor(Color.ForestGreen), 
				PixelColor.FromColor(Color.Sienna), 
				PixelColor.FromColor(Color.LightPink), 
				PixelColor.FromColor(Color.Purple),
				PixelColor.FromColor(Color.Cyan), 
				PixelColor.FromColor(Color.LawnGreen), 
				PixelColor.FromColor(Color.PaleGoldenrod), 
				PixelColor.FromColor(Color.Red), 
				PixelColor.FromColor(Color.Yellow), 
				PixelColor.FromColor(Color.LightSkyBlue), 
				PixelColor.FromColor(Color.DarkGray), 
				PixelColor.FromColor(Color.Magenta)
			};
		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// This (re)builds the nodes for the whole map.
		/// </summary>
		public void BuildNodes()
		{
			// There is no API available to do this directly, but we export the map which will
			// cause the DB core to build the nodes (with testing parameters)
			General.Interface.DisplayStatus(StatusType.Busy, "Building map nodes...");
			string tempfile = BuilderPlug.MakeTempFilename(".wad");
			General.Map.IsChanged = true;
			General.Map.ExportToFile(tempfile);
			File.Delete(tempfile);
		}

		/// <summary>
		/// This loads all nodes structures data from the lumps
		/// </summary>
		private void LoadClassicStructures()
		{
			// Load the nodes structure
			MemoryStream nodesstream = General.Map.GetLumpData("NODES");
			BinaryReader nodesreader = new BinaryReader(nodesstream);
			int numnodes = (int)nodesstream.Length / 28;
			nodes = new Node[numnodes];
			for(int i = 0; i < nodes.Length; i++)
			{
				nodes[i].linestart.x = nodesreader.ReadInt16();
				nodes[i].linestart.y = nodesreader.ReadInt16();
				nodes[i].linedelta.x = nodesreader.ReadInt16();
				nodes[i].linedelta.y = nodesreader.ReadInt16();
				float top = nodesreader.ReadInt16();
				float bot = nodesreader.ReadInt16();
				float left = nodesreader.ReadInt16();
				float right = nodesreader.ReadInt16();
				nodes[i].rightbox = new RectangleF(left, top, (right - left), (bot - top));
				top = nodesreader.ReadInt16();
				bot = nodesreader.ReadInt16();
				left = nodesreader.ReadInt16();
				right = nodesreader.ReadInt16();
				nodes[i].leftbox = new RectangleF(left, top, (right - left), (bot - top));
				int rightindex = nodesreader.ReadInt16();
				int leftindex = nodesreader.ReadInt16();
				nodes[i].rightchild = rightindex & 0x7FFF;
				nodes[i].leftchild = leftindex & 0x7FFF;
				nodes[i].rightsubsector = (rightindex & 0x8000) != 0;
				nodes[i].leftsubsector = (leftindex & 0x8000) != 0;
			}
			nodesreader.Close();
			nodesstream.Close();
			nodesstream.Dispose();

			// Add additional properties to nodes
			nodes[nodes.Length - 1].parent = -1;
			RecursiveSetupNodes(nodes.Length - 1);

			// Load the segs structure
			MemoryStream segsstream = General.Map.GetLumpData("SEGS");
			BinaryReader segsreader = new BinaryReader(segsstream);
			int numsegs = (int)segsstream.Length / 12;
			segs = new Seg[numsegs];
			for(int i = 0; i < segs.Length; i++)
			{
				segs[i].startvertex = segsreader.ReadInt16();
				segs[i].endvertex = segsreader.ReadInt16();
				segs[i].angle = Angle2D.DoomToReal(segsreader.ReadInt16());
				segs[i].lineindex = segsreader.ReadInt16();
				segs[i].leftside = segsreader.ReadInt16() != 0;
				segs[i].offset = segsreader.ReadInt16();
			}
			segsreader.Close();
			segsstream.Close();
			segsstream.Dispose();

			// Load the vertexes structure
			MemoryStream vertsstream = General.Map.GetLumpData("VERTEXES");
			BinaryReader vertsreader = new BinaryReader(vertsstream);
			int numverts = (int)vertsstream.Length / 4;
			verts = new Vector2D[numverts];
			for(int i = 0; i < verts.Length; i++)
			{
				verts[i].x = vertsreader.ReadInt16();
				verts[i].y = vertsreader.ReadInt16();
			}
			vertsreader.Close();
			vertsstream.Close();
			vertsstream.Dispose();

			// Load the subsectors structure
			MemoryStream ssecstream = General.Map.GetLumpData("SSECTORS");
			BinaryReader ssecreader = new BinaryReader(ssecstream);
			int numssec = (int)ssecstream.Length / 4;
			ssectors = new Subsector[numssec];
			for(int i = 0; i < ssectors.Length; i++)
			{
				ssectors[i].numsegs = ssecreader.ReadInt16();
				ssectors[i].firstseg = ssecreader.ReadInt16();
			}
			ssecreader.Close();
			ssecstream.Close();
			ssecstream.Dispose();

			// Link all segs to their subsectors
			for(int i = 0; i < ssectors.Length; i++)
			{
				int lastseg = ssectors[i].firstseg + ssectors[i].numsegs - 1;
				for(int sg = ssectors[i].firstseg; sg <= lastseg; sg++)
				{
					segs[sg].ssector = i;
				}
			}
		}

		//mxd. This loads all data from the ZNODES lump
		private bool LoadZNodes() 
		{
			List<string> supportedFormats = new List<string> { "XNOD", "XGLN", "XGL2", "XGL3" };
			using(MemoryStream stream = General.Map.GetLumpData("ZNODES")) 
			{
				//boilerplate...
				if(stream.Length < 4) 
				{
					MessageBox.Show("ZNODES lump is empty.", "Nodes Viewer mode", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}

				using(BinaryReader reader = new BinaryReader(stream)) 
				{
					//read signature
					nodesformat = new string(reader.ReadChars(4));
					if(!supportedFormats.Contains(nodesformat)) 
					{
						MessageBox.Show("'" + nodesformat + "' node format is not supported.", "Nodes Viewer mode", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}

					uint vertsCount = reader.ReadUInt32();
					uint newVertsCount = reader.ReadUInt32();

					//boilerplate...
					if(vertsCount != General.Map.Map.Vertices.Count) 
					{
						MessageBox.Show("Error while reading ZNODES: vertices count in ZNODES lump (" + vertsCount + ") doesn't match with map's vertices count (" + General.Map.Map.Vertices.Count + ")!", "Nodes Viewer mode", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}

					//add map vertices
					verts = new Vector2D[vertsCount + newVertsCount];
					int counter = 0;
					foreach(Vertex v in General.Map.Map.Vertices) verts[counter++] = v.Position;

					//read extra vertices
					for(int i = counter; i < counter + newVertsCount; i++) 
					{
						verts[i].x = reader.ReadInt32() / 65536.0f;
						verts[i].y = reader.ReadInt32() / 65536.0f;
					}

					//read subsectors
					uint ssecCount = reader.ReadUInt32();
					ssectors = new Subsector[ssecCount];

					int firstseg = 0;
					for(int i = 0; i < ssectors.Length; i++) 
					{
						ssectors[i].numsegs = (int)reader.ReadUInt32();
						ssectors[i].firstseg = firstseg;
						firstseg += ssectors[i].numsegs;
					}

					//read segments. offset and angle are unused anyway
					uint segsCount = reader.ReadUInt32();
					segs = new Seg[segsCount];

					switch(nodesformat) 
					{
						case "XGLN":
							for(int i = 0; i < segs.Length; i++) 
							{
								segs[i].startvertex = (int)reader.ReadUInt32();
								reader.BaseStream.Position += 4; //skip partner
								segs[i].lineindex = reader.ReadUInt16();
								segs[i].leftside = reader.ReadBoolean();
							}
							break;

						case "XGL3":
						case "XGL2":
							for(int i = 0; i < segs.Length; i++) 
							{
								segs[i].startvertex = (int)reader.ReadUInt32();
								reader.BaseStream.Position += 4; //skip partner
								uint lineindex = reader.ReadUInt32();
								segs[i].lineindex = (lineindex == 0xFFFFFFFF ? -1 : (int)lineindex);
								segs[i].leftside = reader.ReadBoolean();
							}
							break;

						case "XNOD":
							for(int i = 0; i < segs.Length; i++) 
							{
								segs[i].startvertex = (int)reader.ReadUInt32();
								segs[i].endvertex = (int)reader.ReadUInt32();
								segs[i].lineindex = reader.ReadUInt16();
								segs[i].leftside = reader.ReadBoolean();
							}
							break;
					}

					//set second vertex, angle and reverse segs order
					if(nodesformat == "XGLN" || nodesformat == "XGL2" || nodesformat == "XGL3") 
					{
						int index = 0;
						foreach(Subsector ss in ssectors) 
						{
							//set the last vert
							int lastseg = ss.firstseg + ss.numsegs - 1;
							segs[lastseg].endvertex = segs[ss.firstseg].startvertex;

							//set the rest
							for(int i = ss.firstseg + 1; i <= lastseg; i++) segs[i - 1].endvertex = segs[i].startvertex;

							//set angle and subsector index
							for (int i = ss.firstseg; i <= lastseg; i++) 
							{
								segs[i].angle = Vector2D.GetAngle(verts[segs[i].endvertex], verts[segs[i].startvertex]);
								segs[i].ssector = index;
							}

							index++;
						}
					}

					//read nodes
					uint nodesCount = reader.ReadUInt32();

					//boilerplate...
					if(nodesCount < 1) 
					{
						MessageBox.Show("The map has only one subsector.", "Why are you doing this, Stanley?..", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}

					nodes = new Node[nodesCount];

					for(int i = 0; i < nodes.Length; i++) 
					{
						if(nodesformat == "XGL3") 
						{
							nodes[i].linestart.x = reader.ReadInt32() / 65536.0f;
							nodes[i].linestart.y = reader.ReadInt32() / 65536.0f;
							nodes[i].linedelta.x = reader.ReadInt32() / 65536.0f;
							nodes[i].linedelta.y = reader.ReadInt32() / 65536.0f;
						} 
						else 
						{
							nodes[i].linestart.x = reader.ReadInt16();
							nodes[i].linestart.y = reader.ReadInt16();
							nodes[i].linedelta.x = reader.ReadInt16();
							nodes[i].linedelta.y = reader.ReadInt16();
						}

						float top = reader.ReadInt16();
						float bot = reader.ReadInt16();
						float left = reader.ReadInt16();
						float right = reader.ReadInt16();
						nodes[i].rightbox = new RectangleF(left, top, (right - left), (bot - top));

						top = reader.ReadInt16();
						bot = reader.ReadInt16();
						left = reader.ReadInt16();
						right = reader.ReadInt16();
						nodes[i].leftbox = new RectangleF(left, top, (right - left), (bot - top));

						uint rightindex = reader.ReadUInt32();
						uint leftindex = reader.ReadUInt32();
						nodes[i].rightchild = (int)(rightindex & 0x7FFFFFFF);
						nodes[i].leftchild = (int)(leftindex & 0x7FFFFFFF);
						nodes[i].rightsubsector = (rightindex & 0x80000000) != 0;
						nodes[i].leftsubsector = (leftindex & 0x80000000) != 0;
					}

					// Add additional properties to nodes
					nodes[nodes.Length - 1].parent = -1;
					RecursiveSetupNodes(nodes.Length - 1);
				}
			}

			return true;
		}

		/// <summary>
		/// This recursively sets up the nodes structure with additional properties
		/// </summary>
		private void RecursiveSetupNodes(int nodeindex)
		{
			Node n = nodes[nodeindex];
			if(!n.leftsubsector)
			{
				nodes[n.leftchild].parent = nodeindex;
				RecursiveSetupNodes(n.leftchild);
			}
			if(!n.rightsubsector)
			{
				nodes[n.rightchild].parent = nodeindex;
				RecursiveSetupNodes(n.rightchild);
			}
		}

		/// <summary>
		/// This builds the polygons for all subsectors
		/// </summary>
		private void RecursiveBuildSubsectorPoly(int nodeindex, Stack<Split> splits)
		{
			// Do the left side
			Split s = new Split(nodes[nodeindex].linestart, -nodes[nodeindex].linedelta);
			splits.Push(s);
			if(nodes[nodeindex].leftsubsector)
				BuildSubsectorPoly(nodes[nodeindex].leftchild, splits);
			else
				RecursiveBuildSubsectorPoly(nodes[nodeindex].leftchild, splits);

			splits.Pop();

			// Do the right side
			s = new Split(nodes[nodeindex].linestart, nodes[nodeindex].linedelta);
			splits.Push(s);
			if(nodes[nodeindex].rightsubsector)
				BuildSubsectorPoly(nodes[nodeindex].rightchild, splits);
			else
				RecursiveBuildSubsectorPoly(nodes[nodeindex].rightchild, splits);

			splits.Pop();
		}

		/// <summary>
		/// Build the polygon for a specific subsector
		/// </summary>
		private void BuildSubsectorPoly(int ss, IEnumerable<Split> nodesplits)
		{
			// Begin with a giant square polygon that covers the entire map
			List<Vector2D> poly = new List<Vector2D>(16);
			poly.Add(new Vector2D(-General.Map.FormatInterface.MaxCoordinate, General.Map.FormatInterface.MaxCoordinate));
			poly.Add(new Vector2D(General.Map.FormatInterface.MaxCoordinate, General.Map.FormatInterface.MaxCoordinate));
			poly.Add(new Vector2D(General.Map.FormatInterface.MaxCoordinate, -General.Map.FormatInterface.MaxCoordinate));
			poly.Add(new Vector2D(-General.Map.FormatInterface.MaxCoordinate, -General.Map.FormatInterface.MaxCoordinate));

			// Crop the polygon by the node tree splits
			foreach(Split s in nodesplits) CropPolygon(poly, s);

			// Crop the polygon by the subsector segs
			for(int i = 0; i < ssectors[ss].numsegs; i++)
			{
				Split s;
				Seg sg = segs[ssectors[ss].firstseg + i];

				//mxd. Sanity check, because some segs in Doom maps refer to non-existing verts.  
				if(sg.startvertex > verts.Length - 1 || sg.endvertex > verts.Length - 1) continue;

				s.pos = verts[sg.startvertex];
				s.delta = verts[sg.endvertex] - verts[sg.startvertex];
				CropPolygon(poly, s);
			}

			if(poly.Count > 1)
			{
				// Remove any zero-length lines
				Vector2D prevpoint = poly[0];
				for(int i = poly.Count - 1; i >= 0; i--)
				{
					if(Vector2D.DistanceSq(poly[i], prevpoint) < 0.001f)
						poly.RemoveAt(i);
					else
						prevpoint = poly[i];
				}
			}

			ssectors[ss].points = poly.ToArray();

			// Setup vertices for rendering
			if(poly.Count >= 3)
			{
				FlatVertex[] fverts = new FlatVertex[(poly.Count - 2) * 3];
				int intcolor = PixelColor.FromColor(Color.Gray).WithAlpha(100).ToInt();
				int pi = 0;
				for(int t = 0; t < (poly.Count - 2); t++)
				{
					fverts[pi].x = poly[0].x;
					fverts[pi].y = poly[0].y;
					fverts[pi].c = intcolor;
					fverts[pi + 1].x = poly[t + 1].x;
					fverts[pi + 1].y = poly[t + 1].y;
					fverts[pi + 1].c = intcolor;
					fverts[pi + 2].x = poly[t + 2].x;
					fverts[pi + 2].y = poly[t + 2].y;
					fverts[pi + 2].c = intcolor;
					pi += 3;
				}
				ssectors[ss].vertices = fverts;
			}
		}

		/// <summary>
		/// Crop a polygon by a split line
		/// </summary>
		private static void CropPolygon(List<Vector2D> poly, Split split)
		{
			if(poly.Count == 0) return;
			Vector2D prev = poly[poly.Count - 1];

			float side1 = (prev.y - split.pos.y) * split.delta.x - (prev.x - split.pos.x) * split.delta.y;
			List<Vector2D> newp = new List<Vector2D>(poly.Count);
			for(int i = 0; i < poly.Count; i++)
			{
				// Fetch vertex and determine side
				Vector2D cur = poly[i];
				float side2 = (cur.y - split.pos.y) * split.delta.x - (cur.x - split.pos.x) * split.delta.y;

				// Front?
				if(side2 < -EPSILON)
				{
					if(side1 > EPSILON)
					{
						// Split line with plane and insert the vertex
						float u;
						Line2D.GetIntersection(split.pos, split.pos + split.delta, prev.x, prev.y, cur.x, cur.y, out u);
						Vector2D newv = prev + (cur - prev) * u;
						newp.Add(newv);
					}

					newp.Add(cur);
				}
				// Back?
				else if(side2 > EPSILON)
				{
					if(side1 < -EPSILON)
					{
						// Split line with plane and insert the vertex
						float u;
						Line2D.GetIntersection(split.pos, split.pos + split.delta, prev.x, prev.y, cur.x, cur.y, out u);
						Vector2D newv = prev + (cur - prev) * u;
						newp.Add(newv);
					}
				}
				else
				{
					// On the plane
					newp.Add(cur);
				}

				// Next
				prev = cur;
				side1 = side2;
			}
			poly.Clear();
			poly.AddRange(newp);

			// The code below would be more efficient, because it modifies the polygon in place...
			// but it has a bug, some polygons are corrupted.

			/*
			bool prevremoved = false;
			float prevside = (prev.y - split.pos.y) * split.delta.x - (prev.x - split.pos.x) * split.delta.y;
			int i = 0;
			while(i < poly.Count)
			{
				Vector2D cur = poly[i];
				float curside = (cur.y - split.pos.y) * split.delta.x - (cur.x - split.pos.x) * split.delta.y;
				
				// Point is in FRONT of the split?
				if(curside < -EPSILON)
				{
					if(prevside > EPSILON)
					{
						// Previous point was BEHIND the split
						// Line crosses the split, we need to add the intersection point
						float u;
						Line2D.GetIntersection(split.pos, split.pos + split.delta, prev.x, prev.y, cur.x, cur.y, out u);
						Vector2D newv = prev + (cur - prev) * u;
						poly.Insert(i, newv);
						i++;
					}
					else if(prevside < -EPSILON)
					{
						// Previous point was also in FRONT of the split
						// We don't need to do anything
					}
					else
					{
						// Previous point was ON the split
						// If the previous point was removed, we have to add it again
						if(prevremoved)
						{
							poly.Insert(i, prev);
							i++;
						}
					}
					
					i++;
					prevremoved = false;
				}
				// Point is BEHIND the split?
				else if(curside > EPSILON)
				{
					if(prevside < -EPSILON)
					{
						// Previous point was in FRONT of the split
						// Line crosses the split, so we must add the intersection point
						float u;
						Line2D.GetIntersection(split.pos, split.pos + split.delta, prev.x, prev.y, cur.x, cur.y, out u);
						Vector2D newv = prev + (cur - prev) * u;
						poly.Insert(i, newv);
						i++;
					}
					else if(prevside > EPSILON)
					{
						// Previous point was also BEHIND the split
						// We don't need to do anything, this point will be removed
					}
					else
					{
						// Previous point was ON the split
						// We don't need to do anything, this point will be removed
					}
					
					poly.RemoveAt(i);
					prevremoved = true;
				}
				// Point is ON the split?
				else
				{
					if(prevside > EPSILON)
					{
						// Previous point was BEHIND the split
						// Remove this point
						poly.RemoveAt(i);
						prevremoved = true;
					}
					else if(prevside < -EPSILON)
					{
						// Previous point was in FRONT of the split
						// We want to keep this point
						prevremoved = false;
						i++;
					}
					else
					{
						// Previous point is ON the split
						// Only if the previous point was also removed, we remove this one as well
						if(prevremoved)
							poly.RemoveAt(i);
						else
							i++;
					}
				}
				
				prev = cur;
				prevside = curside;
			}
			*/
		}

		/// <summary>
		/// This tests if the given coordinate is inside the specified subsector.
		/// </summary>
		private bool PointInSubsector(int index, Vector2D p) 
		{
			if (ssectors[index].points.Length == 0) return false; //mxd
			
			// Subsectors are convex, so we can simply test if the point is on the front side of all lines.
			Vector2D[] points = ssectors[index].points;
			Vector2D prevpoint = points[points.Length - 1];
			for(int i = 0; i < points.Length; i++)
			{
				float side = Line2D.GetSideOfLine(prevpoint, points[i], p);
				if(side > 0f) return false;
				prevpoint = points[i];
			}
			return true;
		}

		// For rendering
		private void DrawSubsectorArea(FlatVertex[] vertices, PixelColor color)
		{
			if(vertices == null) return;
			if(vertices.Length < 3) return;

			// Copy array and change color
			FlatVertex[] poly = new FlatVertex[vertices.Length];
			vertices.CopyTo(poly, 0);
			int intcolor = color.WithAlpha(100).ToInt();
			for(int i = 0; i < poly.Length; i++) poly[i].c = intcolor;

			// Draw
			renderer.RenderGeometry(poly, General.Map.Data.WhiteTexture, true);
		}

		// For rendering
		private void DrawSubsectorArea(FlatVertex[] vertices)
		{
			if(vertices == null) return;
			if(vertices.Length < 3) return;
			renderer.RenderGeometry(vertices, General.Map.Data.WhiteTexture, true);
		}

		// For rendering
		private void PlotSubsectorLines(Vector2D[] points, PixelColor color)
		{
			if(points.Length < 2) return;
			Vector2D prevpoint = points[points.Length - 1];
			for(int i = 0; i < points.Length; i++)
			{
				renderer.PlotLine(prevpoint, points[i], color);
				prevpoint = points[i];
			}
		}

		// For rendering
		private void DrawSplitArea(RectangleF bbox, int nodeindex, bool left, PixelColor color)
		{
			Node node = nodes[nodeindex];

			// Begin with a square bounding box polygon
			List<Vector2D> poly = new List<Vector2D>(16);
			poly.Add(new Vector2D(bbox.Left, bbox.Top));
			poly.Add(new Vector2D(bbox.Right, bbox.Top));
			poly.Add(new Vector2D(bbox.Right, bbox.Bottom));
			poly.Add(new Vector2D(bbox.Left, bbox.Bottom));

			// Remove everything behind the split from the area
			if(left)
				CropPolygon(poly, new Split(node.linestart, -node.linedelta));
			else
				CropPolygon(poly, new Split(node.linestart, node.linedelta));

			// Remove everything behind parent splits from the area
			int prevnode = nodeindex;
			int parentnode = node.parent;
			while(parentnode > -1)
			{
				Node pn = nodes[parentnode];
				if(!pn.leftsubsector && (pn.leftchild == prevnode))
					CropPolygon(poly, new Split(pn.linestart, -pn.linedelta));
				else if(!pn.rightsubsector && (pn.rightchild == prevnode))
					CropPolygon(poly, new Split(pn.linestart, pn.linedelta));
				prevnode = parentnode;
				parentnode = pn.parent;
			}

			if(poly.Count >= 3)
			{
				// Create render vertices
				FlatVertex[] fverts = new FlatVertex[(poly.Count - 2) * 3];
				int intcolor = color.ToInt();
				int pi = 0;
				for(int t = 0; t < (poly.Count - 2); t++)
				{
					fverts[pi].x = poly[0].x;
					fverts[pi].y = poly[0].y;
					fverts[pi].c = intcolor;
					fverts[pi + 1].x = poly[t + 1].x;
					fverts[pi + 1].y = poly[t + 1].y;
					fverts[pi + 1].c = intcolor;
					fverts[pi + 2].x = poly[t + 2].x;
					fverts[pi + 2].y = poly[t + 2].y;
					fverts[pi + 2].c = intcolor;
					pi += 3;
				}

				// Draw
				renderer.RenderGeometry(fverts, General.Map.Data.WhiteTexture, true);
			}
		}

		#endregion

		#region ================== Events

		// Mode starts
		public override void OnEngage()
		{
			Cursor.Current = Cursors.WaitCursor;
			base.OnEngage();

			//mxd. General.Map.ExportToFile in BuildNodes() won't do the trick if the map was never saved
			if (string.IsNullOrEmpty(General.Map.FilePathName)) 
			{
				MessageBox.Show("Please save the map before running Nodes Viewer mode.", "Nodes Viewer mode", MessageBoxButtons.OK, MessageBoxIcon.Error);
				General.Editing.CancelMode();
				return;
			}

			//mxd. No need to check for these twice
			bool haveNodes = General.Map.LumpExists("NODES");
			bool haveZnodes = General.Map.LumpExists("ZNODES");
			bool haveSectors = General.Map.LumpExists("SSECTORS");
			bool haveSegs = General.Map.LumpExists("SEGS");
			bool haveVerts = General.Map.LumpExists("VERTEXES");

			if(General.Map.IsChanged || !haveNodes || !haveSectors || !haveSegs || !haveVerts || !haveZnodes)
			{
				// We need to build the nodes!
				BuildNodes();
			}

			//mxd
			if(haveZnodes) 
			{
				General.Interface.DisplayStatus(StatusType.Busy, "Reading map nodes...");
				if(!LoadZNodes()) 
				{
					General.Editing.CancelMode();
					return;
				}
			} 
			else 
			{
				if(!haveNodes) 
				{
					MessageBox.Show("Unable to find the NODES lump. It may be that the nodes could not be built correctly.", "Nodes Viewer mode", MessageBoxButtons.OK, MessageBoxIcon.Error);
					General.Editing.CancelMode();
					return;
				}

				if(!haveSectors) 
				{
					MessageBox.Show("Unable to find the SSECTORS lump. It may be that the nodes could not be built correctly.", "Nodes Viewer mode", MessageBoxButtons.OK, MessageBoxIcon.Error);
					General.Editing.CancelMode();
					return;
				}

				if(!haveSegs) 
				{
					MessageBox.Show("Unable to find the SEGS lump. It may be that the nodes could not be built correctly.", "Nodes Viewer mode", MessageBoxButtons.OK, MessageBoxIcon.Error);
					General.Editing.CancelMode();
					return;
				}

				if(!haveVerts) 
				{
					MessageBox.Show("Unable to find the VERTEXES lump. It may be that the nodes could not be built correctly.", "Nodes Viewer mode", MessageBoxButtons.OK, MessageBoxIcon.Error);
					General.Editing.CancelMode();
					return;
				}

				General.Interface.DisplayStatus(StatusType.Busy, "Reading map nodes...");
				LoadClassicStructures();

				//mxd. More boilerplate
				if (nodes.Length < 1) 
				{
					MessageBox.Show("The map has only one subsector.", "Why are you doing this, Stanley?..", MessageBoxButtons.OK, MessageBoxIcon.Error);
					General.Editing.CancelMode();
					return;
				}
			}
			
			// Setup presentation
			CustomPresentation presentation = new CustomPresentation();
			presentation.AddLayer(new PresentLayer(RendererLayer.Background, BlendingMode.Mask, General.Settings.BackgroundAlpha));
			presentation.AddLayer(new PresentLayer(RendererLayer.Grid, BlendingMode.Mask));
			presentation.AddLayer(new PresentLayer(RendererLayer.Overlay, BlendingMode.Alpha, 1f, true));
			presentation.AddLayer(new PresentLayer(RendererLayer.Geometry, BlendingMode.Alpha, 1f, true));
			renderer.SetPresentation(presentation);

			General.Interface.DisplayStatus(StatusType.Busy, "Building subsectors...");

			RecursiveBuildSubsectorPoly(nodes.Length - 1, new Stack<Split>(nodes.Length / 2 + 1));

			// Load and display dialog window
			form = new NodesForm(this);
			form.Text += " (" + nodesformat + " format)";
			form.Show((Form)General.Interface);

			Cursor.Current = Cursors.Default;
			General.Interface.DisplayReady();
			General.Interface.RedrawDisplay();
		}

		// Mode ends
		public override void OnDisengage()
		{
			if(form != null)
			{
				form.Dispose();
				form = null;
			}
			
			base.OnDisengage();
		}

		// Cancelled
		public override void OnCancel()
		{
			// Cancel base class
			base.OnCancel();

			// Return to previous mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Mouse moves
		public override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if(form.SelectedTab == 0)
			{
				int newssector = -1;

				// Traverse the tree to find the subsector we could be in
				Node n = nodes[nodes.Length - 1];
				do
				{
					float side = (mousemappos.y - n.linestart.y) * n.linedelta.x - (mousemappos.x - n.linestart.x) * n.linedelta.y;
					if(side > 0f)
					{
						// Mouse is on the left side of this split
						if(n.leftsubsector)
							newssector = n.leftchild;
						else
							n = nodes[n.leftchild];
					}
					else
					{
						// Mouse is on the right side of this split
						if(n.rightsubsector)
							newssector = n.rightchild;
						else
							n = nodes[n.rightchild];
					}
				}
				while(newssector == -1);

				// The mouse could be outside the map (which can't be determined through the BSP tree).
				// So here we check if the mouse is really inside the subsector.
				if((newssector > -1) && !PointInSubsector(newssector, mousemappos)) newssector = -1;
				
				// Update?
				if(newssector != mouseinssector)
				{
					mouseinssector = newssector;
					General.Interface.RedrawDisplay();
				}
			}
		}

		// Mouse leaves
		public override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			mouseinssector = -1;
			General.Interface.RedrawDisplay();
		}

		// Mouse clicks to select a subsector
		protected override void OnSelectBegin()
		{
			base.OnSelectBegin();
			if(mouseinssector > -1)
			{
				form.ShowSubsector(mouseinssector);
			}
		}

		// Draw the display
		public override void OnRedrawDisplay()
		{
			base.OnRedrawDisplay();

			if(form == null) return;

			if(renderer.StartPlotter(true))
			{
				if(form.SelectedTab == 0)
				{
					// Render all subsectors in original color
					for(int si = 0; si < ssectors.Length; si++) 
					{
						Subsector s = ssectors[si];
						PlotSubsectorLines(s.points, PixelColor.FromColor(Color.Gray));
					}
					
					if(mouseinssector > -1)
					{
						PlotSubsectorLines(ssectors[mouseinssector].points, General.Colors.Highlight);
					}

					// Draw additional vertices
					if(form.ShowSegsVertices)
					{
						for(int i = General.Map.Map.Vertices.Count; i < verts.Length; i++)
							renderer.PlotVertexAt(verts[i], ColorCollection.VERTICES);
					}
				}

				if(form.SelectedTab == 1)
				{
					renderer.PlotLinedefSet(General.Map.Map.Linedefs);

					// Render selected node split
					if((form.ViewSplitIndex >= 0) && (form.ViewSplitIndex < nodes.Length))
					{
						Node n = nodes[form.ViewSplitIndex];

						// Draw parent splits
						int parentsplit = n.parent;
						while(parentsplit > -1)
						{
							Node pn = nodes[parentsplit];
							renderer.PlotLine(pn.linestart, pn.linestart + pn.linedelta, General.Colors.Selection);
							parentsplit = pn.parent;
						}

						// Draw this split
						renderer.PlotLine(n.linestart, n.linestart + n.linedelta, General.Colors.Highlight);
					}
				}

				if(form.SelectedTab == 2)
				{
					renderer.PlotLinedefSet(General.Map.Map.Linedefs);

					// Render selected subsector
					if((form.ViewSubsectorIndex >= 0) && (form.ViewSubsectorIndex < ssectors.Length))
					{
						Subsector s = ssectors[form.ViewSubsectorIndex];
						PlotSubsectorLines(s.points, General.Colors.Highlight);
					}

					// Draw selected segment
					if(form.ViewSegIndex > -1)
					{
						Seg sg = segs[form.ViewSegIndex];
						renderer.PlotLine(verts[sg.startvertex], verts[sg.endvertex], General.Colors.Selection);
					}
				}

				renderer.Finish();
			}

			if(renderer.StartOverlay(true))
			{
				if(form.SelectedTab == 0)
				{
					if(mouseinssector > -1)
					{
						// Render all subsectors in original color
						for(int si = 0; si < ssectors.Length; si++)
						{
							Subsector s = ssectors[si];
							DrawSubsectorArea(s.vertices);
						}
						DrawSubsectorArea(ssectors[mouseinssector].vertices, General.Colors.Highlight);
					}
					else
					{
						// Render all subsectors with distinct colors
						for(int si = 0; si < ssectors.Length; si++)
						{
							Subsector s = ssectors[si];
							PixelColor color = distinctcolors[si % distinctcolors.Count];
							DrawSubsectorArea(s.vertices, color);
						}
					}
				}

				if(form.SelectedTab == 1)
				{
					// Render selected node split
					if((form.ViewSplitIndex >= 0) && (form.ViewSplitIndex < nodes.Length))
					{
						Node n = nodes[form.ViewSplitIndex];

						// Draw areas. We draw these first, because they would otherwise erase any splits we want to show.
						DrawSplitArea(n.leftbox, form.ViewSplitIndex, true, new PixelColor(100, 50, 80, 255));
						DrawSplitArea(n.rightbox, form.ViewSplitIndex, false, new PixelColor(100, 20, 220, 20));

						// Draw parent splits
						int parentsplit = n.parent;
						while(parentsplit > -1)
						{
							Node pn = nodes[parentsplit];
							renderer.RenderLine(pn.linestart, pn.linestart + pn.linedelta, 1f, General.Colors.Selection, true);
							parentsplit = pn.parent;
						}

						// Draw this split
						renderer.RenderLine(n.linestart, n.linestart + n.linedelta, 1f, General.Colors.Highlight, true);
					}
				}

				if(form.SelectedTab == 2)
				{
					// Render selected subsector
					if((form.ViewSubsectorIndex >= 0) && (form.ViewSubsectorIndex < ssectors.Length))
					{
						Subsector s = ssectors[form.ViewSubsectorIndex];

						// Draw area
						DrawSubsectorArea(s.vertices, General.Colors.Highlight);

						// Draw selected segment
						if(form.ViewSegIndex > -1)
						{
							Seg sg = segs[form.ViewSegIndex];
							renderer.RenderLine(verts[sg.startvertex], verts[sg.endvertex], 1f, General.Colors.Selection, true);
						}
					}
				}
				
				renderer.Finish();
			}
			
			renderer.Present();
		}

		#endregion
	}
}
