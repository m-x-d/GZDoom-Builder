
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
using System.Collections.Specialized;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal sealed class DataLocationList : List<DataLocation>
	{
		#region ================== Constructors

		// This creates a new list
		public DataLocationList()
		{
		}
		
		// This creates a list from a configuration structure
		public DataLocationList(Configuration cfg, string path)
		{
			IDictionary resinfo, rlinfo;
			DataLocation res;

			// Go for all items in the map info
			resinfo = cfg.ReadSetting(path, new ListDictionary());
			foreach(DictionaryEntry rl in resinfo)
			{
				// Item is a structure?
				if(rl.Value is IDictionary)
				{
					// Create resource location
					rlinfo = (IDictionary)rl.Value;
					res = new DataLocation();

					// Copy information from Configuration to ResourceLocation
					if(rlinfo.Contains("type") && (rlinfo["type"] is int)) res.type = (int)rlinfo["type"];
					if(rlinfo.Contains("location") && (rlinfo["location"] is string)) res.location = (string)rlinfo["location"];
					if(rlinfo.Contains("textures") && (rlinfo["textures"] is int)) res.textures = General.Int2Bool((int)rlinfo["textures"]);
					if(rlinfo.Contains("flats") && (rlinfo["flats"] is int)) res.flats = General.Int2Bool((int)rlinfo["flats"]);

					// Add resource
					Add(res);
				}
			}
		}
		
		#endregion

		#region ================== Methods

		// This merges two lists together
		public static DataLocationList Combined(DataLocationList a, DataLocationList b)
		{
			DataLocationList result = new DataLocationList();
			result.AddRange(a);
			result.AddRange(b);
			return result;
		}

		// This writes the list to configuration
		public void WriteToConfig(Configuration cfg, string path)
		{
			IDictionary resinfo, rlinfo;
			
			// Fill structure
			resinfo = new ListDictionary();
			for(int i = 0; i < this.Count; i++)
			{
				// Create structure for resource
				rlinfo = new ListDictionary();
				rlinfo.Add("type", this[i].type);
				rlinfo.Add("location", this[i].location);
				rlinfo.Add("textures", General.Bool2Int(this[i].textures));
				rlinfo.Add("flats", General.Bool2Int(this[i].flats));

				// Add structure
				resinfo.Add("resource" + i.ToString(CultureInfo.InvariantCulture), rlinfo);
			}
			
			// Write to config
			cfg.WriteSetting(path, resinfo);
		}
		
		#endregion
	}
}
