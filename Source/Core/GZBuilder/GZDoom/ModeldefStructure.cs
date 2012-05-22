using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using SlimDX;
using SlimDX.Direct3D9;

using CodeImp.DoomBuilder.GZBuilder.Data;

namespace CodeImp.DoomBuilder.GZBuilder.GZDoom {
    public sealed class ModeldefStructure {
        private string[] supportedTextureExtensions = { ".jpg", ".tga", ".png", ".dds" };

        public ModeldefEntry Parse(ModeldefParser parser) {
            string[] textureNames = new string[16];
            string[] modelNames = new string[16];
            string path = "";
            Vector3 scale = new Vector3(1, 1, 1);
            float zOffset = 0;
            string token;
            bool gotErrors = false;

            //read modeldef structure contents
            while (parser.SkipWhitespace(true)) {
                token = parser.ReadToken();

                if (!string.IsNullOrEmpty(token)) {
                    token = token.ToLowerInvariant();
//path
                    if (token == "path") {
                        parser.SkipWhitespace(true);
                        path = parser.StripTokenQuotes(parser.ReadToken()).Replace("/", "\\");

                        if (string.IsNullOrEmpty(path)) {
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line "+parser.GetCurrentLineNumber()+": expected path to model, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        }
//model
                    } else if (token == "model") {
                        parser.SkipWhitespace(true);

                        //model index
                        int modelIndex;
                        token = parser.ReadToken();
                        if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out modelIndex)) {
                            // Not numeric!
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected model index, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        }

                        //model path
                        token = parser.StripTokenQuotes(parser.ReadToken()).ToLowerInvariant();
                        if (string.IsNullOrEmpty(token)) {
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected model name, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        } else {
                            //check extension
                            int dotPos = token.LastIndexOf(".");
                            string fileExt = token.Substring(token.LastIndexOf("."), token.Length - dotPos);
                            if (fileExt != ".md3" && fileExt != ".md2") {
                                GZBuilder.GZGeneral.LogAndTraceWarning("Model '" + token + "' not parsed in " + parser.Source + " at line " + parser.GetCurrentLineNumber() +". Only MD3 and MD2 models are supported.");
                                gotErrors = true;
                                break;
                            }

                            if (modelNames[modelIndex] != null) {
                                GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": already got model for index " + modelIndex);
                                gotErrors = true;
                                break;
                            } else {
                                modelNames[modelIndex] = token;
                            }
                        }
//skin
                    } else if (token == "skin") {
                        parser.SkipWhitespace(true);

                        //skin index
                        int skinIndex;
                        token = parser.ReadToken();
                        if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out skinIndex)) {
                            // Not numeric!
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected skin index, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        }

                        //skin path
                        token = parser.StripTokenQuotes(parser.ReadToken()).ToLowerInvariant();
                        if (string.IsNullOrEmpty(token)) {
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected skin name, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        } else {
                            //check extension
                            int dotPos = token.LastIndexOf(".");
                            string fileExt = token.Substring(token.LastIndexOf("."), token.Length - dotPos);
                            if(Array.IndexOf(supportedTextureExtensions, fileExt) == -1)
                                token = ModeldefParser.INVALID_TEXTURE;

                            if (textureNames[skinIndex] != null) {
                                GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": already got model for index " + skinIndex);
                                gotErrors = true;
                                break;
                            } else {
                                textureNames[skinIndex] = token;
                            }
                        }
//scale
                    } else if (token == "scale") {
                        parser.SkipWhitespace(true);

                        token = parser.ReadToken();

                        int sign = 1;
                        if (token == "-") {
                            sign = -1;
                            token = parser.ReadToken();
                        }

                        if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out scale.X)) {
                            // Not numeric!
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected scale X value, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        }
                        scale.X *= sign;

                        parser.SkipWhitespace(true);

                        token = parser.ReadToken();

                        sign = 1;
                        if (token == "-") {
                            sign = -1;
                            token = parser.ReadToken();
                        }

                        if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out scale.Y)) {
                            // Not numeric!
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected scale Y value, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        }
                        scale.Y *= sign;


                        parser.SkipWhitespace(true);

                        token = parser.ReadToken();

                        sign = 1;
                        if (token == "-") {
                            sign = -1;
                            token = parser.ReadToken();
                        }

                        if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out scale.Z)) {
                            // Not numeric!
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected scale Z value, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        }
                        scale.Z *= sign;
//zoffset
                    } else if (token == "zoffset") {
                        parser.SkipWhitespace(true);

                        token = parser.ReadToken();

                        int sign = 1;
                        if (token == "-") {
                            sign = -1;
                            token = parser.ReadToken();
                        }

                        if (!float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out zOffset)) {
                            // Not numeric!
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected ZOffset value, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        }
                        zOffset *= sign;
//frameindex
                    } else if (token == "frameindex") {
                        //parsed all required fields
                        break;
                    }
                }
            }

            //find closing brace, then quit;
            while (parser.SkipWhitespace(true)) {
                token = parser.ReadToken();
                if (token == "}")
                    break;
            }

            if (gotErrors)
                return null;    

            //classname is set in ModeldefParser
            ModeldefEntry mde = new ModeldefEntry();
            mde.Path = path;
            mde.Scale = scale;
            mde.zOffset = zOffset;

            for (int i = 0; i < textureNames.Length; i++ ) {
                if (textureNames[i] != null && modelNames[i] != null) {
                    mde.TextureNames.Add(textureNames[i]);
                    mde.ModelNames.Add(modelNames[i]);
                }
            }

            return mde;
        }
    }
}
