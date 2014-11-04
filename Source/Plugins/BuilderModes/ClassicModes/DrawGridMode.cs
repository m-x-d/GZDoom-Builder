#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Draw Grid Mode",
			  SwitchAction = "drawgridmode",
			  ButtonImage = "DrawGridMode.png", //mxd
			  ButtonOrder = int.MinValue + 5, //mxd
			  ButtonGroup = "000_drawing", //mxd
			  AllowCopyPaste = false,
			  Volatile = true,
			  Optional = false)]

	public class DrawGridMode : DrawGeometryMode
	{
		#region ================== Variables

		private static int horizontalSlices = 3;
		private static int verticalSlices = 3;
		private static bool triangulate;
		private static bool gridlock;

		private readonly List<DrawnVertex[]> gridpoints;
		private HintLabel hintLabel;
		
		private int width;
		private int height;
		private int slicesH;
		private int slicesV;
		private Vector2D start;
		private Vector2D end;

		//interface
		private readonly DrawGridOptionsPanel panel;

		#endregion

		#region ================== Constructor

		public DrawGridMode() 
		{
			snaptogrid = true;
			gridpoints = new List<DrawnVertex[]>();

			//Options docker
			panel = new DrawGridOptionsPanel();
			panel.MaxHorizontalSlices = (int)General.Map.FormatInterface.MaxCoordinate;
			panel.MaxVerticalSlices = (int)General.Map.FormatInterface.MaxCoordinate;
			panel.OnValueChanged += OptionsPanelOnValueChanged;
			panel.OnGridLockChanged += OptionsPanelOnOnGridLockChanged;
		}

		#endregion

		#region ================== Events

		public override void OnEngage() 
		{
			base.OnEngage();

			//setup settings panel
			panel.Triangulate = triangulate;
			panel.LockToGrid = gridlock;
			panel.HorizontalSlices = horizontalSlices - 1;
			panel.VerticalSlices = verticalSlices - 1;
			panel.Register();
		}

		public override void OnDisengage() 
		{
			base.OnDisengage();
			panel.Unregister();
		}

		override public void OnAccept() 
		{
			Cursor.Current = Cursors.AppStarting;
			General.Settings.FindDefaultDrawSettings();

			// When we have a shape...
			if(gridpoints.Count > 0) 
			{
				// Make undo for the draw
				General.Map.UndoRedo.CreateUndo("Grid draw");

				// Make an analysis and show info
				string[] adjectives = new[] { "gloomy", "sad", "unhappy", "lonely", "troubled", "depressed", "heartsick", "glum", "pessimistic", "bitter", "downcast" }; // aaand my english vocabulary ends here :)
				string word = adjectives[new Random().Next(adjectives.Length - 1)];
				string a = (word[0] == 'u' ? "an " : "a ");

				General.Interface.DisplayStatus(StatusType.Action, "Created " + a + word + " grid.");

				List<Sector> newsectors = new List<Sector>();
				foreach (DrawnVertex[] shape in gridpoints) 
				{
					if(!Tools.DrawLines(shape, true, BuilderPlug.Me.AutoAlignTextureOffsetsOnCreate)) 
					{
						// Drawing failed
						// NOTE: I have to call this twice, because the first time only cancels this volatile mode
						General.Map.UndoRedo.WithdrawUndo();
						General.Map.UndoRedo.WithdrawUndo();
						return;
					}

					// Update cached values after each step...
					General.Map.Map.Update();

					newsectors.AddRange(General.Map.Map.GetMarkedSectors(true));
				}

				// Snap to map format accuracy
				General.Map.Map.SnapAllToAccuracy();

				// Clear selection
				General.Map.Map.ClearAllSelected();

				// Edit new sectors?
				if(BuilderPlug.Me.EditNewSector && (newsectors.Count > 0))
					General.Interface.ShowEditSectors(newsectors);

				// Update the used textures
				General.Map.Data.UpdateUsedTextures();

				// Map is changed
				General.Map.IsChanged = true;
			}

			// Done
			Cursor.Current = Cursors.Default;

			// Return to original mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		private void OptionsPanelOnValueChanged(object sender, EventArgs eventArgs) 
		{
			triangulate = panel.Triangulate;
			horizontalSlices = panel.HorizontalSlices + 1;
			verticalSlices = panel.VerticalSlices + 1;
			Update();
		}

		private void OptionsPanelOnOnGridLockChanged(object sender, EventArgs eventArgs) 
		{
			gridlock = panel.LockToGrid;
			General.Hints.ShowHints(this.GetType(), (gridlock ? "gridlockhelp" : "general"));
			Update();
		}

		public override void OnHelp() 
		{
			General.ShowHelp("/gzdb/features/classic_modes/mode_drawgrid.html");
		}

		#endregion

		#region ================== Methods

		override protected void Update() 
		{
			PixelColor stitchcolor = General.Colors.Highlight;
			PixelColor losecolor = General.Colors.Selection;

			// We WANT snaptogrid and DON'T WANT snaptonearest when lock to grid is enabled
			snaptogrid = gridlock || (General.Interface.ShiftState ^ General.Interface.SnapToGrid);
			snaptonearest = !gridlock && (General.Interface.CtrlState ^ General.Interface.AutoMerge);

			DrawnVertex curp;
			if (points.Count == 1)
			{
				// Handle the case when start point is not on current grid.
				Vector2D gridoffset = General.Map.Grid.SnappedToGrid(points[0].pos) - points[0].pos;
				curp = GetCurrentPosition(mousemappos + gridoffset, snaptonearest, snaptogrid, renderer, points);
				curp.pos -= gridoffset;
			}
			else
			{
				curp = GetCurrentPosition();
			}
			
			float vsize = (renderer.VertexSize + 1.0f) / renderer.Scale;

			// Render drawing lines
			if(renderer.StartOverlay(true)) 
			{
				PixelColor color = snaptonearest ? stitchcolor : losecolor;

				if(points.Count == 1) 
				{
					updateReferencePoints(points[0], curp);
					List<Vector2D[]> shapes = getShapes(start, end);

					//render shape
					foreach (Vector2D[] shape in shapes) 
					{
						for(int i = 1; i < shape.Length; i++)
						renderer.RenderLine(shape[i - 1], shape[i], LINE_THICKNESS, color, true);
					}

					//vertices
					foreach (Vector2D[] shape in shapes) 
					{
						for (int i = 0; i < shape.Length; i++)
							renderer.RenderRectangleFilled(new RectangleF(shape[i].x - vsize, shape[i].y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);
					}

					//and labels
					Vector2D[] labelCoords = new[] { start, new Vector2D(end.x, start.y), end, new Vector2D(start.x, end.y), start };
					for(int i = 1; i < 5; i++) 
					{
						labels[i - 1].Start = labelCoords[i - 1];
						labels[i - 1].End = labelCoords[i];
						renderer.RenderText(labels[i - 1].TextLabel);
					}

					//render hint
					if (horizontalSlices > 1 || verticalSlices > 1) 
					{
						hintLabel.Text = "H: " + (slicesH - 1) + "; V: " + (slicesV - 1);
						if(width > hintLabel.Text.Length * vsize && height > 16 * vsize) 
						{
							float vPos = start.y + height / 2.0f;
							hintLabel.Start = new Vector2D(start.x, vPos);
							hintLabel.End = new Vector2D(end.x, vPos);
							renderer.RenderText(hintLabel.TextLabel);
						}
					}
				} 
				else 
				{
					// Render vertex at cursor
					renderer.RenderRectangleFilled(new RectangleF(curp.pos.x - vsize, curp.pos.y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);
				}

				// Done
				renderer.Finish();
			}

			// Done
			renderer.Present();
		}

		// This draws a point at a specific location
		override public bool DrawPointAt(Vector2D pos, bool stitch, bool stitchline) 
		{
			if(pos.x < General.Map.Config.LeftBoundary || pos.x > General.Map.Config.RightBoundary ||
				pos.y > General.Map.Config.TopBoundary || pos.y < General.Map.Config.BottomBoundary)
				return false;

			DrawnVertex newpoint = new DrawnVertex();
			newpoint.pos = pos;
			newpoint.stitch = true;
			newpoint.stitchline = stitchline;
			points.Add(newpoint);

			if(points.Count == 1) 
			{ 
				// Add labels
				labels.AddRange(new[] { new LineLengthLabel(false), new LineLengthLabel(false), new LineLengthLabel(false), new LineLengthLabel(false) });
				hintLabel = new HintLabel();
				Update();
			} 
			else if(points[0].pos == points[1].pos) 
			{
				// Nothing is drawn
				FinishDraw();
			} 
			else 
			{
				// Handle the case when start point is not on current grid.
				Vector2D gridoffset = General.Map.Grid.SnappedToGrid(points[0].pos) - points[0].pos;
				newpoint = GetCurrentPosition(mousemappos + gridoffset, snaptonearest, snaptogrid, renderer, points);
				newpoint.pos -= gridoffset;
				
				// Create vertices for final shape.
				updateReferencePoints(points[0], newpoint);
				List<Vector2D[]> shapes = getShapes(start, end);

				foreach (Vector2D[] shape in shapes) 
				{
					DrawnVertex[] verts = new DrawnVertex[shape.Length];
					for(int i = 0; i < shape.Length; i++) 
					{
						newpoint = new DrawnVertex();
						newpoint.pos = shape[i];
						newpoint.stitch = true;
						newpoint.stitchline = stitchline;
						verts[i] = newpoint;
					}

					gridpoints.Add(verts);
				}

				FinishDraw();
			}
			return true;
		}

		private List<Vector2D[]> getShapes(Vector2D s, Vector2D e) 
		{
			//no shape
			if(s == e) return new List<Vector2D[]>();

			//setup slices
			if(gridlock) 
			{
				slicesH = width / General.Map.Grid.GridSize;
				slicesV = height / General.Map.Grid.GridSize;
			} 
			else 
			{
				slicesH = horizontalSlices;
				slicesV = verticalSlices;
			}

			//create a segmented line
			List<Vector2D[]> shapes;
			if(width == 0 || height == 0)
			{
				if (slicesH > 0 && width > 0)
				{
					shapes = new List<Vector2D[]>();
					int step = width / slicesH;
					for (int w = 0; w < slicesH; w++)
					{
						shapes.Add(new[] { new Vector2D((int)s.x + step * w, (int)s.y), new Vector2D((int)s.x + step * w + step, (int)s.y) });
					}
					return shapes;
				}

				if (slicesV > 0 && height > 0)
				{
					shapes = new List<Vector2D[]>();
					int step = height / slicesV;
					for (int h = 0; h < slicesV; h++)
					{
						shapes.Add(new[] {new Vector2D((int) s.x, (int) s.y + step * h), new Vector2D((int) s.x, (int) s.y + step * h + step)});
					}
					return shapes;
				}

				//create a line
				return new List<Vector2D[]> {new[] {s, e}};
			}

			float rectWidth = Math.Max(1, (slicesH > 0 ? (float)width / slicesH : width));
			float rectHeight = Math.Max(1, (slicesV > 0 ? (float)height / slicesV : height));

			//create shape
			List<Vector2D> rect = new List<Vector2D> { s, new Vector2D((int)s.x, (int)e.y), e, new Vector2D((int)e.x, (int)s.y), s };
			if(!gridlock && slicesH == 1 && slicesV == 1) 
			{
				if(triangulate) rect.AddRange(new[] { s, e });
				return new List<Vector2D[]> { rect.ToArray() };
			}

			//create blocks
			shapes = new List<Vector2D[]> { rect.ToArray() };
			RectangleF[,] blocks = new RectangleF[slicesH, slicesV];
			for(int w = 0; w < slicesH; w++) 
			{
				for(int h = 0; h < slicesV; h++) 
				{
					blocks[w, h] = RectangleF.FromLTRB((int)Math.Round(s.x + rectWidth * w), (int)Math.Round(s.y + rectHeight * h), (int)Math.Round(s.x + rectWidth * (w + 1)), (int)Math.Round(s.y + rectHeight * (h + 1)));
				}
			}

			//add subdivisions
			if(slicesH > 1) 
			{
				for(int w = 1; w < slicesH; w++) 
				{
					int px = (int) Math.Round(blocks[w, 0].X);
					shapes.Add(new[] {new Vector2D(px, s.y), new Vector2D(px, e.y)});
				}
			}
			if(slicesV > 1) 
			{
				for(int h = 1; h < slicesV; h++) 
				{
					int py = (int) Math.Round(blocks[0, h].Y);
					shapes.Add(new[] { new Vector2D(s.x, py), new Vector2D(e.x, py) });
				}
			}

			//triangulate?
			if (triangulate) 
			{
				bool startflip = ((int)Math.Round(((s.x + e.y) / General.Map.Grid.GridSize) % 2) == 0 /*|| (int)Math.Round((e.y / General.Map.Grid.GridSize) % 2) == 0*/);
				bool flip = startflip;

				for(int w = 0; w < slicesH; w++) 
				{
					for(int h = slicesV - 1; h > -1; h--) 
					{
						if (flip)
							shapes.Add(new[] { new Vector2D(blocks[w, h].X, blocks[w, h].Y), new Vector2D(blocks[w, h].Right, blocks[w, h].Bottom) });
						else
							shapes.Add(new[] { new Vector2D(blocks[w, h].Right, blocks[w, h].Y), new Vector2D(blocks[w, h].X, blocks[w, h].Bottom) });

						flip = !flip;
					}

					startflip = !startflip;
					flip = startflip;
				}
			}

			return shapes;
		}

		//update bottom-left and top-right points, which define drawing shape
		private void updateReferencePoints(DrawnVertex p1, DrawnVertex p2) 
		{
			if(!p1.pos.IsFinite() || !p2.pos.IsFinite()) return;
			
			if (p1.pos.x < p2.pos.x) 
			{
				start.x = p1.pos.x;
				end.x = p2.pos.x;
			} 
			else 
			{
				start.x = p2.pos.x;
				end.x = p1.pos.x;
			}

			if (p1.pos.y < p2.pos.y) 
			{
				start.y = p1.pos.y;
				end.y = p2.pos.y;
			} 
			else 
			{
				start.y = p2.pos.y;
				end.y = p1.pos.y;
			}

			width = (int)(end.x - start.x);
			height = (int)(end.y - start.y);
		}

		#endregion

		#region ================== Actions

		[BeginAction("increasebevel")]
		protected void increaseBevel()
		{
			if(!gridlock && (points.Count < 2 || horizontalSlices < width - 2) && horizontalSlices - 1 < panel.MaxHorizontalSlices) 
			{
				horizontalSlices++;
				panel.HorizontalSlices = horizontalSlices - 1;
				Update();
			}
		}

		[BeginAction("decreasebevel")]
		protected void decreaseBevel()
		{
			if(!gridlock && horizontalSlices > 1) 
			{
				horizontalSlices--;
				panel.HorizontalSlices = horizontalSlices - 1;
				Update();
			}
		}

		[BeginAction("increasesubdivlevel")]
		protected void increaseSubdivLevel()
		{
			if(!gridlock && (points.Count < 2 || verticalSlices < height - 2) && verticalSlices - 1 < panel.MaxVerticalSlices) 
			{
				verticalSlices++;
				panel.VerticalSlices = verticalSlices - 1;
				Update();
			}
		}

		[BeginAction("decreasesubdivlevel")]
		protected void decreaseSubdivLevel()
		{
			if(!gridlock && verticalSlices > 1) 
			{
				verticalSlices--;
				panel.VerticalSlices = verticalSlices - 1;
				Update();
			}
		}

		#endregion

	}
}
