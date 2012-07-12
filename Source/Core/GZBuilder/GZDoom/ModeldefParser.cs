using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.GZBuilder.GZDoom;

namespace CodeImp.DoomBuilder.GZBuilder.GZDoom {
   
    internal class ModeldefParser : ZDTextParser {
        private Dictionary<string, ModeldefEntry> modelDefEntries; //classname, entry
        internal Dictionary<string, ModeldefEntry> ModelDefEntries { get { return modelDefEntries; } }

        private List<string> classNames;

        internal string Source { get { return sourcename; } }

        internal ModeldefParser() {
            modelDefEntries = new Dictionary<string, ModeldefEntry>();
            classNames = new List<string>();
        }

        //should be called after all decorate actors are parsed 
        public override bool Parse(Stream stream, string sourcefilename) {
            base.Parse(stream, sourcefilename);
            modelDefEntries = new Dictionary<string, ModeldefEntry>();

            // Continue until at the end of the stream
            while (SkipWhitespace(true)) {
                string token = ReadToken();

                if (token != null) {
                    token = StripTokenQuotes(token).ToLowerInvariant();

                    if (token == "model") { //model structure start
                        //find classname
                        SkipWhitespace(true);
                        string className = StripTokenQuotes(ReadToken()).ToLowerInvariant();

                        if (!string.IsNullOrEmpty(className)) {
                            if (classNames.IndexOf(className) != -1)
                                continue; //already got this class; continue to next one

                            //now find opening brace
                            SkipWhitespace(true);
                            token = ReadToken();
                            if (token != "{") {
                                GZBuilder.GZGeneral.LogAndTraceWarning("Unexpected token found in "+sourcefilename+" at line "+GetCurrentLineNumber()+": expected '{', but got " + token);
                                continue; //something wrong with modeldef declaration, continue to next one
                            }

                            ModeldefStructure mds = new ModeldefStructure();
                            ModeldefEntry mde = mds.Parse(this);
                            if (mde != null) {
                                mde.ClassName = className;
                                modelDefEntries.Add(className, mde);
                                classNames.Add(mde.ClassName);
                            }
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

            if (modelDefEntries.Count > 0)
                return true;
            return false;
        }
    }
}
