
#if WINDOWS
#define USE_GETTIME
#endif

#region ================== Copyright (c) 2007 Pascal vd Heiden

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
		
		#if USE_GETTIME

		[DllImport("winmm.dll")]
		private static extern uint timeGetTime();
		
		#endif

		#endregion
		
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Settings
		private uint lasttime;
		private uint currenttime;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Settings
		public double CurrentTime { get { return currenttime; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Clock()
		{
			GetCurrentTime();
			
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

		// This returns the current time in milliseconds as a uint
		private static uint GetTime()
		{
			#if USE_GETTIME

			// Use Windows' timeGetTime
			return timeGetTime();
			
			#else
			
			// Use standard clock
			return unchecked((uint)Environment.TickCount);
			
			#endif
		}
		
		// This queries the system for the current time
		public double GetCurrentTime()
		{
			// Get the current system time
			uint nexttime = GetTime();

			// Determine delta time since previous update
			// (this takes care of time wrapping around to 0)
			uint deltatime;
			if(nexttime < lasttime)
				deltatime = (uint.MaxValue - lasttime) + nexttime;
			else
				deltatime = nexttime - lasttime;

			// Add the elapsed time to our internal time
			currenttime += deltatime;

			lasttime = nexttime;

			return currenttime;
		}
		
		#endregion
	}
}
