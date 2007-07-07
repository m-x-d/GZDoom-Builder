
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

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	internal class FrozenOverviewMode : ViewClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public FrozenOverviewMode()
		{
			float left = float.MaxValue;
			float top = float.MaxValue;
			float right = float.MinValue;
			float bottom = float.MinValue;
			float scalew, scaleh, scale;
			float width, height;
			
			// Go for all vertices
			foreach(Vertex v in General.Map.Data.Vertices)
			{
				// Adjust boundaries by vertices
				if(v.Position.x < left) left = v.Position.x;
				if(v.Position.x > right) right = v.Position.x;
				if(v.Position.y < top) top = v.Position.y;
				if(v.Position.y > bottom) bottom = v.Position.y;
			}

			// Calculate width/height
			width = (right - left);
			height = (bottom - top);

			// Calculate scale to view map at
			scalew = (float)General.Map.Graphics.RenderTarget.ClientSize.Width / (width * 1.1f);
			scaleh = (float)General.Map.Graphics.RenderTarget.ClientSize.Height / (height * 1.1f);
			if(scalew < scaleh) scale = scalew; else scale = scaleh;

			// Change the view to see the whole map
			renderer.ScaleView(scale);
			renderer.PositionView(left + (right - left) * 0.5f, top + (bottom - top) * 0.5f);
			General.Map.Data.Update();
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

		// This redraws the display
		public override void RedrawDisplay()
		{
			if(renderer.StartRendering())
			{
				renderer.RenderLinedefs(General.Map.Data, General.Map.Data.Linedefs);
				renderer.RenderVertices(General.Map.Data, General.Map.Data.Vertices);
				renderer.FinishRendering();
			}
		}
		
		#endregion
	}
}
