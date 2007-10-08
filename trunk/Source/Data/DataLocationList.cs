using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.IO;
using System.Collections.Specialized;

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
					if(rlinfo.Contains("textures") && (rlinfo["textures"] is bool)) res.textures = (bool)rlinfo["textures"];
					if(rlinfo.Contains("flats") && (rlinfo["flats"] is bool)) res.flats = (bool)rlinfo["flats"];

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
				rlinfo.Add("textures", this[i].textures);
				rlinfo.Add("flats", this[i].flats);

				// Add structure
				resinfo.Add("resource" + i.ToString(CultureInfo.InvariantCulture), rlinfo);
			}
			
			// Write to config
			cfg.WriteSetting(path, resinfo);
		}
		
		#endregion
	}
}
