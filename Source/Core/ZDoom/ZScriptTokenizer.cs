using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeImp.DoomBuilder.ZDoom
{
    internal class ZScriptTokenString : System.Attribute
    {
        public string Value { get; private set; }

        public ZScriptTokenString(string value)
        {
            Value = value;
        }
    }

    internal enum ZScriptTokenType
    {
        // generic tokens
        Identifier, // meow
        Integer, // -666
        Double, // 1.3
        String, // "..."
        Name, // '...'

        // comments
        LineComment, // // blablabla
        BlockComment, // /* blablabla */
        Whitespace, // whitespace is a legit token.

        // invalid token
        Invalid,

        [ZScriptTokenString("#")] Preprocessor,
        [ZScriptTokenString("\n")] Newline,

        [ZScriptTokenString("{")] OpenCurly,
        [ZScriptTokenString("}")] CloseCurly,

        [ZScriptTokenString("(")] OpenParen,
        [ZScriptTokenString(")")] CloseParen,

        [ZScriptTokenString("[")] OpenSquare,
        [ZScriptTokenString("]")] CloseSquare,

        [ZScriptTokenString(".")] Dot,
        [ZScriptTokenString(",")] Comma,

        // == != < > <= >=
        [ZScriptTokenString("==")] OpEquals,
        [ZScriptTokenString("!=")] OpNotEquals,
        [ZScriptTokenString("<")] OpLessThan,
        [ZScriptTokenString(">")] OpGreaterThan,
        [ZScriptTokenString("<=")] OpLessOrEqual,
        [ZScriptTokenString(">=")] OpGreaterOrEqual,

        // ternary operator (x ? y : z), also the colon after state labels
        [ZScriptTokenString("?")] Questionmark,
        [ZScriptTokenString(":")] Colon,
        [ZScriptTokenString("::")] DoubleColon,

        // + - * / << >> ~ ^ & |
        [ZScriptTokenString("+")] OpAdd,
        [ZScriptTokenString("-")] OpSubtract,
        [ZScriptTokenString("*")] OpMultiply,
        [ZScriptTokenString("/")] OpDivide,
        [ZScriptTokenString("<<")] OpLeftShift,
        [ZScriptTokenString(">>")] OpRightShift,
        [ZScriptTokenString("~")] OpNegate,
        [ZScriptTokenString("^")] OpXor,
        [ZScriptTokenString("&")] OpAnd,
        [ZScriptTokenString("|")] OpOr,

        // = += -= *= /= <<= >>= ~= ^= &= |=
        [ZScriptTokenString("=")] OpAssign,
        [ZScriptTokenString("+=")] OpAssignAdd,
        [ZScriptTokenString("-=")] OpAssignSubtract,
        [ZScriptTokenString("*=")] OpAssignMultiply,
        [ZScriptTokenString("/=")] OpAssignDivide,
        [ZScriptTokenString("<<=")] OpAssignLeftShift,
        [ZScriptTokenString(">>=")] OpAssignRightShift,
        [ZScriptTokenString("~=")] OpAssignNegate,
        [ZScriptTokenString("^=")] OpAssignXor,
        [ZScriptTokenString("&=")] OpAssignAnd,
        [ZScriptTokenString("|=")] OpAssignOr,

        // unary: !
        [ZScriptTokenString("!")] OpUnaryNot,

        // semicolon
        [ZScriptTokenString(";")] Semicolon
    }

    internal class ZScriptToken
    {
        public ZScriptToken()
        {
            IsValid = true;
        }

        public ZScriptTokenType Type { get; internal set; }
        public string Value { get; internal set; }
        public int ValueInt { get; internal set; }
        public double ValueDouble { get; internal set; }
        public bool IsValid { get; internal set; }

        public override string ToString()
        {
            return string.Format("<Token.{0} ({1})>", Type.ToString(), Value);
        }
    }

    internal class ZScriptTokenizer
    {
        private BinaryReader reader;
        private Dictionary<string, ZScriptTokenType> namedtokentypes; // these are tokens that have precise equivalent in the enum (like operators)
        private Dictionary<ZScriptTokenType, string> namedtokentypesreverse; // these are tokens that have precise equivalent in the enum (like operators)
        private List<string> namedtokentypesorder; // this is the list of said tokens ordered by length.

        public BinaryReader Reader { get { return reader; } }
        public long LastPosition { get; private set; }

        public ZScriptTokenizer(BinaryReader br)
        {
            reader = br;
            namedtokentypes = new Dictionary<string, ZScriptTokenType>();
            namedtokentypesreverse = new Dictionary<ZScriptTokenType, string>();
            namedtokentypesorder = new List<string>();
            // initialize the token type list.
            IEnumerable<ZScriptTokenType> tokentypes = Enum.GetValues(typeof(ZScriptTokenType)).Cast<ZScriptTokenType>();
            foreach (ZScriptTokenType tokentype in tokentypes)
            {
                // 
                FieldInfo fi = typeof(ZScriptTokenType).GetField(tokentype.ToString());
                ZScriptTokenString[] attrs = (ZScriptTokenString[])fi.GetCustomAttributes(typeof(ZScriptTokenString), false);
                if (attrs.Length == 0) continue;
                // 
                namedtokentypes.Add(attrs[0].Value, tokentype);
                namedtokentypesreverse.Add(tokentype, attrs[0].Value);
                namedtokentypesorder.Add(attrs[0].Value);
            }

            namedtokentypesorder.Sort(delegate (string a, string b)
            {
                if (a.Length > b.Length)
                    return -1;
                if (a.Length < b.Length)
                    return 1;
                return 0;
            });
        }

        public void SkipWhitespace() // note that this skips both whitespace, newlines AND comments
        {
            while (true)
            {
                ZScriptToken tok = ExpectToken(ZScriptTokenType.Newline, ZScriptTokenType.BlockComment, ZScriptTokenType.LineComment, ZScriptTokenType.Whitespace);
                if (tok == null || !tok.IsValid) break;
            }
        }

        private ZScriptToken TryReadWhitespace()
        {
            long cpos = LastPosition = reader.BaseStream.Position;
            char c = reader.ReadChar();

            // 
            string whitespace = " \r\t\u00A0";

            // check whitespace
            if (whitespace.Contains(c))
            {
                string ws_content = "";
                ws_content += c;
                while (true)
                {
                    char cnext = reader.ReadChar();
                    if (whitespace.Contains(cnext))
                    {
                        ws_content += cnext;
                        continue;
                    }

                    reader.BaseStream.Position--;
                    break;
                }

                ZScriptToken tok = new ZScriptToken();
                tok.Type = ZScriptTokenType.Whitespace;
                tok.Value = ws_content;
                return tok;
            }

            reader.BaseStream.Position = cpos;
            return null;
        }

        private ZScriptToken TryReadIdentifier()
        {
            long cpos = LastPosition = reader.BaseStream.Position;
            char c = reader.ReadChar();

            // check identifier
            if ((c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                (c == '_'))
            {
                string id_content = "";
                id_content += c;
                while (true)
                {
                    char cnext = reader.ReadChar();
                    if ((cnext >= 'a' && cnext <= 'z') ||
                        (cnext >= 'A' && cnext <= 'Z') ||
                        (cnext == '_') ||
                        (cnext >= '0' && cnext <= '9'))
                    {
                        id_content += cnext;
                        continue;
                    }

                    reader.BaseStream.Position--;
                    break;
                }

                ZScriptToken tok = new ZScriptToken();
                tok.Type = ZScriptTokenType.Identifier;
                tok.Value = id_content;
                return tok;
            }

            reader.BaseStream.Position = cpos;
            return null;
        }

        private ZScriptToken TryReadNumber()
        {
            long cpos = LastPosition = reader.BaseStream.Position;
            char c = reader.ReadChar();

            // check integer
            if ((c >= '0' && c <= '9') || c == '.')
            {
                bool isint = true;
                bool isdouble = (c == '.');
                bool isexponent = false;
                if (isdouble) // make sure next character is an integer, otherwise its probably a member access
                {
                    char cnext = reader.ReadChar();
                    if (!(cnext >= '0' && cnext <= '9'))
                    {
                        isint = false;
                        reader.BaseStream.Position--;
                    }
                }

                if (isint)
                {
                    bool isoctal = (c == '0');
                    bool ishex = false;
                    string i_content = "";
                    i_content += c;
                    while (true)
                    {
                        char cnext = reader.ReadChar();
                        if (!isdouble && (cnext == 'x') && i_content.Length == 1)
                        {
                            isoctal = false;
                            ishex = true;
                        }
                        else if ((cnext >= '0' && cnext <= '7') ||
                                 (!isoctal && cnext >= '8' && cnext <= '9') ||
                                 (ishex && ((cnext >= 'a' && cnext <= 'f') || (cnext >= 'A' && cnext <= 'F'))))
                        {
                            i_content += cnext;
                        }
                        else if (!ishex && !isdouble && !isexponent && cnext == '.')
                        {
                            isdouble = true;
                            isoctal = false;
                            i_content += '.';
                        }
                        else if (!isoctal && !ishex && !isexponent && (cnext == 'e' || cnext == 'E'))
                        {
                            isexponent = true;
                            isdouble = true;
                            i_content += 'e';
                            cnext = reader.ReadChar();
                            if (cnext == '-') i_content += '-';
                            else reader.BaseStream.Position--;
                        }
                        else
                        {
                            reader.BaseStream.Position--;
                            break;
                        }
                    }

                    ZScriptToken tok = new ZScriptToken();
                    tok.Type = (isdouble ? ZScriptTokenType.Double : ZScriptTokenType.Integer);
                    tok.Value = i_content;
                    try
                    {
                        if (ishex || isoctal || !isdouble)
                        {
                            int numbase = 10;
                            if (ishex) numbase = 16;
                            else if (isoctal) numbase = 8;
                            tok.ValueInt = Convert.ToInt32(tok.Value, numbase);
                            tok.ValueDouble = tok.ValueInt;
                        }
                        else if (isdouble)
                        {
                            string dval = (tok.Value[0] == '.') ? "0" + tok.Value : tok.Value;
                            tok.ValueDouble = Convert.ToDouble(dval);
                            tok.ValueInt = (int)tok.ValueDouble;
                        }
                    }
                    catch (Exception)
                    {
                        //throw new Exception(tok.ToString());
                        return null;
                    }

                    return tok;
                }
            }

            reader.BaseStream.Position = cpos;
            return null;
        }

        private ZScriptToken TryReadStringOrComment(bool allowstring, bool allowname, bool allowblock, bool allowline)
        {
            long cpos = LastPosition = reader.BaseStream.Position;
            char c = reader.ReadChar();

            switch (c)
            {
                case '/': // comment
                    {
                        if (!allowblock && !allowline) break;
                        char cnext = reader.ReadChar();
                        if (cnext == '/')
                        {
                            if (!allowline) break;
                            // line comment: read until newline but not including it
                            string cmt = "";
                            while (true)
                            {
                                cnext = reader.ReadChar();
                                if (cnext == '\n')
                                {
                                    reader.BaseStream.Position--;
                                    break;
                                }

                                cmt += cnext;
                            }

                            ZScriptToken tok = new ZScriptToken();
                            tok.Type = ZScriptTokenType.LineComment;
                            tok.Value = cmt;
                            return tok;
                        }
                        else if (cnext == '*')
                        {
                            if (!allowblock) break;
                            // block comment: read until closing sequence
                            string cmt = "";
                            while (true)
                            {
                                cnext = reader.ReadChar();
                                if (cnext == '*')
                                {
                                    char cnext2 = reader.ReadChar();
                                    if (cnext2 == '/')
                                        break;

                                    reader.BaseStream.Position--;
                                }

                                cmt += cnext;
                            }

                            ZScriptToken tok = new ZScriptToken();
                            tok.Type = ZScriptTokenType.BlockComment;
                            tok.Value = cmt;
                            return tok;
                        }
                        break;
                    }

                case '"':
                case '\'':
                    {
                        if ((c == '"' && !allowstring) || (c == '\'' && !allowname)) break;
                        ZScriptTokenType type = (c == '"' ? ZScriptTokenType.String : ZScriptTokenType.Name);
                        string s = "";
                        while (true)
                        {
                            // todo: parse escape sequences properly
                            char cnext = reader.ReadChar();
                            if (cnext == '\\') // escape sequence. right now, do nothing
                            {
                                cnext = reader.ReadChar();
                                s += cnext;
                            }
                            else if (cnext == c)
                            {
                                ZScriptToken tok = new ZScriptToken();
                                tok.Type = type;
                                tok.Value = s;
                                return tok;
                            }
                            else s += cnext;
                        }
                    }

            }

            reader.BaseStream.Position = cpos;
            return null;
        }

        private ZScriptToken TryReadNamedToken()
        {
            long cpos = LastPosition = reader.BaseStream.Position;

            // named tokens
            char[] namedtoken_buf = reader.ReadChars(namedtokentypesorder[0].Length);
            string namedtoken = new string(namedtoken_buf);
            foreach (string namedtokentype in namedtokentypesorder)
            {
                if (namedtoken.StartsWith(namedtokentype))
                {
                    // found the token.
                    reader.BaseStream.Position = cpos + namedtokentype.Length;
                    ZScriptToken tok = new ZScriptToken();
                    tok.Type = namedtokentypes[namedtokentype];
                    tok.Value = namedtokentype;
                    return tok;
                }
            }

            reader.BaseStream.Position = cpos;
            return null;
        }

        public ZScriptToken ExpectToken(params ZScriptTokenType[] oneof)
        {
            long cpos = reader.BaseStream.Position;

            try
            {
                // try to expect whitespace
                if (oneof.Contains(ZScriptTokenType.Whitespace))
                {
                    ZScriptToken tok = TryReadWhitespace();
                    if (tok != null) return tok;
                }

                // try to expect an identifier
                if (oneof.Contains(ZScriptTokenType.Identifier))
                {
                    ZScriptToken tok = TryReadIdentifier();
                    if (tok != null) return tok;
                }

                bool blinecomment = oneof.Contains(ZScriptTokenType.LineComment);
                bool bblockcomment = oneof.Contains(ZScriptTokenType.BlockComment);
                bool bstring = oneof.Contains(ZScriptTokenType.String);
                bool bname = oneof.Contains(ZScriptTokenType.Name);

                if (bstring || bname || bblockcomment || blinecomment)
                {
                    // try to expect a string
                    ZScriptToken tok = TryReadStringOrComment(bstring, bname, bblockcomment, blinecomment);
                    if (tok != null) return tok;
                }

                // if we are expecting a number (double or int), try to read it
                if (oneof.Contains(ZScriptTokenType.Integer) || oneof.Contains(ZScriptTokenType.Double))
                {
                    ZScriptToken tok = TryReadNumber();
                    if (tok != null && oneof.Contains(tok.Type)) return tok;
                }

                // try to expect special tokens
                // read max
                char[] namedtoken_buf = reader.ReadChars(namedtokentypesorder[0].Length);
                string namedtoken = new string(namedtoken_buf);
                foreach (ZScriptTokenType expected in oneof)
                {
                    // check if there is a value for this one
                    if (!namedtokentypesreverse.ContainsKey(expected))
                        continue;
                    string namedtokentype = namedtokentypesreverse[expected];
                    if (namedtoken.StartsWith(namedtokentype))
                    {
                        // found the token.
                        reader.BaseStream.Position = cpos + namedtokentype.Length;
                        ZScriptToken tok = new ZScriptToken();
                        tok.Type = namedtokentypes[namedtokentype];
                        tok.Value = namedtokentype;
                        return tok;
                    }
                }
            }
            catch (IOException)
            {
                reader.BaseStream.Position = cpos;
                return null;
            }

            // token was not found, try to read the token that was actually found and return that
            reader.BaseStream.Position = cpos;
            ZScriptToken invalid = ReadToken();
            if (invalid != null) invalid.IsValid = false;
            reader.BaseStream.Position = cpos;
            return invalid;
        }

        // short_circuit only checks for string literals and "everything else", reporting "everything else" as invalid tokens
        public ZScriptToken ReadToken(bool short_circuit = false)
        {
            try
            {
                ZScriptToken tok;

                // 
                tok = TryReadWhitespace();
                if (tok != null) return tok;

                if (!short_circuit)
                {
                    tok = TryReadIdentifier();
                    if (tok != null) return tok;

                    tok = TryReadNumber();
                    if (tok != null) return tok;
                }

                tok = TryReadStringOrComment(true, true, true, true);
                if (tok != null) return tok;

                if (!short_circuit)
                {
                    tok = TryReadNamedToken();
                    if (tok != null) return tok;
                }

                // token not found.
                tok = new ZScriptToken();
                tok.Type = ZScriptTokenType.Invalid;
                tok.Value = "" + reader.ReadChar();
                tok.IsValid = false;
                return tok;
            }
            catch (IOException)
            {
                return null;
            }
        }

        public static string TokensToString(IEnumerable<ZScriptToken> tokens)
        {
            string outs = "";
            foreach (ZScriptToken tok in tokens)
                outs += tok.Value;
            return outs;
        }
    }
}
