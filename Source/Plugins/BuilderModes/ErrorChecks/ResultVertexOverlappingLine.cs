#region ================== Namespaces

using System.Collections.Generic;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class ResultVertexOverlappingLine : ErrorResult
	{
		#region ================== Variables

		private Linedef line;
		private Vertex vertex;

		#endregion

		#region ================== Properties

		public override int Buttons { get { return 1; } }
		public override string Button1Text { get { return "Split Linedef"; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public ResultVertexOverlappingLine(Vertex v, Linedef l)
		{
			// Initialize
			this.line = l;
			this.vertex = v;
			this.viewobjects.Add(l);
			this.viewobjects.Add(v);
			this.description = "This vertex overlaps this linedef without splitting it.";
		}

		#endregion

		#region ================== Methods

		// This must return the string that is displayed in the listbox
		public override string ToString() {
			return "Vertex " + vertex.Index + " overlaps line " + line.Index + " without splitting it";
		}

		// Rendering
		public override void PlotSelection(IRenderer2D renderer) {
			renderer.PlotLinedef(line, General.Colors.Selection);
			renderer.PlotVertex(line.Start, ColorCollection.VERTICES);
			renderer.PlotVertex(line.End, ColorCollection.VERTICES);

			renderer.PlotVertex(vertex, ColorCollection.SELECTION);
		}

		// Fix by splitting the line
		public override bool Button1Click(bool batchMode) {
			if(!batchMode) General.Map.UndoRedo.CreateUndo("Split Linedef");
			line.Split(vertex);

			//check that we don't have duplicate lines
			List<Linedef> lines = new List<Linedef>(vertex.Linedefs);

			for(int i = 0; i < lines.Count - 1; i++) {
				for(int c = i + 1; c < lines.Count; c++) {
					if( (lines[i].Start == lines[c].Start && lines[i].End == lines[c].End) || 
						(lines[i].Start == lines[c].End && lines[i].End == lines[c].Start)) {
						lines[c].Join(lines[i]);
					}
				}
			}

			General.Map.Map.Update();
			return true;
		}
		
		#endregion
	}
}
