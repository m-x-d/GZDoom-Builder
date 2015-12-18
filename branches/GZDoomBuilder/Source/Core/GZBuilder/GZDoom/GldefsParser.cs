#region ================== Namespaces

using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.GZDoom
{
	internal sealed class GldefsParser : ZDTextParser
	{
		#region ================== Constants

		private const int DEFAULT_GLOW_HEIGHT = 64;

		#endregion

		#region ================== Structs

		private struct GldefsLightType
		{
			public const string POINT = "pointlight";
			public const string PULSE = "pulselight";
			public const string FLICKER = "flickerlight";
			public const string FLICKER2 = "flickerlight2";
			public const string SECTOR = "sectorlight";

			public static readonly Dictionary<string, DynamicLightType> GLDEFS_TO_GZDOOM_LIGHT_TYPE = new Dictionary<string, DynamicLightType>(StringComparer.Ordinal) { { POINT, DynamicLightType.NORMAL }, { PULSE, DynamicLightType.PULSE }, { FLICKER, DynamicLightType.FLICKER }, { FLICKER2, DynamicLightType.RANDOM }, { SECTOR, DynamicLightType.SECTOR } };
		}

		#endregion

		#region ================== Delegates

		public delegate void IncludeDelegate(GldefsParser parser, string includefile, bool clearerrors);
		public IncludeDelegate OnInclude;

		#endregion

		#region ================== Variables

		private readonly Dictionary<string, DynamicLightData> lightsByName; //LightName, light definition
		private readonly Dictionary<string, string> objects; //ClassName, LightName
		private readonly Dictionary<long, GlowingFlatData> glowingflats;
		private readonly List<string> parsedlumps;

		#endregion

		#region ================== Properties

		public Dictionary<string, DynamicLightData> LightsByName { get { return lightsByName; } }
		public Dictionary<string, string> Objects { get { return objects; } }
		internal Dictionary<long, GlowingFlatData> GlowingFlats { get { return glowingflats; } }

		#endregion

		#region ================== Constructor

		public GldefsParser() 
		{
			// Syntax
			whitespace = "\n \t\r\u00A0";
			specialtokens = ",{}\n";
			
			parsedlumps = new List<string>();
			lightsByName = new Dictionary<string, DynamicLightData>(StringComparer.Ordinal); //LightName, Light params
			objects = new Dictionary<string, string>(StringComparer.Ordinal); //ClassName, LightName
			glowingflats = new Dictionary<long, GlowingFlatData>(); // Texture name hash, Glowing Flat Data
		}

		#endregion

		#region ================== Parsing

		public override bool Parse(Stream stream, string sourcefilename, bool clearerrors) 
		{
			parsedlumps.Add(sourcefilename);
			if(!base.Parse(stream, sourcefilename, clearerrors)) return false;

			// Keep local data
			Stream localstream = datastream;
			string localsourcename = sourcename;
			BinaryReader localreader = datareader;

			// Continue until at the end of the stream
			while(SkipWhitespace(true)) 
			{
				string token = ReadToken();
				if(!string.IsNullOrEmpty(token)) 
				{
					token = StripTokenQuotes(token.ToLowerInvariant()); //Quotes can be anywhere! ANYWHERE!!! And GZDoom will still parse data correctly

					//got light structure
					if(token == GldefsLightType.POINT || token == GldefsLightType.PULSE || token == GldefsLightType.FLICKER 
						|| token == GldefsLightType.FLICKER2 || token == GldefsLightType.SECTOR) 
					{
						string lightType = token;

						DynamicLightData light = new DynamicLightData();
						light.Type = GldefsLightType.GLDEFS_TO_GZDOOM_LIGHT_TYPE[lightType];

						//find classname
						SkipWhitespace(true);
						string lightName = StripTokenQuotes(ReadToken()).ToLowerInvariant();

						if(!string.IsNullOrEmpty(lightName)) 
						{
							//now find opening brace
							if(!NextTokenIs("{")) continue;

							//read gldefs light structure
							while(SkipWhitespace(true)) 
							{
								token = ReadToken();
								if(!string.IsNullOrEmpty(token)) 
								{
									token = token.ToLowerInvariant();
//color
									if(token == "color") 
									{
										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										if(!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Color.Red)) 
										{
											// Not numeric!
											ReportError("expected Red color value, but got '" + token + "'");
											return false;
										}

										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										if(!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Color.Green)) 
										{
											// Not numeric!
											ReportError("expected Green color value, but got '" + token + "'");
											return false;
										}

										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Color.Blue)) 
										{
											// Not numeric!
											ReportError("expected Blue color value, but got '" + token + "'");
											return false;
										}
//size
									} 
									else if(token == "size") 
									{
										if(lightType != GldefsLightType.SECTOR) 
										{
											SkipWhitespace(true);

											token = StripTokenQuotes(ReadToken());
											if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out light.PrimaryRadius)) 
											{
												// Not numeric!
												ReportError("expected Size value, but got '" + token + "'");
												return false;
											}
											light.PrimaryRadius *= 2;

										} 
										else 
										{
											ReportError("'" + token + "' is not valid property for " + lightType + ".");
											return false;
										}
//offset
									} 
									else if(token == "offset") 
									{
										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										if(!ReadSignedFloat(token, ref light.Offset.X)) 
										{
											// Not numeric!
											ReportError("expected Offset X value, but got '" + token + "'");
											return false;
										}

										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										if(!ReadSignedFloat(token, ref light.Offset.Z)) 
										{
											// Not numeric!
											ReportError("expected Offset Y value, but got '" + token + "'");
											return false;
										}

										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										if(!ReadSignedFloat(token, ref light.Offset.Y)) 
										{
											// Not numeric!
											ReportError("expected Offset Z value, but got '" + token + "'");
											return false;
										}
//subtractive
									} 
									else if(token == "subtractive") 
									{
										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										int i;
										if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out i)) 
										{
											// Not numeric!
											ReportError("expected Subtractive value, but got '" + token + "'");
											return false;
										}

										light.Subtractive = i == 1;
//dontlightself
									} 
									else if(token == "dontlightself") 
									{
										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										int i;
										if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out i)) 
										{
											// Not numeric!
											ReportError("expected Dontlightself value, but got '" + token + "'");
											return false;
										}

										light.DontLightSelf = (i == 1);
//interval
									} 
									else if(token == "interval") 
									{
										if(lightType == GldefsLightType.PULSE || lightType == GldefsLightType.FLICKER2) 
										{
											SkipWhitespace(true);

											token = StripTokenQuotes(ReadToken());
											float interval;
											if(!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out interval)) 
											{
												// Not numeric!
												ReportError("expected Interval value, but got '" + token + "'");
												return false;
											}

											if(interval == 0) LogWarning("Interval value should be greater than zero");

											//I wrote logic for dynamic lights animation first, so here I modify gldefs settings to fit in existing logic
											if(lightType == GldefsLightType.PULSE) 
											{
												light.Interval = (int)(interval * 35); //measured in tics (35 per second) in PointLightPulse, measured in seconds in gldefs' PulseLight
											} 
											else //FLICKER2. Seems like PointLightFlickerRandom to me
											{ 
												light.Interval = (int)(interval * 350); //0.1 is one second for FlickerLight2
											}
										} 
										else 
										{
											ReportError("'" + token + "' is not valid property for " + lightType);
											return false;
										}
//secondarysize
									} 
									else if(token == "secondarysize") 
									{
										if(lightType == GldefsLightType.PULSE || lightType == GldefsLightType.FLICKER || lightType == GldefsLightType.FLICKER2) 
										{
											SkipWhitespace(true);

											token = StripTokenQuotes(ReadToken());
											if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out light.SecondaryRadius)) 
											{
												// Not numeric!
												ReportError("expected SecondarySize value, but got '" + token + "'");
												return false;
											}
											light.SecondaryRadius *= 2;
										} 
										else 
										{
											ReportError("'" + token + "' is not valid property for " + lightType);
											return false;
										}
//chance
									} 
									else if(token == "chance") 
									{
										if(lightType == GldefsLightType.FLICKER) 
										{
											SkipWhitespace(true);

											token = StripTokenQuotes(ReadToken());
											float chance;
											if(!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out chance)) 
											{
												// Not numeric!
												ReportError("expected Chance value, but got '" + token + "'");
												return false;
											}

											//transforming from 0.0 .. 1.0 to 0 .. 359 to fit in existing logic
											light.Interval = (int)(chance * 359.0f);
										} 
										else 
										{
											ReportError("'" + token + "' is not valid property for " + lightType);
											return false;
										}
//scale
									} 
									else if(token == "scale") 
									{
										if(lightType == GldefsLightType.SECTOR) 
										{
											SkipWhitespace(true);

											token = StripTokenQuotes(ReadToken());
											float scale;
											if(!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out scale)) 
											{
												// Not numeric!
												ReportError("expected Scale value, but got '" + token + "'");
												return false;
											}

											if(scale > 1.0f) 
											{
												ReportError("scale must be in 0.0 - 1.0 range, but is " + scale);
												return false;
											}

											//sector light doesn't have animation, so we will store it's size in Interval
											//transforming from 0.0 .. 1.0 to 0 .. 10 to preserve value.
											light.Interval = (int)(scale * 10.0f);
										} 
										else 
										{
											ReportError("'" + token + "' is not valid property for " + lightType);
											return false;
										}
									} 
									//end of structure
									else if(token == "}") 
									{
										bool skip = false;

										//general checks
										if(light.Color.Red == 0.0f && light.Color.Green == 0.0f && light.Color.Blue == 0.0f) 
										{
											LogWarning("'" + lightName + "' light Color is " + light.Color.Red + "," + light.Color.Green + "," + light.Color.Blue + ". It won't be shown in GZDoom");
											skip = true;
										}

										//light-type specific checks
										if(light.Type == DynamicLightType.NORMAL && light.PrimaryRadius == 0) 
										{
											LogWarning("'" + lightName + "' light Size is 0. It won't be shown in GZDoom");
											skip = true;
										}

										if(light.Type == DynamicLightType.FLICKER || light.Type == DynamicLightType.PULSE || light.Type == DynamicLightType.RANDOM) 
										{
											if(light.PrimaryRadius == 0 && light.SecondaryRadius == 0)
											{
												LogWarning("'" + lightName + "' light Size and SecondarySize are 0. This light won't be shown in GZDoom");
												skip = true;
											}
										}

										//offset it slightly to avoid shading glitches
										if(light.Offset.Z == 0.0f) light.Offset.Z = 0.1f;

										// Add to the collection?
										if(!skip) lightsByName[lightName] = light;

										//break out of this parsing loop
										break; 
									}
								}
							}
						}
					} 
					else if(token == "object") 
					{
						SkipWhitespace(true);

						//read object class
						string objectClass = StripTokenQuotes(ReadToken()).ToLowerInvariant();

						if(!string.IsNullOrEmpty(objectClass)) 
						{
							//now find opening brace
							if(!NextTokenIs("{")) continue;

							int bracesCount = 1;
							bool foundLight = false;
							bool foundFrame = false;

							//read frames structure
							while(SkipWhitespace(true)) 
							{
								token = ReadToken();
								if(!string.IsNullOrEmpty(token)) 
								{
									token = StripTokenQuotes(token).ToLowerInvariant();
									if(!foundLight && !foundFrame && token == "frame") 
									{
										SkipWhitespace(true);
										token = ReadToken().ToLowerInvariant(); //should be frame name

										//use this frame if it's 4 characters long or it's the first frame
										foundFrame = (token.Length == 4 || (token.Length > 4 && token[4] == 'a'));
									} 
									else if(!foundLight && foundFrame && token == "light") //just use first light and be done with it
									{ 
										SkipWhitespace(true);
										token = ReadToken().ToLowerInvariant(); //should be light name

										if(!string.IsNullOrEmpty(token)) 
										{
											if(lightsByName.ContainsKey(token)) 
											{
												objects[objectClass] = token;
												foundLight = true;
											} 
											else 
											{
												LogWarning("Light declaration not found for light '" + token + "'");
											}
										}
									} 
									else if(token == "{") //continue in this loop until object structure ends
									{ 
										bracesCount++;
									} 
									else if(token == "}") 
									{
										if(--bracesCount < 1) break; //This was Cave Johnson. And we are done here.
									}
								}
							}
						}
					}
					//Glowing flats block start
					else if(token == "glow")
					{
						// Next sould be opening brace
						if(!NextTokenIs("{")) continue;

						// Parse inner blocks
						while(SkipWhitespace(true))
						{
							token = ReadToken().ToLowerInvariant();
							if(token == "flats" || token == "walls") 
							{
								// Next sould be opening brace
								if(!NextTokenIs("{")) break;

								// Read flat names
								while(SkipWhitespace(true))
								{
									token = ReadToken();
									if(token == "}") break;

									// Add glow data
									glowingflats[General.Map.Data.GetFullLongFlatName(Lump.MakeLongName(token, General.Map.Options.UseLongTextureNames))] = new GlowingFlatData {
										Height = DEFAULT_GLOW_HEIGHT * 2,
										Fullbright = true,
										Color = new PixelColor(255, 255, 255, 255),
										CalculateTextureColor = true
									};
								}
							} 
							// GLOOME subtractive flats
							else if(token == "subflats" || token == "subwalls")
							{
								// Next sould be opening brace
								if(!NextTokenIs("{")) break;

								// Read flat names
								while(SkipWhitespace(true))
								{
									token = ReadToken();
									if(token == "}") break;

									// Add glow data
									glowingflats[General.Map.Data.GetFullLongFlatName(Lump.MakeLongName(token, General.Map.Options.UseLongTextureNames))] = new GlowingFlatData {
										Height = DEFAULT_GLOW_HEIGHT * 2,
										Fullblack = true,
										Subtractive = true,
										Color = new PixelColor(255, 0, 0, 0),
										CalculateTextureColor = false
									};
								}
							}
							else if(token == "texture" || token == "subtexture")
							{
								int color;
								int glowheight = DEFAULT_GLOW_HEIGHT;
								bool subtractivetexture = (token == "subtexture");
								string texturename = StripTokenQuotes(ReadToken(false));

								if(string.IsNullOrEmpty(texturename))
								{
									ReportError("expected " + token + " name");
									return false;
								}
								
								// Now we should find a comma
								if(!NextTokenIs(",")) break;

								// Next is color
								SkipWhitespace(true);
								token = ReadToken();

								if(!int.TryParse(token, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out color)) 
								{
									//probably it's a color name?
									Color c = Color.FromName(token); //should be similar to C++ color name detection, I suppose
									if(c.IsKnownColor)
									{
										color = PixelColor.FromColor(c).ToInt();
									}
									else
									{
										ReportError("expected glow color value, but got '" + token + "'");
										return false;
									}
								}

								// The glow data is valid at thispoint. Let's get texture hash 
								long texturehash = General.Map.Data.GetFullLongFlatName(Lump.MakeLongName(texturename, General.Map.Options.UseLongTextureNames));

								// Now we can find a comma
								if(!NextTokenIs(",", false))
								{
									// Add glow data
									glowingflats[texturehash] = new GlowingFlatData {
										Height = glowheight * 2,
										Subtractive = subtractivetexture,
										Color = PixelColor.FromInt(color).WithAlpha(255),
										CalculateTextureColor = false
									};

									continue;
								}

								// Can be glow height
								SkipWhitespace(true);
								token = ReadToken();

								int h;
								if(int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out h))
								{
									// Can't pass glowheight directly cause TryParse will unconditionally set it to 0
									glowheight = h;
									
									// Now we can find a comma
									if(!NextTokenIs(",", false))
									{
										// Add glow data
										glowingflats[texturehash] = new GlowingFlatData {
											Height = glowheight * 2,
											Subtractive = subtractivetexture,
											Color = PixelColor.FromInt(color).WithAlpha(255),
											CalculateTextureColor = false
										};

										continue;
									}

									// Read the flag
									SkipWhitespace(true);
									token = ReadToken().ToLowerInvariant();
								}

								// Next is "fullbright" or "fullblack" flag
								bool fullbright = (token == "fullbright");
								bool fullblack = (!subtractivetexture && token == "fullblack");

								if(!fullblack && !fullbright)
								{
									string expectedflags = (subtractivetexture ? "'fullbright'" : "'fullbright' or 'fullblack'");
									ReportError("expected " + expectedflags + " flag, but got '" + token + "'");
									return false;
								}

								// Add glow data
								glowingflats[texturehash] = new GlowingFlatData {
									Height = glowheight * 2,
									Fullbright = fullbright,
									Fullblack = fullblack,
									Subtractive = subtractivetexture,
									Color = PixelColor.FromInt(color).WithAlpha(255),
									CalculateTextureColor = false
								};
							}
						}

						// Now find closing brace
						while(SkipWhitespace(true))
						{
							token = ReadToken();
							if(string.IsNullOrEmpty(token) || token == "}") break;
						}
					}
					else if(token == "#include") 
					{
						SkipWhitespace(true);
						string includelump = ReadToken(false); // Don't skip newline

						// Sanity checks
						if(!includelump.StartsWith("\"") || !includelump.EndsWith("\""))
						{
							ReportError("#include filename should be quoted");
							return false;
						}

						includelump = StripTokenQuotes(includelump).ToLowerInvariant();

						// More sanity checks
						if(string.IsNullOrEmpty(includelump))
						{
							ReportError("Expected file name to include");
							return false;
						}

						includelump = includelump.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
						
						// Already parsed?
						if(parsedlumps.IndexOf(includelump) != -1)
						{
							ReportError("already parsed '" + includelump + "'. Check your #include directives");
							return false;
						}

						// Callback to parse this file
						if(OnInclude != null) OnInclude(this, includelump, clearerrors);

						// Set our buffers back to continue parsing
						datastream = localstream;
						datareader = localreader;
						sourcename = localsourcename;
					} 
					else if(token == "$gzdb_skip") //mxd
					{
						break;
					}
					else 
					{
						// Unknown structure!
						string token2;
						do 
						{
							if(!SkipWhitespace(true)) break;
							token2 = ReadToken();
							if(string.IsNullOrEmpty(token2)) break;
						}
						while (token2 != "{");
						int scopelevel = 1;
						do 
						{
							if(!SkipWhitespace(true)) break;
							token2 = ReadToken();
							if(string.IsNullOrEmpty(token2)) break;
							if(token2 == "{") scopelevel++;
							if(token2 == "}") scopelevel--;
						}
						while(scopelevel > 0);
					}
				}
			}

			return objects.Count > 0;
		}

		#endregion

		#region ================== Methods

		internal void ClearIncludesList() 
		{
			parsedlumps.Clear();
		}

		protected override string GetLanguageType()
		{
			return "GLDEFS";
		}

		#endregion
	}
}