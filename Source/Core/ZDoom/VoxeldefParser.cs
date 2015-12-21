#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.IO;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Data;
using SlimDX;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public sealed class VoxeldefParser : ZDTextParser
	{
		private Dictionary<string, ModelData> entries; //sprite name, entry
		internal Dictionary<string, ModelData> Entries { get { return entries; } }

		public override bool Parse(Stream stream, string sourcefilename, bool clearerrors) 
		{
			entries = new Dictionary<string, ModelData>(StringComparer.Ordinal);
			if(!base.Parse(stream, sourcefilename, clearerrors)) return false;
			string prevToken = string.Empty;

			List<string> spriteNames = new List<string>();
			string modelName = string.Empty;

			// Continue until at the end of the stream
			while(SkipWhitespace(true)) 
			{
				string token = ReadToken();

				if(!string.IsNullOrEmpty(token)) 
				{
					token = StripTokenQuotes(token).ToLowerInvariant();

					if(token == ",") //previous token was a sprite name
					{ 
						if(!string.IsNullOrEmpty(prevToken) && !spriteNames.Contains(prevToken)) spriteNames.Add(prevToken);
						prevToken = token.ToUpperInvariant();
					} 
					else if(token == "=") //next token should be a voxel model name
					{ 
						if(!string.IsNullOrEmpty(prevToken) && !spriteNames.Contains(prevToken)) spriteNames.Add(prevToken);

						SkipWhitespace(true);
						token = ReadToken();

						if(string.IsNullOrEmpty(token)) 
						{
							ReportError("Expected voxel name");
							return false;
						}

						modelName = StripTokenQuotes(token).ToLowerInvariant();
					} 
					else if(token == "{") //read the settings
					{
						ModelData mde = new ModelData { IsVoxel = true };
						float scale = 1.0f;
						float angleoffset = 0;

						while(SkipWhitespace(true)) 
						{
							token = ReadToken();

							if(!string.IsNullOrEmpty(token)) 
							{
								token = StripTokenQuotes(token).ToLowerInvariant();

								if(token == "}") //store data
								{ 
									if(!string.IsNullOrEmpty(modelName) && spriteNames.Count > 0) 
									{
										mde.ModelNames.Add(modelName);
										mde.SetTransform(Matrix.RotationZ(Angle2D.DegToRad(angleoffset)), Matrix.Identity, new Vector3(scale));

										foreach(string s in spriteNames)
										{
											//TODO: is this the proper behaviour?
											entries[s] = mde;
										}

										//reset local data
										modelName = string.Empty;
										prevToken = string.Empty;
										spriteNames.Clear();
									}

									break;
								} 
								else if(token == "overridepalette")
								{
									mde.OverridePalette = true;
								} 
								else if(token == "angleoffset")
								{
									if(!NextTokenIs("=")) return false;

									token = StripTokenQuotes(ReadToken());
									if(!ReadSignedFloat(token, ref angleoffset))
									{
										// Not numeric!
										ReportError("Expected AngleOffset value, but got '" + token + "'");
										return false;
									}
								} 
								else if(token == "scale") 
								{
									if(!NextTokenIs("=")) return false;

									token = StripTokenQuotes(ReadToken());
									if(!ReadSignedFloat(token, ref scale)) 
									{
										// Not numeric!
										ReportError("Expected Scale value, but got '" + token + "'");
										return false;
									}
								}

								prevToken = token.ToUpperInvariant();
							}
						}
					} 
					else 
					{
						prevToken = token.ToUpperInvariant();
					}
				}
			}

			return entries.Count > 0;
		}

		protected override string GetLanguageType()
		{
			return "VOXELDEF";
		}
	}
}
