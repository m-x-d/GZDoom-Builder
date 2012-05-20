using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using CodeImp.DoomBuilder.GZBuilder.Data;

namespace CodeImp.DoomBuilder.GZBuilder.IO
{
    class ModelDefParser
    {
        public static string INVALID_TEXTURE = "**INVALID_TEXTURE**";
        private static string[] SUPPORTED_TEXTURE_EXTENSIONS = { ".jpg", ".tga", ".png", ".dds", ".bmp" };
        
        public static void ParseFolder(Dictionary<string, ModelDefEntry> modelDefEntriesByName, string path) {
            string[] files = Directory.GetFiles(path);

            foreach (string fileName in files) {
                if (fileName.ToLower().IndexOf("modeldef") != -1)
                    Parse(modelDefEntriesByName, path, fileName);
            }

            logAndTrace("ModelDefParser: parsed " + modelDefEntriesByName.Count + " definitions;");
        }

        public static void Parse(Dictionary<string, ModelDefEntry> modelDefEntriesByName, string path, string fileName) {
            logAndTrace("ModelDefParser: Parsing '" + fileName + "'");

            if (File.Exists(fileName)) {
                StreamReader s = File.OpenText(fileName);
                string contents = s.ReadToEnd();
                s.Close();

                contents = StripComments(contents).ToLower();

                int startIndex = 0;
                int mdlIndex = 0;

                while ((mdlIndex = contents.IndexOf("model", startIndex)) != -1) {
                    startIndex = contents.IndexOf("}", mdlIndex);
                    parseModelDef(modelDefEntriesByName, path, contents.Substring(mdlIndex, startIndex - mdlIndex));
                }

            } else {
                logAndTrace("ModelDefParser: File '" + fileName + "' doesn't exist!");
            }
        }

        private static void parseModelDef(Dictionary<string, ModelDefEntry> modelDefEntriesByName, string path, string modelDef) {
            string[] modelNames = new string[16];
            string[] textureNames = new string[16];
            
            string[] lines = modelDef.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            char[] space = new char[] { ' ' };
            string[] parts = lines[0].Split(space, StringSplitOptions.RemoveEmptyEntries);
            string name = parts[1].Trim().Replace("{", "");

            if (modelDefEntriesByName.ContainsKey(name)) {
                General.WriteLogLine("Already have ModelDef for '" + name + "'");
                return;
            }
            char[] splitter = new char[] { ' ', '"' };
            ModelDefEntry mde = new ModelDefEntry();
            mde.Name = name;

            for (int i = 1; i < lines.Length; i++) {
                parts = lines[i].Split(space, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 0) {
                    string s = parts[0].Trim();

                    //LOTS of boilerplate! Yay!!!
                    if (s.IndexOf("frameindex") != -1) {
                        if (mde.Name != String.Empty && mde.Path != String.Empty) {
                            for (int c = 0; c < modelNames.Length; c++) {
                                if (modelNames[c] != null && textureNames[c] != null) {
                                    mde.ModelNames.Add(modelNames[c]);
                                    mde.TextureNames.Add(textureNames[c]);
                                }
                            }

                            if (mde.ModelNames.Count > 0 && mde.TextureNames.Count > 0)
                                modelDefEntriesByName[mde.Name] = mde;
                            else
                                logAndTrace("Error while parsing ModelDef. Not all required fileds are present." + Environment.NewLine + "Parsed data: [" + Environment.NewLine + ModelDefEntry_ToString(mde) + "]" + Environment.NewLine);
                            return; //we don't want to parse all frames
  
                        } 
                        logAndTrace("Error while parsing ModelDef. Not all required fileds are present." + Environment.NewLine + "Parsed data: [" + Environment.NewLine + ModelDefEntry_ToString(mde) + "]" + Environment.NewLine);
                        return;

                    } else if (s.IndexOf("model") != -1) {
                        if (parts.Length != 3) {
                            logAndTrace("Incorrect syntax in 'model' token for class '" + name + "': expected 3 entries, but got " + parts.Length + "!");
                            return;
                        }

                        int fileIndex = int.Parse(parts[1].Trim(splitter), NumberStyles.Integer);
                        string fileName = parts[2].Trim(splitter);
                        string fileExt = Path.GetExtension(fileName);

                        if (fileExt == ".md3" || fileExt == ".md2") {
                            if (modelNames[fileIndex] != null) {
                                logAndTrace("Incorrect syntax in 'model' token for class '" + name + "': already got model with index " + fileIndex + "!");
                                return;
                            }
                            modelNames[fileIndex] = fileName;

                        } else {
                            logAndTrace("Model '" + fileName + "' not parsed. Only MD3 and MD2 models are supported.");
                            return;
                        }

                    } else if (s.IndexOf("path") != -1) {
                        if (parts.Length != 2) {
                            logAndTrace("Incorrect syntax in 'path' token for class '" + name + "': expected 2 entries, but got " + parts.Length + "!");
                            return;
                        }

                        mde.Path = path + "\\" + parts[1].Trim(splitter).Replace("/", "\\");

                    } else if (s.IndexOf("skin") != -1) {
                        if (parts.Length != 3) {
                            logAndTrace("Incorrect syntax in 'skin' token for class '" + name + "': expected 3 entries, but got " + parts.Length + "!");
                            return;
                        }

                        int index = int.Parse(parts[1].Trim(splitter), NumberStyles.Integer);
                        if (textureNames[index] != null) {
                            logAndTrace("Incorrect syntax in 'skin' token for class '" + name + "': already got skin with index " + index + "!");
                            return;
                        }

                        string textureName = parts[2].Trim(splitter);
                        string fileExt = Path.GetExtension(textureName);
                        textureNames[index] = Array.IndexOf(SUPPORTED_TEXTURE_EXTENSIONS, fileExt) == -1 ? INVALID_TEXTURE : textureName;

                    } else if (s.IndexOf("scale") != -1) {
                        if (parts.Length != 4) {
                            logAndTrace("Incorrect syntax in 'scale' token for class '" + name + "': expected 4 entries, but got " + parts.Length + "!");
                            return;
                        }

                        mde.Scale.X = float.Parse(parts[1], NumberStyles.Float);
                        mde.Scale.Y = float.Parse(parts[2], NumberStyles.Float);
                        mde.Scale.Z = float.Parse(parts[3], NumberStyles.Float);

                    } else if (s.IndexOf("zoffset") != -1) {
                        if (parts.Length != 2) {
                            logAndTrace("Incorrect syntax in 'zoffset' token for class '" + name + "': expected 2 entries, but got " + parts.Length + "!");
                            return;
                        }

                        mde.zOffset = float.Parse(parts[1], NumberStyles.Float);
                    }
                }
            }
        }

        private static string ModelDefEntry_ToString(ModelDefEntry mde) {
            string[] models = new string[mde.ModelNames.Count];
            mde.ModelNames.CopyTo(models);

            string[] textures = new string[mde.TextureNames.Count];
            mde.TextureNames.CopyTo(textures);

            return "Name: " + mde.Name + Environment.NewLine
                   + "Path: " + mde.Path + Environment.NewLine
                   + "Models: " + String.Join(", ", models) + Environment.NewLine
                   + "Skins: " + String.Join(", ", textures) + Environment.NewLine
                   + "Scale: " + mde.Scale + Environment.NewLine
                   + "zOffset: " + mde.zOffset + Environment.NewLine;
        }

        private static string StripComments(string contents) {
            int start, end;

            //comments
            while ((start = contents.IndexOf("//")) != -1) {
                end = contents.IndexOf(Environment.NewLine, start);
                contents = contents.Remove(start, end - start);
            }

            //block comments
            while ((start = contents.IndexOf("/*")) != -1) {
                end = contents.IndexOf("*/");
                contents = contents.Remove(start, end - start + 2);
            }

            return contents;
        }

        private static void logAndTrace(string message) {
            General.ErrorLogger.Add(ErrorType.Warning, message);
            General.WriteLogLine(message);
        }
    }
}