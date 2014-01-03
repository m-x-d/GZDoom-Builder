using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using SlimDX;
using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.GZBuilder.Data;

namespace CodeImp.DoomBuilder.GZBuilder.GZDoom {
	public sealed class MapinfoParser : ZDTextParser {

		private MapInfo mapInfo;
		public MapInfo MapInfo { get { return mapInfo; } }

		public bool Parse(Stream stream, string sourcefilename, string mapName) {
			base.Parse(stream, sourcefilename);

			mapName = mapName.ToLowerInvariant();
			mapInfo = new MapInfo();

			while (SkipWhitespace(true)) {
				string token = ReadToken();
				if (token != null) {
					token = token.ToLowerInvariant();

					if (parseBlock(token, mapName))
						break;
				}
			}

			//check values
			if (mapInfo.FadeColor.Red > 0 || mapInfo.FadeColor.Green > 0 || mapInfo.FadeColor.Blue > 0)
				mapInfo.HasFadeColor = true;

			if (mapInfo.OutsideFogColor.Red > 0 || mapInfo.OutsideFogColor.Green > 0 || mapInfo.OutsideFogColor.Blue > 0)
				mapInfo.HasOutsideFogColor = true;

			//Cannot fail here
			return true;
		}

		//returns true if parsing is finished
		private bool parseBlock(string token, string mapName) {
			string curBlockName;

			if (token == "map" || token == "defaultmap" || token == "adddefaultmap") {
				curBlockName = token;

				if (token == "map") { //check map name
					//get map name
					SkipWhitespace(true);
					token = ReadToken().ToLowerInvariant();

					if (token != mapName)
						return false; //not finished, search for next "map", "defaultmap" or "adddefaultmap" block
				} else if (token == "defaultmap") {
					//reset MapInfo
					mapInfo = new MapInfo();
				}

				//search for required keys
				while (SkipWhitespace(true)) {
					token = ReadToken().ToLowerInvariant();

//sky1 or sky2
					if (token == "sky1" || token == "sky2") {
						string skyType = token;
						SkipWhitespace(true);
						token = StripTokenQuotes(ReadToken()).ToLowerInvariant();

						//new format
						if (token == "=") {
							SkipWhitespace(true);

							//should be sky texture name
							token = StripTokenQuotes(ReadToken());
							bool gotComma = (token.IndexOf(",") != -1);
							if (gotComma) token = token.Replace(",", "");
							string skyTexture = StripTokenQuotes(token).ToLowerInvariant();

							if (!string.IsNullOrEmpty(skyTexture)) {
								if (skyType == "sky1")
									mapInfo.Sky1 = skyTexture;
								else
									mapInfo.Sky2 = skyTexture;

								//check if we have scrollspeed
								SkipWhitespace(true);
								token = StripTokenQuotes(ReadToken());

								if (!gotComma && token == ",") {
									gotComma = true;
									SkipWhitespace(true);
									token = ReadToken();
								}

								if (gotComma) {
									float scrollSpeed = 0;

									if (!ReadSignedFloat(token, ref scrollSpeed)) {
										// Not numeric!
										General.ErrorLogger.Add(ErrorType.Warning, "Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + skyType + " scroll speed value, but got '" + token + "'");
										datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
										continue;
									}

									if (skyType == "sky1")
										mapInfo.Sky1ScrollSpeed = scrollSpeed;
									else
										mapInfo.Sky2ScrollSpeed = scrollSpeed;
								} else {
									datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
								}
							} else {
								datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
								General.ErrorLogger.Add(ErrorType.Error, "Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + skyType + " texture name.");
							}
							//old format
						} else {
							//token should be sky1/2 name
							if (!string.IsNullOrEmpty(token)) {
								if (skyType == "sky1")
									mapInfo.Sky1 = token;
								else
									mapInfo.Sky2 = token;

								//try to read scroll speed
								SkipWhitespace(true);
								token = StripTokenQuotes(ReadToken());

								float scrollSpeed = 0;
								if (!ReadSignedFloat(token, ref scrollSpeed)) {
									// Not numeric!
									datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
									continue;
								}

								if (skyType == "sky1")
									mapInfo.Sky1ScrollSpeed = scrollSpeed;
								else
									mapInfo.Sky2ScrollSpeed = scrollSpeed;

							} else {
								datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
								General.ErrorLogger.Add(ErrorType.Error, "Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + skyType + " texture name.");
							}
						}
//fade or outsidefog
					} else if (token == "fade" || token == "outsidefog") {
						string fadeType = token;
						SkipWhitespace(true);
						token = StripTokenQuotes(ReadToken()).ToLowerInvariant();

						//new format?
						if (token == "=") {
							SkipWhitespace(true);
							token = ReadToken();
						}

						//get the color value
						string colorVal = StripTokenQuotes(token).ToLowerInvariant().Replace(" ", "");
						if(!string.IsNullOrEmpty(colorVal)) {
							Color4 color = new Color4();

							//try to get the color...
							if(getColor(colorVal, ref color)) {
								if(fadeType == "fade")
									mapInfo.FadeColor = color;
								else
									mapInfo.OutsideFogColor = color;
							} else { //...or not
								General.ErrorLogger.Add(ErrorType.Error, "Failed to parse " + fadeType + " value from string '" + colorVal + "' in '" + sourcename + "' at line " + GetCurrentLineNumber());
								datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
							}
						} else {
							General.ErrorLogger.Add(ErrorType.Error, "Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + fadeType + " color value.");
							datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
						}
//vertwallshade or horizwallshade
					} else if (token == "vertwallshade" || token == "horizwallshade") {
						string shadeType = token;
						SkipWhitespace(true);
						token = StripTokenQuotes(ReadToken());

						//new format
						if(token == "=") {
							SkipWhitespace(true);
							token = StripTokenQuotes(ReadToken());
						}

						int val = 0;
						if(!ReadSignedInt(token, ref val)) {
							// Not numeric!
							General.ErrorLogger.Add(ErrorType.Error, "Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + shadeType + " value, but got '" + token + "'");
							datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
							continue;
						}

						if(shadeType == "vertwallshade")
							mapInfo.VertWallShade = General.Clamp(val, -255, 255);
						else
							mapInfo.HorizWallShade = General.Clamp(val, -255, 255);
//fogdensity or outsidefogdensity
					} else if(token == "fogdensity" || token == "outsidefogdensity") {
						string densityType = token;
						SkipWhitespace(true);
						token = StripTokenQuotes(ReadToken());

						//new format
						if(token == "=") {
							SkipWhitespace(true);
							token = StripTokenQuotes(ReadToken());
						}

						int val;
						if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out val)) {
							// Not numeric!
							General.ErrorLogger.Add(ErrorType.Error, "Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + densityType + " value, but got '" + token + "'");
							datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
							continue;
						}

						if (densityType == "fogdensity") {
							mapInfo.FogDensity = (int)(1024 * (256.0f / val));
						} else {
							mapInfo.OutsideFogDensity = (int)(1024 * (256.0f / val));
						}
						//doublesky
					} else if (token == "doublesky") {
						mapInfo.DoubleSky = true;
//evenlighting
					} else if (token == "evenlighting") {
						mapInfo.EvenLighting = true;
//smoothlighting
					} else if (token == "smoothlighting") {
						mapInfo.SmoothLighting = true;
						//block end
					} else if (token == "}") {
						return (curBlockName == "map" || parseBlock(token, mapName));
					}
				}
			}
			return false;
		}

		private bool getColor(string name, ref Color4 color) {
			if (name == "black") return true;

			//probably it's a hex color (like FFCC11)?
			int ci;
			if (int.TryParse(name, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ci)) {
				color = new Color4(ci) {Alpha = 1.0f};
				return true;
			}

			//probably it's a color name?
			Color c = Color.FromName(name); //should be similar to C++ color name detection, I suppose
			if (c.IsKnownColor) {
				color = new Color4(c);
				return true;
			}
			return false;
		}
	}
}
