
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
using CodeImp.DoomBuilder.Map;
using System.Drawing;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Linedef Actions", BrowseButton = true)]
	internal class FindLinedefTypes : BaseFindLinedef
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		public override Image BrowseImage { get { return Properties.Resources.List; } }
		
		#endregion

		#region ================== Constructor / Destructor

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
		public override FindReplaceObject[] Find(string value, bool withinselection, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Interpret the replacement
			int replaceaction = 0;
			if(replacewith != null)
			{
				// If it cannot be interpreted, set replacewith to null (not replacing at all)
				if(!int.TryParse(replacewith, out replaceaction)) replacewith = null;
				if(replaceaction < 0) replacewith = null;
				if(replaceaction > Int16.MaxValue) replacewith = null;
				if(replacewith == null)
				{
					MessageBox.Show("Invalid replace value for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return objs.ToArray();
				}
			}

			// Interpret the number given
			int action;
			int[] args = null;
			string[] parts = value.Split(';');
			bool match;
			string argtext;

			//For the search, the user may make the following query:
			//	action;arg0,arg1,arg2,arg3,arg4
			//
			//this allows users to search for lines that contain actions with specific arguments.
			//useful for locating script lines
			//
			//Since the Linedef object does not contain a reference to arg0str, this search cannot match named scripts

			if(int.TryParse(parts[0], out action))
			{
				//parse the arg value out
				if (parts.Length > 1) {
					args = new[] {0, 0, 0, 0, 0};
					string[] argparts = parts[1].Split(',');
					int argout;
					for(int i = 0; i < argparts.Length && i < args.Length; i++) {
						if (int.TryParse(argparts[i], out argout)) {
							args[i] = argout;
						}
					}
				}

				// Where to search?
				ICollection<Linedef> list = withinselection ? General.Map.Map.GetSelectedLinedefs(true) : General.Map.Map.Linedefs;

				// Go for all linedefs
				foreach(Linedef l in list)
				{
					// Action matches?
					if(l.Action == action)
					{
						match = true;
						argtext = "";

						//if args were specified, then process them
						if (args != null) {
							argtext = " args: (";
							for (int x = 0; x < args.Length; x++)
							{
								if (args[x] != 0 && args[x] != l.Args[x]) {
									match = false;
									break;
								}
								argtext += (x == 0 ? "" : ",") + l.Args[x];
							}
							argtext += ")";
						}

						if (match) {
							// Replace
							if (replacewith != null) l.Action = replaceaction;

							// Add to list
							LinedefActionInfo info = General.Map.Config.GetLinedefActionInfo(l.Action);
							if (!info.IsNull)
								objs.Add(new FindReplaceObject(l, "Linedef " + l.Index + " (" + info.Title + ")" + argtext));
							else
								objs.Add(new FindReplaceObject(l, "Linedef " + l.Index + argtext));
						}
					}
				}
			}

			return objs.ToArray();
		}
		
		#endregion
	}
}
