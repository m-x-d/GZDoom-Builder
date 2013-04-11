
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class ResultStuckThingInLine : ErrorResult
	{
		#region ================== Variables

		private Thing thing;
		
		#endregion
		
		#region ================== Properties

		public override int Buttons { get { return 1; } }
		public override string Button1Text { get { return "Delete Thing"; } }

		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public ResultStuckThingInLine(Thing t)
		{
			// Initialize
			this.thing = t;
			this.viewobjects.Add(t);
			this.description = "This thing is stuck in a wall (single-sided line) and will likely not be able to move around.";
		}
		
		#endregion
		
		#region ================== Methods
		
		// This must return the string that is displayed in the listbox
		public override string ToString()
		{
			return General.Map.Data.GetThingInfo(thing.Type).Title + " is stuck in a wall at " + thing.Position.x + ", " + thing.Position.y;
		}

		// Rendering
		public override void RenderOverlaySelection(IRenderer2D renderer)
		{
			renderer.RenderThing(thing, renderer.DetermineThingColor(thing), 1.0f);
		}

		// This removes the thing
		public override bool Button1Click()
		{
			General.Map.UndoRedo.CreateUndo("Delete thing");
			thing.Dispose();
			General.Map.IsChanged = true;
			General.Map.ThingsFilter.Update();
			return true;
		}
		
		#endregion
	}
}
