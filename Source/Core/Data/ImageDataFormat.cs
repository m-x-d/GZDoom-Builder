
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
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal static class ImageDataFormat
	{
		// Input guess formats
		public const int UNKNOWN = 0;			// No clue.
		public const int DOOMPICTURE = 1;		// Could be Doom Picture format	(column list rendered data)
		public const int DOOMFLAT = 2;			// Could be Doom Flat format	(raw 8-bit pixel data)
		public const int DOOMCOLORMAP = 3;		// Could be Doom Colormap format (raw 8-bit pixel palette mapping)
		
		// File format signatures
		private static readonly int[] PNG_SIGNATURE = new[] { 137, 80, 78, 71, 13, 10, 26, 10 };
		private static readonly int[] GIF_SIGNATURE = new[] { 71, 73, 70 };
		private static readonly int[] BMP_SIGNATURE = new[] { 66, 77 }; 
		private static readonly int[] DDS_SIGNATURE = new[] { 68, 68, 83, 32 };
		private static readonly int[] JPG_SIGNATURE = new[] { 255, 216, 255 }; //mxd
		private static readonly int[] TGA_SIGNATURE = new[] { 0, 0, 2, 0 }; //mxd
		private static readonly int[] PCX_SIGNATURE = new[] { 10, 5, 1, 8 }; //mxd

		// This check image data and returns the appropriate image reader
		public static IImageReader GetImageReader(Stream data, int guessformat, Playpal palette)
		{
			if(data == null) return new UnknownImageReader(); //mxd
			
			// Data long enough to check for signatures?
			if(data.Length > 10) 
			{
				// Check for PNG signature
				data.Seek(0, SeekOrigin.Begin);
				if(CheckSignature(data, PNG_SIGNATURE))
					return new FileImageReader(DevilImageType.IL_PNG);

				// Check for DDS signature
				data.Seek(0, SeekOrigin.Begin);
				if(CheckSignature(data, DDS_SIGNATURE))
					return new FileImageReader(DevilImageType.IL_DDS);

				// Check for GIF signature
				data.Seek(0, SeekOrigin.Begin);
				if(CheckSignature(data, GIF_SIGNATURE))
					return new FileImageReader(DevilImageType.IL_GIF);

				//mxd. Check for PCX signature
				data.Seek(0, SeekOrigin.Begin);
				if(CheckSignature(data, PCX_SIGNATURE))
					return new FileImageReader(DevilImageType.IL_PCX);

				//mxd. Check for JPG signature
				data.Seek(0, SeekOrigin.Begin);
				if(CheckSignature(data, JPG_SIGNATURE))
					return new FileImageReader(DevilImageType.IL_JPG);

				//mxd. Check for TGA signature
				data.Seek(0, SeekOrigin.Begin);
				if(CheckSignature(data, TGA_SIGNATURE))
					return new FileImageReader(DevilImageType.IL_TGA);

				// Check for BMP signature
				data.Seek(0, SeekOrigin.Begin);
				if(CheckSignature(data, BMP_SIGNATURE))
					return new UnknownImageReader(); //mxd. Not supported in (G)ZDoom
			}
				
			// Could it be a doom picture?
			switch(guessformat) 
			{
				case DOOMPICTURE:
					// Check if data is valid for a doom picture
					data.Seek(0, SeekOrigin.Begin);
					DoomPictureReader picreader = new DoomPictureReader(palette);
					if(picreader.Validate(data)) return picreader;
					break;

				case DOOMFLAT:
					// Check if data is valid for a doom flat
					data.Seek(0, SeekOrigin.Begin);
					DoomFlatReader flatreader = new DoomFlatReader(palette);
					if(flatreader.Validate(data)) return flatreader;
					break;

				case DOOMCOLORMAP:
					// Check if data is valid for a doom colormap
					data.Seek(0, SeekOrigin.Begin);
					DoomColormapReader colormapreader = new DoomColormapReader(palette);
					if(colormapreader.Validate(data)) return colormapreader;
					break;
			}
			
			// Format not supported
			return new UnknownImageReader();
		}

		// This checks a signature as byte array
		// NOTE: Expects the stream position to be at the start of the
		// signature, and expects the stream to be long enough.
		private static bool CheckSignature(Stream data, int[] sig)
		{
			int b;
			
			// Go for all bytes
			for(int i = 0; i < sig.Length; i++)
			{
				// When byte doesnt match the signature, leave
				b = data.ReadByte();
				if(b != sig[i]) return false;
			}

			// Signature matches
			return true;
		}
	}
}
