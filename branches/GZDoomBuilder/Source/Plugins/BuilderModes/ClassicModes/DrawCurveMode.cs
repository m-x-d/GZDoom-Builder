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

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Draw Curve Mode",
			  SwitchAction = "drawcurvemode",
			  AllowCopyPaste = false,
			  Volatile = true,
			  Optional = false)]

	public class DrawCurveMode : DrawGeometryMode
	{
		private HintLabel hintLabel;
		private Curve curve;
		private static int segmentLength = 32;
		private int minSegmentLength = 16;
		private int maxSegmentLength = 4096; //just some arbitrary number

		public DrawCurveMode() : base() {
			hintLabel = new HintLabel();
		}

		public override void Dispose() {
			if(!isdisposed && hintLabel != null)
				hintLabel.Dispose();

			base.Dispose();
		}

		protected override void Update() {
			PixelColor stitchcolor = General.Colors.Highlight;
			PixelColor losecolor = General.Colors.Selection;
			PixelColor color;

			snaptogrid = General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

			DrawnVertex curp = GetCurrentPosition();
			float vsize = ((float)renderer.VertexSize + 1.0f) / renderer.Scale;
			float vsizeborder = ((float)renderer.VertexSize + 3.0f) / renderer.Scale;

			// The last label's end must go to the mouse cursor
			if(labels.Count > 0)
				labels[labels.Count - 1].End = curp.pos;

			// Render drawing lines
			if(renderer.StartOverlay(true)) {
				// Go for all points to draw lines
				if(points.Count > 0) {
					//update curve
					List<Vector2D> verts = new List<Vector2D>();
					
					for(int i = 0; i < points.Count; i++)
						verts.Add(points[i].pos);

					if(curp.pos != verts[verts.Count-1])
						verts.Add(curp.pos);

					curve = CurveTools.CurveThroughPoints(verts, 0.5f, 0.75f, segmentLength);

					// Render lines
					for(int i = 1; i < curve.Shape.Count; i++) {
						// Determine line color
						color = snaptonearest ? stitchcolor : losecolor;

						// Render line
						renderer.RenderLine(curve.Shape[i - 1], curve.Shape[i], LINE_THICKNESS, color, true);
					}

					//render "inactive" vertices
					for(int i = 1; i < curve.Shape.Count - 1; i++) {
						// Determine vertex color
						color = !snaptonearest ? stitchcolor : losecolor;

						// Render vertex
						renderer.RenderRectangleFilled(new RectangleF(curve.Shape[i].x - vsize, curve.Shape[i].y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);
					}
				}

				if(points.Count > 0) {
					// Render vertices
					for(int i = 0; i < points.Count; i++) {
						// Determine vertex color
						color = points[i].stitch ? stitchcolor : losecolor;

						// Render vertex
						renderer.RenderRectangleFilled(new RectangleF(points[i].pos.x - vsize, points[i].pos.y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);
					}
				}

				// Determine point color
				color = snaptonearest ? stitchcolor : losecolor;

				// Render vertex at cursor
				renderer.RenderRectangleFilled(new RectangleF(curp.pos.x - vsize, curp.pos.y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);

				// Go for all labels
				foreach(LineLengthLabel l in labels)
					renderer.RenderText(l.TextLabel);

				//Render info label
				hintLabel.Start = new Vector2D(mousemappos.x + (32 / renderer.Scale), mousemappos.y - (16 / renderer.Scale));
				hintLabel.End = new Vector2D(mousemappos.x + (96 / renderer.Scale), mousemappos.y);
				hintLabel.Text = "SEG LEN: " + segmentLength;
				renderer.RenderText(hintLabel.TextLabel);

				// Done
				renderer.Finish();
			}

			// Done
			renderer.Present();
		}

		public override void OnAccept() {
			Cursor.Current = Cursors.AppStarting;

			General.Settings.FindDefaultDrawSettings();

			// When points have been drawn
			if(points.Count > 0) {
				// Make undo for the draw
				General.Map.UndoRedo.CreateUndo("Curve draw");

				// Make an analysis and show info
				string[] adjectives = new string[]
				{ "beautiful", "lovely", "romantic", "stylish", "cheerful", "comical",
				  "awesome", "accurate", "adorable", "adventurous", "attractive", "cute",
				  "elegant", "glamorous", "gorgeous", "handsome", "magnificent", "unusual",
				  "outstanding", "mysterious", "amusing", "charming", "fantastic", "jolly" };
				string word = adjectives[points.Count % adjectives.Length];
				word = (points.Count > adjectives.Length) ? "very " + word : word;
				string a = ((word[0] == 'a') || (word[0] == 'e') || (word[0] == 'o') || (word[0] == 'u')) ? "an " : "a ";
				General.Interface.DisplayStatus(StatusType.Action, "Created " + a + word + " curve.");

				List<DrawnVertex> verts = new List<DrawnVertex>();
				
				//if we have a curve...
				if(points.Count > 2){
					//is it a closed curve?
					int lastPoint = 0;
					if(points[0].pos == points[points.Count - 1].pos) {
						lastPoint = curve.Segments.Count;
					} else {
						lastPoint = curve.Segments.Count - 1;
					}

					for(int i = 0; i < lastPoint; i++) {
						int next = (i == curve.Segments.Count - 1 ? 0 : i + 1);
						bool stitch = points[i].stitch && points[next].stitch;
						bool stitchline = points[i].stitchline && points[next].stitchline;

						//add segment points except the last one
						for(int c = 0; c < curve.Segments[i].Points.Length - 1; c++) {
							DrawnVertex dv = new DrawnVertex();
							dv.pos = curve.Segments[i].Points[c];
							dv.stitch = stitch;
							dv.stitchline = stitchline;
							verts.Add(dv);
						}
					}

					//add last point
					DrawnVertex end = new DrawnVertex();
					end.pos = curve.Segments[lastPoint - 1].End;
					end.stitch = verts[verts.Count - 1].stitch;
					end.stitchline = verts[verts.Count - 1].stitchline;
					verts.Add(end);
				}else{
					verts = points;
				}

				// Make the drawing
				if(!Tools.DrawLines(verts, true, BuilderPlug.Me.AutoAlignTextureOffsetsOnCreate)) //mxd
				{
					// Drawing failed
					// NOTE: I have to call this twice, because the first time only cancels this volatile mode
					General.Map.UndoRedo.WithdrawUndo();
					General.Map.UndoRedo.WithdrawUndo();
					return;
				}

				// Snap to map format accuracy
				General.Map.Map.SnapAllToAccuracy();

				// Clear selection
				General.Map.Map.ClearAllSelected();

				// Update cached values
				General.Map.Map.Update();

				// Edit new sectors?
				List<Sector> newsectors = General.Map.Map.GetMarkedSectors(true);
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

		//mxd. Setup hints for current editing mode
		protected override void SetupHints() {
			string selectKey = Actions.Action.GetShortcutKeyDesc("builder_classicselect");
			string editKey = Actions.Action.GetShortcutKeyDesc("builder_classicedit");
			string acceptKey = Actions.Action.GetShortcutKeyDesc("builder_acceptmode");
			string cancelKey = Actions.Action.GetShortcutKeyDesc("builder_cancelmode");
			string removeKey = Actions.Action.GetShortcutKeyDesc("buildermodes_removepoint");
			string incSub = Actions.Action.GetShortcutKeyDesc("buildermodes_increasesubdivlevel");
			string decSub = Actions.Action.GetShortcutKeyDesc("buildermodes_decreasesubdivlevel");

			hints = new[]{"Press " + selectKey + " to place a vertex",
						  "Use " + incSub + " and " + decSub + " to change detail level of the curve",
						  "Press " + removeKey + " to remove last vertex",
						  "Press " + acceptKey + " to accept",
						  "Press " + cancelKey + " or " + editKey + " to cancel"
			};
		}

		//ACTIONS
		[BeginAction("increasesubdivlevel")]
		protected virtual void increaseSubdivLevel() {
			if(segmentLength < maxSegmentLength) {
				int increment = Math.Max(minSegmentLength, segmentLength / 32 * 16);
				segmentLength += increment;

				if(segmentLength > maxSegmentLength)
					segmentLength = maxSegmentLength;
				Update();
			}
		}

		[BeginAction("decreasesubdivlevel")]
		protected virtual void decreaseSubdivLevel() {
			if(segmentLength > minSegmentLength) {
				int increment = Math.Max(minSegmentLength, segmentLength / 32 * 16);
				segmentLength -= increment;

				if(segmentLength < minSegmentLength)
					segmentLength = minSegmentLength;
				Update();
			}
		}
	}
}
