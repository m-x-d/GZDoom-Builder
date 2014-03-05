#region ================== Namespaces

using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class ResultMissingFlat : ErrorResult
	{
		#region ================== Variables
		
		private Sector sector;
		private bool ceiling;
		
		#endregion
		
		#region ================== Properties

		public override int Buttons { get { return 1; } }
		public override string Button1Text { get { return "Add Default Flat"; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public ResultMissingFlat(Sector s, bool ceiling)
		{
			// Initialize
			this.sector = s;
			this.ceiling = ceiling;
			this.viewobjects.Add(s);
			
			string objname = ceiling ? "ceiling" : "floor";
			this.description = "This sector's " + objname + " is missing a flat where it is required and could cause a 'Hall Of Mirrors' visual problem in the map. Click the 'Add Default Flat' button to add a flat to the sector.";
		}
		
		#endregion
		
		#region ================== Methods
		
		// This must return the string that is displayed in the listbox
		public override string ToString()
		{
			return "Sector " + sector.Index + " has no " + (ceiling ? "ceiling" : "floor") + " flat.";
		}
		
		// Rendering
		public override void PlotSelection(IRenderer2D renderer)
		{
			renderer.PlotSector(sector, General.Colors.Selection);
		}

		//mxd. More rendering
		public override void RenderOverlaySelection(IRenderer2D renderer) {
			if(!BuilderPlug.Me.UseHighlight) return;
			renderer.RenderHighlight(sector.FlatVertices, General.Colors.Selection.WithAlpha(64).ToInt());
		}
		
		// Fix by setting default flat
		public override bool Button1Click(bool batchMode)
		{
			if(!batchMode) General.Map.UndoRedo.CreateUndo("Missing flat correction");
			General.Settings.FindDefaultDrawSettings();
			
			if(ceiling)
				sector.SetCeilTexture(General.Map.Options.DefaultCeilingTexture);
			else
				sector.SetFloorTexture(General.Map.Options.DefaultFloorTexture);
			
			General.Map.Map.Update();
			return true;
		}
		
		#endregion
	}
}
