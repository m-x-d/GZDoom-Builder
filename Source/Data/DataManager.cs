
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	internal class DataManager : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Data containers
		private List<IDataReader> containers;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DataManager()
		{
			// Initialize
			containers = new List<IDataReader>();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				Unload();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Loading / Unloading

		// This loads all data resources
		public void Load(DataLocationList configlist, DataLocationList maplist, DataLocation maplocation)
		{
			DataLocationList all;

			// Create complete list
			all = DataLocationList.Combined(configlist, maplist);
			all.Add(maplocation);

			// Load resources
			Load(all);
		}

		// This loads all data resources
		public void Load(DataLocationList configlist, DataLocationList maplist)
		{
			DataLocationList all;

			// Create complete list
			all = DataLocationList.Combined(configlist, maplist);

			// Load resources
			Load(all);
		}

		// This loads all data resources
		public void Load(DataLocationList locations)
		{
			IDataReader c;
			
			// Go for all locations
			foreach(DataLocation dl in locations)
			{
				// Nothing chosen yet
				c = null;

				// Choose container type
				switch(dl.type)
				{
					// WAD file container
					case DataLocation.RESOURCE_WAD:
						c = new WADReader(dl);
						break;
						
					// Directory container
					case DataLocation.RESOURCE_DIRECTORY:
						c = new DirectoryReader(dl);
						break;
				}

				// Container type chosen?
				if(c != null)
				{
					// TODO: Let the container read stuff

					// Keep container reference
					containers.Add(c);
				}
			}
		}

		// This unloads all data
		public void Unload()
		{
			// Dispose containers
			foreach(IDataReader c in containers) c.Dispose();
			containers.Clear();
		}
		
		// This suspends a data resource location
		public void SuspendLocation(string location)
		{
			// Go for all containers
			foreach(IDataReader d in containers)
			{
				// Check if this is the location to suspend
				if(string.Compare(d.Location, location, true) == 0)
				{
					// Suspend
					General.WriteLogLine("Suspended data resource '" + location + "'");
					d.Suspend();
					return;
				}
			}
			
			General.WriteLogLine("WARNING: Cannot suspended data resource '" + location + "', no such location opened!");
		}

		// This resume a data resource location
		public void ResumeLocation(string location)
		{
			// Go for all containers
			foreach(IDataReader d in containers)
			{
				// Check if this is the location to resume
				if(string.Compare(d.Location, location, true) == 0)
				{
					// Resume
					General.WriteLogLine("Resumed data resource '" + location + "'");
					d.Resume();
					return;
				}
			}

			General.WriteLogLine("WARNING: Cannot resume data resource '" + location + "', no such location opened!");
		}
		
		#endregion
	}
}
