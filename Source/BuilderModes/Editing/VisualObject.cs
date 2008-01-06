
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
using System.Drawing;
using System.ComponentModel;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.Editing
{
	internal class VisualObject : VisualGeometry
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public VisualObject()
		{
			// Initialize
			WorldVertex[] v = new WorldVertex[6];

			v[0].c = -1;
			v[0].x = 0.0f;
			v[0].y = 0.0f;
			v[0].z = 0.0f;
			v[0].u = 0.0f;
			v[0].v = 1.0f;

			v[1].c = -1;
			v[1].x = 0.0f;
			v[1].y = 0.0f;
			v[1].z = 100.0f;
			v[1].u = 0.0f;
			v[1].v = 0.0f;

			v[2].c = -1;
			v[2].x = 100.0f;
			v[2].y = 0.0f;
			v[2].z = 100.0f;
			v[2].u = 1.0f;
			v[2].v = 0.0f;

			v[3].c = -1;
			v[3].x = 0.0f;
			v[3].y = 0.0f;
			v[3].z = 0.0f;
			v[3].u = 0.0f;
			v[3].v = 1.0f;

			v[4].c = -1;
			v[4].x = 100.0f;
			v[4].y = 0.0f;
			v[4].z = 100.0f;
			v[4].u = 1.0f;
			v[4].v = 0.0f;

			v[5].c = -1;
			v[5].x = 100.0f;
			v[5].y = 0.0f;
			v[5].z = 0.0f;
			v[5].u = 1.0f;
			v[5].v = 1.0f;

			this.SetVertices(v);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		#endregion
	}
}
