
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
using System.Reflection;
using System.Diagnostics;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	internal abstract class MapSetIO : IMapSetIO
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// WAD File
		protected WAD wad;

		// Map manager
		protected MapManager manager;

		#endregion

		#region ================== Properties

		public abstract int MaxSidedefs { get; }
		public abstract int VertexDecimals { get; }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal MapSetIO(WAD wad, MapManager manager)
		{
			// Initialize
			this.wad = wad;
			this.manager = manager;
		}
		
		#endregion

		#region ================== Static Methods

		// This returns and instance of the specified IO class
		public static MapSetIO Create(string classname, WAD wadfile, MapManager manager)
		{
			object[] args;
			MapSetIO result;
			string fullname;
			
			try
			{
				// Create arguments
				args = new object[2];
				args[0] = wadfile;
				args[1] = manager;
				
				// Make the full class name
				fullname = "CodeImp.DoomBuilder.IO." + classname;
				
				// Create IO class
				result = (MapSetIO)General.ThisAssembly.CreateInstance(fullname, false,
					BindingFlags.Default, null, args, CultureInfo.CurrentCulture, new object[0]);

				// Check result
				if(result != null)
				{
					// Success
					return result;
				}
				else
				{
					// No such class
					throw new ArgumentException("No such map format interface found: \"" + classname + "\"");
				}
			}
			// Catch errors
			catch(TargetInvocationException e)
			{
				// Throw the actual exception
				Debug.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
				Debug.WriteLine(e.InnerException.Source + " throws " + e.InnerException.GetType().Name + ":");
				Debug.WriteLine(e.InnerException.Message);
				Debug.WriteLine(e.InnerException.StackTrace);
				throw e.InnerException;
			}
		}
		
		#endregion
		
		#region ================== Methods

		// Required implementations
		public abstract MapSet Read(MapSet map, string mapname);
		public abstract void Write(MapSet map, string mapname, int position);
		
		#endregion
	}
}
