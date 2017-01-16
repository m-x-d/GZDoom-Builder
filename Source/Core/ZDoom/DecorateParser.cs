
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
    internal sealed class DecorateStateGoto : StateGoto
    {
        #region ================== DECORATE State Goto parsing

        internal DecorateStateGoto(ActorStructure actor, ZDTextParser zdparser) : base(actor, zdparser)
        {
            DecorateParser parser = (DecorateParser)zdparser;

            string firsttarget = "";
            string secondtarget = "";
            bool commentreached = false;
            bool offsetreached = false;
            string offsetstr = "";
            int cindex = 0;

            // This is a bitch to parse because for some bizarre reason someone thought it
            // was funny to allow quotes here. Read the whole line and start parsing this manually.
            string line = parser.ReadLine();

            // Skip whitespace
            while ((cindex < line.Length) && ((line[cindex] == ' ') || (line[cindex] == '\t')))
                cindex++;

            // Parse first target
            while ((cindex < line.Length) && (line[cindex] != ':'))
            {
                // When a comment is reached, we're done here
                if (line[cindex] == '/')
                {
                    if ((cindex + 1 < line.Length) && ((line[cindex + 1] == '/') || (line[cindex + 1] == '*')))
                    {
                        commentreached = true;
                        break;
                    }
                }

                // Whitespace ends the string
                if ((line[cindex] == ' ') || (line[cindex] == '\t'))
                    break;

                // + sign indicates offset start
                if (line[cindex] == '+')
                {
                    cindex++;
                    offsetreached = true;
                    break;
                }

                // Ignore quotes
                if (line[cindex] != '"')
                    firsttarget += line[cindex];

                cindex++;
            }

            if (!commentreached && !offsetreached)
            {
                // Skip whitespace
                while ((cindex < line.Length) && ((line[cindex] == ' ') || (line[cindex] == '\t')))
                    cindex++;

                // Parse second target
                while (cindex < line.Length)
                {
                    // When a comment is reached, we're done here
                    if (line[cindex] == '/')
                    {
                        if ((cindex + 1 < line.Length) && ((line[cindex + 1] == '/') || (line[cindex + 1] == '*')))
                        {
                            commentreached = true;
                            break;
                        }
                    }

                    // Whitespace ends the string
                    if ((line[cindex] == ' ') || (line[cindex] == '\t'))
                        break;

                    // + sign indicates offset start
                    if (line[cindex] == '+')
                    {
                        cindex++;
                        offsetreached = true;
                        break;
                    }

                    // Ignore quotes and semicolons
                    if ((line[cindex] != '"') && (line[cindex] != ':'))
                        secondtarget += line[cindex];

                    cindex++;
                }
            }

            // Try to find the offset if we still haven't found it yet
            if (!offsetreached)
            {
                // Skip whitespace
                while ((cindex < line.Length) && ((line[cindex] == ' ') || (line[cindex] == '\t')))
                    cindex++;

                if ((cindex < line.Length) && (line[cindex] == '+'))
                {
                    cindex++;
                    offsetreached = true;
                }
            }

            if (offsetreached)
            {
                // Parse offset
                while (cindex < line.Length)
                {
                    // When a comment is reached, we're done here
                    if (line[cindex] == '/')
                    {
                        if ((cindex + 1 < line.Length) && ((line[cindex + 1] == '/') || (line[cindex + 1] == '*')))
                        {
                            commentreached = true;
                            break;
                        }
                    }

                    // Whitespace ends the string
                    if ((line[cindex] == ' ') || (line[cindex] == '\t'))
                        break;

                    // Ignore quotes and semicolons
                    if ((line[cindex] != '"') && (line[cindex] != ':'))
                        offsetstr += line[cindex];

                    cindex++;
                }
            }

            // We should now have a first target, optionally a second target and optionally a sprite offset

            // Check if we don't have the class specified
            if (string.IsNullOrEmpty(secondtarget))
            {
                // First target is the state to go to
                classname = actor.ClassName;
                statename = firsttarget.ToLowerInvariant().Trim();
            }
            else
            {
                // First target is the base class to use
                // Second target is the state to go to
                classname = firsttarget.ToLowerInvariant().Trim();
                statename = secondtarget.ToLowerInvariant().Trim();
            }

            if (offsetstr.Length > 0)
                int.TryParse(offsetstr, out spriteoffset);

            if ((classname == "super") && (actor.BaseClass != null))
                classname = actor.BaseClass.ClassName;
        }

        #endregion
    }

    public sealed class DecorateStateStructure : StateStructure
    {
        #region ================== DECORATE State Structure parsing

        internal DecorateStateStructure(ActorStructure actor, ZDTextParser zdparser, DataManager dataman) : base(actor, zdparser, dataman)
        {
            DecorateParser parser = (DecorateParser)zdparser;
            string lasttoken = "";

            // Skip whitespace
            while (parser.SkipWhitespace(true))
            {
                // Read first token
                string token = parser.ReadToken().ToLowerInvariant();

                // One of the flow control statements?
                if ((token == "loop") || (token == "stop") || (token == "wait") || (token == "fail"))
                {
                    // Ignore flow control
                }
                // Goto?
                else if (token == "goto")
                {
                    gotostate = new DecorateStateGoto(actor, parser);
                    if (parser.HasError) return;
                }
                // Label?
                else if (token == ":")
                {
                    // Rewind so that this label can be read again
                    if (!string.IsNullOrEmpty(lasttoken))
                        parser.DataStream.Seek(-(lasttoken.Length + 1), SeekOrigin.Current);

                    // Done here
                    return;
                }
                //mxd. Start of inner scope?
                else if (token == "{")
                {
                    int bracelevel = 1;
                    while (!string.IsNullOrEmpty(token) && bracelevel > 0)
                    {
                        parser.SkipWhitespace(false);
                        token = parser.ReadToken();
                        switch (token)
                        {
                            case "{": bracelevel++; break;
                            case "}": bracelevel--; break;
                        }
                    }
                }
                // End of scope?
                else if (token == "}")
                {
                    // Rewind so that this scope end can be read again
                    parser.DataStream.Seek(-1, SeekOrigin.Current);

                    // Done here
                    return;
                }
                else
                {
                    // First part of the sprite name
                    token = parser.StripTokenQuotes(token); //mxd. First part of the sprite name can be quoted
                    if (string.IsNullOrEmpty(token))
                    {
                        parser.ReportError("Expected sprite name");
                        return;
                    }

                    // Frames of the sprite name
                    parser.SkipWhitespace(true);
                    string spriteframes = parser.StripTokenQuotes(parser.ReadToken()); //mxd. Frames can be quoted
                    if (string.IsNullOrEmpty(spriteframes))
                    {
                        parser.ReportError("Expected sprite frame");
                        return;
                    }

                    // Label?
                    if (spriteframes == ":")
                    {
                        // Rewind so that this label can be read again
                        parser.DataStream.Seek(-(token.Length + 1), SeekOrigin.Current);

                        // Done here
                        return;
                    }

                    // No first sprite yet?
                    FrameInfo info = new FrameInfo(); //mxd
                    if (spriteframes.Length > 0)
                    {
                        //mxd. I'm not even 50% sure the parser handles all bizzare cases without shifting sprite name / frame blocks,
                        // so let's log it as a warning, not an error...
                        if (token.Length != 4)
                        {
                            parser.LogWarning("Invalid sprite name \"" + token.ToUpperInvariant() + "\". Sprite names must be exactly 4 characters long");
                        }
                        else
                        {
                            // Make the sprite name
                            string spritename = (token + spriteframes[0]).ToUpperInvariant();

                            // Ignore some odd ZDoom things
                            if (!spritename.StartsWith("TNT1") && !spritename.StartsWith("----") && !spritename.Contains("#"))
                            {
                                info.Sprite = spritename; //mxd
                                sprites.Add(info);
                            }
                        }
                    }

                    // Continue until the end of the line
                    parser.SkipWhitespace(false);
                    string t = parser.ReadToken();
                    while (!string.IsNullOrEmpty(t) && t != "\n")
                    {
                        //mxd. Bright keyword support...
                        if (t == "bright")
                        {
                            info.Bright = true;
                        }
                        //mxd. Light() expression support...
                        else if (t == "light")
                        {
                            if (!parser.NextTokenIs("(")) return;

                            if (!parser.SkipWhitespace(true))
                            {
                                parser.ReportError("Unexpected end of the structure");
                                return;
                            }

                            info.LightName = parser.StripTokenQuotes(parser.ReadToken());
                            if (string.IsNullOrEmpty(info.LightName))
                            {
                                parser.ReportError("Expected dynamic light name");
                                return;
                            }

                            if (!parser.SkipWhitespace(true))
                            {
                                parser.ReportError("Unexpected end of the structure");
                                return;
                            }

                            if (!parser.NextTokenIs(")")) return;
                        }
                        //mxd. Inner scope start. Step back and reparse using parent loop
                        else if (t == "{")
                        {
                            // Rewind so that this scope end can be read again
                            parser.DataStream.Seek(-1, SeekOrigin.Current);

                            // Break out of this loop
                            break;
                        }
                        //mxd. Function params start (those can span multiple lines)
                        else if (t == "(")
                        {
                            int bracelevel = 1;
                            while (!string.IsNullOrEmpty(token) && bracelevel > 0)
                            {
                                parser.SkipWhitespace(true);
                                token = parser.ReadToken();
                                switch (token)
                                {
                                    case "(": bracelevel++; break;
                                    case ")": bracelevel--; break;
                                }
                            }
                        }
                        //mxd. Because stuff like this is also valid: "Actor Oneliner { States { Spawn: WOOT A 1 A_FadeOut(0.1) Loop }}"
                        else if (t == "}")
                        {
                            // Rewind so that this scope end can be read again
                            parser.DataStream.Seek(-1, SeekOrigin.Current);

                            // Done here
                            return;
                        }

                        // Read next token
                        parser.SkipWhitespace(false);
                        t = parser.ReadToken().ToLowerInvariant();
                    }
                }

                lasttoken = token;
            }
        }

        #endregion
    }

    public sealed class DecorateActorStructure : ActorStructure
    {
        #region ================== DECORATE Actor Structure parsing

        internal DecorateActorStructure(ZDTextParser zdparser, DecorateCategoryInfo catinfo) : base(zdparser, catinfo)
        {
            DecorateParser parser = (DecorateParser)zdparser;
            bool done = false; //mxd

            // First next token is the class name
            parser.SkipWhitespace(true);
            classname = parser.StripTokenQuotes(parser.ReadToken(ACTOR_CLASS_SPECIAL_TOKENS));

            if (string.IsNullOrEmpty(classname))
            {
                parser.ReportError("Expected actor class name");
                return;
            }

            //mxd. Fail on duplicates
            if (parser.ActorsByClass.ContainsKey(classname.ToLowerInvariant()))
            {
                parser.ReportError("Actor \"" + classname + "\" is double-defined");
                return;
            }

            // Parse tokens before entering the actor scope
            while (parser.SkipWhitespace(true))
            {
                string token = parser.ReadToken();
                if (!string.IsNullOrEmpty(token))
                {
                    token = token.ToLowerInvariant();

                    switch (token)
                    {
                        case ":":
                            // The next token must be the class to inherit from
                            parser.SkipWhitespace(true);
                            inheritclass = parser.StripTokenQuotes(parser.ReadToken());
                            if (string.IsNullOrEmpty(inheritclass))
                            {
                                parser.ReportError("Expected class name to inherit from");
                                return;
                            }

                            // Find the actor to inherit from
                            baseclass = parser.GetArchivedActorByName(inheritclass);
                            break;

                        case "replaces":
                            // The next token must be the class to replace
                            parser.SkipWhitespace(true);
                            replaceclass = parser.StripTokenQuotes(parser.ReadToken());
                            if (string.IsNullOrEmpty(replaceclass))
                            {
                                parser.ReportError("Expected class name to replace");
                                return;
                            }
                            break;

                        case "native":
                            // Igore this token
                            break;

                        case "{":
                            // Actor scope begins here,
                            // break out of this parse loop
                            done = true;
                            break;

                        case "-":
                            // This could be a negative doomednum (but our parser sees the - as separate token)
                            // So read whatever is after this token and ignore it (negative doomednum indicates no doomednum)
                            parser.ReadToken();
                            break;

                        default:
                            //mxd. Property begins with $? Then the whole line is a single value
                            if (token.StartsWith("$"))
                            {
                                // This is for editor-only properties such as $sprite and $category
                                props[token] = new List<string> { (parser.SkipWhitespace(false) ? parser.ReadLine() : "") };
                                continue;
                            }

                            if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out doomednum)) // Check if numeric
                            {
                                // Not numeric!
                                parser.ReportError("Expected editor number or start of actor scope while parsing \"" + classname + "\"");
                                return;
                            }

                            //mxd. Range check
                            if ((doomednum < General.Map.FormatInterface.MinThingType) || (doomednum > General.Map.FormatInterface.MaxThingType))
                            {
                                // Out of bounds!
                                parser.ReportError("Actor \"" + classname + "\" has invalid editor number. Editor number must be between "
                                    + General.Map.FormatInterface.MinThingType + " and " + General.Map.FormatInterface.MaxThingType);
                                return;
                            }
                            break;
                    }

                    if (done) break; //mxd
                }
                else
                {
                    parser.ReportError("Unexpected end of structure");
                    return;
                }
            }

            // Now parse the contents of actor structure
            string previoustoken = "";
            done = false; //mxd
            while (parser.SkipWhitespace(true))
            {
                string token = parser.ReadToken();
                token = token.ToLowerInvariant();

                switch (token)
                {
                    case "+":
                    case "-":
                        // Next token is a flag (option) to set or remove
                        bool flagvalue = (token == "+");
                        parser.SkipWhitespace(true);
                        string flagname = parser.ReadToken();
                        if (!string.IsNullOrEmpty(flagname))
                        {
                            // Add the flag with its value
                            flagname = flagname.ToLowerInvariant();
                            flags[flagname] = flagvalue;
                        }
                        else
                        {
                            parser.ReportError("Expected flag name");
                            return;
                        }
                        break;

                    case "action":
                    case "native":
                        // We don't need this, ignore up to the first next ;
                        while (parser.SkipWhitespace(true))
                        {
                            string t = parser.ReadToken();
                            if (string.IsNullOrEmpty(t) || t == ";") break;
                        }
                        break;

                    case "skip_super":
                        skipsuper = true;
                        break;

                    case "states":
                        // Now parse actor states until we reach the end of the states structure
                        while (parser.SkipWhitespace(true))
                        {
                            string statetoken = parser.ReadToken();
                            if (!string.IsNullOrEmpty(statetoken))
                            {
                                // Start of scope?
                                if (statetoken == "{")
                                {
                                    // This is fine
                                }
                                // End of scope?
                                else if (statetoken == "}")
                                {
                                    // Done with the states,
                                    // break out of this parse loop
                                    break;
                                }
                                // State label?
                                else if (statetoken == ":")
                                {
                                    if (!string.IsNullOrEmpty(previoustoken))
                                    {
                                        // Parse actor state
                                        StateStructure st = new DecorateStateStructure(this, parser, parser.DataManager);
                                        if (parser.HasError) return;
                                        states[previoustoken.ToLowerInvariant()] = st;
                                    }
                                    else
                                    {
                                        parser.ReportError("Expected actor state name");
                                        return;
                                    }
                                }
                                else
                                {
                                    // Keep token
                                    previoustoken = statetoken;
                                }
                            }
                            else
                            {
                                parser.ReportError("Unexpected end of structure");
                                return;
                            }
                        }
                        break;

                    case "var": //mxd
                                // Type
                        parser.SkipWhitespace(true);
                        string typestr = parser.ReadToken().ToUpperInvariant();
                        UniversalType type = UniversalType.EnumOption; // There is no Unknown type, so let's use something impossiburu...
                        switch (typestr)
                        {
                            case "INT": type = UniversalType.Integer; break;
                            case "FLOAT": type = UniversalType.Float; break;
                            default: parser.LogWarning("Unknown user variable type"); break;
                        }

                        // Name
                        parser.SkipWhitespace(true);
                        string name = parser.ReadToken();
                        if (string.IsNullOrEmpty(name))
                        {
                            parser.ReportError("Expected User Variable name");
                            return;
                        }
                        if (!name.StartsWith("user_", StringComparison.OrdinalIgnoreCase))
                        {
                            parser.ReportError("User Variable name must start with \"user_\" prefix");
                            return;
                        }
                        if (uservars.ContainsKey(name))
                        {
                            parser.ReportError("User Variable \"" + name + "\" is double defined");
                            return;
                        }
                        if (!skipsuper && baseclass != null && baseclass.uservars.ContainsKey(name))
                        {
                            parser.ReportError("User variable \"" + name + "\" is already defined in one of the parent classes");
                            return;
                        }

                        // Rest
                        parser.SkipWhitespace(true);
                        string next = parser.ReadToken();
                        if (next == "[") // that's User Array. Let's skip it...
                        {
                            int arrlen = -1;
                            if (!parser.ReadSignedInt(ref arrlen))
                            {
                                parser.ReportError("Expected User Array length");
                                return;
                            }
                            if (arrlen < 1)
                            {
                                parser.ReportError("User Array length must be a positive value");
                                return;
                            }
                            if (!parser.NextTokenIs("]") || !parser.NextTokenIs(";"))
                            {
                                return;
                            }
                        }
                        else if (next != ";")
                        {
                            parser.ReportError("Expected \";\", but got \"" + next + "\"");
                            return;
                        }
                        else
                        {
                            // Add to collection
                            uservars.Add(name, type);
                        }
                        break;

                    case "}":
                        //mxd. Get user vars from the BaseClass, if we have one
                        if (!skipsuper && baseclass != null && baseclass.uservars.Count > 0)
                        {
                            foreach (var group in baseclass.uservars)
                                uservars.Add(group.Key, group.Value);
                        }

                        // Actor scope ends here, break out of this parse loop
                        done = true;
                        break;

                    // Monster property?
                    case "monster":
                        // This sets certain flags we are interested in
                        flags["shootable"] = true;
                        flags["countkill"] = true;
                        flags["solid"] = true;
                        flags["canpushwalls"] = true;
                        flags["canusewalls"] = true;
                        flags["activatemcross"] = true;
                        flags["canpass"] = true;
                        flags["ismonster"] = true;
                        break;

                    // Projectile property?
                    case "projectile":
                        // This sets certain flags we are interested in
                        flags["noblockmap"] = true;
                        flags["nogravity"] = true;
                        flags["dropoff"] = true;
                        flags["missile"] = true;
                        flags["activateimpact"] = true;
                        flags["activatepcross"] = true;
                        flags["noteleport"] = true;
                        break;

                    // Clearflags property?
                    case "clearflags":
                        // Clear all flags
                        flags.Clear();
                        break;

                    // Game property?
                    case "game":
                        // Include all tokens on the same line
                        List<string> games = new List<string>();
                        while (parser.SkipWhitespace(false))
                        {
                            string v = parser.ReadToken();
                            if (string.IsNullOrEmpty(v))
                            {
                                parser.ReportError("Expected \"Game\" property value");
                                return;
                            }
                            if (v == "\n") break;
                            if (v == "}") return; //mxd
                            if (v != ",") games.Add(v.ToLowerInvariant());
                        }
                        props[token] = games;
                        break;

                    // Property
                    default:
                        // Property begins with $? Then the whole line is a single value
                        if (token.StartsWith("$"))
                        {
                            // This is for editor-only properties such as $sprite and $category
                            props[token] = new List<string> { (parser.SkipWhitespace(false) ? parser.ReadLine() : "") };
                        }
                        else
                        {
                            // Next tokens up until the next newline are values
                            List<string> values = new List<string>();
                            while (parser.SkipWhitespace(false))
                            {
                                string v = parser.ReadToken();
                                if (string.IsNullOrEmpty(v))
                                {
                                    parser.ReportError("Unexpected end of structure");
                                    return;
                                }
                                if (v == "\n") break;
                                if (v == "}") return; //mxd
                                if (v != ",") values.Add(v);
                            }

                            //mxd. Translate scale to xscale and yscale
                            if (token == "scale")
                            {
                                props["xscale"] = values;
                                props["yscale"] = values;
                            }
                            else
                            {
                                props[token] = values;
                            }
                        }
                        break;
                }

                if (done) break; //mxd

                // Keep token
                previoustoken = token;
            }

            //mxd. Check if baseclass is valid
            if (inheritclass.ToLowerInvariant() != "actor" && doomednum > -1 && baseclass == null)
            {
                //check if this class inherits from a class defined in game configuration
                Dictionary<int, ThingTypeInfo> things = General.Map.Config.GetThingTypes();
                string inheritclasscheck = inheritclass.ToLowerInvariant();

                foreach (KeyValuePair<int, ThingTypeInfo> ti in things)
                {
                    if (!string.IsNullOrEmpty(ti.Value.ClassName) && ti.Value.ClassName.ToLowerInvariant() == inheritclasscheck)
                    {
                        //states
                        if (states.Count == 0 && !string.IsNullOrEmpty(ti.Value.Sprite))
                            states.Add("spawn", new StateStructure(ti.Value.Sprite.Substring(0, 5)));

                        //flags
                        if (ti.Value.Hangs && !flags.ContainsKey("spawnceiling"))
                            flags["spawnceiling"] = true;

                        if (ti.Value.Blocking > 0 && !flags.ContainsKey("solid"))
                            flags["solid"] = true;

                        //properties
                        if (!props.ContainsKey("height"))
                            props["height"] = new List<string> { ti.Value.Height.ToString() };

                        if (!props.ContainsKey("radius"))
                            props["radius"] = new List<string> { ti.Value.Radius.ToString() };

                        return;
                    }
                }

                parser.LogWarning("Unable to find \"" + inheritclass + "\" class to inherit from, while parsing \"" + classname + ":" + doomednum + "\"");
            }
        }

        #endregion
    }

	public sealed class DecorateParser : ZDTextParser
	{
		#region ================== Delegates

		public delegate void IncludeDelegate(DecorateParser parser, string includefile);
		
		public IncludeDelegate OnInclude;
		
		#endregion
		
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables

		//mxd. Script type
		internal override ScriptType ScriptType { get { return ScriptType.DECORATE; } }

		// These are actors we want to keep
		private Dictionary<string, ActorStructure> actors;
		
		// These are all parsed actors, also those from other games
		private Dictionary<string, ActorStructure> archivedactors;

        // These are actors from ZScript. Don't even try to expose this list.
        private Dictionary<string, ActorStructure> zscriptactors;

        //mxd. Includes tracking
        private HashSet<string> parsedlumps;

		//mxd. Custom damagetypes
		private HashSet<string> damagetypes;

		//mxd. Disposing. Is that really needed?..
		private bool isdisposed;
		
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

		/// <summary>
		/// mxd. Custom DamageTypes (http://zdoom.org/wiki/Damage_types).
		/// </summary>
		public IEnumerable<string> DamageTypes { get { return damagetypes; } }
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		public DecorateParser(DataManager dataman, Dictionary<string, ActorStructure> _zscriptactors) : base(dataman)
		{
			// Syntax
			whitespace = "\n \t\r\u00A0"; //mxd. non-breaking space is also space :)
			specialtokens = ":{}[]()+-\n;,";
			skipregions = false; //mxd

            ClearActors();
            zscriptactors = _zscriptactors;
		}
		
		// Disposer
		public void Dispose()
		{
			//mxd. Not already disposed?
			if(!isdisposed)
			{
				foreach(KeyValuePair<string, ActorStructure> a in archivedactors)
					a.Value.Dispose();

				actors = null;
				archivedactors = null;

				isdisposed = true;
			}
		}
		
		#endregion

		#region ================== Parsing

		// This parses the given decorate stream
		// Returns false on errors
		public override bool Parse(TextResourceData data, bool clearerrors)
		{
			//mxd. Already parsed?
			if(!base.AddTextResource(data))
			{
				if(clearerrors) ClearError();
				return true;
			}

			// Cannot process?
			if(!base.Parse(data, clearerrors)) return false;

			//mxd. Region-as-category stuff...
			List<DecorateCategoryInfo> regions = new List<DecorateCategoryInfo>();
			
			//mxd. Regions tracking...
			List<KeyValuePair<int, string>> regionlines = new List<KeyValuePair<int, string>>(); // <line number, region title>
			
			// Keep local data
			Stream localstream = datastream;
			string localsourcename = sourcename;
			BinaryReader localreader = datareader;
			DataLocation locallocation = datalocation; //mxd
			string localtextresourcepath = textresourcepath; //mxd
			
			// Continue until at the end of the stream
			while(SkipWhitespace(true))
			{
				// Read a token
				string objdeclaration = ReadToken();
				if(!string.IsNullOrEmpty(objdeclaration))
				{
					objdeclaration = objdeclaration.ToLowerInvariant();
					if(objdeclaration == "$gzdb_skip") break;
					switch(objdeclaration)
					{
						case "actor":
						{
							// Read actor structure
							ActorStructure actor = new DecorateActorStructure(this, (regions.Count > 0 ? regions[regions.Count - 1] : null));
							if(this.HasError) return false;
						
							// Add the actor
							archivedactors[actor.ClassName.ToLowerInvariant()] = actor;
							if(actor.CheckActorSupported())
								actors[actor.ClassName.ToLowerInvariant()] = actor;
						
							// Replace an actor?
							if(actor.ReplacesClass != null)
							{
								if(GetArchivedActorByName(actor.ReplacesClass) != null)
									archivedactors[actor.ReplacesClass.ToLowerInvariant()] = actor;
								else
									LogWarning("Unable to find \"" + actor.ReplacesClass + "\" class to replace, while parsing \"" + actor.ClassName + "\"");
							
								if(actor.CheckActorSupported() && GetActorByName(actor.ReplacesClass) != null)
									actors[actor.ReplacesClass.ToLowerInvariant()] = actor;
							}

							//mxd. Add to current text resource
							if(!scriptresources[textresourcepath].Entries.Contains(actor.ClassName)) scriptresources[textresourcepath].Entries.Add(actor.ClassName);
						}
						break;

						case "#include":
						{
							//INFO: ZDoom DECORATE include paths can't be relative ("../actor.txt") 
							//or absolute ("d:/project/actor.txt") 
							//or have backward slashes ("info\actor.txt")
							//include paths are relative to the first parsed entry, not the current one 
							//also include paths may or may not be quoted
							SkipWhitespace(true);
							string filename = StripQuotes(ReadToken(false)); //mxd. Don't skip newline

							//mxd. Sanity checks
							if(string.IsNullOrEmpty(filename))
							{
								ReportError("Expected file name to include");
								return false;
							}

							//mxd. Check invalid path chars
							if(!CheckInvalidPathChars(filename)) return false;

							//mxd. Absolute paths are not supported...
							if(Path.IsPathRooted(filename))
							{
								ReportError("Absolute include paths are not supported by ZDoom");
								return false;
							}

							//mxd. Relative paths are not supported
							if(filename.StartsWith(RELATIVE_PATH_MARKER) || filename.StartsWith(CURRENT_FOLDER_PATH_MARKER) ||
							   filename.StartsWith(ALT_RELATIVE_PATH_MARKER) || filename.StartsWith(ALT_CURRENT_FOLDER_PATH_MARKER))
							{
								ReportError("Relative include paths are not supported by ZDoom");
								return false;
							}

							//mxd. Backward slashes are not supported
							if(filename.Contains(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
							{
								ReportError("Only forward slashes are supported by ZDoom");
								return false;
							}

							//mxd. Already parsed?
							if(parsedlumps.Contains(filename))
							{
								ReportError("Already parsed \"" + filename + "\". Check your include directives");
								return false;
							}

							//mxd. Add to collection
							parsedlumps.Add(filename);

							// Callback to parse this file now
							if(OnInclude != null) OnInclude(this, filename);

							//mxd. Bail out on error
							if(this.HasError) return false;

							// Set our buffers back to continue parsing
							datastream = localstream;
							datareader = localreader;
							sourcename = localsourcename;
							datalocation = locallocation; //mxd
							textresourcepath = localtextresourcepath; //mxd
						}
						break;

						case "damagetype": //mxd
							// Get DamageType name
							SkipWhitespace(true);
							string damagetype = StripQuotes(ReadToken(false));
							if(string.IsNullOrEmpty(damagetype))
							{
								ReportError("Expected DamageType name");
								return false;
							}

							// Next should be "{"
							SkipWhitespace(true);
							if(!NextTokenIs("{")) return false;

							// Skip the structure
							while(SkipWhitespace(true))
							{
								string t = ReadToken();
								if(string.IsNullOrEmpty(t) || t == "}") break;
							}

							// Add to collection
							if(!damagetypes.Contains(damagetype)) damagetypes.Add(damagetype);
							break;

						case "enum":
						case "native":
						case "const":
							while(SkipWhitespace(true))
							{
								string t = ReadToken();
								if(string.IsNullOrEmpty(t) || t == ";") break;
							}
							break;

						//mxd. Region-as-category handling
						case "#region":
							int line = GetCurrentLineNumber();
							SkipWhitespace(false);
							string cattitle = ReadLine();
							regionlines.Add(new KeyValuePair<int, string>(line, cattitle));
							if(!string.IsNullOrEmpty(cattitle))
							{
								// Make new category info
								string[] parts = cattitle.Split(DataManager.CATEGORY_SPLITTER, StringSplitOptions.RemoveEmptyEntries);

								DecorateCategoryInfo info = new DecorateCategoryInfo();
								if(regions.Count > 0)
								{
									// Preserve nesting
									info.Category.AddRange(regions[regions.Count - 1].Category);
									info.Properties = new Dictionary<string, List<string>>(regions[regions.Count - 1].Properties);
								}
								info.Category.AddRange(parts);

								// Add to collection
								regions.Add(info);
							}
							break;

						//mxd. Region-as-category handling
						case "#endregion":
							if(regions.Count > 0) regions.RemoveAt(regions.Count - 1);
							if(regionlines.Count > 0)
								regionlines.RemoveAt(regionlines.Count - 1);
							else
								LogWarning("Unexpected #endregion");
							break;

						default:
						{
							//mxd. In some special cases (like the whole actor commented using "//") our special comments will be detected here...
							if(objdeclaration.StartsWith("$"))
							{
								if(regions.Count > 0)
								{
									// Store region property
									regions[regions.Count - 1].Properties[objdeclaration] = new List<string> { (SkipWhitespace(false) ? ReadLine() : "") };
								}
								else
								{
									// Skip the whole line, then carry on
									ReadLine();
								}
								break;
							}
							
							// Unknown structure!
							// Best we can do now is just find the first { and then
							// follow the scopes until the matching } is found
							string token2;
							do
							{
								if(!SkipWhitespace(true)) break;
								token2 = ReadToken();
								if(string.IsNullOrEmpty(token2)) break;
							}
							while(token2 != "{");

							int scopelevel = 1;
							do
							{
								if(!SkipWhitespace(true)) break;
								token2 = ReadToken();
								if(string.IsNullOrEmpty(token2)) break;
								if(token2 == "{") scopelevel++;
								if(token2 == "}") scopelevel--;
							}
							while(scopelevel > 0);
						}
						break;
					}
				}
			}

			//mxd. Check unclosed #regions
			if(regionlines.Count > 0)
			{
				foreach(var group in regionlines)
				{
					if(!string.IsNullOrEmpty(group.Value))
						LogWarning("Unclosed #region \"" + group.Value + "\"", group.Key);
					else
						LogWarning("Unclosed #region", group.Key);
				}
			}
			
			// Return true when no errors occurred
			return (ErrorDescription == null);
		}
		
		#endregion
		
		#region ================== Methods
		
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
			foreach(ActorStructure a in actors.Values)
				if(a.DoomEdNum == doomednum) return a;
			return null;
		}

		// This returns an actor by name
		// Returns null when actor cannot be found
		internal ActorStructure GetArchivedActorByName(string name)
		{
			name = name.ToLowerInvariant();
            ActorStructure zscriptactor = (zscriptactors.ContainsKey(name) ? zscriptactors[name] : null);
            if (zscriptactor != null) return zscriptactor;
			return (archivedactors.ContainsKey(name) ? archivedactors[name] : null);
		}

        internal void ClearActors()
        {
            // Initialize
            actors = new Dictionary<string, ActorStructure>(StringComparer.OrdinalIgnoreCase);
            archivedactors = new Dictionary<string, ActorStructure>(StringComparer.OrdinalIgnoreCase);
            parsedlumps = new HashSet<string>(StringComparer.OrdinalIgnoreCase); //mxd
            damagetypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase); //mxd
        }

        #endregion
    }
}
