
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Interface;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	internal class LinedefsMode : ClassicMode
	{
		#region ================== Constants

		protected const float LINEDEF_HIGHLIGHT_RANGE = 20f;

		#endregion

		#region ================== Variables

		// Highlighted item
		private Linedef highlighted;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public LinedefsMode()
		{
		}

		// Diposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// Mode engages
		public override void Engage()
		{
			base.Engage();

			// Check linedefs button on main window
			General.MainWindow.SetLinedefsChecked(true);
		}

		// Mode disengages
		public override void Disengage()
		{
			base.Disengage();

			// Check linedefs button on main window
			General.MainWindow.SetLinedefsChecked(false);
		}

		// This redraws the display
		public unsafe override void RedrawDisplay()
		{
			// Start with a clear display
			if(renderer.StartRendering(true))
			{
				// Render things
				renderer.SetThingsRenderOrder(false);
				renderer.RenderThingSet(General.Map.Map.Things);
				
				// Render lines
				renderer.RenderLinedefSet(General.Map.Map.Linedefs);

				// Render highlighted item
				if(highlighted != null)
					renderer.RenderLinedef(highlighted, General.Colors.Highlight);

				// Render vertices
				renderer.RenderVerticesSet(General.Map.Map.Vertices);

				// Done
				renderer.FinishRendering();
			}
		}

		// This highlights a new item
		protected void Highlight(Linedef l)
		{
			// Update display
			if(renderer.StartRendering(false))
			{
				// Undraw previous highlight
				if(highlighted != null)
				{
					renderer.RenderLinedef(highlighted, renderer.DetermineLinedefColor(highlighted));
					renderer.RenderVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
					renderer.RenderVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
				}
				
				// Set new highlight
				highlighted = l;

				// Render highlighted item
				if(highlighted != null)
				{
					renderer.RenderLinedef(highlighted, General.Colors.Highlight);
					renderer.RenderVertex(highlighted.Start, renderer.DetermineVertexColor(highlighted.Start));
					renderer.RenderVertex(highlighted.End, renderer.DetermineVertexColor(highlighted.End));
				}
				
				// Done
				renderer.FinishRendering();
			}
		}

		// Mouse moves
		public override void MouseMove(MouseEventArgs e)
		{
			base.MouseMove(e);

			// Find the nearest linedef within highlight range
			Linedef l = General.Map.Map.NearestLinedefRange(mousemappos, LINEDEF_HIGHLIGHT_RANGE / renderer.Scale);

			// Highlight if not the same
			if(l != highlighted) Highlight(l);
		}

		// Mouse leaves
		public override void MouseLeave(EventArgs e)
		{
			base.MouseLeave(e);

			// Highlight nothing
			Highlight(null);
		}

		#endregion
	}
}
