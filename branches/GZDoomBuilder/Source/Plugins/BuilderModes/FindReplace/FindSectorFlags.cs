#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Sector Flags", BrowseButton = true)]
	internal class FindSectorFlags : BaseFindSector
	{
		#region ================== Properties

		public override Image BrowseImage { get { return Properties.Resources.List; } }

		#endregion

		#region ================== Methods

		// This is called to test if the item should be displayed
		public override bool DetermineVisiblity() 
		{
			return General.Map.Config.SectorFlags.Count > 0;
		}

		// This is called when the browse button is pressed
		public override string Browse(string initialvalue)
		{
			return FlagsForm.ShowDialog(Form.ActiveForm, initialvalue, General.Map.Config.SectorFlags);
		}

		// This is called to perform a search (and replace)
		// Returns a list of items to show in the results list
		// replacewith is null when not replacing
		public override FindReplaceObject[] Find(string value, bool withinselection, bool replace, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Where to search?
			ICollection<Sector> list = withinselection ? General.Map.Map.GetSelectedSectors(true) : General.Map.Map.Sectors;

			// Find what? (mxd)
			List<string> findflagslist = new List<string>();
			foreach(string flag in value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) 
			{
				string f = flag.Trim();
				if(General.Map.Config.SectorFlags.ContainsKey(f)) findflagslist.Add(f);
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
					if(!General.Map.Config.SectorFlags.ContainsKey(f))
					{
						MessageBox.Show("Invalid replace value '" + f + "' for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return objs.ToArray();
					}
					replaceflagslist.Add(f);
				}
			}

			// Go for all linedefs
			foreach(Sector s in list)
			{
				bool match = true;

				// Parse the value string...
				foreach(string flag in findflagslist)
				{
					// ... and check if the flags don't match
					if(!s.IsFlagSet(flag))
					{
						match = false;
						break;
					}
				}

				// Flags matches?
				if(match)
				{
					// Replace flags (mxd)
					if(replace) 
					{
						// Clear all flags
						s.ClearFlags();

						// Set new flags
						foreach(string flag in replaceflagslist) s.SetFlag(flag, true);
					}
					
					// Add to list
					SectorEffectInfo info = General.Map.Config.GetSectorEffectInfo(s.Effect);
					if(!info.IsNull)
						objs.Add(new FindReplaceObject(s, "Sector " + s.Index + " (" + info.Title + ")"));
					else
						objs.Add(new FindReplaceObject(s, "Sector " + s.Index));
				}
			}

			return objs.ToArray();
		}

		#endregion
	}
}
