using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using CodeImp.DoomBuilder.ZDoom;

using SlimDX;
using SlimDX.Direct3D9;

namespace CodeImp.DoomBuilder.GZBuilder.GZDoom {
    public class GldefsParser : ZDTextParser {
        private Dictionary<string, GldefsLight> gldefsEntries;
        public Dictionary<string, GldefsLight> GldefsEntries { get { return gldefsEntries; } }
        public string Source { get { return sourcename; } }

        public override bool Parse(Stream stream, string sourcefilename) {
            base.Parse(stream, sourcefilename);

            gldefsEntries = new Dictionary<string, GldefsLight>();
            Dictionary<string, GldefsLight> lightsByName = new Dictionary<string, GldefsLight>();
            List<string> objects = new List<string>();

            // Continue until at the end of the stream
            while (SkipWhitespace(true)) {
                string token = ReadToken();
                if (!string.IsNullOrEmpty(token)) {
                    token = token.ToLowerInvariant();

                    //got light structure
                    if (token == GldefsLightType.POINT || token == GldefsLightType.PULSE || token == GldefsLightType.FLICKER || token == GldefsLightType.FLICKER2 || token == GldefsLightType.SECTOR) {
                        bool gotErrors = false;
                        string lightType = token; //todo: set correct type
                        GldefsLight light = new GldefsLight();

                        //find classname
                        SkipWhitespace(true);
                        string lightName = ReadToken().ToLowerInvariant();

                        if (!string.IsNullOrEmpty(lightName)) {
                            if (lightsByName.ContainsKey(lightName)) {
                                GZBuilder.GZGeneral.LogAndTraceWarning("Already got light '" + lightName + "'; entry skipped");
                                continue; //already got this light; continue to next one
                            }

                            //now find opening brace
                            SkipWhitespace(true);
                            token = ReadToken();
                            if (token != "{") {
                                //Form1.Trace("Unexpected token found in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected '{', but got " + token);
                                GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected '{', but got " + token);
                                continue; //something wrong with modeldef declaration, continue to next one
                            }

                            //read gldefs light structure
                            while (SkipWhitespace(true)) {
                                token = ReadToken();

                                if (!string.IsNullOrEmpty(token)) {
                                    token = token.ToLowerInvariant();
                                    //color
                                    if (token == "color") {
                                        SkipWhitespace(true);

                                        token = ReadToken();
                                        if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Color.Red)) {
                                            // Not numeric!
                                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Red Color value, but got '" + token + "'");
                                            gotErrors = true;
                                            break;
                                        }

                                        SkipWhitespace(true);

                                        token = ReadToken();
                                        if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Color.Green)) {
                                            // Not numeric!
                                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Green Color value, but got '" + token + "'");
                                            gotErrors = true;
                                            break;
                                        }

                                        SkipWhitespace(true);

                                        token = ReadToken();
                                        if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Color.Blue)) {
                                            // Not numeric!
                                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Blue Color value, but got '" + token + "'");
                                            gotErrors = true;
                                            break;
                                        }
                                        //size
                                    } else if (token == "size") {
                                        SkipWhitespace(true);

                                        token = ReadToken();
                                        if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out light.Size)) {
                                            // Not numeric!
                                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Size value, but got '" + token + "'");
                                            gotErrors = true;
                                            break;
                                        }
                                        //offset
                                    } else if (token == "offset") {
                                        SkipWhitespace(true);

                                        token = ReadToken();
                                        if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Offset.X)) {
                                            // Not numeric!
                                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Offset X value, but got '" + token + "'");
                                            gotErrors = true;
                                            break;
                                        }

                                        SkipWhitespace(true);

                                        token = ReadToken();
                                        if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Offset.Y)) {
                                            // Not numeric!
                                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Offset Y value, but got '" + token + "'");
                                            gotErrors = true;
                                            break;
                                        }

                                        SkipWhitespace(true);

                                        token = ReadToken();
                                        if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Offset.Z)) {
                                            // Not numeric!
                                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Offset Z value, but got '" + token + "'");
                                            gotErrors = true;
                                            break;
                                        }
                                        //subtractive
                                    } else if (token == "subtractive") {
                                        SkipWhitespace(true);

                                        token = ReadToken();
                                        int i;
                                        if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out i)) {
                                            // Not numeric!
                                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Subtractive value, but got '" + token + "'");
                                            gotErrors = true;
                                            break;
                                        }

                                        light.Subtractive = i == 1;
                                        //dontlightself
                                    } else if (token == "dontlightself") {
                                        SkipWhitespace(true);

                                        token = ReadToken();
                                        int i;
                                        if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out i)) {
                                            // Not numeric!
                                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Dontlightself value, but got '" + token + "'");
                                            gotErrors = true;
                                            break;
                                        }

                                        light.DontLightSelf = i == 1;
                                        //interval
                                    } else if (token == "interval" && (lightType == GldefsLightType.PULSE || lightType == GldefsLightType.FLICKER2)) {
                                        SkipWhitespace(true);

                                        token = ReadToken();
                                        if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Interval)) {
                                            // Not numeric!
                                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Interval value, but got '" + token + "'");
                                            gotErrors = true;
                                            break;
                                        }
                                        //todo: modify Interval based on light type

                                        //secondarysize
                                    } else if (token == "secondarysize" && (lightType == GldefsLightType.PULSE || lightType == GldefsLightType.FLICKER || lightType == GldefsLightType.FLICKER2)) {
                                        SkipWhitespace(true);

                                        token = ReadToken();
                                        if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out light.SecondarySize)) {
                                            // Not numeric!
                                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected SecondarySize value, but got '" + token + "'");
                                            gotErrors = true;
                                            break;
                                        }
                                        //chance
                                    } else if (token == "chance" && lightType == GldefsLightType.FLICKER) {
                                        SkipWhitespace(true);

                                        token = ReadToken();
                                        if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Chance)) {
                                            // Not numeric!
                                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Chance value, but got '" + token + "'");
                                            gotErrors = true;
                                            break;
                                        }
                                        //scale
                                    } else if (token == "scale" && lightType == GldefsLightType.SECTOR) {
                                        SkipWhitespace(true);

                                        token = ReadToken();
                                        if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out light.Scale)) {
                                            // Not numeric!
                                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected Scale value, but got '" + token + "'");
                                            gotErrors = true;
                                            break;
                                        }

                                        if (light.Scale < 0 || light.Scale > 1) {
                                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": scale must be in 0.0 - 1.0 range, but is " + light.Scale);
                                            gotErrors = true;
                                            break;
                                        }
                                        //end of structure
                                    } else if (token == "}") {
                                        if (!gotErrors) {
                                            //check light
                                            bool valid = true;
                                            if (light.Color.Red == 0.0f && light.Color.Green == 0.0f && light.Color.Blue == 0.0f) {
                                                GZBuilder.GZGeneral.LogAndTraceWarning("'" + lightName + "' light Color is 0,0,0. It won't be shown in GZDoom!");
                                                valid = false;
                                            }
                                            if (light.Size == 0) {
                                                GZBuilder.GZGeneral.LogAndTraceWarning("'" + lightName + "' light Size is 0. It won't be shown in GZDoom!");
                                                valid = false;
                                            }

                                            if (valid) lightsByName.Add(lightName, light);
                                        }
                                        break; //break out of this parsing loop
                                    }
                                }
                            }
                        }

                    } else if (token == "object") {
                        SkipWhitespace(true);

                        //read object class
                        string objectClass = ReadToken().ToLowerInvariant();

                        if (!string.IsNullOrEmpty(objectClass)) {
                            if (objects.Contains(objectClass)) {
                                GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": already got object '" + objectClass + "'; entry skipped");
                                continue; //already got this object; continue to next one
                            }

                            objects.Add(objectClass);

                            //now find opening brace
                            SkipWhitespace(true);
                            token = ReadToken();
                            if (token != "{") {
                                GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": expected '{', but got " + token);
                                continue;
                            }

                            //read frames structure
                            while (SkipWhitespace(true)) {
                                token = ReadToken();

                                if (!string.IsNullOrEmpty(token)) {
                                    token = token.ToLowerInvariant();

                                    if (token == "light") { //just use first light from first frame and be done with it
                                        SkipWhitespace(true);
                                        token = ReadToken().ToLowerInvariant(); //should be light name

                                        if (!string.IsNullOrEmpty(token)) {
                                            if (lightsByName.ContainsKey(token)) {
                                                gldefsEntries.Add(objectClass, lightsByName[token]);
                                            } else {
                                                GZBuilder.GZGeneral.LogAndTraceWarning("Light declaration not found for light '" + token + "' in '" + sourcefilename+"'");
                                            }
                                            break;
                                        }
                                    }

                                }
                            }

                        }

                    } else if (token == "#include") {
                        SkipWhitespace(true);
                        string includeLump = StripTokenQuotes(ReadToken()).ToLowerInvariant();

                        if (!string.IsNullOrEmpty(includeLump) && includeLump.IndexOf(".gl") != -1) {
                            //todo: load included file. check for recursive includes?
                        } else {
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in '" + sourcefilename + "' at line " + GetCurrentLineNumber() + ": got #include directive with missing or incorrect include path: '" + includeLump + "'");
                        }

                    } else {
                        // Unknown structure!
                        string token2;
                        do {
                            if (!SkipWhitespace(true)) break;
                            token2 = ReadToken();
                            if (token2 == null) break;
                        }
                        while (token2 != "{");
                        int scopelevel = 1;
                        do {
                            if (!SkipWhitespace(true)) break;
                            token2 = ReadToken();
                            if (token2 == null) break;
                            if (token2 == "{") scopelevel++;
                            if (token2 == "}") scopelevel--;
                        }
                        while (scopelevel > 0);
                    }
                }
            }

            if (gldefsEntries.Count > 0)
                return true;
            return false;
        }
    }

    public class GldefsLight {
        public int Type;
        public Color3 Color;
        public int Size;
        public int SecondarySize;
        public float Interval;
        public Vector3 Offset;
        public float Chance;
        public float Scale;
        public bool Subtractive;
        public bool DontLightSelf;

        public GldefsLight() {
            Color = new Color3();
            Offset = new Vector3();
        }
    }

    public struct GldefsLightType {
        public const string POINT = "pointlight";
        public const string PULSE = "pulselight";
        public const string FLICKER = "flickerlight";
        public const string FLICKER2 = "flickerlight2";
        public const string SECTOR = "sectorlight";
    }
}
