using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Text;

using SlimDX;
using SlimDX.Direct3D9;

using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.GZBuilder.Data;

namespace CodeImp.DoomBuilder.GZBuilder.GZDoom {
    public class MapinfoParser : ZDTextParser {

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
            if (mapInfo.FadeColor != null && (mapInfo.FadeColor.Red > 0 || mapInfo.FadeColor.Green > 0 || mapInfo.FadeColor.Blue > 0))
                mapInfo.HasFadeColor = true;

            if (mapInfo.OutsideFogColor != null && (mapInfo.OutsideFogColor.Red > 0 || mapInfo.OutsideFogColor.Green > 0 || mapInfo.OutsideFogColor.Blue > 0))
                mapInfo.HasOutsideFogColor = true;

            //Cannot fail here
            return true;
        }

        //returns true if parsing is finished
        private bool parseBlock(string token, string mapName) {
            string curBlockName;
            mapName = mapName.ToLowerInvariant();

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
                        //Form1.Trace("Got sky " + token);

                        string skyType = token;
                        SkipWhitespace(true);
                        token = StripTokenQuotes(ReadToken()).ToLowerInvariant();

                        //new format
                        if (token == "=") {
                            SkipWhitespace(true);

                            //should be sky texture name
                            token = StripTokenQuotes(ReadToken());
                            bool gotComma = (token.IndexOf(",") != -1);
                            if (gotComma)
                                token = token.Replace(",", "");
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
                                        GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + skyType + " scroll speed value, but got '" + token + "'");
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
                                GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + skyType + " texture name.");
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
                                    GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + skyType + " scroll speed value, but got '" + token + "'");
                                    datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
                                    continue;
                                }

                                if (skyType == "sky1")
                                    mapInfo.Sky1ScrollSpeed = scrollSpeed;
                                else
                                    mapInfo.Sky2ScrollSpeed = scrollSpeed;

                            } else {
                                datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
                                GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + skyType + " texture name.");
                            }
                        }
                        //fade or outsidefog
                    } else if (token == "fade" || token == "outsidefog") {
                        string fadeType = token;
                        SkipWhitespace(true);
                        token = StripTokenQuotes(ReadToken()).ToLowerInvariant();

                        //new format
                        if (token == "=") {
                            SkipWhitespace(true);

                            //red color value or color name...
                            token = ReadToken();
                            string colorVal = StripTokenQuotes(token).ToLowerInvariant();
                            if (!string.IsNullOrEmpty(colorVal)) {
                                Color4 color = new Color4();
                                //color.Alpha = 1.0f;

                                //is it color name?
                                if (getColorByName(colorVal, ref color)) {
                                    if (fadeType == "fade")
                                        mapInfo.FadeColor = color;
                                    else
                                        mapInfo.OutsideFogColor = color;
                                } else { //no, it's not
                                    //try to get color values
                                    int r, g, b;
                                    string[] parts = colorVal.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                                    if (parts.Length != 3) {
                                        GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + fadeType + " color values, but got '" + token + "'");
                                        datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
                                        continue;
                                    }

                                    if (!int.TryParse(parts[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r)) {
                                        GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + fadeType + " red value, but got '" + parts[0] + "'");
                                        datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
                                        continue;
                                    }
                                    if (!int.TryParse(parts[1], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g)) {
                                        GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + fadeType + " green value, but got '" + parts[1] + "'");
                                        datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
                                        continue;
                                    }
                                    if (!int.TryParse(parts[2], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b)) {
                                        GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + fadeType + " blue value, but got '" + parts[2] + "'");
                                        datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
                                        continue;
                                    }

                                    color.Red = (float)r / 255;
                                    color.Green = (float)g / 255;
                                    color.Blue = (float)b / 255;

                                    if (fadeType == "fade")
                                        mapInfo.FadeColor = color;
                                    else
                                        mapInfo.OutsideFogColor = color;
                                }
                            } else {
                                GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + fadeType + " color value.");
                                datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
                            }

                            //old format
                        } else {
                            //token should contain red color value or color name...
                            if (!string.IsNullOrEmpty(token)) {
                                int r, g, b;
                                Color4 color = new Color4();

                                if (!int.TryParse(token, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r)) {
                                    //Not numeric! Maybe it's a color name?
                                    if (getColorByName(token, ref color)) {
                                        if (fadeType == "fade")
                                            mapInfo.FadeColor = color;
                                        else
                                            mapInfo.OutsideFogColor = color;
                                    } else {
                                        datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
                                    }
                                    continue;
                                }

                                SkipWhitespace(true);
                                token = ReadToken();

                                //should be color, let's continue parsing it.
                                if (!int.TryParse(token, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g)) {
                                    // Not numeric!
                                    GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + fadeType + " green value, but got '" + token + "'");
                                    datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
                                    continue;
                                }

                                SkipWhitespace(true);
                                token = StripTokenQuotes(ReadToken());

                                if (!int.TryParse(token, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b)) {
                                    // Not numeric!
                                    GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + fadeType + " blue value, but got '" + token + "'");
                                    datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
                                    continue;
                                }

                                color.Red = (float)r / 255;
                                color.Green = (float)g / 255;
                                color.Blue = (float)b / 255;

                                if (fadeType == "fade")
                                    mapInfo.FadeColor = color;
                                else
                                    mapInfo.OutsideFogColor = color;

                            } else {
                                GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + fadeType + " color value.");
                                datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
                            }
                        }
                        //vertwallshade or horizwallshade
                    } else if (token == "vertwallshade" || token == "horizwallshade") {
                        string shadeType = token;
                        SkipWhitespace(true);
                        token = StripTokenQuotes(ReadToken());

                        //new format
                        if (token == "=") {
                            SkipWhitespace(true);
                            token = StripTokenQuotes(ReadToken());
                        }

                        int val = 0;
                        if (!ReadSignedInt(token, ref val)) {
                            // Not numeric!
                            GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in '" + sourcename + "' at line " + GetCurrentLineNumber() + ": expected " + shadeType + " value, but got '" + token + "'");
                            datastream.Seek(-token.Length - 1, SeekOrigin.Current); //step back and try parsing this token again
                            continue;
                        }

                        if (shadeType == "vertwallshade")
                            mapInfo.VertWallShade = General.Clamp(val, -255, 255);
                        else
                            mapInfo.HorizWallShade = General.Clamp(val, -255, 255);
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
                        if (curBlockName == "map") {
                            return true; //we are done here
                        } else {
                            return parseBlock(token, mapName);
                        }
                    }
                }
            }
            return false;
        }

        private bool getColorByName(string name, ref Color4 color) {
            if (name == "black")
                return true;

            Color c = Color.FromName(name); //should be similar to C++ color name detection, I suppose
            if (c.IsKnownColor) {
                color = new Color4(c);
                return true;
            }
            return false;
        }
    }
}
