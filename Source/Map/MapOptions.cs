
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
using CodeImp.DoomBuilder.IO;
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.Map
{
	internal class MapOptions
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Game configuration
		private string configfile;
		
		// Map header name
		private string currentname;
		private string previousname;
		
		// Additional resources
		private List<ResourceLocation> resources;

		#endregion

		#region ================== Properties

		public string ConfigFile { get { return configfile; } set { configfile = value; } }
		public ICollection<ResourceLocation> Resources { get { return resources; } }
		public string PreviousName { get { return previousname; } }
		public string CurrentName
		{
			get { return currentname; }

			set
			{
				// Change the name, but keep previous name
				if(currentname != value)
				{
					if(previousname == "") previousname = currentname;
					currentname = value;
				}
			}
		}
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public MapOptions()
		{
			// Initialize
			this.previousname = "";
			this.currentname = "";
			this.configfile = "";
			this.resources = new List<ResourceLocation>();
		}

		~MapOptions()
		{
			// Clean up
			this.resources = null;
		}
		
		#endregion

		#region ================== Methods

		// This adds a resource location and returns the index where the item was added
		public int AddResource(ResourceLocation res)
		{
			// Get a fully qualified path
			res.location = Path.GetFullPath(res.location);
			
			// Go for all items in the list
			for(int i = 0; i < resources.Count; i++)
			{
				// Check if location is already added
				if(Path.GetFullPath(resources[i].location) == res.location)
				{
					// Update the item in the list
					resources[i] = res;
					return i;
				}
			}

			// Add to list
			resources.Add(res);
			return resources.Count - 1;
		}

		// This clears all reasource
		public void ClearResources()
		{
			// Clear list
			resources.Clear();
		}
		
		// This removes a resource by index
		public void RemoveResource(int index)
		{
			// Remove the item
			resources.RemoveAt(index);
		}
		
		#endregion
	}
}
