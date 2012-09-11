using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;


namespace CodeImp.DoomBuilder.BuilderModes.ClassicModes
{
	[EditMode(DisplayName = "Redraw Sector Mode",
			  SwitchAction = "redrawsectormode",
			  AllowCopyPaste = false,
			  Volatile = true,
			  Optional = true)]

	public class RedrawSectorMode : BaseClassicMode
	{

		public RedrawSectorMode() : base() {
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		public override void OnEngage() {
			base.OnEngage();

			//Get selection
			General.Map.Map.ClearAllMarks(false);
			General.Map.Map.MarkSelectedSectors(true, true);
			List<Sector> sectors = General.Map.Map.GetMarkedSectors(true);

			if(sectors.Count == 0) {
				disengage("Select some sectors first!");
				return;
			}

			Cursor.Current = Cursors.AppStarting;
			//General.Settings.FindDefaultDrawSettings();
			ICollection<Vertex> points = General.Map.Map.GetVerticesFromSectorsMarks(true);

			// Make undo for the draw
			General.Map.UndoRedo.CreateUndo(sectors.Count > 1 ? "Redraw " + sectors.Count + " sectors" : "Redraw sector");

			foreach(Sector s in sectors){
				// Make the drawing
				if(s.Sidedefs == null) {
					GZBuilder.GZGeneral.LogAndTraceWarning("RedrawSectorMode: sector " + s.Index + " has no sidedefs!");
					continue;
				}

				if(!Tools.DrawLines(getPoints(s.Sidedefs))) {
					// Drawing failed
					// NOTE: I have to call this twice, because the first time only cancels this volatile mode
					General.Map.UndoRedo.WithdrawUndo();
					General.Map.UndoRedo.WithdrawUndo();
					disengage("Drawing failed");
					return;
				}
			}
			
			General.Interface.DisplayStatus(StatusType.Info, "Redrawn " + sectors.Count + " sectors.");
			base.OnAccept();
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name); // Return to original mode
		}

		// Disenagaging
		public override void OnDisengage() {
			base.OnDisengage();
			Cursor.Current = Cursors.AppStarting;

			if(!cancelled) {
				General.Map.Map.SnapAllToAccuracy(); // Snap to map format accuracy
				General.Map.Map.ClearAllSelected(); // Clear selection
				General.Map.Map.Update(); // Update cached values
				General.Map.Data.UpdateUsedTextures(); // Update the used textures
				General.Map.IsChanged = true; // Map is changed
			}

			Cursor.Current = Cursors.Default; // Done
		}

		private List<DrawnVertex> getPoints(ICollection<Sidedef> sidedefs) {
			List<int> indeces = new List<int>();
			List<Vector2D> points = new List<Vector2D>();
			
			Sidedef[] sidedefArr = new Sidedef[sidedefs.Count];
			sidedefs.CopyTo(sidedefArr, 0);

			points.Add(sidedefArr[0].Line.Start.Position);
			points.Add(sidedefArr[0].Line.End.Position);
			indeces.Add(sidedefArr[0].Line.Index);

			//sort points
			for(int i = 1; i < sidedefArr.Length; i++){
				if(indeces.Contains(sidedefArr[i].Line.Index))
					continue;

				Vector2D nextPoint = new Vector2D();
				if(getNextPoint(points[points.Count - 1], sidedefArr, ref indeces, ref nextPoint))
					points.Add(nextPoint);
			}

			if(points.Count == sidedefArr.Length) //add closing point
				points.Add(sidedefArr[0].Line.Start.Position); 
			
			List<DrawnVertex> vl = new List<DrawnVertex>();

			foreach(Vector2D v in points) {
				DrawnVertex dv = new DrawnVertex();
				dv.pos = v;
				dv.stitch = true;
				dv.stitchline = true;
				vl.Add(dv);
			}

			return vl;
		}

		private bool getNextPoint(Vector2D v, Sidedef[] sidedefArr, ref List<int> indeces, ref Vector2D nextPoint) {
			foreach(Sidedef s in sidedefArr) {
				if(indeces.Contains(s.Line.Index))
					continue;

				if(s.Line.Start.Position == v) {
					indeces.Add(s.Line.Index);
					nextPoint = s.Line.End.Position;
					return true;
				} else if(s.Line.End.Position == v) {
					indeces.Add(s.Line.Index);
					nextPoint = s.Line.Start.Position;
					return true;
				}
			}
			return false;
		}

		private void disengage(string reason) {
			General.Interface.DisplayStatus(StatusType.Warning, reason);
			base.OnCancel();
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}
	}
}
