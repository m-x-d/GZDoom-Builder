
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
using System.Runtime.InteropServices;
using System.Diagnostics;

#endregion

namespace CodeImp.DoomBuilder
{
	public class Clock
	{
		#region ================== Declarations
		
		//#if !LINUX
		
		[DllImport("kernel32.dll")]
		private static extern short QueryPerformanceCounter(out long x);
		
		[DllImport("kernel32.dll")]
		private static extern short QueryPerformanceFrequency(out long x);
		
		//#endif

		#endregion
		
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Settings
		private double currenttime;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Settings
		public double CurrentTime { get { return currenttime; } }

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Clock()
		{
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

				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// This queries the system for the current time
		public double GetCurrentTime()
		{
			// Only windows has QPC
			//#if !LINUX

			long timefrequency;

			// Get the high resolution clock frequency
			if(QueryPerformanceFrequency(out timefrequency) == 0)
			{
				// No high resolution clock available
				currenttime = (double)Environment.TickCount;
			}
			else
			{
				long timecount;

				// Get the high resolution count
				QueryPerformanceCounter(out timecount);

				// Calculate high resolution time in milliseconds
				// TODO: It seems there is a loss of precision here when the
				// result of this math is assigned to currenttime, WHY?!
				currenttime = (double)timecount / (double)timefrequency * (double)1000.0;
			}
			
			/*
			#else
			
			// In LINUX always use standard clock
			currenttime = (double)Environment.TickCount;
			
			#endif
			*/

			// Return the current time
			return currenttime;
		}
		
		#endregion
	}
}
