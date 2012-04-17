using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using CodeImp.DoomBuilder.GZBuilder.Data;

namespace CodeImp.DoomBuilder.GZBuilder.IO
{
    class ModelDefParser
    {
        public static void ParseFolder(Dictionary<string, ModelDefEntry> modelDefEntriesByName, string path) {
            string[] files = Directory.GetFiles(path);

            foreach (string fileName in files) {
                if (fileName.ToLower().IndexOf("modeldef") != -1)
                    Parse(modelDefEntriesByName, path, fileName);
            }

            //dbg
            #if DEBUG
                General.ErrorLogger.Add(ErrorType.Warning, "ModelDefParser: parsed " + modelDefEntriesByName.Count + " definitions;");
            #else
                General.WriteLogLine("ModelDefParser: parsed " + modelDefEntriesByName.Count + " definitions;");
            #endif

        }

        public static void Parse(Dictionary<string, ModelDefEntry> modelDefEntriesByName, string path, string fileName) {
            //dbg
            #if DEBUG
                General.ErrorLogger.Add(ErrorType.Warning, "ModelDefParser: Parsing '" + fileName + "'");
            #else
				General.WriteLogLine("ModelDefParser: Parsing '" + fileName + "'");
            #endif

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
                General.WriteLogLine("File '" + fileName + "' doesn't exist!");
            }
        }

        private static void parseModelDef(Dictionary<string, ModelDefEntry> modelDefEntriesByName, string path, string modelDef) {
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

                    switch (s) {
                        case "frameindex":
                            if (mde.Name != String.Empty && mde.Path != String.Empty && mde.ModelNames.Count > 0 && mde.TextureNames.Count > 0) {
                                modelDefEntriesByName[mde.Name] = mde;
                            } else {
                                General.ErrorLogger.Add(ErrorType.Warning, "Error while parsing ModelDef. Not all required fileds are present." + Environment.NewLine + "Parsed data: [" + Environment.NewLine + ModelDefEntry_ToString(mde) + "]" + Environment.NewLine);
                                return;
                            }
                            break;

                        case "model":
                            string fileName = parts[2].Trim(splitter);
                            //string fileExt = fileName.Substring(fileName.Length - 4);
                            string fileExt = Path.GetExtension(fileName);
                            
                            if(fileExt == ".md3" || fileExt == ".md2"){
                                mde.ModelNames.Add(fileName);
                            }else{
                                General.ErrorLogger.Add(ErrorType.Warning, "Model '" + fileName + "' not parsed. Only MD3 and MD2 models are supported.");
                                return;
                            }
                            break;

                        case "path":
                            mde.Path = path + "\\" + parts[1].Trim(splitter).Replace("/", "\\");
                            break;

                        case "skin":
                            mde.TextureNames.Add(parts[2].Trim(splitter));
                            break;

                        case "scale":
                            mde.Scale.X = float.Parse(parts[1], NumberStyles.Float);
                            mde.Scale.Y = float.Parse(parts[2], NumberStyles.Float);
                            mde.Scale.Z = float.Parse(parts[3], NumberStyles.Float);
                            break;

                        case "zoffset":
                            mde.zOffset = float.Parse(parts[1], NumberStyles.Float);
                            break;
                    }
                }
            }
        }

        private static string ModelDefEntry_ToString(ModelDefEntry mde) {
            string[] models = new string[mde.ModelNames.Count];
            mde.ModelNames.CopyTo(models);

            string[] textures = new string[mde.TextureNames.Count];
            mde.TextureNames.CopyTo(textures);

            string s = "Name: " + mde.Name + Environment.NewLine
                       + "Path: " + mde.Path + Environment.NewLine
                       + "Models: " + String.Join(", ", models) + Environment.NewLine
                       + "Textures: " + String.Join(", ", textures) + Environment.NewLine
                       + "Scale: " + mde.Scale + Environment.NewLine
                       + "zOffset: " + mde.zOffset + Environment.NewLine;
            return s;
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
    }
}