using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

namespace CodeImp.DoomBuilder.BuilderModes.ErrorChecks {
	public class ResultUnknownThing : ErrorResult {
		public override int Buttons { get { return 1; } }
		public override string Button1Text { get { return "Delete Thing"; } }

		private Thing thing;

		// Constructor
		public ResultUnknownThing(Thing t) {
			// Initialize
			this.thing = t;
			this.viewobjects.Add(t);
			this.description = "This thing has unknown type (eg. it's not defined in DECORATE or current game configuration).";
		}

		// This must return the string that is displayed in the listbox
		public override string ToString() {
			return "Thing " + thing.Index + " at " + thing.Position.x + ", " + thing.Position.y + " has unknown type (" + thing.Type + ").";
		}

		// Rendering
		public override void  RenderOverlaySelection(IRenderer2D renderer) {
			renderer.RenderThing(thing, renderer.DetermineThingColor(thing), 1.0f);
		}
		
		// This removes the thing
		public override bool Button1Click(bool batchMode) {
			if(!batchMode) General.Map.UndoRedo.CreateUndo("Delete thing");
			thing.Dispose();
			General.Map.IsChanged = true;
			General.Map.ThingsFilter.Update();
			return true;
		}
	}
}
