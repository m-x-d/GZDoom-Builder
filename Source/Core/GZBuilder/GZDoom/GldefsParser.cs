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

		public delegate void IncludeDelegate(GldefsParser parser, string includefile);
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

		public override bool Parse(Stream stream, string sourcefilename) 
		{
			base.Parse(stream, sourcefilename);

			if (parsedlumps.IndexOf(sourcefilename) != -1) 
			{
				General.ErrorLogger.Add(ErrorType.Error, "already parsed '" + sourcefilename + "'. Check your #include directives!");
				return false;
			}
			parsedlumps.Add(sourcefilename);

			// Keep local data
			Stream localstream = datastream;
			string localsourcename = sourcename;
			BinaryReader localreader = datareader;

			// Continue until at the end of the stream
			while (SkipWhitespace(true)) 
			{
				string token = ReadToken();
				if (!string.IsNullOrEmpty(token)) 
				{
					token = StripTokenQuotes(token.ToLowerInvariant()); //Quotes can be anywhere! ANYWHERE!!! And GZDoom will still parse data correctly

					//got light structure
					if (token == GldefsLightType.POINT || token == GldefsLightType.PULSE || token == GldefsLightType.FLICKER 
						|| token == GldefsLightType.FLICKER2 || token == GldefsLightType.SECTOR) 
					{
						bool gotErrors = false;
						string lightType = token;

						DynamicLightData light = new DynamicLightData();
						light.Type = GldefsLightType.GLDEFS_TO_GZDOOM_LIGHT_TYPE[lightType];

						//find classname
						SkipWhitespace(true);
						string lightName = StripTokenQuotes(ReadToken()).ToLowerInvariant();

						if (!string.IsNullOrEmpty(lightName)) 
						{
							//now find opening brace
							if(!NextTokenIs("{")) continue;

							//read gldefs light structure
							while (SkipWhitespace(true)) 
							{
								token = ReadToken();
								if (!string.IsNullOrEmpty(token)) 
								{
									token = token.ToLowerInvariant();
//color
									if (token == "color") 
									{
										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Color.Red)) 
										{
											// Not numeric!
											General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Red Color value, but got '" + token + "'.");
											gotErrors = true;
											break;
										}

										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Color.Green)) 
										{
											// Not numeric!
											General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Green Color value, but got '" + token + "'.");
											gotErrors = true;
											break;
										}

										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Color.Blue)) 
										{
											// Not numeric!
											General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Blue Color value, but got '" + token + "'.");
											gotErrors = true;
											break;
										}
//size
									} 
									else if (token == "size") 
									{
										if (lightType != GldefsLightType.SECTOR) 
										{
											SkipWhitespace(true);

											token = StripTokenQuotes(ReadToken());
											if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out light.PrimaryRadius)) 
											{
												// Not numeric!
												General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Size value, but got '" + token + "'.");
												gotErrors = true;
												break;
											}
											light.PrimaryRadius *= 2;

										} 
										else 
										{
											General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": '" + token + "' is not valid property for " + lightType + ".");
											gotErrors = true;
											break;
										}
//offset
									} 
									else if (token == "offset") 
									{
										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										if (!ReadSignedFloat(token, ref light.Offset.X)) 
										{
											// Not numeric!
											General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Offset X value, but got '" + token + "'.");
											gotErrors = true;
											break;
										}

										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										if (!ReadSignedFloat(token, ref light.Offset.Z)) 
										{
											// Not numeric!
											General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Offset Y value, but got '" + token + "'");
											gotErrors = true;
											break;
										}

										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										if (!ReadSignedFloat(token, ref light.Offset.Y)) 
										{
											// Not numeric!
											General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Offset Z value, but got '" + token + "'.");
											gotErrors = true;
											break;
										}
//subtractive
									} 
									else if (token == "subtractive") 
									{
										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										int i;
										if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out i)) 
										{
											// Not numeric!
											General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Subtractive value, but got '" + token + "'.");
											gotErrors = true;
											break;
										}

										light.Subtractive = i == 1;
//dontlightself
									} 
									else if (token == "dontlightself") 
									{
										SkipWhitespace(true);

										token = StripTokenQuotes(ReadToken());
										int i;
										if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out i)) 
										{
											// Not numeric!
											General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Dontlightself value, but got '" + token + "'.");
											gotErrors = true;
											break;
										}

										light.DontLightSelf = (i == 1);
//interval
									} 
									else if (token == "interval") 
									{
										if (lightType == GldefsLightType.PULSE || lightType == GldefsLightType.FLICKER2) 
										{
											SkipWhitespace(true);

											token = StripTokenQuotes(ReadToken());
											float interval;
											if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out interval)) 
											{
												// Not numeric!
												General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Interval value, but got '" + token + "'.");
												gotErrors = true;
												break;
											}

											if(interval == 0) 
												General.ErrorLogger.Add(ErrorType.Warning, "Warning in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": Interval value should be greater than zero.");

											//I wrote logic for dynamic lights animation first, so here I modify gldefs settings to fit in existing logic
											if (lightType == GldefsLightType.PULSE) 
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
											General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": '" + token + "' is not valid property for " + lightType + ".");
											gotErrors = true;
											break;
										}
//secondarysize
									} 
									else if (token == "secondarysize") 
									{
										if (lightType == GldefsLightType.PULSE || lightType == GldefsLightType.FLICKER || lightType == GldefsLightType.FLICKER2) 
										{
											SkipWhitespace(true);

											token = StripTokenQuotes(ReadToken());
											if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out light.SecondaryRadius)) 
											{
												// Not numeric!
												General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected SecondarySize value, but got '" + token + "'.");
												gotErrors = true;
												break;
											}
											light.SecondaryRadius *= 2;
										} 
										else 
										{
											General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": '" + token + "' is not valid property for " + lightType + ".");
											gotErrors = true;
											break;
										}
//chance
									} 
									else if (token == "chance") 
									{
										if (lightType == GldefsLightType.FLICKER) 
										{
											SkipWhitespace(true);

											token = StripTokenQuotes(ReadToken());
											float chance;
											if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out chance)) 
											{
												// Not numeric!
												General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Chance value, but got '" + token + "'.");
												gotErrors = true;
												break;
											}

											//transforming from 0.0 .. 1.0 to 0 .. 359 to fit in existing logic
											light.Interval = (int)(chance * 359.0f);
										} 
										else 
										{
											General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": '" + token + "' is not valid property for " + lightType + ".");
											gotErrors = true;
											break;
										}
//scale
									} 
									else if (token == "scale") 
									{
										if (lightType == GldefsLightType.SECTOR) 
										{
											SkipWhitespace(true);

											token = StripTokenQuotes(ReadToken());
											float scale;
											if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out scale)) 
											{
												// Not numeric!
												General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Scale value, but got '" + token + "'.");
												gotErrors = true;
												break;
											}

											if (scale > 1.0f) 
											{
												General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": scale must be in 0.0 - 1.0 range, but is " + scale + ".");
												gotErrors = true;
												break;
											}

											//sector light doesn't have animation, so we will store it's size in Interval
											//transforming from 0.0 .. 1.0 to 0 .. 10 to preserve value.
											light.Interval = (int)(scale * 10.0f);
										} 
										else 
										{
											General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": '" + token + "' is not valid property for " + lightType + ".");
											gotErrors = true;
											break;
										}
									} 
									//end of structure
									else if (token == "}") 
									{
										if (!gotErrors) 
										{
											//general checks
											if (light.Color.Red == 0.0f && light.Color.Green == 0.0f && light.Color.Blue == 0.0f) 
											{
												General.ErrorLogger.Add(ErrorType.Warning, "'" + sourcefilename + "', line " + GetCurrentLineNumber() + ": light Color is " + light.Color.Red + "," + light.Color.Green + "," + light.Color.Blue + ". It won't be shown in GZDoom!");
												gotErrors = true;
											}

											//light-type specific checks
											if (light.Type == DynamicLightType.NORMAL && light.PrimaryRadius == 0) 
											{
												General.ErrorLogger.Add(ErrorType.Warning, "'" + sourcefilename + "', line " + GetCurrentLineNumber() + ": light Size is 0. It won't be shown in GZDoom!");
												gotErrors = true;
											}

											if (light.Type == DynamicLightType.FLICKER || light.Type == DynamicLightType.PULSE || light.Type == DynamicLightType.RANDOM) 
											{
												if (light.PrimaryRadius == 0 && light.SecondaryRadius == 0) 
												{
													General.ErrorLogger.Add(ErrorType.Warning, "'" + sourcefilename + "', line " + GetCurrentLineNumber() + ": 'Size' and 'SecondarySize' are 0. This light won't be shown in GZDoom!");
													gotErrors = true;
												}
											}

											//offset it slightly to avoid shading glitches
											if (light.Offset.Z == 0.0f) light.Offset.Z = 0.1f;

											if (!gotErrors) 
											{
												if (lightsByName.ContainsKey(lightName))
													lightsByName[lightName] = light;
												else
													lightsByName.Add(lightName, light);
											}
										}
										break; //break out of this parsing loop
									}
								}
							}
						}
					} 
					else if (token == "object") 
					{
						SkipWhitespace(true);

						//read object class
						string objectClass = StripTokenQuotes(ReadToken()).ToLowerInvariant();

						if (!string.IsNullOrEmpty(objectClass)) 
						{
							//now find opening brace
							if(!NextTokenIs("{")) continue;

							int bracesCount = 1;
							bool foundLight = false;
							bool foundFrame = false;

							//read frames structure
							while (SkipWhitespace(true)) 
							{
								token = ReadToken();
								if (!string.IsNullOrEmpty(token)) 
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

										if (!string.IsNullOrEmpty(token)) 
										{
											if (lightsByName.ContainsKey(token)) 
											{
												if (objects.ContainsKey(objectClass))
													objects[objectClass] = token;
												else
													objects.Add(objectClass, token);
												foundLight = true;
											} 
											else 
											{
												General.ErrorLogger.Add(ErrorType.Warning, "Light declaration not found for light '" + token + "' ('" + sourcefilename + "', line " + GetCurrentLineNumber()+").");
											}
										}
									} 
									else if (token == "{") //continue in this loop until object structure ends
									{ 
										bracesCount++;
									} 
									else if (token == "}") 
									{
										if (--bracesCount < 1) break; //This was Cave Johnson. And we are done here.
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
										Height = 128,
										Fullbright = true,
										Color = new PixelColor(255, 255, 255, 255),
										CalculateTextureColor = true
									};
								}
							} 
							else if(token == "texture") 
							{
								string texturename = StripTokenQuotes(ReadToken());
								if(string.IsNullOrEmpty(texturename))
								{
									General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected a texture name.");
									break;
								}
								
								// Now we should find a comma
								if(!NextTokenIs(",")) break;

								// Next is color
								SkipWhitespace(true);
								token = ReadToken();

								int color;
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
										General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected glow color value, but got '" + token + "'.");
										break;
									}
								}

								// The glow data is valid at thispoint. Let's get texture hash 
								long texturehash = General.Map.Data.GetFullLongFlatName(Lump.MakeLongName(texturename, General.Map.Options.UseLongTextureNames));

								// Now we can find a comma
								if(!NextTokenIs(",", false))
								{
									// Add glow data
									glowingflats[texturehash] = new GlowingFlatData {
										Height = 128,
										Fullbright = false,
										Color = PixelColor.FromInt(color).WithAlpha(255),
										CalculateTextureColor = false
									};

									continue;
								}

								// Should be glow height
								int height;
								if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out height)) 
								{
									General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected glow height value, but got '" + token + "'.");
									break;
								}

								// Now we can find a comma
								if(!NextTokenIs(",", false)) 
								{
									// Add glow data
									glowingflats[texturehash] = new GlowingFlatData {
										Height = height * 2,
										Fullbright = false,
										Color = PixelColor.FromInt(color).WithAlpha(255),
										CalculateTextureColor = false
									};

									continue;
								}

								// Next is "fullbright" flag
								SkipWhitespace(true);
								bool fullbright = (ReadToken().ToLowerInvariant() == "fullbright");

								// Add glow data
								glowingflats[texturehash] = new GlowingFlatData {
									Height = height,
									Fullbright = fullbright,
									Color = PixelColor.FromInt(color).WithAlpha(255),
									CalculateTextureColor = false
								};
							}
						}

						// Now find closing brace
						while(SkipWhitespace(true))
						{
							string t = ReadToken();
							if(string.IsNullOrEmpty(t) || t == "}") break;
						}
					}
					else if (token == "#include") 
					{
						SkipWhitespace(true);
						string includeLump = StripTokenQuotes(ReadToken()).ToLowerInvariant();
						if (!string.IsNullOrEmpty(includeLump)) 
						{
							// Callback to parse this file
							if (OnInclude != null)
								OnInclude(this, includeLump.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));

							// Set our buffers back to continue parsing
							datastream = localstream;
							datareader = localreader;
							sourcename = localsourcename;
						} 
						else 
						{
							General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": got #include directive with missing or incorrect path: '" + includeLump + "'.");
						}
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
							if (!SkipWhitespace(true)) break;
							token2 = ReadToken();
							if (string.IsNullOrEmpty(token2)) break;
						}
						while (token2 != "{");
						int scopelevel = 1;
						do 
						{
							if (!SkipWhitespace(true)) break;
							token2 = ReadToken();
							if(string.IsNullOrEmpty(token2)) break;
							if (token2 == "{") scopelevel++;
							if (token2 == "}") scopelevel--;
						}
						while (scopelevel > 0);
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

		#endregion
	}
}