
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

using System;
using SlimDX;

namespace CodeImp.DoomBuilder
{
	public class Clock
	{
		// Disposing
		private bool isdisposed = false;

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		// Constructor
		public Clock(){
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// Disposer
		public void Dispose(){
			// Not already disposed?
			if(!isdisposed) {
				isdisposed = true;
			}
		}

		// This queries the system for the current time
		public double GetCurrentTime(){
            return SlimDX.Configuration.Timer.ElapsedMilliseconds;
		}
	}
}
