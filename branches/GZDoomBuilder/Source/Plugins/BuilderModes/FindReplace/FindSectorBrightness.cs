using System.Collections.Generic;
using CodeImp.DoomBuilder.Map;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Rendering;

namespace CodeImp.DoomBuilder.BuilderModes.FindReplace
{
	[FindReplace("Sector Brightness", BrowseButton = false)]
	internal class FindSectorBrightness : FindReplaceType
	{
		// Constructor
		public FindSectorBrightness() {}

		// Destructor
		~FindSectorBrightness() {}

		// This is called to perform a search (and replace)
		// Returns a list of items to show in the results list
		// replacewith is null when not replacing
		public override FindReplaceObject[] Find(string value, bool withinselection, string replacewith, bool keepselection) {
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Interpret the replacement
			int replacebrightness = 0;
			if(replacewith != null) {
				// If it cannot be interpreted, set replacewith to null (not replacing at all)
				if(!int.TryParse(replacewith, out replacebrightness)) replacewith = null;
				if(replacebrightness < 0) replacewith = null;
				if(replacebrightness > 255) replacewith = null;
				if(replacewith == null) {
					MessageBox.Show("Invalid replace value for this search type!", "Find and Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return objs.ToArray();
				}
			}

			// Interpret the number given
			int brightness = 0;
			if(int.TryParse(value, out brightness)) {
				// Where to search?
				ICollection<Sector> list = withinselection ? General.Map.Map.GetSelectedSectors(true) : General.Map.Map.Sectors;

				// Go for all sectors
				foreach(Sector s in list) {
					// Brightness matches?
					if(s.Brightness == brightness) {
						// Replace
						if(replacewith != null) s.Brightness = replacebrightness;

						objs.Add(new FindReplaceObject(s, "Sector " + s.Index));
					}
				}
			}

			//refresh map
			if(replacewith != null) {
				General.Map.Map.Update();
				General.Map.IsChanged = true;
			}

			return objs.ToArray();
		}

		// This is called when a specific object is selected from the list
		public override void ObjectSelected(FindReplaceObject[] selection) {
			if(selection.Length == 1) {
				ZoomToSelection(selection);
				General.Interface.ShowSectorInfo(selection[0].Sector);
			} else
				General.Interface.HideInfo();

			General.Map.Map.ClearAllSelected();
			foreach(FindReplaceObject obj in selection) obj.Sector.Selected = true;
		}

		// Render selection
		public override void PlotSelection(IRenderer2D renderer, FindReplaceObject[] selection) {
			foreach(FindReplaceObject o in selection) {
				foreach(Sidedef sd in o.Sector.Sidedefs) {
					renderer.PlotLinedef(sd.Line, General.Colors.Selection);
				}
			}
		}

		// Edit objects
		public override void EditObjects(FindReplaceObject[] selection) {
			List<Sector> sectors = new List<Sector>(selection.Length);
			foreach(FindReplaceObject o in selection) sectors.Add(o.Sector);
			General.Interface.ShowEditSectors(sectors);
		}
	}
}
