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
        private List<string> namedtokentypesorder; // this is the list of said tokens ordered by length.

        public ZScriptTokenizer(BinaryReader br)
        {
            reader = br;
            namedtokentypes = new Dictionary<string, ZScriptTokenType>();
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

        public ZScriptToken ExpectToken(params ZScriptTokenType[] oneof)
        {
            long cpos = reader.BaseStream.Position;

            ZScriptToken tok = ReadToken();
            if (tok == null) return null;

            tok.IsValid = (oneof.Contains(tok.Type));
            if (!tok.IsValid) reader.BaseStream.Position = cpos;
            return tok;
        }

        public ZScriptToken ReadToken()
        {
            try
            {
                ZScriptToken tok;

                // general tokens
                /*
                Identifier, // meow
                Integer, // -666
                Double, // 1.3
                String, // "..."
                Name, // '...'

                LineComment,
                BlockComment,
                Whitespace,

                Preprocessor*/
                long cpos = reader.BaseStream.Position; // I really hope we can rewind this <_<
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

                    tok = new ZScriptToken();
                    tok.Type = ZScriptTokenType.Whitespace;
                    tok.Value = ws_content;
                    return tok;
                }

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

                    tok = new ZScriptToken();
                    tok.Type = ZScriptTokenType.Identifier;
                    tok.Value = id_content;
                    return tok;
                }

                // check everything else
                switch (c)
                {
                    case '\n': // newline
                        {
                            ZScriptToken newline = new ZScriptToken();
                            newline.Type = ZScriptTokenType.Newline;
                            newline.Value += c;
                            return newline;
                        }

                    case '/': // comment
                        {
                            char cnext = reader.ReadChar();
                            if (cnext == '/')
                            {
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

                                tok = new ZScriptToken();
                                tok.Type = ZScriptTokenType.LineComment;
                                tok.Value = cmt;
                                return tok;
                            }
                            else if (cnext == '*')
                            {
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

                                tok = new ZScriptToken();
                                tok.Type = ZScriptTokenType.BlockComment;
                                tok.Value = cmt;
                                return tok;
                            }
                            break;
                        }

                    case '"':
                    case '\'':
                        {
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
                                    tok = new ZScriptToken();
                                    tok.Type = type;
                                    tok.Value = s;
                                    return tok;
                                }
                                else s += cnext;
                            }
                        }

                }

                reader.BaseStream.Position = cpos;

                // named tokens
                char[] namedtoken_buf = reader.ReadChars(namedtokentypesorder[0].Length);
                string namedtoken = new string(namedtoken_buf);
                foreach (string namedtokentype in namedtokentypesorder)
                {
                    if (namedtoken.StartsWith(namedtokentype))
                    {
                        // found the token.
                        reader.BaseStream.Position = cpos + namedtokentype.Length;
                        tok = new ZScriptToken();
                        tok.Type = namedtokentypes[namedtokentype];
                        tok.Value = namedtokentype;
                        return tok;
                    }
                }

                // token not found.
                tok = new ZScriptToken();
                tok.Type = ZScriptTokenType.Invalid;
                tok.Value = "" + c;
                tok.IsValid = false;
                reader.BaseStream.Position = cpos+1;
                return tok;
            }
            catch (IOException)
            {
                return null;
            }
        }
    }
}
