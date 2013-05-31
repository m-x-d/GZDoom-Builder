using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using System.Drawing;

namespace CodeImp.DoomBuilder.BuilderModes.ErrorChecks
{
	public class ResultStrayVertex : ErrorResult
	{
		public override int Buttons { get { return 1; } }
		public override string Button1Text { get { return "Delete Vertex"; } }

		private Vertex vertex;

		// Constructor
		public ResultStrayVertex(Vertex v) {
			// Initialize
			this.vertex = v;
			this.viewobjects.Add(v);
			this.description = "This vertex is not connected to any linedef.";
		}

		// This must return the string that is displayed in the listbox
		public override string ToString() {
			return "Vertex " + vertex.Index + " at " + vertex.Position.x + ", " + vertex.Position.y + " is not connected to any linedef.";
		}

		// Rendering
		public override void RenderOverlaySelection(IRenderer2D renderer) {
			renderer.RenderRectangleFilled(new RectangleF(vertex.Position.x - 3, vertex.Position.y - 3, 6f, 6f), General.Colors.Selection, true);
		}

		// This removes the vertex
		public override bool Button1Click() {
			General.Map.UndoRedo.CreateUndo("Delete vertex");
			vertex.Dispose();
			General.Map.IsChanged = true;
			General.Map.ThingsFilter.Update();
			return true;
		}
	}
}
