
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
using System.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal class UniversalMapSetIO : MapSetIO
	{
		#region ================== Constants

		// Name of the UDMF configuration file
		private const string UDMF_CONFIG_NAME = "UDMF.cfg";
		
		#endregion

		#region ================== Variables

		private Configuration config;
		
		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		public UniversalMapSetIO(WAD wad, MapManager manager) : base(wad, manager)
		{
			// Make configuration
			config = new Configuration();
			
			// Find a resource named Font.cfg
			string[] resnames = General.ThisAssembly.GetManifestResourceNames();
			foreach(string rn in resnames)
			{
				// Found it?
				if(rn.EndsWith(UDMF_CONFIG_NAME, StringComparison.InvariantCultureIgnoreCase))
				{
					// Get a stream from the resource
					Stream udmfcfg = General.ThisAssembly.GetManifestResourceStream(rn);
					StreamReader udmfcfgreader = new StreamReader(udmfcfg, Encoding.ASCII);
					
					// Load configuration from stream
					config.InputConfiguration(udmfcfgreader.ReadToEnd());

					// Done
					udmfcfgreader.Dispose();
					udmfcfg.Dispose();
					break;
				}
			}
		}

		#endregion

		#region ================== Properties

		public override int MaxSidedefs { get { return int.MaxValue; } }
		public override int VertexDecimals { get { return 3; } }
		public override string DecimalsFormat { get { return "0.000"; } }

		#endregion

		#region ================== Reading

		// This reads a map from the file and returns a MapSet
		public override MapSet Read(MapSet map, string mapname)
		{
			UniversalParser textmap = new UniversalParser();
			StreamReader lumpreader = null;
			
			// Find the index where first map lump begins
			int firstindex = wad.FindLumpIndex(mapname) + 1;
			
			// Get the TEXTMAP lump from wad file
			Lump lump = wad.FindLump("TEXTMAP", firstindex);
			if(lump == null) throw new Exception("Could not find required lump TEXTMAP!");

			try
			{
				// Parse the UDMF data
				lumpreader = new StreamReader(lump.Stream);
				textmap.InputConfiguration(lumpreader.ReadToEnd());

				// Check for errors
				if(textmap.ErrorResult != 0)
				{
					// Show parse error
					General.ShowErrorMessage("Error while parsing UDMF map data:\n" + textmap.ErrorDescription, MessageBoxButtons.OK);
				}
				else
				{

				}
			}
			catch(Exception e)
			{
				General.ShowErrorMessage("Unexpected error reading UDMF map data. " + e.GetType().Name + ": " + e.Message, MessageBoxButtons.OK);
			}
			finally
			{
				if(lumpreader != null) lumpreader.Dispose();
			}

			// Return result
			return map;
		}

		#endregion

		#region ================== Writing

		// This writes a MapSet to the file
		public override void Write(MapSet map, string mapname, int position)
		{

		}

		#endregion
	}
}
