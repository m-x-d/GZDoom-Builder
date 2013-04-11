
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using System.Drawing;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Thing Action", BrowseButton = true)]
	internal class FindThingAction : FindReplaceType
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

		// Constructor
		public FindThingAction()
		{
			// Initialize

		}

		// Destructor
		~FindThingAction()
		{
		}

		#endregion

		#region ================== Methods

		// This is called to test if the item should be displayed
		public override bool DetermineVisiblity()
		{
			return General.Map.FormatInterface.HasThingAction;
		}


		// This is called when the browse button is pressed
		public override string Browse(string initialvalue)
		{
			int action;
			int.TryParse(initialvalue, out action);
			action = General.Interface.BrowseLinedefActions(BuilderPlug.Me.FindReplaceForm, action);
			return action.ToString();
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
			int findaction = 0;
			int[] args = null;
			string[] parts = value.Split(';');
			bool match;
			string argtext;
			
			//For the search, the user may make the following query:
			//	action;arg0,arg1,arg2,arg3,arg4
			//
			//this allows users to search for things that contain actions with specific arguments.
			//useful for locating enemies that trigger a certain script
			//
			//Since the Thing object does not contain a reference to arg0str, this search cannot match named scripts
			

			if (int.TryParse(parts[0], out findaction))
			{
				//parse the arg value out
				if (parts.Length > 1)
				{
					args = new int[] { 0, 0, 0, 0, 0 };
					string[] argparts = parts[1].Split(',');
					int argout;
					for (int i = 0; i < argparts.Length && i < args.Length; i++)
					{
						if (int.TryParse(argparts[i], out argout))
						{
							args[i] = argout;
						}
					}
				}

				// Where to search?
				ICollection<Thing> list = withinselection ? General.Map.Map.GetSelectedThings(true) : General.Map.Map.Things;

				// Go for all things
				foreach(Thing t in list)
				{
					// Match?
					if(t.Action == findaction)
					{
						match = true;
						argtext = "";

						//if args were specified, then process them
						if (args != null) {
							argtext = " args: (";
							for (int x = 0; x < args.Length; x++)
							{
								if (args[x] != 0 && args[x] != t.Args[x]) {
									match = false;
									break;
								}
								argtext += (x == 0 ? "" : ",") + t.Args[x].ToString();
							}
							argtext += ")";
						}

						if (match)
						{
							// Replace
							if (replacewith != null) t.Action = replaceaction;

							// Add to list
							ThingTypeInfo ti = General.Map.Data.GetThingInfo(t.Type);
							objs.Add(new FindReplaceObject(t, "Thing " + t.Index + " (" + ti.Title + ")" + argtext));
						}
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
				General.Interface.ShowThingInfo(selection[0].Thing);
			}
			else
				General.Interface.HideInfo();

			General.Map.Map.ClearAllSelected();
			foreach(FindReplaceObject obj in selection) obj.Thing.Selected = true;
		}

		// Render selection
		public override void RenderThingsSelection(IRenderer2D renderer, FindReplaceObject[] selection)
		{
			foreach(FindReplaceObject o in selection)
			{
				renderer.RenderThing(o.Thing, General.Colors.Selection, 1.0f);
			}
		}

		// Edit objects
		public override void EditObjects(FindReplaceObject[] selection)
		{
			List<Thing> things = new List<Thing>(selection.Length);
			foreach(FindReplaceObject o in selection) things.Add(o.Thing);
			General.Interface.ShowEditThings(things);
		}

		#endregion
	}
}
