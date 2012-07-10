using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.GZBuilder.Data;

namespace CodeImp.DoomBuilder.GZBuilder.ZDoom
{
    public class AcsParser : ZDTextParser
    {
        private List<ScriptItem> namedScripts;
        private List<ScriptItem> numberedScripts;

        public List<ScriptItem> NamedScripts { get { return namedScripts; } }
        public List<ScriptItem> NumberedScripts { get { return numberedScripts; } }

        public AcsParser() {
            namedScripts = new List<ScriptItem>();
            numberedScripts = new List<ScriptItem>();
        }

        public override bool Parse(Stream stream, string sourcefilename) {
            base.Parse(stream, sourcefilename);

            // Continue until at the end of the stream
            while (SkipWhitespace(true)) {
                string token = ReadToken();

                if (!string.IsNullOrEmpty(token)) {
                    token = token.ToLowerInvariant();

                    if (token == "script") {
                        int startPos = (int)stream.Position - 7;
                        SkipWhitespace(true);
                        token = ReadToken();

                        //is it named script?
                        if (token.IndexOf('"') != -1) {
                            token = StripTokenQuotes(token);
                            ScriptItem i = new ScriptItem(0, token, startPos, (int)stream.Position-1);
                            namedScripts.Add(i);
                        } else { //should be numbered script
                            int n = 0;
                            if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out n)) {
                                int endPos = (int)stream.Position - 1;

                                //now find opening brace
                                do {
                                    SkipWhitespace(true);
                                    token = ReadToken();
                                } while (token != "{");

                                token = ReadLine();
                                string name = "";

                                if (token.Length > 0) {
                                    int commentStart = token.IndexOf("//");
                                    if (commentStart != -1) { //found comment
                                        commentStart += 2;
                                        name = token.Substring(commentStart, token.Length - commentStart);
                                    }
                                }

                                name = (name != "" ? "[" + n + "] " + name : "Script " + n);
                                ScriptItem i = new ScriptItem(n, name, startPos, endPos);
                                numberedScripts.Add(i);
                            }
                        }

                    }
                }
            }
            return true;
        }
    }
}
