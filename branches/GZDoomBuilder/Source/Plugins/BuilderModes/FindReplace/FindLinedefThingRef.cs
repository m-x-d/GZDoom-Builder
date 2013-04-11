
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

using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Linedef Thing Reference", BrowseButton = false)]
	internal class FindLinedefThingRef : FindReplaceType
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public FindLinedefThingRef()
		{
			// Initialize

		}

		// Destructor
		~FindLinedefThingRef()
		{
		}

		#endregion

		#region ================== Methods

		// This is called to test if the item should be displayed
		public override bool DetermineVisiblity()
		{
			return General.Map.FormatInterface.HasActionArgs;
		}


		// This is called when the browse button is pressed
		public override string Browse(string initialvalue)
		{
			return "";
		}


		// This is called to perform a search (and replace)
		// Returns a list of items to show in the results list
		// replacewith is null when not replacing
		public override FindReplaceObject[] Find(string value, bool withinselection, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Interpret the replacement
			int replacetag = 0;
			if(replacewith != null)
			{
				// If it cannot be interpreted, set replacewith to null (not replacing at all)
				if(!int.TryParse(replacewith, out replacetag)) replacewith = null;
				if(replacetag < 0) replacewith = null;
				if(replacetag > 255) replacewith = null;
				if(replacewith == null)
				{
					MessageBox.Show("Invalid replace value for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return objs.ToArray();
				}
			}

			// Interpret the number given
			int tag = 0;
			if(int.TryParse(value, out tag))
			{
				// Where to search?
				ICollection<Linedef> list = withinselection ? General.Map.Map.GetSelectedLinedefs(true) : General.Map.Map.Linedefs;

				// Go for all linedefs
				foreach(Linedef l in list)
				{
					bool addline = false;

					LinedefActionInfo info = General.Map.Config.GetLinedefActionInfo(l.Action);
					if(info.IsKnown && !info.IsNull)
					{
						// Go for all args
						for(int i = 0; i < Linedef.NUM_ARGS; i++)
						{
							// Argument type matches?
							if(info.Args[i].Used && (info.Args[i].Type == (int)UniversalType.ThingTag))
							{
								if(l.Args[i] == tag)
								{
									// Replace
									if(replacewith != null) l.Args[i] = replacetag;
									addline = true;
								}
							}
						}
					}

					if(addline)
					{
						// Add to list
						if(!info.IsNull)
							objs.Add(new FindReplaceObject(l, "Linedef " + l.Index + " (" + info.Title + ")"));
						else
							objs.Add(new FindReplaceObject(l, "Linedef " + l.Index));
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
				General.Interface.ShowLinedefInfo(selection[0].Linedef);
			}
			else
				General.Interface.HideInfo();

			General.Map.Map.ClearAllSelected();
			foreach(FindReplaceObject obj in selection) obj.Linedef.Selected = true;
		}

		// Render selection
		public override void PlotSelection(IRenderer2D renderer, FindReplaceObject[] selection)
		{
			foreach(FindReplaceObject o in selection)
			{
				renderer.PlotLinedef(o.Linedef, General.Colors.Selection);
			}
		}

		// Edit objects
		public override void EditObjects(FindReplaceObject[] selection)
		{
			List<Linedef> lines = new List<Linedef>(selection.Length);
			foreach(FindReplaceObject o in selection) lines.Add(o.Linedef);
			General.Interface.ShowEditLinedefs(lines);
		}

		#endregion
	}
}
