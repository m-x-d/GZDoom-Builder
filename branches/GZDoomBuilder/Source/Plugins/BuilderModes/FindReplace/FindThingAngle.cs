
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
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Thing Angle", BrowseButton = true)]
	internal class FindThingAngle : FindReplaceType
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		public override Presentation RenderPresentation { get { return Presentation.Things; } }
		public override Image BrowseImage { get { return Properties.Resources.Angle; } }
		
		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public FindThingAngle()
		{
			// Initialize

		}

		// Destructor
		~FindThingAngle()
		{
		}

		#endregion

		#region ================== Methods

		// This is called when the browse button is pressed
		public override string Browse(string initialvalue)
		{
			int initangle;
			int.TryParse(initialvalue, out initangle);
			return AngleForm.ShowDialog(Form.ActiveForm, initangle).ToString();
		}


		// This is called to perform a search (and replace)
		// Returns a list of items to show in the results list
		// replacewith is null when not replacing
		public override FindReplaceObject[] Find(string value, bool withinselection, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Interpret the replacement
			int replaceangle = 0;
			if(replacewith != null)
			{
				// If it cannot be interpreted, set replacewith to null (not replacing at all)
				if(!int.TryParse(replacewith, out replaceangle)) replacewith = null;
				if(replacewith == null)
				{
					MessageBox.Show("Invalid replace value for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return objs.ToArray();
				}
			}

			// Interpret the number given
			int angle = 0;
			if(int.TryParse(value, out angle))
			{
				// Where to search?
				ICollection<Thing> list = withinselection ? General.Map.Map.GetSelectedThings(true) : General.Map.Map.Things;

				// Go for all things
				foreach(Thing t in list)
				{
					// Match?
					if(Angle2D.RealToDoom(t.Angle) == angle)
					{
						// Replace
						if(replacewith != null) t.Rotate(Angle2D.DoomToReal(replaceangle));

						// Add to list
						ThingTypeInfo ti = General.Map.Data.GetThingInfo(t.Type);
						objs.Add(new FindReplaceObject(t, "Thing " + t.Index + " (" + ti.Title + ")"));
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
