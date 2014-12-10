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

		public override bool Parse(Stream stream, string sourcefilename) 
		{
			base.Parse(stream, sourcefilename);
			entries = new Dictionary<string, ModelData>(StringComparer.Ordinal);
			string prevToken = string.Empty;

			List<string> spriteNames = new List<string>();
			string modelName = string.Empty;

			// Continue until at the end of the stream
			while(SkipWhitespace(true)) 
			{
				string token = ReadToken();

				if(token != null) 
				{
					token = StripTokenQuotes(token).ToLowerInvariant();

					if(token == ",") //previous token was a sprite name
					{ 
						if(!string.IsNullOrEmpty(prevToken)) 
						{
							if(!spriteNames.Contains(prevToken)) spriteNames.Add(prevToken);
						}
						prevToken = token.ToUpperInvariant();

					} 
					else if(token == "=") //next token should be a voxel model name
					{ 
						if(!string.IsNullOrEmpty(prevToken)) 
						{
							if(!spriteNames.Contains(prevToken)) spriteNames.Add(prevToken);
						}

						SkipWhitespace(true);
						token = ReadToken();

						if(string.IsNullOrEmpty(token)) 
						{
							General.ErrorLogger.Add(ErrorType.Error, "Unable to get voxel model name from '" + sourcefilename + "', line " + GetCurrentLineNumber());
							spriteNames.Clear();
							continue;
						}

						modelName = StripTokenQuotes(token).ToLowerInvariant();
					} 
					else if(token == "{") //read the settings
					{
						ModelData data = new ModelData();
						data.IsVoxel = true;

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
										data.ModelNames.Add(modelName);

										foreach(string s in spriteNames)
										{
											if(entries.ContainsKey(s)) //TODO: is this a proper behaviour? 
											{ 
												entries[s] = data;
											} 
											else 
											{
												entries.Add(s, data);
											}
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
									data.OverridePalette = true;
								} 
								else if(token == "angleoffset") 
								{
									SkipWhitespace(true);

									token = StripTokenQuotes(ReadToken());
									if(token != "=") 
									{
										General.ErrorLogger.Add(ErrorType.Error, "Error in " + sourcefilename + " at line " + GetCurrentLineNumber() + ": expected '=', but got '" + token + "'");
										break;
									}

									float angleOffset = 0; //90?
									token = StripTokenQuotes(ReadToken());
									if(!ReadSignedFloat(token, ref angleOffset)) 
									{
										// Not numeric!
										General.ErrorLogger.Add(ErrorType.Error, "Error in " + sourcefilename + " at line " + GetCurrentLineNumber() + ": expected AngleOffset value, but got '" + token + "'");
									}
									data.AngleOffset = Angle2D.DegToRad(angleOffset);

								} 
								else if(token == "scale") 
								{
									SkipWhitespace(true);

									token = StripTokenQuotes(ReadToken());
									if(token != "=") 
									{
										General.ErrorLogger.Add(ErrorType.Error, "Error in " + sourcefilename + " at line " + GetCurrentLineNumber() + ": expected '=', but got '" + token + "'");
										break;
									}

									float scale = 1.0f;
									token = StripTokenQuotes(ReadToken());
									if(!ReadSignedFloat(token, ref scale)) 
									{
										// Not numeric!
										General.ErrorLogger.Add(ErrorType.Error, "Error in " + sourcefilename + " at line " + GetCurrentLineNumber() + ": expected Scale value, but got '" + token + "'");
									}

									data.Scale = Matrix.Scaling(scale, scale, scale);
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
	}
}
