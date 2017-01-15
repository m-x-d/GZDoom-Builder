
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
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public class ActorStructure
	{
		#region ================== Constants
		
		private readonly string[] SPRITE_CHECK_STATES = { "idle", "see", "inactive", "spawn" }; //mxd
		internal const string ACTOR_CLASS_SPECIAL_TOKENS = ":{}\n;,"; //mxd

        #endregion

        #region ================== Variables

        // Declaration
        internal string classname;
        internal string inheritclass;
        internal string replaceclass;
        internal int doomednum = -1;

        // Inheriting
        internal ActorStructure baseclass;
        internal bool skipsuper;

        // Flags
        internal Dictionary<string, bool> flags;

        // Properties
        internal Dictionary<string, List<string>> props;
        internal Dictionary<string, UniversalType> uservars; //mxd

        //mxd. Categories
        internal DecorateCategoryInfo catinfo;

        // States
        internal Dictionary<string, StateStructure> states;
		
		#endregion
		
		#region ================== Properties
		
		public Dictionary<string, bool> Flags { get { return flags; } }
		public Dictionary<string, List<string>> Properties { get { return props; } }
		public string ClassName { get { return classname; } }
		public string InheritsClass { get { return inheritclass; } }
		public string ReplacesClass { get { return replaceclass; } }
		public ActorStructure BaseClass { get { return baseclass; } }
		internal int DoomEdNum { get { return doomednum; } set { doomednum = value; } }
		public Dictionary<string, UniversalType> UserVars { get { return uservars; } } //mxd
		internal DecorateCategoryInfo CategoryInfo { get { return catinfo; } } //mxd

		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		internal ActorStructure(ZDTextParser parser, DecorateCategoryInfo catinfo)
		{
			// Initialize
			this.catinfo = catinfo; //mxd
			flags = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
			props = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
			states = new Dictionary<string, StateStructure>(StringComparer.OrdinalIgnoreCase);
			uservars = new Dictionary<string, UniversalType>(StringComparer.OrdinalIgnoreCase);//mxd
			
			// Always define a game property, but default to 0 values
			props["game"] = new List<string>();
			
			inheritclass = "actor";
			replaceclass = null;
			baseclass = null;
			skipsuper = false;
		}
		
		// Disposer
		public void Dispose()
		{
			baseclass = null;
			flags = null;
			props = null;
			states = null;
		}
		
		#endregion
		
		#region ================== Methods
		
		/// <summary>
		/// This checks if the actor has a specific property.
		/// </summary>
		public bool HasProperty(string propname)
		{
			if(props.ContainsKey(propname)) return true;
			if(!skipsuper && (baseclass != null)) return baseclass.HasProperty(propname);
			return false;
		}
		
		/// <summary>
		/// This checks if the actor has a specific property with at least one value.
		/// </summary>
		public bool HasPropertyWithValue(string propname)
		{
			if(props.ContainsKey(propname) && (props[propname].Count > 0)) return true;
			if(!skipsuper && (baseclass != null)) return baseclass.HasPropertyWithValue(propname);
			return false;
		}
		
		/// <summary>
		/// This returns values of a specific property as a complete string. Returns an empty string when the propery has no values.
		/// </summary>
		public string GetPropertyAllValues(string propname)
		{
			if(props.ContainsKey(propname) && (props[propname].Count > 0))
				return string.Join(" ", props[propname].ToArray());
			if(!skipsuper && (baseclass != null))
				return baseclass.GetPropertyAllValues(propname);
			return "";
		}
		
		/// <summary>
		/// This returns a specific value of a specific property as a string. Returns an empty string when the propery does not have the specified value.
		/// </summary>
		public string GetPropertyValueString(string propname, int valueindex) { return GetPropertyValueString(propname, valueindex, true); } //mxd. Added "stripquotes" parameter
		public string GetPropertyValueString(string propname, int valueindex, bool stripquotes)
		{
			if(props.ContainsKey(propname) && (props[propname].Count > valueindex))
				return (stripquotes ? ZDTextParser.StripQuotes(props[propname][valueindex]) : props[propname][valueindex]);
			if(!skipsuper && (baseclass != null))
				return baseclass.GetPropertyValueString(propname, valueindex, stripquotes);
			return "";
		}
		
		/// <summary>
		/// This returns a specific value of a specific property as an integer. Returns 0 when the propery does not have the specified value.
		/// </summary>
		public int GetPropertyValueInt(string propname, int valueindex)
		{
			string str = GetPropertyValueString(propname, valueindex, false);

			//mxd. It can be negative...
			if(str == "-" && props.Count > valueindex + 1)
				str += GetPropertyValueString(propname, valueindex + 1, false);
			
			int intvalue;
			if(int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out intvalue))
				return intvalue;
			return 0;
		}
		
		/// <summary>
		/// This returns a specific value of a specific property as a float. Returns 0.0f when the propery does not have the specified value.
		/// </summary>
		public float GetPropertyValueFloat(string propname, int valueindex)
		{
			string str = GetPropertyValueString(propname, valueindex, false);

			//mxd. It can be negative...
			if(str == "-" && props.Count > valueindex + 1)
				str += GetPropertyValueString(propname, valueindex + 1, false);

			float fvalue;
			if(float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out fvalue))
				return fvalue;
			return 0.0f;
		}
		
		/// <summary>
		/// This returns the status of a flag.
		/// </summary>
		public bool HasFlagValue(string flag)
		{
			if(flags.ContainsKey(flag)) return true;
			if(!skipsuper && (baseclass != null)) return baseclass.HasFlagValue(flag);
			return false;
		}
		
		/// <summary>
		/// This returns the status of a flag.
		/// </summary>
		public bool GetFlagValue(string flag, bool defaultvalue)
		{
			if(flags.ContainsKey(flag)) return flags[flag];
			if(!skipsuper && (baseclass != null)) return baseclass.GetFlagValue(flag, defaultvalue);
			return defaultvalue;
		}
		
		/// <summary>
		/// This checks if a state has been defined.
		/// </summary>
		public bool HasState(string statename)
		{
			if(states.ContainsKey(statename)) return true;
			if(!skipsuper && (baseclass != null)) return baseclass.HasState(statename);
			return false;
		}
		
		/// <summary>
		/// This returns a specific state, or null when the state can't be found.
		/// </summary>
		public StateStructure GetState(string statename)
		{
			if(states.ContainsKey(statename)) return states[statename];
			if(!skipsuper && (baseclass != null)) return baseclass.GetState(statename);
			return null;
		}
		
		/// <summary>
		/// This creates a list of all states, also those inherited from the base class.
		/// </summary>
		public Dictionary<string, StateStructure> GetAllStates()
		{
			Dictionary<string, StateStructure> list = new Dictionary<string, StateStructure>(states, StringComparer.OrdinalIgnoreCase);
			
			if(!skipsuper && (baseclass != null))
			{
				Dictionary<string, StateStructure> baselist = baseclass.GetAllStates();
				foreach(KeyValuePair<string, StateStructure> s in baselist)
					if(!list.ContainsKey(s.Key)) list.Add(s.Key, s.Value);
			}
			
			return list;
		}
		
		/// <summary>
		/// This checks if this actor is meant for the current decorate game support
		/// </summary>
		public bool CheckActorSupported()
		{
			// Check if we want to include this actor
			string includegames = General.Map.Config.DecorateGames.ToLowerInvariant();
			bool includeactor = (props["game"].Count == 0);
			foreach(string g in props["game"])
				includeactor |= includegames.Contains(g);
			
			return includeactor;
		}
		
		/// <summary>
		/// This finds the best suitable sprite to use when presenting this actor to the user.
		/// </summary>
		public StateStructure.FrameInfo FindSuitableSprite()
		{
			// Info: actual sprites are resolved in ThingTypeInfo.SetupSpriteFrame() - mxd
			// Sprite forced?
			if(HasPropertyWithValue("$sprite"))
			{
				string sprite = GetPropertyValueString("$sprite", 0, true); //mxd

				//mxd. Valid when internal or exists
				if(sprite.StartsWith(DataManager.INTERNAL_PREFIX, StringComparison.OrdinalIgnoreCase) || General.Map.Data.GetSpriteExists(sprite))
					return new StateStructure.FrameInfo { Sprite = sprite };

				//mxd. Bitch and moan
				General.ErrorLogger.Add(ErrorType.Warning, "DECORATE warning in " + classname + ":" + doomednum + ". The sprite \"" + sprite + "\" assigned by the \"$sprite\" property does not exist.");
			}

			//mxd. Try to get a suitable sprite from our hardcoded states list
			foreach(string state in SPRITE_CHECK_STATES)
			{
				if(!HasState(state)) continue;

				StateStructure s = GetState(state);
				StateStructure.FrameInfo info = s.GetSprite(0);
				if(!string.IsNullOrEmpty(info.Sprite)) return info;
			}
			
			// Still no sprite found? then just pick the first we can find
			Dictionary<string, StateStructure> list = GetAllStates();
			foreach(StateStructure s in list.Values)
			{
				StateStructure.FrameInfo info = s.GetSprite(0);
				if(!string.IsNullOrEmpty(info.Sprite)) return info;
			}
			
			//mxd. No dice...
			return null;
		}
		
		#endregion
	}
}
