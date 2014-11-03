
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class ThingFlagsCompare
	{
		public enum CompareMethod
		{
			Equal,
			And
		};

		#region ================== Constants

		#endregion

		#region ================== Variables

		private readonly string flag;
		private string requiredgroup; //mxd. This flag only works if at least one flag is set in the "requiredgroup"
		private string ignoredgroup; //mxd. If this flag is set, flags from ignoredgroup can be... well... ignored!
		private string requiredflag; //mxd. This flag only works if requiredflag is set.
		private readonly bool ingnorethisgroupwhenunset; //mxd
		private readonly CompareMethod comparemethod;
		private readonly bool invert;
		private readonly string group;

		#endregion

		#region ================== Properties

		public string Flag { get { return flag; } }
		public string Group { get { return group; } }
		public string RequiredGroup { get { return requiredgroup; } internal set { requiredgroup = value; } } //mxd
		public string IgnoredGroup { get { return ignoredgroup; } internal set { ignoredgroup = value; } } //mxd
		public string RequiredFlag { get { return requiredflag; } internal set { requiredflag = value; } } //mxd

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ThingFlagsCompare(Configuration cfg, string group, string flag)
		{
			string cfgpath = "thingflagscompare." + group + "." + flag;
			this.flag = flag;
			this.group = group;

			string cm = cfg.ReadSetting(cfgpath + ".comparemethod", "and");

			switch (cm)
			{
				default:
					General.ErrorLogger.Add(ErrorType.Warning, "Unrecognized value \"" + cm + "\" for comparemethod in " + cfgpath + " in game configuration " + cfg.ReadSetting("game", "<unnamed game>") + ". Defaulting to \"and\".");
					goto case "and";
				case "and":
					comparemethod = CompareMethod.And;
					break;
				case "equal":
					comparemethod = CompareMethod.Equal;
					break;
			}

			invert = cfg.ReadSetting(cfgpath + ".invert", false);
			requiredgroup = cfg.ReadSetting(cfgpath + ".requiredgroup", string.Empty); //mxd
			ignoredgroup = cfg.ReadSetting(cfgpath + ".ignoredgroup", string.Empty); //mxd
			requiredflag = cfg.ReadSetting(cfgpath + ".requiredflag", string.Empty); //mxd
			ingnorethisgroupwhenunset = cfg.ReadSetting(cfgpath + ".ingnorethisgroupwhenunset", false); //mxd
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		// Compares the flag of the two things.
		// Returns:
		// -1 if the flag does not overlap
		//	0 if the flag should be ignored
		//	1 if the flag overlaps
		public int Compare(Thing t1, Thing t2)
		{
			bool t1flag;
			bool t2flag;

			// Check if the flags exist
			if (!t1.Flags.ContainsKey(flag) || !t2.Flags.ContainsKey(flag)) return 0;

			//mxd. We should ignore the flag if requiredgroup doesn't have any flags set
			if(!string.IsNullOrEmpty(requiredgroup)) 
			{
				bool t1hasrequiredflags = false;
				bool t2hasrequiredflags = false;
				foreach(string key in General.Map.Config.ThingFlagsCompare[requiredgroup].Keys)
				{
					if(t1.Flags.ContainsKey(key) && (General.Map.Config.ThingFlagsCompare[requiredgroup][key].invert ? !t1.Flags[key] : t1.Flags[key])) 
						t1hasrequiredflags = true;
					if(t2.Flags.ContainsKey(key) && (General.Map.Config.ThingFlagsCompare[requiredgroup][key].invert ? !t2.Flags[key] : t2.Flags[key]))
						t2hasrequiredflags = true;
				}

				// Can't compare...
				if (!t1hasrequiredflags || !t2hasrequiredflags) return 0;
			}

			//mxd. We should ignore the flag if requiredflag is not set
			if(!string.IsNullOrEmpty(requiredflag))
			{
				bool inverted = General.Map.Config.ThingFlagsCompare[group].ContainsKey(requiredflag) && General.Map.Config.ThingFlagsCompare[group][requiredflag].invert;

				bool t1hasrequiredflag = inverted ? !t1.Flags[requiredflag] : t1.Flags[requiredflag];
				bool t2hasrequiredflag = inverted ? !t2.Flags[requiredflag] : t2.Flags[requiredflag];

				// Can't compare...
				if(!t1hasrequiredflag || !t2hasrequiredflag) return 0;
			}

			//mxd. We should also ignore the flag if it's in ingoredgroup
			foreach(KeyValuePair<string, Dictionary<string, ThingFlagsCompare>> pair in General.Map.Config.ThingFlagsCompare)
			{
				foreach(KeyValuePair<string, ThingFlagsCompare> flaggrp in pair.Value)
				{
					if (!string.IsNullOrEmpty(flaggrp.Value.ignoredgroup) && group == flaggrp.Value.ignoredgroup)
					{
						bool t1ignoreflagset = flaggrp.Value.invert ? !t1.Flags[flaggrp.Key] : t1.Flags[flaggrp.Key];
						bool t2ignoreflagset = flaggrp.Value.invert ? !t2.Flags[flaggrp.Key] : t2.Flags[flaggrp.Key];

						// Can't compare...
						if(!t1ignoreflagset || !t2ignoreflagset) return 0;
					}	
				}
			}
			
			// Take flag inversion into account
			t1flag = invert ? !t1.Flags[flag] : t1.Flags[flag];
			t2flag = invert ? !t2.Flags[flag] : t2.Flags[flag];

			if (comparemethod == CompareMethod.And && (t1flag && t2flag)) return 1;
			if (comparemethod == CompareMethod.Equal && (t1flag == t2flag)) return 1;
			return 0;
		}

		//mxd
		public static string CheckThingEditFormFlags(List<CheckBox> checkboxes)
		{
			Dictionary<string, bool> flags = new Dictionary<string, bool>(checkboxes.Count);
			Dictionary<string, Dictionary<string, bool>> flagspergroup = new Dictionary<string, Dictionary<string, bool>>(General.Map.Config.ThingFlagsCompare.Count);
			Dictionary<string, bool> requiredgroups = new Dictionary<string, bool>();
			Dictionary<string, bool> ignoredgroups = new Dictionary<string, bool>();

			// Gather flags
			foreach (CheckBox cb in checkboxes)
			{
				flags.Add(cb.Tag.ToString(), cb.CheckState == CheckState.Checked);
			}

			// Gather flags per group
			foreach (KeyValuePair<string, Dictionary<string, ThingFlagsCompare>> group in General.Map.Config.ThingFlagsCompare)
			{
				flagspergroup.Add(group.Key, new Dictionary<string, bool>());

				foreach (KeyValuePair<string, ThingFlagsCompare> flaggrp in group.Value)
				{
					bool flagset = IsFlagSet(flags, flaggrp.Key, flaggrp.Value.invert) && (string.IsNullOrEmpty(flaggrp.Value.requiredflag) || IsFlagSet(flags, flaggrp.Value.requiredflag, group.Value[flaggrp.Value.requiredflag].invert));

					if(flagset)
					{
						flagspergroup[group.Key].Add(flaggrp.Key, true);

						if(!string.IsNullOrEmpty(flaggrp.Value.requiredgroup) && !requiredgroups.ContainsKey(flaggrp.Value.requiredgroup))
							requiredgroups.Add(flaggrp.Value.requiredgroup, false);
					} 
					else if(flaggrp.Value.ingnorethisgroupwhenunset)
					{
						ignoredgroups.Add(group.Key, false);
					}
				}
			}

			// Check dependancies
			foreach (KeyValuePair<string, Dictionary<string, bool>> group in flagspergroup)
			{
				foreach(KeyValuePair<string, bool> flaggrp in group.Value)
				{
					if(!flaggrp.Value) continue;

					string ignoredgrp = General.Map.Config.ThingFlagsCompare[group.Key][flaggrp.Key].ignoredgroup;
					if (!string.IsNullOrEmpty(ignoredgrp) && !requiredgroups.ContainsKey(ignoredgrp))
					{
						ignoredgroups.Add(ignoredgrp, false);
					}
				}
			}

			// Get rid of ignoredgroups
			foreach (KeyValuePair<string, bool> group in ignoredgroups)
			{
				flagspergroup.Remove(group.Key);
			}

			// Return message
			string result = string.Empty;

			foreach (KeyValuePair<string, Dictionary<string, bool>> group in flagspergroup)
			{
				if (group.Value.Count == 0)
				{
					switch(group.Key) 
					{
						case "skills":
							result += "Thing is not used in any skill level.";
							break;
						case "gamemodes":
							result += "Thing is not used in any game mode.";
							break;
						case "classes":
							result += "Thing is not used by any class.";
							break;
						default:
							result += "At least one '" + group.Key + "' flag should be set.";
							break;
					}
				}
			}

			return result;
		}

		//mxd
		private static bool IsFlagSet(Dictionary<string, bool> flags, string flag, bool invert)
		{
			bool result = flags.ContainsKey(flag) && flags[flag];
			return (invert ? !result : result);
		}

		#endregion
	}
}

