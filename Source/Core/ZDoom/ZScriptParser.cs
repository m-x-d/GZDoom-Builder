using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeImp.DoomBuilder.ZDoom
{
    public sealed class ZScriptActorStructure : ActorStructure
    {
        private bool ParseDefaultBlock(ZScriptParser parser, Stream stream, ZScriptTokenizer tokenizer)
        {
            tokenizer.SkipWhitespace();
            ZScriptToken token = tokenizer.ExpectToken(ZScriptTokenType.OpenCurly);
            if (token == null || !token.IsValid)
            {
                parser.ReportError("Expected {, got " + ((Object)token ?? "<null>").ToString());
                return false;
            }

            // todo parse defaults block
            stream.Position--;
            List<ZScriptToken> tokens = parser.ParseBlock(false); // do nothing for now

            return (tokens != null);
        }

        private bool ParseStatesBlock(ZScriptParser parser, Stream stream, ZScriptTokenizer tokenizer)
        {
            tokenizer.SkipWhitespace();
            ZScriptToken token = tokenizer.ExpectToken(ZScriptTokenType.OpenParen, ZScriptTokenType.OpenCurly);
            if (token == null || !token.IsValid)
            {
                parser.ReportError("Expected ( or {, got " + ((Object)token ?? "<null>").ToString());
                return false;
            }
            
            // we can have some weirdass class name list after States keyword. handle that here.
            if (token.Type == ZScriptTokenType.OpenParen)
            {
                parser.ParseExpression(true);
                tokenizer.SkipWhitespace();
                token = tokenizer.ExpectToken(ZScriptTokenType.CloseParen);
                if (token == null || !token.IsValid)
                {
                    parser.ReportError("Expected ), got " + ((Object)token ?? "<null>").ToString());
                    return false;
                }

                tokenizer.SkipWhitespace();
                token = tokenizer.ExpectToken(ZScriptTokenType.OpenCurly);
                if (token == null || !token.IsValid)
                {
                    parser.ReportError("Expected {, got " + ((Object)token ?? "<null>").ToString());
                    return false;
                }
            }

            // todo parse states block
            stream.Position--;
            List<ZScriptToken> tokens = parser.ParseBlock(false); // do nothing for now

            return (tokens != null);
        }

        private string ParseTypeName(ZScriptParser parser, Stream stream, ZScriptTokenizer tokenizer)
        {
            ZScriptToken token = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
            if (token == null || !token.IsValid)
            {
                parser.ReportError("Expected type name, got " + ((Object)token ?? "<null>").ToString());
                return null;
            }

            string outs = token.Value;

            long cpos = stream.Position;
            tokenizer.SkipWhitespace();
            token = tokenizer.ReadToken();
            if (token != null && token.Type == ZScriptTokenType.OpLessThan) // <
            {
                string internal_type = ParseTypeName(parser, stream, tokenizer);
                if (internal_type == null)
                    return null;
                token = tokenizer.ExpectToken(ZScriptTokenType.OpGreaterThan);
                if (token == null || !token.IsValid)
                {
                    parser.ReportError("Expected >, got " + ((Object)token ?? "<null>").ToString());
                    return null;
                }
                return outs + "<" + internal_type + ">";
            }
            else
            {
                stream.Position = cpos;
                return outs;
            }
        }

        internal ZScriptActorStructure(ZDTextParser zdparser, DecorateCategoryInfo catinfo, string _classname, string _replacesname, string _parentname) : base(zdparser, catinfo)
        {
            ZScriptParser parser = (ZScriptParser)zdparser;
            Stream stream = parser.DataStream;
            ZScriptTokenizer tokenizer = new ZScriptTokenizer(parser.DataReader);
            parser.tokenizer = tokenizer;

            classname = _classname;
            replaceclass = _replacesname;

            ZScriptToken cls_open = tokenizer.ExpectToken(ZScriptTokenType.OpenCurly);
            if (cls_open == null || !cls_open.IsValid)
            {
                parser.ReportError("Expected {, got " + ((Object)cls_open ?? "<null>").ToString());
                return;
            }

            // in the class definition, we can have the following:
            // - Defaults block
            // - States block
            // - method signature: [native] [action] <type [, type [...]]> <name> (<arguments>);
            // - method: <method signature (except native)> <block>
            // - field declaration: [native] <type> <name>;
            // - enum definition: enum <name> <block>;
            // we are skipping everything, except Defaults and States.
            while (true)
            {
                tokenizer.SkipWhitespace();
                long ocpos = stream.Position;
                ZScriptToken token = tokenizer.ExpectToken(ZScriptTokenType.Identifier, ZScriptTokenType.CloseCurly);
                if (token == null || !token.IsValid)
                {
                    parser.ReportError("Expected identifier, got " + ((Object)cls_open ?? "<null>").ToString());
                    return;
                }
                if (token.Type == ZScriptTokenType.CloseCurly) // end of class
                    break;

                string b_lower = token.Value.ToLowerInvariant();
                switch (b_lower)
                {
                    case "default":
                        if (!ParseDefaultBlock(parser, stream, tokenizer))
                            return;
                        continue;

                    case "states":
                        if (!ParseStatesBlock(parser, stream, tokenizer))
                            return;
                        continue;

                    case "enum":
                        if (!parser.ParseEnum())
                            return;
                        continue;

                    case "const":
                        if (!parser.ParseConst())
                            return;
                        continue;

                        // apparently we can have a struct inside a class, but not another class.
                    case "struct":
                        if (!parser.ParseClassOrStruct(true, false))
                            return;
                        continue;

                    default:
                        stream.Position = ocpos;
                        break;
                }

                // try to read in a variable/method.
                bool bmethod = false;
                string[] availablemodifiers = new string[] { "static", "native", "action", "readonly", "protected", "private", "virtual", "override", "meta", "deprecated", "final" };
                string[] methodmodifiers = new string[] { "action", "virtual", "override", "final" };
                HashSet<string> modifiers = new HashSet<string>();
                List<string> types = new List<string>();
                List<string> names = new List<string>();
                List<int> arraylens = new List<int>();
                List<ZScriptToken> args = null; // this is for the future
                List<ZScriptToken> body = null;

                while (true)
                {
                    tokenizer.SkipWhitespace();
                    long cpos = stream.Position;
                    token = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
                    if (token == null || !token.IsValid)
                    {
                        parser.ReportError("Expected modifier or name, got " + ((Object)cls_open ?? "<null>").ToString());
                        return;
                    }

                    b_lower = token.Value.ToLowerInvariant();
                    if (availablemodifiers.Contains(b_lower))
                    {
                        if (modifiers.Contains(b_lower))
                        {
                            parser.ReportError("Field/method modifier '" + b_lower + "' was specified twice");
                            return;
                        }

                        if (methodmodifiers.Contains(b_lower))
                            bmethod = true;

                        modifiers.Add(b_lower);
                    }
                    else
                    {
                        stream.Position = cpos;
                        break;
                    }
                }

                // read in the type name(s)
                // type name can be:
                //  - identifier
                //  - identifier<identifier>
                while (true)
                {
                    tokenizer.SkipWhitespace();
                    string typename = ParseTypeName(parser, stream, tokenizer);
                    if (typename == null)
                        return;
                    types.Add(typename.ToLowerInvariant());
                    long cpos = stream.Position;
                    tokenizer.SkipWhitespace();
                    token = tokenizer.ReadToken();
                    if (token == null || token.Type != ZScriptTokenType.Comma)
                    {
                        stream.Position = cpos;
                        break;
                    }
                }

                while (true)
                {
                    string name = null;
                    int arraylen = 0;

                    // read in the method/field name
                    tokenizer.SkipWhitespace();
                    token = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
                    if (token == null || !token.IsValid)
                    {
                        parser.ReportError("Expected field/method name, got " + ((Object)token ?? "<null>").ToString());
                        return;
                    }
                    name = token.Value.ToLowerInvariant();

                    // check the token. if it's a (, then it's a method. if it's a ;, then it's a field, if it's a [ it's an array field.
                    // if it's a field and bmethod=true, report error.
                    tokenizer.SkipWhitespace();
                    token = tokenizer.ExpectToken(ZScriptTokenType.Comma, ZScriptTokenType.OpenParen, ZScriptTokenType.OpenSquare, ZScriptTokenType.Semicolon);
                    if (token == null || !token.IsValid)
                    {
                        parser.ReportError("Expected comma, ;, [, or argument list, got " + ((Object)token ?? "<null>").ToString());
                        return;
                    }

                    if (token.Type == ZScriptTokenType.OpenParen)
                    {
                        // if we have multiple names
                        if (names.Count > 1)
                        {
                            parser.ReportError("Cannot have multiple names in a method");
                            return;
                        }

                        bmethod = true;
                        // now, I could parse this properly, but it won't be used anyway, so I'll do it as a fake expression.
                        args = parser.ParseExpression(true);
                        token = tokenizer.ExpectToken(ZScriptTokenType.CloseParen);
                        if (token == null || !token.IsValid)
                        {
                            parser.ReportError("Expected ), got " + ((Object)token ?? "<null>").ToString());
                            return;
                        }

                        // also get the body block, if any.
                        tokenizer.SkipWhitespace();
                        long cpos = stream.Position;
                        token = tokenizer.ExpectToken(ZScriptTokenType.Semicolon, ZScriptTokenType.OpenCurly);
                        if (token == null || !token.IsValid)
                        {
                            parser.ReportError("Expected ; or {, got " + ((Object)token ?? "<null>").ToString());
                            return;
                        }

                        //
                        if (token.Type == ZScriptTokenType.OpenCurly)
                        {
                            stream.Position = cpos;
                            body = parser.ParseBlock(false);
                        }
                    }
                    else
                    {
                        if (bmethod)
                        {
                            parser.ReportError("Cannot have virtual, override or action fields");
                            return;
                        }

                        // array
                        if (token.Type == ZScriptTokenType.OpenSquare)
                        {
                            // read in the size or a constant.
                            tokenizer.SkipWhitespace();
                            token = tokenizer.ExpectToken(ZScriptTokenType.Integer, ZScriptTokenType.Identifier);
                            if (token == null || !token.IsValid)
                            {
                                parser.ReportError("Expected integer or constant, got " + ((Object)token ?? "<null>").ToString());
                                return;
                            }

                            // 
                            if (token.Type == ZScriptTokenType.Integer)
                                arraylen = token.ValueInt;
                            else arraylen = -1;

                            tokenizer.SkipWhitespace();
                            token = tokenizer.ExpectToken(ZScriptTokenType.CloseSquare);
                            if (token == null || !token.IsValid)
                            {
                                parser.ReportError("Expected ], got " + ((Object)token ?? "<null>").ToString());
                                return;
                            }

                            tokenizer.SkipWhitespace();
                            token = tokenizer.ExpectToken(ZScriptTokenType.Semicolon, ZScriptTokenType.Comma);
                            if (token == null || !token.IsValid)
                            {
                                parser.ReportError("Expected ;, got " + ((Object)token ?? "<null>").ToString());
                                return;
                            }
                        }
                    }

                    names.Add(name);
                    arraylens.Add(arraylen);

                    if (token.Type != ZScriptTokenType.Comma) // next name
                        break;
                }

                // validate modifiers here.
                // protected and private cannot be combined.
                if (bmethod)
                {
                    if (modifiers.Contains("protected") && modifiers.Contains("private"))
                    {
                        parser.ReportError("Cannot have protected and private on the same method");
                        return;
                    }
                    // virtual and override cannot be combined.
                    int cvirtual = modifiers.Contains("virtual")?1:0;
                    cvirtual += modifiers.Contains("override")?1:0;
                    cvirtual += modifiers.Contains("final")?1:0;
                    if (cvirtual > 1)
                    {
                        parser.ReportError("Cannot have virtual, override and final on the same method");
                        return;
                    }
                    // meta (what the fuck is that?) probably cant be on a method
                    if (modifiers.Contains("meta"))
                    {
                        parser.ReportError("Cannot have meta on a method");
                        return;
                    }
                }

                // finished method or field parsing.
                /*for (int i = 0; i < names.Count; i++)
                {
                    string name = names[i];
                    int arraylen = arraylens[i];

                    string _args = "";
                    if (args != null) _args = " (" + ZScriptTokenizer.TokensToString(args) + ")";
                    else if (arraylen != 0) _args = " [" + arraylen.ToString() + "]";
                    parser.LogWarning(string.Format("{0} {1} {2}{3}", string.Join(" ", modifiers.ToArray()), string.Join(", ", types.ToArray()), name, _args));
                }*/
            }
        }
    }

    public sealed class ZScriptParser : ZDTextParser
    {
        #region ================== Internal classes

        public class ZScriptClassStructure
        {
            // these are used for the class itself
            public string ClassName { get; internal set; }
            public string ReplacementName { get; internal set; }
            public string ParentName { get; internal set; }
            public ZScriptActorStructure Actor { get; internal set; }

            // these are used for parsing and error reporting
            public ZScriptParser Parser { get; internal set; }
            public Stream Stream { get; internal set; }
            public long Position { get; internal set; }
            public BinaryReader DataReader { get; internal set; }
            public string SourceName { get; internal set; }
            public DataLocation DataLocation { get; internal set; }

            // textresourcepath
            public string TextResourcePath { get; internal set; }

            public ZScriptClassStructure(ZScriptParser parser, string classname, string replacesname, string parentname)
            {
                Parser = parser;

                Stream = parser.datastream;
                Position = parser.datastream.Position;
                DataReader = parser.datareader;
                SourceName = parser.sourcename;
                DataLocation = parser.datalocation;
                TextResourcePath = parser.textresourcepath;

                ClassName = classname;
                ReplacementName = replacesname;
                ParentName = parentname;
                Actor = null;
            }

            internal void RestoreStreamData()
            {
                Parser.datastream = Stream;
                Parser.datastream.Position = Position;
                Parser.datareader = DataReader;
                Parser.sourcename = SourceName;
                Parser.datalocation = DataLocation;
                Parser.prevstreamposition = -1;
            }

            public bool Process()
            {
                RestoreStreamData();

                bool isactor = false;
                ZScriptClassStructure _pstruct = this;
                while (_pstruct != null)
                {
                    if (_pstruct.ClassName.ToLowerInvariant() == "actor")
                    {
                        isactor = true;
                        break;
                    }

                    if (_pstruct.ParentName != null)
                    {
                        string _pname = _pstruct.ParentName.ToLowerInvariant();
                        Parser.allclasses.TryGetValue(_pname, out _pstruct);
                    }
                    else _pstruct = null;
                }

                string log_inherits = ((ParentName != null) ? "inherits " + ParentName : "");
                if (ReplacementName != null) log_inherits += ((log_inherits.Length > 0) ? ", " : "") + "replaces " + ReplacementName;
                if (log_inherits.Length > 0) log_inherits = " (" + log_inherits + ")";

                if (isactor)
                {
                    Actor = new ZScriptActorStructure(Parser, null, ClassName, ReplacementName, ParentName);
                    if (Parser.HasError)
                    {
                        Actor = null;
                        return false;
                    }

                    // check actor replacement.
                    Parser.archivedactors[Actor.ClassName.ToLowerInvariant()] = Actor;
                    if (Actor.CheckActorSupported())
                        Parser.actors[Actor.ClassName.ToLowerInvariant()] = Actor;

                    // Replace an actor?
                    if (Actor.ReplacesClass != null)
                    {
                        if (Parser.GetArchivedActorByName(Actor.ReplacesClass) != null)
                            Parser.archivedactors[Actor.ReplacesClass.ToLowerInvariant()] = Actor;
                        else
                            Parser.LogWarning("Unable to find \"" + Actor.ReplacesClass + "\" class to replace, while parsing \"" + Actor.ClassName + "\"");

                        if (Actor.CheckActorSupported() && Parser.GetActorByName(Actor.ReplacesClass) != null)
                            Parser.actors[Actor.ReplacesClass.ToLowerInvariant()] = Actor;
                    }

                    //mxd. Add to current text resource
                    if (!Parser.scriptresources[TextResourcePath].Entries.Contains(Actor.ClassName)) Parser.scriptresources[TextResourcePath].Entries.Add(Actor.ClassName);
                }

                //Parser.LogWarning(string.Format("Parsed {0}class {1}{2}", isactor?"actor ":"", ClassName, log_inherits));

                return true;
            }
        }

        #endregion

        #region ================== Delegates

        public delegate void IncludeDelegate(ZScriptParser parser, string includefile);

        public IncludeDelegate OnInclude;

        #endregion

        #region ================== Constants

        #endregion

        #region ================== Variables

        //mxd. Script type
        internal override ScriptType ScriptType { get { return ScriptType.ZSCRIPT; } }

        // These are actors we want to keep
        private Dictionary<string, ActorStructure> actors;

        // These are all parsed actors, also those from other games
        private Dictionary<string, ActorStructure> archivedactors;

        // In ZScript, you can inherit an actor before defining it. So we need to postpone all processing after the classes have been gathered.
        private Dictionary<string, ZScriptClassStructure> allclasses;
        private List<ZScriptClassStructure> allclasseslist;

        //mxd. Includes tracking
        private HashSet<string> parsedlumps;

        //mxd. Disposing. Is that really needed?..
        private bool isdisposed;

        // [ZZ] custom tokenizer class
        internal ZScriptTokenizer tokenizer;

        #endregion

        #region ================== Properties

        /// <summary>
        /// All actors that are supported by the current game.
        /// </summary>
        public IEnumerable<ActorStructure> Actors { get { return actors.Values; } }

        /// <summary>
        /// All actors defined in the loaded DECORATE structures. This includes actors not supported in the current game.
        /// </summary>
        public ICollection<ActorStructure> AllActors { get { return archivedactors.Values; } }

        /// <summary>
        /// mxd. All actors that are supported by the current game.
        /// </summary>
        internal Dictionary<string, ActorStructure> ActorsByClass { get { return actors; } }

        /// <summary>
        /// mxd. All actors defined in the loaded DECORATE structures. This includes actors not supported in the current game.
        /// </summary>
        internal Dictionary<string, ActorStructure> AllActorsByClass { get { return archivedactors; } }

        #endregion

        #region ================== Constructor / Disposer

        // Constructor
        public ZScriptParser()
        {
            ClearActors();
        }

        // Disposer
        public void Dispose()
        {
            //mxd. Not already disposed?
            if (!isdisposed)
            {
                foreach (KeyValuePair<string, ActorStructure> a in archivedactors)
                    a.Value.Dispose();

                actors = null;
                archivedactors = null;

                isdisposed = true;
            }
        }

        #endregion

        #region ================== Parsing

        private bool ParseInclude(string filename)
        {
            Stream localstream = datastream;
            string localsourcename = sourcename;
            BinaryReader localreader = datareader;
            DataLocation locallocation = datalocation; //mxd
            string localtextresourcepath = textresourcepath; //mxd
            ZScriptTokenizer localtokenizer = tokenizer; // [ZZ]

            //INFO: ZDoom DECORATE include paths can't be relative ("../actor.txt") 
            //or absolute ("d:/project/actor.txt") 
            //or have backward slashes ("info\actor.txt")
            //include paths are relative to the first parsed entry, not the current one 
            //also include paths may or may not be quoted
            //mxd. Sanity checks
            if (string.IsNullOrEmpty(filename))
            {
                ReportError("Expected file name to include");
                return false;
            }

            //mxd. Check invalid path chars
            if (!CheckInvalidPathChars(filename)) return false;

            //mxd. Absolute paths are not supported...
            if (Path.IsPathRooted(filename))
            {
                ReportError("Absolute include paths are not supported by ZDoom");
                return false;
            }

            //mxd. Relative paths are not supported
            if (filename.StartsWith(RELATIVE_PATH_MARKER) || filename.StartsWith(CURRENT_FOLDER_PATH_MARKER) ||
                filename.StartsWith(ALT_RELATIVE_PATH_MARKER) || filename.StartsWith(ALT_CURRENT_FOLDER_PATH_MARKER))
            {
                ReportError("Relative include paths are not supported by ZDoom");
                return false;
            }

            //mxd. Backward slashes are not supported
            if (filename.Contains(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
            {
                ReportError("Only forward slashes are supported by ZDoom");
                return false;
            }

            //mxd. Already parsed?
            if (parsedlumps.Contains(filename))
            {
                ReportError("Already parsed \"" + filename + "\". Check your include directives");
                return false;
            }

            //mxd. Add to collection
            parsedlumps.Add(filename);

            // Callback to parse this file now
            if (OnInclude != null) OnInclude(this, filename);

            //mxd. Bail out on error
            if (this.HasError) return false;

            // Set our buffers back to continue parsing
            datastream = localstream;
            datareader = localreader;
            sourcename = localsourcename;
            datalocation = locallocation; //mxd
            textresourcepath = localtextresourcepath; //mxd
            tokenizer = localtokenizer;

            return true;
        }

        // read in an expression as a token list.
        internal List<ZScriptToken> ParseExpression(bool betweenparen = false)
        {
            List<ZScriptToken> ol = new List<ZScriptToken>();
            //
            int nestingLevel = 0;
            //
            while (true)
            {
                long cpos = datastream.Position;
                ZScriptToken token = tokenizer.ReadToken();
                if (token == null)
                {
                    ReportError("Expected a token");
                    return null;
                }

                if ((token.Type == ZScriptTokenType.Semicolon ||
                     token.Type == ZScriptTokenType.Comma) && nestingLevel == 0 && !betweenparen)
                {
                    datastream.Position = cpos;
                    return ol;
                }
                
                if (token.Type == ZScriptTokenType.OpenParen)
                {
                    nestingLevel++;
                }
                else if (token.Type == ZScriptTokenType.CloseParen)
                {
                    nestingLevel--;
                    if (nestingLevel < 0) // for example, function call
                    {
                        datastream.Position = cpos;
                        return ol;
                    }
                }

                ol.Add(token);
            }
        }

        internal List<ZScriptToken> ParseBlock(bool allowsingle)
        {
            List<ZScriptToken> ol = new List<ZScriptToken>();
            //
            int nestingLevel = 0;
            //
            long cpos = datastream.Position;
            ZScriptToken token = tokenizer.ReadToken();
            if (token == null)
            {
                ReportError("Expected a code block, got <null>");
                return null;
            }

            if (token.Type != ZScriptTokenType.OpenCurly)
            {
                if (!allowsingle)
                {
                    ReportError("Expected opening curly brace, got " + token);
                    return null;
                }

                // otherwise this is an expression
                datastream.Position = cpos;
                List<ZScriptToken> ol_expression = ParseExpression();
                token = tokenizer.ReadToken();
                if (token == null || token.Type != ZScriptTokenType.Semicolon)
                {
                    ReportError("Expected ;, got " + ((Object)token ?? "<null>").ToString());
                    return null;
                }

                ol_expression.Add(token);
                return ol_expression;
            }

            // parse everything between { and }
            nestingLevel = 1;
            while (nestingLevel > 0)
            {
                cpos = datastream.Position;
                token = tokenizer.ReadToken();
                if (token == null)
                {
                    ReportError("Expected a token");
                    return null;
                }

                if (token.Type == ZScriptTokenType.OpenCurly)
                {
                    nestingLevel++;
                }
                else if (token.Type == ZScriptTokenType.CloseCurly)
                {
                    nestingLevel--;
                    if (nestingLevel < 0)
                    {
                        ReportError("Closing parenthesis without an opening one");
                        return null;
                    }
                }

                ol.Add(token);
            }

            // there is POTENTIALLY a semicolon after the class definition. it's not supposed to be there, but it's acceptable (GZDoom.pk3 has this)
            ZScriptToken tailtoken = tokenizer.ReadToken();
            cpos = datastream.Position;
            if (tailtoken == null || tailtoken.Type != ZScriptTokenType.Semicolon)
                datastream.Position = cpos;
            else ol.Add(tailtoken);

            return ol;
        }

        internal bool ParseConst()
        {
            // const blablabla = <expression>;
            tokenizer.SkipWhitespace();
            ZScriptToken token = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
            if (token == null || !token.IsValid)
            {
                ReportError("Expected const name, got " + ((Object)token ?? "<null>").ToString());
                return false;
            }
            string constname = token.Value;
            tokenizer.SkipWhitespace();
            token = tokenizer.ExpectToken(ZScriptTokenType.OpAssign);
            if (token == null || !token.IsValid)
            {
                ReportError("Expected =, got " + ((Object)token ?? "<null>").ToString());
                return false;
            }
            tokenizer.SkipWhitespace();
            if (ParseExpression() == null) return false; // anything until a semicolon or a comma, + anything between parentheses
            tokenizer.SkipWhitespace();
            token = tokenizer.ExpectToken(ZScriptTokenType.Semicolon);
            if (token == null || !token.IsValid)
            {
                ReportError("Expected ;, got " + ((Object)token ?? "<null>").ToString());
                return false;
            }
            //LogWarning(string.Format("Parsed const {0}", constname));
            return true;
        }

        internal bool ParseEnum()
        {
            // enum blablabla {}
            tokenizer.SkipWhitespace();
            ZScriptToken token = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
            if (token == null || !token.IsValid)
            {
                ReportError("Expected enum name, got " + ((Object)token ?? "<null>").ToString());
                return false;
            }
            tokenizer.SkipWhitespace();
            if (ParseBlock(false) == null) return false; // anything between { and }
            //LogWarning(string.Format("Parsed enum {0}", token.Value));
            return true;
        }

        internal bool ParseClassOrStruct(bool isstruct, bool extend)
        {
            // 'class' keyword is already parsed
            tokenizer.SkipWhitespace();
            ZScriptToken tok_classname = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
            if (tok_classname == null || !tok_classname.IsValid)
            {
                ReportError("Expected class name, got " + ((Object)tok_classname ?? "<null>").ToString());
                return false;
            }

            // name [replaces name] [: name] [native]
            ZScriptToken tok_replacename = null;
            ZScriptToken tok_parentname = null;
            ZScriptToken tok_native = null;
            int tokens = 0;
            while (tokens++ < 4)
            {
                tokenizer.SkipWhitespace();
                ZScriptToken token = tokenizer.ReadToken();

                if (token == null)
                {
                    ReportError("Expected a token");
                    return false;
                }

                if (token.Type == ZScriptTokenType.Identifier)
                {
                    if (token.Value.ToLowerInvariant() == "replaces")
                    {
                        if (tok_native != null)
                        {
                            ReportError("Cannot have replacement after native");
                            return false;
                        }

                        if (tok_replacename != null)
                        {
                            ReportError("Cannot have two replacements per class");
                            return false;
                        }

                        tokenizer.SkipWhitespace();
                        tok_replacename = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
                        if (tok_replacename == null || !tok_replacename.IsValid)
                        {
                            ReportError("Expected replacement class name, got " + ((Object)tok_replacename ?? "<null>").ToString());
                            return false;
                        }
                    }
                    else if (token.Value.ToLowerInvariant() == "native")
                    {
                        if (tok_native != null)
                        {
                            ReportError("Cannot have two native keywords");
                            return false;
                        }

                        tok_native = token;
                    }
                    else
                    {
                        ReportError("Unexpected token " + ((Object)token ?? "<null>").ToString());
                    }
                }
                else if (token.Type == ZScriptTokenType.Colon)
                {
                    if (tok_parentname != null)
                    {
                        ReportError("Cannot have two parent classes");
                        return false;
                    }

                    if (tok_replacename != null || tok_native != null)
                    {
                        ReportError("Cannot have parent class after replacement class or native keyword");
                        return false;
                    }

                    tokenizer.SkipWhitespace();
                    tok_parentname = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
                    if (tok_parentname == null || !tok_parentname.IsValid)
                    {
                        ReportError("Expected replacement class name, got " + ((Object)tok_parentname ?? "<null>").ToString());
                        return false;
                    }
                }
                else if (token.Type == ZScriptTokenType.OpenCurly)
                {
                    datastream.Position--;
                    break;
                }
            }

            // do nothing else atm, except remember the position to put it into the class parsing code
            tokenizer.SkipWhitespace();
            long cpos = datastream.Position;
            List<ZScriptToken> classblocktokens = ParseBlock(false);
            if (classblocktokens == null) return false;

            string log_inherits = ((tok_parentname != null) ? "inherits " + tok_parentname.Value : "");
            if (tok_replacename != null) log_inherits += ((log_inherits.Length > 0) ? ", " : "") + "replaces " + tok_replacename.Value;
            if (extend) log_inherits += ((log_inherits.Length > 0) ? ", " : "") + "extends";
            if (log_inherits.Length > 0) log_inherits = " (" + log_inherits + ")";

            // now if we are a class, and we inherit actor, parse this entry as an actor. don't process extensions.
            if (!isstruct && !extend)
            {
                ZScriptClassStructure cls = new ZScriptClassStructure(this, tok_classname.Value, (tok_replacename != null) ? tok_replacename.Value : null, (tok_parentname != null) ? tok_parentname.Value : null);
                cls.Position = cpos;
                string clskey = cls.ClassName.ToLowerInvariant();
                if (allclasses.ContainsKey(clskey))
                {
                    ReportError("Class "+cls.ClassName+" is double-defined");
                    return false;
                }

                allclasses.Add(cls.ClassName.ToLowerInvariant(), cls);
                allclasseslist.Add(cls);
            }

            //LogWarning(string.Format("Parsed {0} {1}{2}", (isstruct ? "struct" : "class"), tok_classname.Value, log_inherits));

            return true;
        }

        // This parses the given decorate stream
        // Returns false on errors
        public override bool Parse(TextResourceData data, bool clearerrors)
        {
            //mxd. Already parsed?
            if (!base.AddTextResource(data))
            {
                if (clearerrors) ClearError();
                return true;
            }

            // Cannot process?
            if (!base.Parse(data, clearerrors)) return false;

            // [ZZ] For whatever reason, the parser is closely tied to the tokenizer, and to the general scripting lumps framework (see scripttype).
            //      For this reason I have to still inherit the old tokenizer while only using the new one.
            //ReportError("found zscript? :)");
            prevstreamposition = -1;
            tokenizer = new ZScriptTokenizer(datareader);

            while (true)
            {
                ZScriptToken token = tokenizer.ExpectToken(ZScriptTokenType.Identifier, // const, enum, class, etc
                                                           ZScriptTokenType.Whitespace,
                                                           ZScriptTokenType.Newline,
                                                           ZScriptTokenType.BlockComment, ZScriptTokenType.LineComment,
                                                           ZScriptTokenType.Preprocessor);

                if (token == null) // EOF reached, whatever.
                    break;

                if (!token.IsValid)
                {
                    ReportError("Expected preprocessor statement, const, enum or class declaraction, got " + token);
                    return false;
                }

                // toplevel tokens allowed are only Preprocessor and Identifier.
                switch (token.Type)
                {
                    case ZScriptTokenType.Whitespace:
                    case ZScriptTokenType.Newline:
                    case ZScriptTokenType.BlockComment:
                    case ZScriptTokenType.LineComment:
                        break;

                    case ZScriptTokenType.Preprocessor:
                        {
                            tokenizer.SkipWhitespace();
                            ZScriptToken directive = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
                            if (directive == null || !directive.IsValid)
                            {
                                ReportError("Expected preprocessor directive, got " + ((Object)directive ?? "<null>").ToString());
                                return false;
                            }

                            if (directive.Value.ToLowerInvariant() == "include")
                            {
                                tokenizer.SkipWhitespace();
                                ZScriptToken include_name = tokenizer.ExpectToken(ZScriptTokenType.Identifier, ZScriptTokenType.String, ZScriptTokenType.Name);
                                if (include_name == null || !include_name.IsValid)
                                {
                                    ReportError("Cannot include: expected a string value, got " + ((Object)include_name ?? "<null>").ToString());
                                    return false;
                                }

                                if (!ParseInclude(include_name.Value))
                                    return false;
                            }
                            else
                            {
                                ReportError("Unknown preprocessor directive: " + directive.Value);
                                return false;
                            }
                            break;
                        }

                    case ZScriptTokenType.Identifier:
                        {
                            // identifier can be one of: class, enum, const, struct
                            // the only type that we really care about is class, as it's the one that has all actors.
                            switch (token.Value.ToLowerInvariant())
                            {
                                case "extend":
                                    tokenizer.SkipWhitespace();
                                    token = tokenizer.ExpectToken(ZScriptTokenType.Identifier);
                                    if (token == null || !token.IsValid || ((token.Value.ToLowerInvariant() != "class") && (token.Value.ToLowerInvariant() != "struct")))
                                    {
                                        ReportError("Expected class or struct, got " + ((Object)token ?? "<null>").ToString());
                                        return false;
                                    }
                                    if (!ParseClassOrStruct((token.Value.ToLowerInvariant() == "struct"), true))
                                        return false;
                                    break;
                                case "class":
                                    // todo parse class
                                    if (!ParseClassOrStruct(false, false))
                                        return false;
                                    break;
                                case "struct":
                                    // todo parse struct
                                    if (!ParseClassOrStruct(true, false))
                                        return false;
                                    break;
                                case "const":
                                    if (!ParseConst())
                                        return false;
                                    break;
                                case "enum":
                                    if (!ParseEnum())
                                        return false;
                                    break;
                                default:
                                    ReportError("Expected preprocessor statement, const, enum or class declaraction, got " + token);
                                    return false;
                            }
                            break;
                        }
                }
            }

            return true;
        }

        public bool Finalize()
        {
            ClearError();

            foreach (ZScriptClassStructure cls in allclasseslist)
            {
                if (!cls.Process())
                    return false;
            }

            return true;
        }

        #endregion

        #region ================== Methods

        protected override int GetCurrentLineNumber()
        {
            prevstreamposition = (tokenizer != null) ? tokenizer.LastPosition : -1;
            return base.GetCurrentLineNumber();
        }

        /// <summary>
        /// This returns a supported actor by name. Returns null when no supported actor with the specified name can be found. This operation is of O(1) complexity.
        /// </summary>
        public ActorStructure GetActorByName(string name)
        {
            name = name.ToLowerInvariant();
            return actors.ContainsKey(name) ? actors[name] : null;
        }

        /// <summary>
        /// This returns a supported actor by DoomEdNum. Returns null when no supported actor with the specified name can be found. Please note that this operation is of O(n) complexity!
        /// </summary>
        public ActorStructure GetActorByDoomEdNum(int doomednum)
        {
            foreach (ActorStructure a in actors.Values)
                if (a.DoomEdNum == doomednum) return a;
            return null;
        }

        // This returns an actor by name
        // Returns null when actor cannot be found
        internal ActorStructure GetArchivedActorByName(string name)
        {
            name = name.ToLowerInvariant();
            return (archivedactors.ContainsKey(name) ? archivedactors[name] : null);
        }

        internal void ClearActors()
        {
            // Initialize
            actors = new Dictionary<string, ActorStructure>(StringComparer.OrdinalIgnoreCase);
            archivedactors = new Dictionary<string, ActorStructure>(StringComparer.OrdinalIgnoreCase);
            parsedlumps = new HashSet<string>(StringComparer.OrdinalIgnoreCase); //mxd
            allclasses = new Dictionary<string, ZScriptClassStructure>();
            allclasseslist = new List<ZScriptClassStructure>();
        }

        #endregion

    }
}
