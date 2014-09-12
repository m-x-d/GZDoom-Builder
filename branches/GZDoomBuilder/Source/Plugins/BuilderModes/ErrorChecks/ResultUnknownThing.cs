#region ================== Namespaces

using System;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.ErrorChecks {
	public class ResultUnknownThing : ErrorResult
	{

		#region ================== Variables

		private readonly Thing thing;

		#endregion

		#region ================== Properties

		public override int Buttons { get { return 1; } }
		public override string Button1Text { get { return "Delete Thing"; } }

		#endregion

		#region ================== Constructor / Destructor

		public ResultUnknownThing(Thing t) 
		{
			// Initialize
			thing = t;
			viewobjects.Add(t);
			hidden = t.IgnoredErrorChecks.Contains(this.GetType()); //mxd
			description = "This thing has unknown type (eg. it's not defined in DECORATE or current game configuration).";
		}

		#endregion

		#region ================== Methods

		// This sets if this result is displayed in ErrorCheckForm (mxd)
		internal override void Hide(bool hide) 
		{
			hidden = hide;
			Type t = this.GetType();
			if(hide) thing.IgnoredErrorChecks.Add(t);
			else if(thing.IgnoredErrorChecks.Contains(t)) thing.IgnoredErrorChecks.Remove(t);
		}

		// This must return the string that is displayed in the listbox
		public override string ToString() 
		{
			return "Thing " + thing.Index + " at " + thing.Position.x + ", " + thing.Position.y + " has unknown type (" + thing.Type + ").";
		}

		// Rendering
		public override void  RenderOverlaySelection(IRenderer2D renderer) 
		{
			renderer.RenderThing(thing, renderer.DetermineThingColor(thing), 1.0f);
		}
		
		// This removes the thing
		public override bool Button1Click(bool batchMode) 
		{
			if(!batchMode) General.Map.UndoRedo.CreateUndo("Delete thing");
			thing.Dispose();
			General.Map.IsChanged = true;
			General.Map.ThingsFilter.Update();
			return true;
		}

		#endregion
	}
}
