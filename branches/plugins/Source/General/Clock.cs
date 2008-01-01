
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
		private static extern short QueryPerformanceCounter(ref long x);
		
		[DllImport("kernel32.dll")]
		private static extern short QueryPerformanceFrequency(ref long x);
		
		//#endif

		#endregion
		
		#region ================== Constants

		// Set to true enable QPC if possible
		private const bool USE_QPC = true;
		
		// Frequency indicating QPC unavailable
		private const long FREQ_NO_QPC = -1;
		
		#endregion

		#region ================== Variables

		// Settings
		private long timefrequency = FREQ_NO_QPC;
		private double timescale;
		private int currenttime;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Settings
		public bool IsUsingQPC { get { return (timefrequency != FREQ_NO_QPC); } }
		public int CurrentTime { get { return currenttime; } }

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Clock()
		{
			// Only windows has QPC
			//#if !LINUX
			if(Environment.OSVersion.Platform != PlatformID.Unix)
			{
				// Get the high resolution clock frequency
				if((QueryPerformanceFrequency(ref timefrequency) == 0) || !USE_QPC)
				{
					// No high resolution clock available
					timefrequency = FREQ_NO_QPC;
				}
				else
				{
					// Calculate the time scale
					timescale = (1d / (double)timefrequency) * 1000d;
				}
			}
			//#endif

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
		public int GetCurrentTime()
		{
			// Only windows has QPC
			//#if !LINUX
			
			long timecount = 0;

			// High resolution clock available?
			if(timefrequency != FREQ_NO_QPC)
			{
				// Get the high resolution count
				QueryPerformanceCounter(ref timecount);
				
				// Calculate high resolution time in milliseconds
				currenttime = (int)((double)timecount * timescale);
			}
			else
			{
				// Use standard clock
				currenttime = Environment.TickCount;
			}
			
			/*
			#else
			
			// In LINUX always use standard clock
			currenttime = Environment.TickCount;
			
			#endif
			*/

			// Return the current time
			return currenttime;
		}
		
		#endregion
	}
}
