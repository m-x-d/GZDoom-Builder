#region ================== Namespaces

using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using SlimDX;
using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.GZBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.GZDoom 
{
	internal sealed class MapinfoParser : ZDTextParser
	{
		#region ================== Delegates

		public delegate void IncludeDelegate(MapinfoParser parser, string includefile);
		public IncludeDelegate OnInclude;

		#endregion

		#region ================== Variables

		private MapInfo mapinfo;
		private readonly Dictionary<int, string> spawnnums;
		private readonly Dictionary<int, string> doomednums; // <DoomEdNum, <lowercase classname, List of default arguments>>

		#endregion

		#region ================== Properties

		public MapInfo MapInfo { get { return mapinfo; } }
		public Dictionary<int, string> SpawnNums { get { return spawnnums; } }
		public Dictionary<int, string> DoomEdNums { get { return doomednums; } }

		#endregion

		#region ================== Constructor

		public MapinfoParser()
		{
			// Syntax
			whitespace = "\n \t\r\u00A0";
			specialtokens = ",{}\n";
			
			mapinfo = new MapInfo();
			spawnnums = new Dictionary<int, string>();
			doomednums = new Dictionary<int, string>();
		}

		#endregion

		#region ================== Parsing

		public bool Parse(Stream stream, string sourcefilename, string mapname) 
		{
			base.Parse(stream, sourcefilename);
			mapname = mapname.ToLowerInvariant();

			while (SkipWhitespace(true)) 
			{
				string token = ReadToken();
				if (token != null) 
				{
					token = token.ToLowerInvariant();
					if (ParseBlock(token, mapname)) break;
				}
			}

			//check values
			if (mapinfo.FadeColor.Red > 0 || mapinfo.FadeColor.Green > 0 || mapinfo.FadeColor.Blue > 0)
				mapinfo.HasFadeColor = true;

			if (mapinfo.OutsideFogColor.Red > 0 || mapinfo.OutsideFogColor.Green > 0 || mapinfo.OutsideFogColor.Blue > 0)
				mapinfo.HasOutsideFogColor = true;

			//Cannot fail here
			return true;
		}

		//returns true if parsing is finished
		private bool ParseBlock(string token, string mapName) 
		{
			// Keep local data
			Stream localstream = datastream;
			string localsourcename = sourcename;
			BinaryReader localreader = datareader;
			
			if (token == "map" || token == "defaultmap" || token == "adddefaultmap") 
			{
				string curBlockName = token;
				switch(token)
				{
					case "map":
						//get map name
						SkipWhitespace(true);
						token = ReadToken().ToLowerInvariant();
						if (token != mapName) return false; //not finished, search for next "map", "defaultmap" or "adddefaultmap" block
						break;

					case "defaultmap":
						//reset MapInfo
						mapinfo = new MapInfo();
						break;
				}

				//search for required keys
				while (SkipWhitespace(true)) 
				{
					token = ReadToken().ToLowerInvariant();
//sky1 or sky2
					if (token == "sky1" || token == "sky2") 
					{
						string skyType = token;
						SkipWhitespace(true);
						token = StripTokenQuotes(ReadToken()).ToLowerInvariant();

						//new format
						if (token == "=") 
						{
							SkipWhitespace(true);

							//should be sky texture name
							token = StripTokenQuotes(ReadToken());
							bool gotComma = (token.IndexOf(",") != -1);
							if (gotComma) token = token.Replace(",", "");
							string skyTexture = StripTokenQuotes(token).ToLowerInvariant();

							if (!string.IsNullOrEmpty(skyTexture)) 
							{
								if (skyType == "sky1")
									mapinfo.Sky1 = skyTexture;
								else
									mapinfo.Sky2 = skyTexture;

								//check if we have scrollspeed
								SkipWhitespace(true);
								token = StripTokenQuotes(ReadToken());

								if (!gotComma && token == ",") 
								{
									gotComma = true;
									SkipWhitespace(true);
									token = ReadToken();
								}

								if (gotComma) 
								{
									float scrollSpeed = 0;
									if (!ReadSignedFloat(token, ref scrollSpeed)) 
									{
										// Not numeric!
										General.ErrorLogger.Add(ErrorType.Warning, "Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + skyType + " scroll speed value, but got '" + token + "'");
										datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
										continue;
									}

									if (skyType == "sky1")
										mapinfo.Sky1ScrollSpeed = scrollSpeed;
									else
										mapinfo.Sky2ScrollSpeed = scrollSpeed;
								} 
								else 
								{
									datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
								}
							} 
							else 
							{
								datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
								General.ErrorLogger.Add(ErrorType.Error, "Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + skyType + " texture name.");
							}
						}
						//old format
						else 
						{
							//token should be sky1/2 name
							if (!string.IsNullOrEmpty(token)) 
							{
								if (skyType == "sky1")
									mapinfo.Sky1 = token;
								else
									mapinfo.Sky2 = token;

								//try to read scroll speed
								SkipWhitespace(true);
								token = StripTokenQuotes(ReadToken());

								float scrollSpeed = 0;
								if (!ReadSignedFloat(token, ref scrollSpeed)) 
								{
									// Not numeric!
									datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
									continue;
								}

								if (skyType == "sky1")
									mapinfo.Sky1ScrollSpeed = scrollSpeed;
								else
									mapinfo.Sky2ScrollSpeed = scrollSpeed;

							} 
							else 
							{
								datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
								General.ErrorLogger.Add(ErrorType.Error, "Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + skyType + " texture name.");
							}
						}
					}
//fade or outsidefog
					else if (token == "fade" || token == "outsidefog") 
					{
						string fadeType = token;
						SkipWhitespace(true);
						token = StripTokenQuotes(ReadToken()).ToLowerInvariant();

						//new format?
						if (token == "=") 
						{
							SkipWhitespace(true);
							token = ReadToken();
						}

						//get the color value
						string colorVal = StripTokenQuotes(token).ToLowerInvariant().Replace(" ", "");
						if(!string.IsNullOrEmpty(colorVal)) 
						{
							Color4 color = new Color4();
							//try to get the color...
							if(GetColor(colorVal, ref color)) 
							{
								if(fadeType == "fade")
									mapinfo.FadeColor = color;
								else
									mapinfo.OutsideFogColor = color;
							} 
							else //...or not
							{ 
								General.ErrorLogger.Add(ErrorType.Error, "Failed to parse " + fadeType + " value from string '" + colorVal + "' in '" + sourcename + "' at line " + GetCurrentLineNumber());
								datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
							}
						} 
						else 
						{
							General.ErrorLogger.Add(ErrorType.Error, "Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + fadeType + " color value.");
							datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
						}
					}
//vertwallshade or horizwallshade
					else if (token == "vertwallshade" || token == "horizwallshade") 
					{
						string shadeType = token;
						SkipWhitespace(true);
						token = StripTokenQuotes(ReadToken());

						//new format
						if(token == "=") 
						{
							SkipWhitespace(true);
							token = StripTokenQuotes(ReadToken());
						}

						int val = 0;
						if(!ReadSignedInt(token, ref val)) 
						{
							// Not numeric!
							General.ErrorLogger.Add(ErrorType.Error, "Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + shadeType + " value, but got '" + token + "'");
							datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
							continue;
						}

						if(shadeType == "vertwallshade")
							mapinfo.VertWallShade = General.Clamp(val, -255, 255);
						else
							mapinfo.HorizWallShade = General.Clamp(val, -255, 255);
					}
//fogdensity or outsidefogdensity
					else if(token == "fogdensity" || token == "outsidefogdensity") 
					{
						string densityType = token;
						SkipWhitespace(true);
						token = StripTokenQuotes(ReadToken());

						//new format
						if(token == "=") 
						{
							SkipWhitespace(true);
							token = StripTokenQuotes(ReadToken());
						}

						int val;
						if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out val)) 
						{
							// Not numeric!
							General.ErrorLogger.Add(ErrorType.Error, "Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + densityType + " value, but got '" + token + "'");
							datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
							continue;
						}

						if (densityType == "fogdensity")
							mapinfo.FogDensity = (int)(1024 * (256.0f / val));
						else
							mapinfo.OutsideFogDensity = (int)(1024 * (256.0f / val));
					}
//doublesky
					else if (token == "doublesky") 
					{
						mapinfo.DoubleSky = true;

					}
//evenlighting
					else if (token == "evenlighting") 
					{
						mapinfo.EvenLighting = true;
					}
//smoothlighting
					else if (token == "smoothlighting") 
					{
						mapinfo.SmoothLighting = true;
					}
//block end
					else if (token == "}") 
					{
						return (curBlockName == "map" || ParseBlock(token, mapName));
					}
				}
			} 
			else if(token == "#include")
			{
				SkipWhitespace(true);
				string includeLump = StripTokenQuotes(ReadToken()).ToLowerInvariant();
				if(!string.IsNullOrEmpty(includeLump)) 
				{
					// Callback to parse this file
					if(OnInclude != null)
						OnInclude(this, includeLump.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));

					// Set our buffers back to continue parsing
					datastream = localstream;
					datareader = localreader;
					sourcename = localsourcename;
				} 
				else 
				{
					General.ErrorLogger.Add(ErrorType.Error, "Error in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": got #include directive with missing or incorrect path: '" + includeLump + "'.");
				}
			} 
			else if(token == "doomednums")
			{
				if(!NextTokenIs("{")) return true; // Finished with this file

				while(SkipWhitespace(true)) 
				{
					token = ReadToken();
					if(string.IsNullOrEmpty(token)) 
					{
						General.ErrorLogger.Add(ErrorType.Error, "Error while parisng '" + sourcename + "' at line " + GetCurrentLineNumber() + ": failed to find the end of DoomEdNums block");
						return true; // Finished with this file
					}
					if(token == "}") break;

					// First must be a number
					int id;
					if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out id)) 
					{
						// Not numeric!
						General.ErrorLogger.Add(ErrorType.Error, "Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected DoomEdNums entry number, but got '" + token + "'");
						return true; // Finished with this file
					}

					// Then "="
					if(!NextTokenIs("=")) return true; // Finished with this file

					// Then actor class
					SkipWhitespace(false);
					string classname = StripTokenQuotes(ReadToken());
					if(string.IsNullOrEmpty(classname)) 
					{
						General.ErrorLogger.Add(ErrorType.Error, "Error while parisng '" + sourcename + "' at line " + GetCurrentLineNumber() + ": unable to get DoomEdNums entry class definition");
						return true; // Finished with this file
					}

					// Possible special and args. We'll skip them
					for (int i = 0; i < 6; i++)
					{
						if(!NextTokenIs(",", false)) break;

						// Read special name or arg value
						if(!SkipWhitespace(true) || string.IsNullOrEmpty(ReadToken())) return false;
					}

					// Add to collection?
					if(id != 0) doomednums[id] = classname.ToLowerInvariant();
				}
			} 
			else if(token == "spawnnums")
			{
				if(!NextTokenIs("{")) return true; // Finished with this file

				while (SkipWhitespace(true))
				{
					token = ReadToken();
					if(string.IsNullOrEmpty(token)) 
					{
						General.ErrorLogger.Add(ErrorType.Error, "Error while parisng '" + sourcename + "' at line " + GetCurrentLineNumber() + ": failed to find the end of SpawnNums block");
						return true; // Finished with this file
					}
					if(token == "}") break;

					// First must be a number
					int id;
					if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out id)) 
					{
						// Not numeric!
						General.ErrorLogger.Add(ErrorType.Error, "Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected SpawnNums number, but got '" + token + "'");
						return true; // Finished with this file
					}

					// Then "="
					if(!NextTokenIs("=")) return true; // Finished with this file

					// Then actor class
					SkipWhitespace(false);
					token = StripTokenQuotes(ReadToken());
					if(string.IsNullOrEmpty(token))
					{
						General.ErrorLogger.Add(ErrorType.Error, "Error while parisng '" + sourcename + "' at line " + GetCurrentLineNumber() + ": unable to get SpawnNums entry class definition");
						return true;
					}

					// Add to collection
					spawnnums[id] = token.ToLowerInvariant();
				}
			} 
			else if(token == "$gzdb_skip")
			{
				return true; // Finished with this file
			}

			return false; // Not done yet
		}

		#endregion

		#region ================== Methods

		private static bool GetColor(string name, ref Color4 color) 
		{
			if (name == "black") return true;

			//probably it's a hex color (like FFCC11)?
			int ci;
			if (int.TryParse(name, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ci)) 
			{
				color = new Color4(ci) {Alpha = 1.0f};
				return true;
			}

			//probably it's a color name?
			Color c = Color.FromName(name); //should be similar to C++ color name detection, I suppose
			if (c.IsKnownColor) 
			{
				color = new Color4(c);
				return true;
			}
			return false;
		}

		#endregion
	}
}
