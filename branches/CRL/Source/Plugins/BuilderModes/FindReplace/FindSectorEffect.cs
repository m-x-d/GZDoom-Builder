
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Sector Effect", BrowseButton = true)]
	internal class FindSectorEffect : FindReplaceType
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		public override Image BrowseImage { get { return Properties.Resources.List; } }
		
		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public FindSectorEffect()
		{
			// Initialize

		}

		// Destructor
		~FindSectorEffect()
		{
		}

		#endregion

		#region ================== Methods

		// This is called when the browse button is pressed
		public override string Browse(string initialvalue)
		{
			int effect;
			int.TryParse(initialvalue, out effect);
			effect = General.Interface.BrowseSectorEffect(BuilderPlug.Me.FindReplaceForm, effect);
			return effect.ToString();
		}


		// This is called to perform a search (and replace)
		// Returns a list of items to show in the results list
		// replacewith is null when not replacing
		public override FindReplaceObject[] Find(string value, bool withinselection, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Interpret the replacement
			int replaceeffect = 0;
			if(replacewith != null)
			{
				// If it cannot be interpreted, set replacewith to null (not replacing at all)
				if(!int.TryParse(replacewith, out replaceeffect)) replacewith = null;
				if(replaceeffect < 0) replacewith = null;
				if(replaceeffect > Int16.MaxValue) replacewith = null;
				if(replacewith == null)
				{
					MessageBox.Show("Invalid replace value for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return objs.ToArray();
				}
			}

			// Interpret the number given
			int effect = 0;
			if(int.TryParse(value, out effect))
			{
				// Where to search?
				ICollection<Sector> list = withinselection ? General.Map.Map.GetSelectedSectors(true) : General.Map.Map.Sectors;

				// Go for all sectors
				foreach(Sector s in list)
				{
					// Tag matches?
					if(s.Effect == effect)
					{
						// Replace
						if(replacewith != null) s.Effect = replaceeffect;
						
						SectorEffectInfo info = General.Map.Config.GetSectorEffectInfo(s.Effect);
						if(!info.IsNull)
							objs.Add(new FindReplaceObject(s, "Sector " + s.Index + " (" + info.Title + ")"));
						else
							objs.Add(new FindReplaceObject(s, "Sector " + s.Index));
					}
				}
			}

			return objs.ToArray();
		}

		// This is called when a specific object is selected from the list
		public override void ObjectSelected(FindReplaceObject[] selection)
		{
			if(selection.Length == 1)
			{
				ZoomToSelection(selection);
				General.Interface.ShowSectorInfo(selection[0].Sector);
			}
			else
				General.Interface.HideInfo();

			General.Map.Map.ClearAllSelected();
			foreach(FindReplaceObject obj in selection) obj.Sector.Selected = true;
		}

		// Render selection
		public override void PlotSelection(IRenderer2D renderer, FindReplaceObject[] selection)
		{
			foreach(FindReplaceObject o in selection)
			{
				foreach(Sidedef sd in o.Sector.Sidedefs)
				{
					renderer.PlotLinedef(sd.Line, General.Colors.Selection);
				}
			}
		}

		// Edit objects
		public override void EditObjects(FindReplaceObject[] selection)
		{
			List<Sector> sectors = new List<Sector>(selection.Length);
			foreach(FindReplaceObject o in selection) sectors.Add(o.Sector);
			General.Interface.ShowEditSectors(sectors);
		}

		#endregion
	}
}
