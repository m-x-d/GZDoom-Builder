
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
using System.Collections.Generic;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using System.Drawing;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[FindReplace("Sector Effect", BrowseButton = true)]
	internal class FindSectorEffect : BaseFindSector
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		public override Image BrowseImage { get { return Properties.Resources.List; } }
		
		#endregion

		#region ================== Constructor / Destructor

		#endregion

		#region ================== Methods

		// This is called when the browse button is pressed
		public override string Browse(string initialvalue)
		{
			int effect;
			int.TryParse(initialvalue, out effect);
			return General.Interface.BrowseSectorEffect(BuilderPlug.Me.FindReplaceForm, effect, true).ToString();
		}

		//mxd. This is called when the browse replace button is pressed
		public override string BrowseReplace(string initialvalue)
		{
			int effect;
			int.TryParse(initialvalue, out effect);
			return General.Interface.BrowseSectorEffect(BuilderPlug.Me.FindReplaceForm, effect).ToString();
		}

		// This is called to perform a search (and replace)
		// Returns a list of items to show in the results list
		// replacewith is null when not replacing
		public override FindReplaceObject[] Find(string value, bool withinselection, bool replace, string replacewith, bool keepselection)
		{
			List<FindReplaceObject> objs = new List<FindReplaceObject>();

			// Interpret the replacement
			int replaceeffect = 0;
			if(replace)
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
			int effect;
			if(int.TryParse(value, out effect))
			{
				//mxd
				List<int> expectedbits = GetGeneralizedBits(effect);
				
				// Where to search?
				ICollection<Sector> list = withinselection ? General.Map.Map.GetSelectedSectors(true) : General.Map.Map.Sectors;

				// Go for all sectors
				foreach(Sector s in list)
				{
					// Effect matches? -1 means any effect (mxd)
					if((effect == -1 && s.Effect > 0) || (effect > -1 && (s.Effect == effect || BitsMatch(s.Effect, expectedbits))))
					{
						// Replace
						if(replace) s.Effect = replaceeffect;
						
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

		//mxd
		private static List<int> GetGeneralizedBits(int effect) 
		{
			if(!General.Map.Config.GeneralizedEffects) return new List<int>();
			List<int> bits = new List<int>();

			foreach(GeneralizedOption option in General.Map.Config.GenEffectOptions) 
			{
				foreach(GeneralizedBit bit in option.Bits) 
				{
					if(bit.Index > 0 && (effect & bit.Index) == bit.Index)
						bits.Add(bit.Index);
				}
			}

			return bits;
		}

		//mxd
		private static bool BitsMatch(int effect, List<int> expectedbits) 
		{
			if(!General.Map.Config.GeneralizedEffects) return false;

			foreach(int bit in expectedbits) 
			{
				if((effect & bit) != bit) return false;
			}

			return true;
		}

		#endregion
	}
}
