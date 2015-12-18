#region ================== Namespaces

using System.Collections.Generic;
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public struct CameraTextureData
	{
		public string Name;
		public int Width;
		public int Height;
		public float ScaleX;
		public float ScaleY;
		public bool WorldPanning;
		public bool FitTexture;
	}
	
	//mxd. Currently this only parses cameratextures
	internal sealed class AnimdefsParser : ZDTextParser
	{
		#region ================== Variables

		private readonly Dictionary<string, CameraTextureData> cameratextures;

		#endregion

		#region ================== Properties

		public Dictionary<string, CameraTextureData> CameraTextures { get { return cameratextures; } }

		#endregion

		#region ================== Constructor

		internal AnimdefsParser()
		{
			cameratextures = new Dictionary<string, CameraTextureData>();
		}

		#endregion

		#region ================== Parsing

		public override bool Parse(Stream stream, string sourcefilename, bool clearerrors)
		{
			if(!base.Parse(stream, sourcefilename, clearerrors)) return false;

			// Continue until at the end of the stream
			while(SkipWhitespace(true))
			{
				string token = ReadToken();
				if(string.IsNullOrEmpty(token) || string.Compare(token, "CAMERATEXTURE", true) != 0) continue;

				// Texture name
				string texturename = StripTokenQuotes(ReadToken(false));
				if(string.IsNullOrEmpty(texturename))
				{
					ReportError("Expected camera texture name");
					break;
				}

				// Width
				int width = -1;
				SkipWhitespace(true);
				if(!ReadSignedInt(ref width) || width < 1)
				{
					ReportError("Expected camera texture width");
					break;
				}

				// Height
				int height = -1;
				SkipWhitespace(true);
				if(!ReadSignedInt(ref height) || height < 1)
				{
					ReportError("Expected camera texture height");
					break;
				}

				// "Fit" keyword?
				bool worldpanning = false;
				bool fit = false;
				float scalex = 1.0f;
				float scaley = 1.0f;

				if(NextTokenIs("fit", false))
				{
					fit = true;
					int fitwidth = width;
					int fitheight = height;
					
					// Fit width
					SkipWhitespace(true);
					if(!ReadSignedInt(ref fitwidth) || fitwidth < 1)
					{
						ReportError("Expected camera texture fit width");
						break;
					}

					// Fit height
					SkipWhitespace(true);
					if(!ReadSignedInt(ref fitheight) || fitheight < 1)
					{
						ReportError("Expected camera texture fit height");
						break;
					}

					// Update scale
					scalex = (float)fitwidth / width;
					scaley = (float)fitheight / height;

					// WorldPanning
					worldpanning = NextTokenIs("worldpanning", false);
				}
				else if(NextTokenIs("worldpanning", false))
				{
					worldpanning = true;
				}

				// Check results
				if(cameratextures.ContainsKey(texturename.ToUpperInvariant()))
				{
					ReportError("Camera texture '" + texturename + "' is defined more than once");
					break;
				}

				// Store results
				texturename = texturename.ToUpperInvariant();
				cameratextures[texturename] = new CameraTextureData { Name = texturename, Width = width, Height = height, 
																	  ScaleX = scalex, ScaleY = scaley, 
																	  WorldPanning = worldpanning, FitTexture = fit };
			}

			return true;
		}

		protected override string GetLanguageType()
		{
			return "ANIMDEFS";
		}

		#endregion
	}
}
