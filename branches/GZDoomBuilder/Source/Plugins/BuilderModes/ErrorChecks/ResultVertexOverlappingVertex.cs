#region ================== Namespaces

using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class ResultVertexOverlappingVertex : ErrorResult
	{
		#region ================== Variables

		private Vertex vertex1;
		private Vertex vertex2;

		#endregion

		#region ================== Properties

		public override int Buttons { get { return 1; } }
		public override string Button1Text { get { return "Merge Vertices"; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public ResultVertexOverlappingVertex(Vertex v1, Vertex v2)
		{
			// Initialize
			this.vertex1 = v1;
			this.vertex2 = v2;
			this.viewobjects.Add(v1);
			this.viewobjects.Add(v2);
			this.description = "These vertices have the same position.";
		}

		#endregion

		#region ================== Methods

		// This must return the string that is displayed in the listbox
		public override string ToString() {
			return "Vertices " + vertex1.Index + " and " + vertex2.Index + " have the same position";
		}

		// Rendering
		public override void PlotSelection(IRenderer2D renderer) {
			renderer.PlotVertex(vertex1, ColorCollection.SELECTION);
		}

		// Fix by splitting the line
		public override bool Button1Click(bool batchMode) {
			if(!batchMode) General.Map.UndoRedo.CreateUndo("Merge vertices");
			vertex2.Join(vertex1);
			General.Map.Map.Update();
			return true;
		}

		#endregion
	}
}
