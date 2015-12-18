#region ================== Namespaces

using System;
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

		public delegate void IncludeDelegate(MapinfoParser parser, string includefile, bool clearerror);
		public IncludeDelegate OnInclude;

		#endregion

		#region ================== Variables

		private MapInfo mapinfo;
		private string mapname;
		private readonly Dictionary<int, string> spawnnums;
		private readonly Dictionary<int, string> doomednums; // <DoomEdNum, <lowercase classname, List of default arguments>>
		private readonly HashSet<string> parsedlumps;

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
			specialtokens = ",{}=\n";
			
			mapinfo = new MapInfo();
			spawnnums = new Dictionary<int, string>();
			doomednums = new Dictionary<int, string>();
			parsedlumps = new HashSet<string>();
		}

		#endregion

		#region ================== Parsing


		override public bool Parse(Stream stream, string sourcefilename, bool clearerrors)
		{
			if(string.IsNullOrEmpty(mapname)) throw new NotSupportedException("MapName is required!");
			return Parse(stream, sourcefilename, mapname, clearerrors);
		}

		public bool Parse(Stream stream, string sourcefilename, string mapname, bool clearerrors) 
		{
			this.mapname = mapname.ToLowerInvariant();
			parsedlumps.Add(sourcefilename);
			if(!base.Parse(stream, sourcefilename, clearerrors)) return false;

			while(SkipWhitespace(true)) 
			{
				string token = ReadToken();
				if(!string.IsNullOrEmpty(token)) 
				{
					token = token.ToLowerInvariant();
					if(ParseBlock(token, clearerrors)) break;
				}
			}

			//check values
			if(mapinfo.FadeColor.Red > 0 || mapinfo.FadeColor.Green > 0 || mapinfo.FadeColor.Blue > 0)
				mapinfo.HasFadeColor = true;

			if(mapinfo.OutsideFogColor.Red > 0 || mapinfo.OutsideFogColor.Green > 0 || mapinfo.OutsideFogColor.Blue > 0)
				mapinfo.HasOutsideFogColor = true;

			//Cannot fail here
			return true;
		}

		//returns true if parsing is finished
		private bool ParseBlock(string token, bool clearerrors) 
		{
			// Keep local data
			Stream localstream = datastream;
			string localsourcename = sourcename;
			BinaryReader localreader = datareader;
			
			if(token == "map" || token == "defaultmap" || token == "adddefaultmap") 
			{
				switch(token)
				{
					case "map":
						//get map name
						SkipWhitespace(true);
						token = ReadToken().ToLowerInvariant();
						if(token != mapname) return false; //not finished, search for next "map", "defaultmap" or "adddefaultmap" block
						break;

					case "defaultmap":
						//reset MapInfo
						mapinfo = new MapInfo();
						break;
				}

				// Track brace level
				int bracelevel = 0;

				//search for required keys
				while(SkipWhitespace(true)) 
				{
					token = ReadToken().ToLowerInvariant();
//sky1 or sky2
					if(token == "sky1" || token == "sky2") 
					{
						string skyType = token;
						SkipWhitespace(true);
						token = StripTokenQuotes(ReadToken()).ToLowerInvariant();

						//new format
						if(token == "=") 
						{
							SkipWhitespace(true);

							//should be sky texture name
							token = StripTokenQuotes(ReadToken());
							bool gotComma = (token.IndexOf(",") != -1);
							if(gotComma) token = token.Replace(",", "");
							string skyTexture = StripTokenQuotes(token).ToLowerInvariant();

							if(!string.IsNullOrEmpty(skyTexture)) 
							{
								if(skyType == "sky1")
									mapinfo.Sky1 = skyTexture;
								else
									mapinfo.Sky2 = skyTexture;

								//check if we have scrollspeed
								SkipWhitespace(true);
								token = StripTokenQuotes(ReadToken());

								if(!gotComma && token == ",") 
								{
									gotComma = true;
									SkipWhitespace(true);
									token = ReadToken();
								}

								if(gotComma) 
								{
									float scrollSpeed = 0;
									if(!ReadSignedFloat(token, ref scrollSpeed)) 
									{
										// Not numeric!
										ReportError("expected " + skyType + " scroll speed value, but got '" + token + "'");
										return false;
									}

									if(skyType == "sky1")
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
								ReportError("expected " + skyType + " texture name");
								return false;
							}
						}
						//old format
						else 
						{
							//token should be sky1/2 name
							if(!string.IsNullOrEmpty(token)) 
							{
								if(skyType == "sky1")
									mapinfo.Sky1 = token;
								else
									mapinfo.Sky2 = token;

								//try to read scroll speed
								SkipWhitespace(true);
								token = StripTokenQuotes(ReadToken());

								float scrollSpeed = 0;
								if(!ReadSignedFloat(token, ref scrollSpeed)) 
								{
									// Not numeric!
									datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
									continue;
								}

								if(skyType == "sky1")
									mapinfo.Sky1ScrollSpeed = scrollSpeed;
								else
									mapinfo.Sky2ScrollSpeed = scrollSpeed;

							} 
							else 
							{
								ReportError("expected " + skyType + " texture name");
								return false;
							}
						}
					}
//fade or outsidefog
					else if(token == "fade" || token == "outsidefog") 
					{
						string fadeType = token;
						SkipWhitespace(true);
						token = StripTokenQuotes(ReadToken()).ToLowerInvariant();

						//new format?
						if(token == "=") 
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
								ReportError("failed to parse " + fadeType + " value from string '" + colorVal + "'");
								return false;
							}
						} 
						else 
						{
							ReportError("expected " + fadeType + " color value");
							return false;
						}
					}
//vertwallshade or horizwallshade
					else if(token == "vertwallshade" || token == "horizwallshade") 
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
							ReportError("expected " + shadeType + " value, but got '" + token + "'");
							return false;
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
							ReportError("expected " + densityType + " value, but got '" + token + "'");
							return false;
						}

						if(densityType == "fogdensity")
							mapinfo.FogDensity = (int)(1024 * (256.0f / val));
						else
							mapinfo.OutsideFogDensity = (int)(1024 * (256.0f / val));
					}
//doublesky
					else if(token == "doublesky") 
					{
						mapinfo.DoubleSky = true;
					}
//evenlighting
					else if(token == "evenlighting") 
					{
						mapinfo.EvenLighting = true;
					}
//smoothlighting
					else if (token == "smoothlighting") 
					{
						mapinfo.SmoothLighting = true;
					}
//block end
					else if(token == "}") 
					{
						return ParseBlock(token, clearerrors);
					}
//child block
					else if(token == "{")
					{
						// Skip inner properties
						bracelevel++;
						if(bracelevel > 1)
						{
							do
							{
								SkipWhitespace(true);
								token = ReadToken();
								if(token == "{") bracelevel++;
								else if(token == "}") bracelevel--;
							} while(!string.IsNullOrEmpty(token) && bracelevel > 1);
						}
					}
				}
			} 
			else if(token == "include")
			{
				SkipWhitespace(true);
				string includelump = StripTokenQuotes(ReadToken(false)).ToLowerInvariant(); // Don't skip newline
				
				if(!string.IsNullOrEmpty(includelump)) 
				{
					// Callback to parse this file
					if(OnInclude != null)
						OnInclude(this, includelump.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar), clearerrors);

					// Set our buffers back to continue parsing
					datastream = localstream;
					datareader = localreader;
					sourcename = localsourcename;
				} 
				else 
				{
					ReportError("got include directive with missing or incorrect path: '" + includelump + "'");
					return false;
				}
			}
			else if(token == "gameinfo")
			{
				if(!NextTokenIs("{")) return false; // Finished with this file

				while(SkipWhitespace(true))
				{
					token = ReadToken();
					if(string.IsNullOrEmpty(token))
					{
						ReportError("failed to find the end of GameInfo block");
						return false; // Finished with this file
					}
					if(token == "}") break;

					if(token == "skyflatname")
					{
						if(!NextTokenIs("=")) return false; // Finished with this file
						SkipWhitespace(true);
						string skyflatname = StripTokenQuotes(ReadToken());
						if(string.IsNullOrEmpty(skyflatname)) 
						{
							ReportError("unable to get SkyFlatName value");
							return false; // Finished with this file
						}

						General.Map.Config.SkyFlatName = skyflatname.ToUpperInvariant();
					}
				}
			}
			else if(token == "doomednums")
			{
				if(!NextTokenIs("{")) return false; // Finished with this file

				while(SkipWhitespace(true)) 
				{
					token = ReadToken();
					if(string.IsNullOrEmpty(token)) 
					{
						ReportError("failed to find the end of DoomEdNums block");
						return false; // Finished with this file
					}
					if(token == "}") break;

					// First must be a number
					int id;
					if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out id)) 
					{
						// Not numeric!
						ReportError("expected DoomEdNums entry number, but got '" + token + "'");
						return false; // Finished with this file
					}

					// Then "="
					if(!NextTokenIs("=")) return false; // Finished with this file

					// Then actor class
					SkipWhitespace(false);
					string classname = StripTokenQuotes(ReadToken());
					if(string.IsNullOrEmpty(classname)) 
					{
						ReportError("unable to get DoomEdNums entry class definition");
						return false; // Finished with this file
					}

					// Possible special and args. We'll skip them
					for(int i = 0; i < 6; i++)
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
				if(!NextTokenIs("{")) return false; // Finished with this file

				while(SkipWhitespace(true))
				{
					token = ReadToken();
					if(string.IsNullOrEmpty(token)) 
					{
						ReportError("failed to find the end of SpawnNums block");
						return false; // Finished with this file
					}
					if(token == "}") break;

					// First must be a number
					int id;
					if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out id)) 
					{
						// Not numeric!
						ReportError("expected SpawnNums number, but got '" + token + "'");
						return false; // Finished with this file
					}

					// Then "="
					if(!NextTokenIs("=")) return false; // Finished with this file

					// Then actor class
					SkipWhitespace(false);
					token = StripTokenQuotes(ReadToken());
					if(string.IsNullOrEmpty(token))
					{
						ReportError("unable to get SpawnNums entry class definition");
						return false;
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
			if(name == "black") return true;

			//probably it's a hex color (like FFCC11)?
			int ci;
			if(int.TryParse(name, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ci)) 
			{
				color = new Color4(ci) {Alpha = 1.0f};
				return true;
			}

			//probably it's a color name?
			Color c = Color.FromName(name); //should be similar to C++ color name detection, I suppose
			if(c.IsKnownColor) 
			{
				color = new Color4(c);
				return true;
			}
			return false;
		}

		protected override string GetLanguageType()
		{
			return "MAPINFO";
		}

		#endregion
	}
}
