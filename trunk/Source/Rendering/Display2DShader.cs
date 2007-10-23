
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
	internal sealed class Display2DShader : D3DShader
	{
		#region ================== Variables

		// Property handlers
		private EffectHandle texture1;
		private EffectHandle settings;

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
				settings = effect.GetParameter(null, "settings");
			}
			
			// Initialize world vertex declaration
			VertexElement[] elements = new VertexElement[]
			{
				new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
				new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
				new VertexElement(0, 20, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
				VertexElement.VertexDeclarationEnd
			};
			vertexdecl = new VertexDeclaration(General.Map.Graphics.Device, elements);

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				texture1.Dispose();
				settings.Dispose();
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This sets the settings
		public void SetSettings(float texelx, float texely, float fsaafactor, float alpha)
		{
			Vector4 values = new Vector4(texelx, texely, fsaafactor, alpha);
			if(manager.Enabled) effect.SetValue(settings, values);
		}

		#endregion
	}
}
