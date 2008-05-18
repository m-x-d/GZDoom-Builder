
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
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal sealed class Display2DShader : D3DShader
	{
		#region ================== Variables

		// Property handlers
		private EffectHandle texture1;
		private EffectHandle rendersettings;
		private EffectHandle transformsettings;

		#endregion

		#region ================== Properties

		public Texture Texture1 { set { if(manager.Enabled) effect.SetValue(texture1, value); } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Display2DShader(ShaderManager manager) : base(manager)
		{
			// Load effect from file
			effect = LoadEffect("display2d.fx");

			// Get the property handlers from effect
			if(effect != null)
			{
				texture1 = effect.GetParameter(null, "texture1");
				rendersettings = effect.GetParameter(null, "rendersettings");
				transformsettings = effect.GetParameter(null, "transformsettings");
			}
			
			// Initialize world vertex declaration
			VertexElement[] elements = new VertexElement[]
			{
				new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0, 12, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
				new VertexElement(0, 16, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
				VertexElement.VertexDeclarationEnd
			};
			vertexdecl = new VertexDeclaration(General.Map.Graphics.Device, elements);

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				if(texture1 != null) texture1.Dispose();
				if(rendersettings != null) rendersettings.Dispose();
				if(transformsettings != null) transformsettings.Dispose();
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This sets the settings
		public void SetSettings(float texelx, float texely, float fsaafactor, float alpha)
		{
			if(manager.Enabled)
			{
				Vector4 values = new Vector4(texelx, texely, fsaafactor, alpha);
				effect.SetValue(rendersettings, values);
				Matrix world = manager.D3DDevice.Device.GetTransform(TransformState.World);
				Matrix view = manager.D3DDevice.Device.GetTransform(TransformState.View);
				effect.SetValue(transformsettings, Matrix.Multiply(world, view));
			}
		}

		#endregion
	}
}
