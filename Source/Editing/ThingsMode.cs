
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
	internal class ThingsMode : ClassicMode
	{
		#region ================== Constants

		protected const float THING_HIGHLIGHT_RANGE = 10f;

		#endregion

		#region ================== Variables

		// Highlighted item
		private Thing highlighted;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ThingsMode()
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

			// Check things button on main window
			General.MainWindow.SetThingsChecked(true);
		}

		// Mode disengages
		public override void Disengage()
		{
			base.Disengage();

			// Check things button on main window
			General.MainWindow.SetThingsChecked(false);
		}

		// This redraws the display
		public unsafe override void RedrawDisplay()
		{
			// Start with a clear display
			if(renderer.StartRendering(true, true))
			{
				// Render lines and vertices
				renderer.RenderLinedefSet(General.Map.Map.Linedefs);
				renderer.RenderVerticesSet(General.Map.Map.Vertices);

				// Render things
				renderer.SetThingsRenderOrder(true);
				renderer.RenderThingSet(General.Map.Map.Things);

				// Render highlighted item
				if(highlighted != null)
					renderer.RenderThing(highlighted, General.Colors.Highlight);

				// Done
				renderer.FinishRendering();
			}
		}

		// This highlights a new item
		protected void Highlight(Thing t)
		{
			// Update display
			if(renderer.StartRendering(false, false))
			{
				// Undraw previous highlight
				if(highlighted != null)
					renderer.RenderThing(highlighted, renderer.DetermineThingColor(highlighted));

				// Set new highlight
				highlighted = t;

				// Render highlighted item
				if(highlighted != null)
					renderer.RenderThing(highlighted, General.Colors.Highlight);

				// Done
				renderer.FinishRendering();
			}
		}

		// Mouse moves
		public override void MouseMove(MouseEventArgs e)
		{
			base.MouseMove(e);

			// Find the nearest vertex within highlight range
			Thing t = General.Map.Map.NearestThingSquareRange(mousemappos, THING_HIGHLIGHT_RANGE / renderer.Scale);

			// Highlight if not the same
			if(t != highlighted) Highlight(t);
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
