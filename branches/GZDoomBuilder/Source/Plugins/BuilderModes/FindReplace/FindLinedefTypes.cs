
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
using CodeImp.DoomBuilder.GZBuilder;
using CodeImp.DoomBuilder.Map;
using System.Drawing;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Linedef Action and Arguments", BrowseButton = true)]
	internal class FindLinedefTypes : BaseFindLinedef
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private List<int> generalizedbits;

		#endregion

		#region ================== Properties

		public override Image BrowseImage { get { return Properties.Resources.List; } }
		
		#endregion

		#region ================== Constructor / Destructor

		//mxd
		public FindLinedefTypes() 
		{
			if (!General.Map.Config.GeneralizedActions) return;

			// Get all them generalized bits
			generalizedbits = new List<int>();
			foreach(GeneralizedCategory cat in General.Map.Config.GenActionCategories) 
			{
				foreach(GeneralizedOption option in cat.Options) 
				{
					foreach(GeneralizedBit bit in option.Bits) 
					{
						if(bit.Index > 0) generalizedbits.Add(bit.Index);
					}
				}
			}
		}

		#endregion

		#region ================== Methods

		// This is called when the browse button is pressed
		public override string Browse(string initialvalue)
		{
			int num;
			int.TryParse(initialvalue, out num);
			return General.Interface.BrowseLinedefActions(BuilderPlug.Me.FindReplaceForm, num).ToString();
		}


		// This is called to perform a search (and replace)
		// Returns a list of items to show in the results list
		// replacewith is null when not replacing
		public override FindReplaceObject[] Find(string value, bool withinselection, bool replace, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Interpret the replacement
			int replaceaction = 0;
			string replacearg0str = string.Empty; //mxd
			int[] replaceargs = null; //mxd
			if(replace)
			{
				string[] replaceparts = replacewith.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				
				// If it cannot be interpreted, set replacewith to null (not replacing at all)
				if(replaceparts.Length == 0) replacewith = null; //mxd
				if(!int.TryParse(replaceparts[0], out replaceaction)) replacewith = null;
				if(replaceaction < 0) replacewith = null;
				if(replaceaction > Int16.MaxValue) replacewith = null;
				if(replacewith == null)
				{
					MessageBox.Show("Invalid replace value for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return objs.ToArray();
				}

				//mxd. Now try parsing the args
				if(replaceparts.Length > 1)
				{
					replaceargs = new[] { int.MinValue, int.MinValue, int.MinValue, int.MinValue, int.MinValue };
					int i = 1;

					//mxd. Named script search support...
					if(General.Map.UDMF && Array.IndexOf(GZGeneral.ACS_SPECIALS, replaceaction) != -1) 
					{
						string possiblescriptname = replaceparts[1].Trim().Replace("\"", "").ToLowerInvariant();
						int tmp;
						if(!string.IsNullOrEmpty(possiblescriptname) && possiblescriptname != "*" && !int.TryParse(possiblescriptname, out tmp)) 
						{
							replacearg0str = possiblescriptname;
							i = 2;
						}
					}

					for(; i < replaceparts.Length && i < replaceargs.Length + 1; i++) 
					{
						int argout;
						if(replaceparts[i].Trim() == "*") continue; //mxd. Any arg value support
						if(int.TryParse(replaceparts[i].Trim(), out argout)) replaceargs[i - 1] = argout;
					}
				}
			}

			// Interpret the number given
			int action;
			string[] parts = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			//For the search, the user may make the following queries:
			//	action arg0 arg1 arg2 arg3 arg4
			//	action arg0str arg1 arg2 arg3 arg4
			//
			//this allows users to search for lines that contain actions with specific arguments.
			//useful for locating script lines

			if(int.TryParse(parts[0], out action))
			{
				int[] args = null;
				string arg0str = string.Empty; //mxd

				//parse the arg values out
				if(parts.Length > 1) 
				{
					args = new[] { int.MinValue, int.MinValue, int.MinValue, int.MinValue, int.MinValue };
					int i = 1;

					//mxd. Named script search support...
					if(General.Map.UDMF && Array.IndexOf(GZGeneral.ACS_SPECIALS, action) != -1)
					{
						string possiblescriptname = parts[1].Trim().Replace("\"", "").ToLowerInvariant();
						int tmp;
						if(!string.IsNullOrEmpty(possiblescriptname) && possiblescriptname != "*" && !int.TryParse(possiblescriptname, out tmp))
						{
							arg0str = possiblescriptname;
							i = 2;
						}
					}

					for(; i < parts.Length && i < args.Length + 1; i++)
					{
						int argout;
						if(parts[i].Trim() == "*") continue; //mxd. Any arg value support
						if(int.TryParse(parts[i].Trim(), out argout)) args[i - 1] = argout;
					}
				}

				//mxd
				List<int> expectedbits = GetGeneralizedBits(action);

				// Where to search?
				ICollection<Linedef> list = withinselection ? General.Map.Map.GetSelectedLinedefs(true) : General.Map.Map.Linedefs;

				// Go for all linedefs
				foreach(Linedef l in list)
				{
					// Action matches?
					if(l.Action != action && !BitsMatch(l.Action, expectedbits)) continue;

					bool match = true;
					string argtext = "";

					//if args were specified, then process them
					if(args != null) 
					{
						int x = 0;
						argtext = " args: (";

						//mxd. Check script name...
						if(!string.IsNullOrEmpty(arg0str))
						{
							string s = l.Fields.GetValue("arg0str", string.Empty);
							if(s.ToLowerInvariant() != arg0str)
								match = false;
							else
								argtext += "\"" + s + "\"";

							x = 1;
						}

						for(; x < args.Length; x++)
						{
							if(args[x] != int.MinValue && args[x] != l.Args[x]) 
							{
								match = false;
								break;
							}
							argtext += (x == 0 ? "" : ",") + l.Args[x];
						}
						argtext += ")";
					}

					if(match) 
					{
						// Replace
						if(replace)
						{
							l.Action = replaceaction;

							//mxd. Replace args as well?
							if(replaceargs != null)
							{
								int i = 0;
								if(!string.IsNullOrEmpty(replacearg0str))
								{
									l.Fields["arg0str"] = new UniValue(UniversalType.String, replacearg0str);
									i = 1;
								}

								for(; i < replaceargs.Length; i++)
								{
									if(replaceargs[i] != int.MinValue) l.Args[i] = replaceargs[i];
								}
							}
						}

						// Add to list
						LinedefActionInfo info = General.Map.Config.GetLinedefActionInfo(l.Action);
						if(!info.IsNull)
							objs.Add(new FindReplaceObject(l, "Linedef " + l.Index + " (" + info.Title + ")" + argtext));
						else
							objs.Add(new FindReplaceObject(l, "Linedef " + l.Index + argtext));
					}
				}
			}

			return objs.ToArray();
		}

		//mxd
		private static List<int> GetGeneralizedBits(int effect) 
		{
			if(!General.Map.Config.GeneralizedActions) return new List<int>();
			List<int> bits = new List<int>();

			foreach(GeneralizedCategory cat in General.Map.Config.GenActionCategories) 
			{
				foreach(GeneralizedOption option in cat.Options) 
				{
					foreach(GeneralizedBit bit in option.Bits) 
					{
						if(bit.Index > 0 && (effect & bit.Index) == bit.Index)
							bits.Add(bit.Index);
					}
				}
			}

			return bits;
		}

		//mxd
		private static bool BitsMatch(int action, List<int> expectedbits) 
		{
			if(!General.Map.Config.GeneralizedActions) return false;

			foreach(int bit in expectedbits) 
			{
				if((action & bit) != bit) return false;
			}

			return true;
		}
		
		#endregion
	}
}
