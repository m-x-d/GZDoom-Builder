using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using SlimDX;
using SlimDX.Direct3D9;

using CodeImp.DoomBuilder.GZBuilder.Data;

namespace CodeImp.DoomBuilder.GZBuilder.GZDoom {
    public sealed class ModeldefStructure {
        private const int MAX_MODELS = 3; //maximum models per modeldef entry, zero-based

        public ModeldefEntry Parse(ModeldefParser parser) {
            string[] textureNames = new string[4];
            string[] modelNames = new string[4];
            string path = "";
            Vector3 scale = new Vector3(1, 1, 1);
            float zOffset = 0;
            string token;
            bool gotErrors = false;

            //read modeldef structure contents
            while (parser.SkipWhitespace(true)) {
                token = parser.ReadToken();

                if (!string.IsNullOrEmpty(token)) {
                    token = parser.StripTokenQuotes(token).ToLowerInvariant(); //ANYTHING can be quoted...
                    //path
                    if (token == "path") {
                        parser.SkipWhitespace(true);
                        path = parser.StripTokenQuotes(parser.ReadToken()).Replace("/", "\\");

                        if (string.IsNullOrEmpty(path)) {
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected path to model, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        }
                        //model
                    } else if (token == "model") {
                        parser.SkipWhitespace(true);

                        //model index
                        int modelIndex;
                        token = parser.StripTokenQuotes(parser.ReadToken());
                        if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out modelIndex)) {
                            // Not numeric!
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected model index, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        }

                        if (modelIndex > MAX_MODELS) {
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": GZDoom doesn't allow more than " + MAX_MODELS + " per MODELDEF entry!");
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
                                GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": model '" + token + "' not parsed. Only MD3 and MD2 models are supported.");
                                gotErrors = true;
                                break;
                            }

                            //GZDoom allows models with identical modelIndex, it uses the last one encountered
                            modelNames[modelIndex] = token;
                        }
                        //skin
                    } else if (token == "skin") {
                        parser.SkipWhitespace(true);

                        //skin index
                        int skinIndex;
                        token = parser.StripTokenQuotes(parser.ReadToken());
                        if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out skinIndex)) {
                            // Not numeric!
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected skin index, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        }

                        if (skinIndex > MAX_MODELS) {
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": GZDoom doesn't allow more than " + MAX_MODELS + " per MODELDEF entry!");
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
                            if (Array.IndexOf(TextureData.SUPPORTED_TEXTURE_EXTENSIONS, fileExt) == -1)
                                token = TextureData.INVALID_TEXTURE;

                            //GZDoom allows skins with identical modelIndex, it uses the last one encountered
                            textureNames[skinIndex] = token;
                        }
                        //scale
                    } else if (token == "scale") {
                        parser.SkipWhitespace(true);

                        token = parser.StripTokenQuotes(parser.ReadToken());
                        if (!parser.ReadSignedFloat(token, ref scale.X)) {
                            // Not numeric!
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected scale X value, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        }

                        parser.SkipWhitespace(true);

                        token = parser.StripTokenQuotes(parser.ReadToken());
                        if (!parser.ReadSignedFloat(token, ref scale.Y)) {
                            // Not numeric!
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected scale Y value, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        }

                        parser.SkipWhitespace(true);

                        token = parser.StripTokenQuotes(parser.ReadToken());
                        if (!parser.ReadSignedFloat(token, ref scale.Z)) {
                            // Not numeric!
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected scale Z value, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        }
                        //zoffset
                    } else if (token == "zoffset") {
                        parser.SkipWhitespace(true);

                        token = parser.StripTokenQuotes(parser.ReadToken());
                        if (!parser.ReadSignedFloat(token, ref zOffset)) {
                            // Not numeric!
                            GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected ZOffset value, but got '" + token + "'");
                            gotErrors = true;
                            break;
                        }
                        //frameindex
                    } else if (token == "frameindex") {
                        //parsed all required fields. if got more than one model - find which one(s) should be displayed 
                        int len = modelNames.GetLength(0);
                        if (!gotErrors && len > 1) {
                            string spriteLump = null;
                            string spriteFrame = null;
                            bool[] modelsUsed = new bool[MAX_MODELS];

                            //step back
                            parser.DataStream.Seek(-token.Length - 1, SeekOrigin.Current);

                            //here we check which models are used in first encountered lump and frame
                            while (parser.SkipWhitespace(true)) {
                                token = parser.StripTokenQuotes(parser.ReadToken()).ToLowerInvariant();

                                if (token == "frameindex") {
                                    parser.SkipWhitespace(true);

                                    //should be sprite lump
                                    token = parser.StripTokenQuotes(parser.ReadToken()).ToLowerInvariant();

                                    if (string.IsNullOrEmpty(spriteLump)) {
                                        spriteLump = token;
                                    } else if (spriteLump != token) { //got another lump
                                        for (int i = 0; i < modelsUsed.Length; i++) {
                                            if (!modelsUsed[i]) {
                                                modelNames[i] = null;
                                                textureNames[i] = null;
                                            }
                                        }
                                        break;
                                    }

                                    parser.SkipWhitespace(true);

                                    //should be sprite frame
                                    token = parser.StripTokenQuotes(parser.ReadToken()).ToLowerInvariant();

                                    if (string.IsNullOrEmpty(spriteFrame)) {
                                        spriteFrame = token;
                                    } else if (spriteFrame != token) { //got another frame
                                        for (int i = 0; i < modelsUsed.Length; i++) {
                                            if (!modelsUsed[i]) {
                                                modelNames[i] = null;
                                                textureNames[i] = null;
                                            }
                                        }
                                        break;
                                    }

                                    parser.SkipWhitespace(true);

                                    //should be model index
                                    token = parser.StripTokenQuotes(parser.ReadToken());

                                    int modelIndex;
                                    if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out modelIndex)) {
                                        // Not numeric!
                                        GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected model index, but got '" + token + "'");
                                        gotErrors = true;
                                        break;
                                    }

                                    if (modelIndex > MAX_MODELS) {
                                        GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": GZDoom doesn't allow more than " + MAX_MODELS + " per MODELDEF entry!");
                                        gotErrors = true;
                                        break;
                                    }

                                    if (modelNames[modelIndex] == null) {
                                        GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": got model index, which doesn't correspond to any defined model!");
                                        gotErrors = true;
                                        break;
                                    }

                                    modelsUsed[modelIndex] = true;

                                    parser.SkipWhitespace(true);

                                    //should be frame index. Currently I have no use for it
                                    token = parser.StripTokenQuotes(parser.ReadToken());
                                    int frame;
                                    if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out frame)) {
                                        // Not numeric!
                                        GZBuilder.GZGeneral.LogAndTraceWarning("Error in " + parser.Source + " at line " + parser.GetCurrentLineNumber() + ": expected model frame, but got '" + token + "'");
                                        gotErrors = true;
                                        break;
                                    }

                                } else {
                                    //must be "}", step back
                                    parser.DataStream.Seek(-token.Length - 1, SeekOrigin.Current);
                                    break;
                                }
                            }
                        }
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

            for (int i = 0; i < textureNames.Length; i++) {
                if (textureNames[i] != null && modelNames[i] != null) {
                    mde.TextureNames.Add(textureNames[i]);
                    mde.ModelNames.Add(modelNames[i]);
                }
            }

            return mde;
        }
    }
}
