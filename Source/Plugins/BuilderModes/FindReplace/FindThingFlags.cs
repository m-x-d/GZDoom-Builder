
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
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using System.Drawing;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Thing Flags", BrowseButton = true)]
	internal class FindThingFlag : BaseFindThing
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		public override Presentation RenderPresentation { get { return Presentation.Things; } }
		public override Image BrowseImage { get { return Properties.Resources.List; } }

		#endregion

		#region ================== Constructor / Destructor

		#endregion

		#region ================== Methods

		// This is called to test if the item should be displayed
		public override bool DetermineVisiblity() 
		{
			return General.Map.Config.ThingFlags.Count > 0;
		}

		// This is called when the browse button is pressed
		public override string Browse(string initialvalue)
		{
			return FlagsForm.ShowDialog(Form.ActiveForm, initialvalue, General.Map.Config.ThingFlags);
		}

		// This is called to perform a search (and replace)
		// Returns a list of items to show in the results list
		// replacewith is null when not replacing
		public override FindReplaceObject[] Find(string value, bool withinselection, bool replace, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Where to search?
			ICollection<Thing> list = withinselection ? General.Map.Map.GetSelectedThings(true) : General.Map.Map.Things;

			// Find what? (mxd)
			List<string> findflagslist = new List<string>();
			foreach(string flag in value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				string f = flag.Trim();
				if(General.Map.Config.ThingFlags.ContainsKey(f)) findflagslist.Add(f);
			}
			if(findflagslist.Count == 0) 
			{
				MessageBox.Show("Invalid value for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return objs.ToArray();
			}

			// Replace with what? (mxd)
			List<string> replaceflagslist = new List<string>();
			if(replace)
			{
				string[] replaceflags = replacewith.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				foreach(string flag in replaceflags) 
				{
					string f = flag.Trim();
					if(!General.Map.Config.ThingFlags.ContainsKey(f))
					{
						MessageBox.Show("Invalid replace value \"" + f + "\" for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return objs.ToArray();
					}
					replaceflagslist.Add(f);
				}
			}

			// Go for all things
			foreach(Thing t in list)
			{
				bool match = true;

				// Parse the value string...
				foreach(string flag in findflagslist)
				{
					// ... and check if the flags don't match
					if(!t.IsFlagSet(flag))
					{
						match = false;
						break;
					}
				}

				// Match?
				if(match)
				{
					// Replace flags (mxd)
					if(replace)
					{
						// Clear all flags
						t.ClearFlags();

						// Set new flags
						foreach(string flag in replaceflagslist) t.SetFlag(flag, true);
					}
					
					// Add to list
					ThingTypeInfo ti = General.Map.Data.GetThingInfo(t.Type);
					objs.Add(new FindReplaceObject(t, "Thing " + t.Index + " (" + ti.Title + ")"));
				}
			}

			return objs.ToArray();
		}

		#endregion
	}
}
