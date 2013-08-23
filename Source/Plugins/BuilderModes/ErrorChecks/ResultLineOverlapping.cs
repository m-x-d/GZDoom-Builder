
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
	public class ResultLineOverlapping : ErrorResult
	{
		#region ================== Variables
		
		private Linedef line1;
		private Linedef line2;
		
		#endregion
		
		#region ================== Properties

		public override int Buttons { get { return 0; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public ResultLineOverlapping(Linedef l1, Linedef l2)
		{
			// Initialize
			this.line1 = l1;
			this.line2 = l2;
			this.viewobjects.Add(l1);
			this.viewobjects.Add(l2);
			this.description = "These linedefs are overlapping and they do not reference the same sector on all sides. Overlapping lines is only allowed when they reference the same sector on all sides.";
		}
		
		#endregion
		
		#region ================== Methods
		
		// This must return the string that is displayed in the listbox
		public override string ToString()
		{
            return "Linedefs " + line1.Index + " and " + line2.Index + " are overlapping and reference different sectors";
		}
		
		// Rendering
		public override void PlotSelection(IRenderer2D renderer)
		{
			renderer.PlotLinedef(line1, General.Colors.Selection);
			renderer.PlotLinedef(line2, General.Colors.Selection);
			renderer.PlotVertex(line1.Start, ColorCollection.VERTICES);
			renderer.PlotVertex(line1.End, ColorCollection.VERTICES);
			renderer.PlotVertex(line2.Start, ColorCollection.VERTICES);
			renderer.PlotVertex(line2.End, ColorCollection.VERTICES);
		}
		
		#endregion
	}
}
