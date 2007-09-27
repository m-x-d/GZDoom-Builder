using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.IO;
using System.Collections.Specialized;

namespace CodeImp.DoomBuilder.Images
{
	internal class ResourceLocationList : List<ResourceLocation>
	{
		#region ================== Constructors

		// This creates a new list
		public ResourceLocationList()
		{
		}
		
		// This creates a list from a configuration structure
		public ResourceLocationList(Configuration cfg, string path)
		{
			IDictionary resinfo, rlinfo;
			ResourceLocation res;

			// Go for all items in the map info
			resinfo = cfg.ReadSetting(path, new ListDictionary());
			foreach(DictionaryEntry rl in resinfo)
			{
				// Item is a structure?
				if(rl.Value is IDictionary)
				{
					// Create resource location
					rlinfo = (IDictionary)rl.Value;
					res = new ResourceLocation();

					// Copy information from Configuration to ResourceLocation
					if(resinfo.Contains("type") && (resinfo["type"] is int)) res.type = (int)resinfo["type"];
					if(resinfo.Contains("location") && (resinfo["location"] is string)) res.location = (string)resinfo["location"];
					if(resinfo.Contains("textures") && (resinfo["textures"] is bool)) res.textures = (bool)resinfo["textures"];
					if(resinfo.Contains("flats") && (resinfo["flats"] is bool)) res.flats = (bool)resinfo["flats"];

					// Add resource
					Add(res);
				}
			}
		}
		
		#endregion

		#region ================== Methods

		// This merges two lists together
		public static ResourceLocationList Combined(ResourceLocationList a, ResourceLocationList b)
		{
			ResourceLocationList result = new ResourceLocationList();
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
