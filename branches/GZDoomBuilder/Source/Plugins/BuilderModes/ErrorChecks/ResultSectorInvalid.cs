#region ================== Namespaces

using System;
using System.Collections.Generic;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class ResultSectorInvalid : ErrorResult
	{
		#region ================== Variables

		private readonly Sector sector;

		#endregion

		#region ================== Properties

		public override int Buttons { get { return 1; } }
		public override string Button1Text { get { return "Dissolve"; } }

		#endregion

		#region ================== Constructor / Destructor
		
		// Constructor
		public ResultSectorInvalid(Sector s)
		{
			// Initialize
			sector = s;
			viewobjects.Add(s);
			hidden = s.IgnoredErrorChecks.Contains(this.GetType()); //mxd
			description = "This sector has invalid geometry (it has less than 3 sidedefs or it's area is 0). This could cause problems with clipping and rendering in the game.";
		}
		
		#endregion
		
		#region ================== Methods

		// This sets if this result is displayed in ErrorCheckForm (mxd)
		internal override void Hide(bool hide) 
		{
			hidden = hide;
			Type t = this.GetType();
			if(hide) sector.IgnoredErrorChecks.Add(t);
			else if(sector.IgnoredErrorChecks.Contains(t)) sector.IgnoredErrorChecks.Remove(t);
		}
		
		// This must return the string that is displayed in the listbox
		public override string ToString() {
			if (sector.Sidedefs != null && sector.Sidedefs.Count > 2)
				return "Area of sector " + sector.Index + " is 0";
			return "Sector " + sector.Index + " has " + (sector.Sidedefs == null ? "no" : sector.Sidedefs.Count.ToString()) + " sidedefs";
		}
		
		// Rendering
		public override void PlotSelection(IRenderer2D renderer)
		{
			renderer.PlotSector(sector, General.Colors.Selection);
		}

		// Fix by merging with surrounding geometry/removing
		public override bool Button1Click(bool batchMode) {
			if(!batchMode) General.Map.UndoRedo.CreateUndo("Invalid sector correction");

			//collect the lines
			List<Linedef> lines = new List<Linedef>();
			foreach (Sidedef side in sector.Sidedefs) {
				if (!lines.Contains(side.Line) && !side.Line.IsDisposed) 
					lines.Add(side.Line);
			}

			//get rid of lines with zero length
			foreach (Linedef line in lines)
				if (line.Length == 0) line.Dispose();

			if (lines.Count == 0) {
				sector.Dispose();
			} else if(lines.Count < 3) { //merge with surrounding geometry
				bool merged = false;
				foreach(Sidedef side in sector.Sidedefs) {
					if(side.Other != null && side.Other.Sector != null) {
						sector.Join(side.Other.Sector);
						merged = true;
						break;
					}
				}

				//oh well, I don't know what else I can do here...
				if(!merged) sector.Dispose();
			} else { //redraw the lines
				foreach(Linedef line in lines) {
					if(line.IsDisposed) continue;
					DrawnVertex start = new DrawnVertex { pos = line.Start.Position, stitch = true, stitchline = true };
					DrawnVertex end = new DrawnVertex { pos = line.End.Position, stitch = true, stitchline = true };
					Tools.DrawLines(new List<DrawnVertex> { start, end }, false, false);
				}
			}

			General.Map.Map.Update();
			return true;
		}
		
		#endregion
	}
}
