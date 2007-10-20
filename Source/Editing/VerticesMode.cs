
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
	internal class VerticesMode : ClassicMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		/// <summary>
		/// Fresh mode
		/// </summary>
		public VerticesMode()
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
		
		// This redraws the display
		public unsafe override void RedrawDisplay()
		{
			// Start with a clear display
			if(renderer.StartRendering(true))
			{
				// Render stuff
				renderer.RenderLinedefs(General.Map.Map, General.Map.Map.Linedefs, null);
				renderer.RenderVertices(General.Map.Map, General.Map.Map.Vertices, null);

				// Done
				renderer.FinishRendering();
			}
		}
		
		#endregion
	}
}
