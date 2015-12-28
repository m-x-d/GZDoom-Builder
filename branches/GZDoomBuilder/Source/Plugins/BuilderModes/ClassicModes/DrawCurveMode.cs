﻿#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Draw Curve Mode",
			  SwitchAction = "drawcurvemode",
			  ButtonImage = "DrawCurveMode.png", //mxd	
			  ButtonOrder = int.MinValue + 2, //mxd
			  ButtonGroup = "000_drawing", //mxd
			  AllowCopyPaste = false,
			  Volatile = true,
			  Optional = false)]

	public class DrawCurveMode : DrawGeometryMode
	{
		#region ================== Variables

		private readonly HintLabel hintlabel;
		private Curve curve;
		private static int segmentLength = 32;
		private const int MIN_SEGMENT_LENGTH = 16;
		private const int MAX_SEGMENT_LENGTH = 4096; //just some arbitrary number

		//interface
		private readonly DrawCurveOptionsPanel panel;

		#endregion

		#region ================== Constructor/Disposer

		public DrawCurveMode() 
		{
			hintlabel = new HintLabel(General.Colors.InfoLine);
			labeluseoffset = false;

			//Options docker
			panel = new DrawCurveOptionsPanel(MIN_SEGMENT_LENGTH, MAX_SEGMENT_LENGTH);
			panel.OnValueChanged += OptionsPanelOnValueChanged;
		}

		public override void Dispose() 
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				if(hintlabel != null) hintlabel.Dispose();

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		protected override void Update() 
		{
			PixelColor stitchcolor = General.Colors.Highlight;
			PixelColor losecolor = General.Colors.Selection;

			snaptocardinaldirection = General.Interface.ShiftState && General.Interface.AltState;
			snaptogrid = snaptocardinaldirection || General.Interface.ShiftState ^ General.Interface.SnapToGrid;
			snaptonearest = General.Interface.CtrlState ^ General.Interface.AutoMerge;

			DrawnVertex curp = GetCurrentPosition();
			float vsize = (renderer.VertexSize + 1.0f) / renderer.Scale;

			// Update label positions (mxd)
			if(labels.Count > 0)
			{
				// Update labels for already drawn lines
				for(int i = 0; i < labels.Count - 1; i++)
					labels[i].Move(points[i].pos, points[i + 1].pos);

				// Update label for active line
				labels[labels.Count - 1].Move(points[points.Count - 1].pos, curp.pos);
			}

			// Render drawing lines
			if(renderer.StartOverlay(true)) 
			{
				// Go for all points to draw lines
				if(points.Count > 0) 
				{
					//update curve
					List<Vector2D> verts = new List<Vector2D>();
					for(int i = 0; i < points.Count; i++) verts.Add(points[i].pos);
					if(curp.pos != verts[verts.Count-1]) verts.Add(curp.pos);
					curve = CurveTools.CurveThroughPoints(verts, 0.5f, 0.75f, segmentLength);

					// Render lines
					for(int i = 1; i < curve.Shape.Count; i++) 
					{
						// Determine line color
						PixelColor c = snaptonearest ? stitchcolor : losecolor;

						// Render line
						renderer.RenderLine(curve.Shape[i - 1], curve.Shape[i], LINE_THICKNESS, c, true);
					}

					//render "inactive" vertices
					for(int i = 1; i < curve.Shape.Count - 1; i++) 
					{
						// Determine vertex color
						PixelColor c = !snaptonearest ? stitchcolor : losecolor;

						// Render vertex
						renderer.RenderRectangleFilled(new RectangleF(curve.Shape[i].x - vsize, curve.Shape[i].y - vsize, vsize * 2.0f, vsize * 2.0f), c, true);
					}
				}

				if(points.Count > 0) 
				{
					// Render vertices
					for(int i = 0; i < points.Count; i++) 
					{
						// Determine vertex color
						PixelColor c = points[i].stitch ? stitchcolor : losecolor;

						// Render vertex
						renderer.RenderRectangleFilled(new RectangleF(points[i].pos.x - vsize, points[i].pos.y - vsize, vsize * 2.0f, vsize * 2.0f), c, true);
					}
				}

				// Determine point color
				PixelColor color = snaptonearest ? stitchcolor : losecolor;

				// Render vertex at cursor
				renderer.RenderRectangleFilled(new RectangleF(curp.pos.x - vsize, curp.pos.y - vsize, vsize * 2.0f, vsize * 2.0f), color, true);

				// Go for all labels
				foreach(LineLengthLabel l in labels) renderer.RenderText(l.TextLabel);

				//Render info label
				Vector2D start = new Vector2D(mousemappos.x + (32 / renderer.Scale), mousemappos.y - (16 / renderer.Scale));
				Vector2D end = new Vector2D(mousemappos.x + (96 / renderer.Scale), mousemappos.y);
				hintlabel.Move(start, end);
				hintlabel.Text = "SEG LEN: " + segmentLength;
				renderer.RenderText(hintlabel.TextLabel);

				// Done
				renderer.Finish();
			}

			// Done
			renderer.Present();
		}

		#endregion

		#region ================== Events

		public override void OnEngage() 
		{
			base.OnEngage();

			//setup settings panel
			panel.SegmentLength = segmentLength;
			panel.Register();
		}

		public override void OnAccept() 
		{
			Cursor.Current = Cursors.AppStarting;
			General.Settings.FindDefaultDrawSettings();

			// When points have been drawn
			if(points.Count > 0) 
			{
				// Make undo for the draw
				General.Map.UndoRedo.CreateUndo("Curve draw");

				// Make an analysis and show info
				string[] adjectives = new[] 
				{
				  "beautiful", "lovely", "romantic", "stylish", "cheerful", "comical",
				  "awesome", "accurate", "adorable", "adventurous", "attractive", "cute",
				  "elegant", "glamorous", "gorgeous", "handsome", "magnificent", "unusual",
				  "outstanding", "mysterious", "amusing", "charming", "fantastic", "jolly" 
				};
				string word = adjectives[points.Count % adjectives.Length];
				word = (points.Count > adjectives.Length) ? "very " + word : word;
				string a = ((word[0] == 'a') || (word[0] == 'e') || (word[0] == 'o') || (word[0] == 'u')) ? "an " : "a ";
				General.Interface.DisplayStatus(StatusType.Action, "Created " + a + word + " curve.");

				List<DrawnVertex> verts = new List<DrawnVertex>();
				
				//if we have a curve...
				if(points.Count > 2)
				{
					//is it a closed curve?
					int lastPoint;
					if(points[0].pos == points[points.Count - 1].pos) 
					{
						lastPoint = curve.Segments.Count;
					}
					else 
					{
						lastPoint = curve.Segments.Count - 1;
					}

					for(int i = 0; i < lastPoint; i++) 
					{
						int next = (i == curve.Segments.Count - 1 ? 0 : i + 1);
						bool stitch = points[i].stitch && points[next].stitch;
						bool stitchline = points[i].stitchline && points[next].stitchline;

						//add segment points except the last one
						for(int c = 0; c < curve.Segments[i].Points.Length - 1; c++) 
						{
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
				}
				else
				{
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

				//mxd
				General.Map.Renderer2D.UpdateExtraFloorFlag();

				// Map is changed
				General.Map.IsChanged = true;
			}

			// Done
			Cursor.Current = Cursors.Default;

			// Return to original mode
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		public override void OnDisengage() 
		{
			base.OnDisengage();
			panel.Unregister();
		}

		private void OptionsPanelOnValueChanged(object sender, EventArgs eventArgs) 
		{
			segmentLength = panel.SegmentLength;
			Update();
		}

		public override void OnHelp() 
		{
			General.ShowHelp("/gzdb/features/classic_modes/mode_drawcurve.html");
		}

		#endregion

		#region ================== Actions

		[BeginAction("increasesubdivlevel")]
		protected virtual void IncreaseSubdivLevel() 
		{
			if(segmentLength < MAX_SEGMENT_LENGTH) 
			{
				int increment = Math.Max(MIN_SEGMENT_LENGTH, segmentLength / 32 * 16);
				segmentLength += increment;

				if(segmentLength > MAX_SEGMENT_LENGTH) segmentLength = MAX_SEGMENT_LENGTH;
				panel.SegmentLength = segmentLength;
				Update();
			}
		}

		[BeginAction("decreasesubdivlevel")]
		protected virtual void DecreaseSubdivLevel() 
		{
			if(segmentLength > MIN_SEGMENT_LENGTH) 
			{
				int increment = Math.Max(MIN_SEGMENT_LENGTH, segmentLength / 32 * 16);
				segmentLength -= increment;

				if(segmentLength < MIN_SEGMENT_LENGTH) segmentLength = MIN_SEGMENT_LENGTH;
				panel.SegmentLength = segmentLength;
				Update();
			}
		}

		#endregion

	}
}
