
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

using System.IO;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.ZDoom;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public enum TexturePathRenderStyle
	{
		Copy,
		Blend,
		Add,
		Subtract,
		ReverseSubtract,
		Modulate,
		CopyAlpha,
		CopyNewAlpha, //mxd
		Overlay, //mxd
	}

	public enum TexturePathBlendStyle //mxd
	{
		None,
		Blend,
		Tint
	}
	
	internal struct TexturePatch
	{
		public readonly string lumpname;
		public readonly int x;
		public readonly int y;
		public readonly bool flipx;
		public readonly bool flipy;
		public readonly bool haslongname; //mxd
		public readonly int rotate;
		public PixelColor blend;
		public readonly float alpha;
		public readonly TexturePathRenderStyle style;
		public readonly TexturePathBlendStyle blendstyle; //mxd
		public readonly float tintammount;//mxd
		public readonly bool skip; //mxd
		
		// Constructor for simple patches
		public TexturePatch(string lumpname, int x, int y)
		{
			// Initialize
			this.lumpname = lumpname;
			this.x = x;
			this.y = y;
			this.flipx = false;
			this.flipy = false;
			this.rotate = 0;
			this.blend = new PixelColor(0, 0, 0, 0);
			this.alpha = 1.0f;
			this.style = TexturePathRenderStyle.Copy;
			this.blendstyle = TexturePathBlendStyle.None;//mxd
			this.tintammount = 0; //mxd
			this.haslongname = false; //mxd
			this.skip = false; //mxd
		}

		//mxd. Constructor for hires patches
		public TexturePatch(PatchStructure patch) 
		{
			// Initialize
			this.lumpname = patch.Name.ToUpperInvariant();
			this.x = patch.OffsetX;
			this.y = patch.OffsetY;
			this.flipx = patch.FlipX;
			this.flipy = patch.FlipY;
			this.rotate = patch.Rotation;
			this.blend = patch.BlendColor;
			this.alpha = patch.Alpha;
			this.style = patch.RenderStyle;
			this.blendstyle = patch.BlendStyle;
			this.tintammount = patch.TintAmmount;
			this.haslongname = (Path.GetFileNameWithoutExtension(this.lumpname) != this.lumpname);
			this.skip = patch.Skip;

			//mxd. Check data so we don't perform unneeded operations later on
			if(this.alpha == 1.0f) 
			{
				switch(this.style)
				{
					case TexturePathRenderStyle.Blend:
					case TexturePathRenderStyle.CopyAlpha:
					case TexturePathRenderStyle.CopyNewAlpha:
					case TexturePathRenderStyle.Overlay:
						this.style = TexturePathRenderStyle.Copy;
						break;
				}
			}

			//mxd. and get rid of render styles we don't support
			if(this.style == TexturePathRenderStyle.Overlay) this.style = TexturePathRenderStyle.Copy;
		}
	}
}
