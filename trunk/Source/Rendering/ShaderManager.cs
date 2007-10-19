
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
using SlimDX.Direct3D;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal class ShaderManager : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Settings
		private string shadertechnique;
		private bool useshaders;
		
		// Shaders
		private Base2DShader base2dshader;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public bool Enabled { get { return useshaders; } }
		public string ShaderTechnique { get { return shadertechnique; } }
		public Base2DShader Base2D { get { return base2dshader; } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ShaderManager()
		{
			Capabilities caps;

			// Check if we can use shaders
			caps = General.Map.Graphics.Device.GetDeviceCaps();
			useshaders = (caps.PixelShaderVersion.Major >= 2);
			shadertechnique = "SM20";
			
			// Initialize effects
			base2dshader = new Base2DShader(this);
			
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
				base2dshader.Dispose();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion
	}
}
