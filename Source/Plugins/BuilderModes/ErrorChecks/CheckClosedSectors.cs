
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
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;
using System.Threading;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[ErrorChecker("Check closed sectors", true, 300)]
	public class CheckClosedSectors : ErrorChecker
	{
		#region ================== Constants

		private const int PROGRESS_STEP = 40;

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public CheckClosedSectors()
		{
			// Total progress is done when all sectors are checked
			SetTotalProgress(General.Map.Map.Sectors.Count / PROGRESS_STEP);
		}
		
		#endregion
		
		#region ================== Methods
		
		// This runs the check
		public override void Run()
		{
			int progress = 0;
			int stepprogress = 0;
			
			// Go for all the sectors
			foreach(Sector s in General.Map.Map.Sectors)
			{
				// Continue until all sidedefs have been traced
				List<Vertex> foundholes = new List<Vertex>(2);
				Dictionary<Sidedef, Sidedef> donesides = new Dictionary<Sidedef, Sidedef>(s.Sidedefs.Count);
				foreach(Sidedef sd in s.Sidedefs)
				{
					if(!donesides.ContainsKey(sd))
					{
						// Trace the closest path starting at sd
						Sidedef traceside = sd;
						Vertex startvertex = (sd.IsFront) ? sd.Line.Start : sd.Line.End;
						while(traceside != null)
						{
							// Mark this sidedef as done
							donesides.Add(traceside, traceside);
							
							Vertex other = (startvertex == sd.Line.Start) ? sd.Line.End : sd.Line.Start;
							List<Sidedef> nextsides = new List<Sidedef>(other.Linedefs.Count * 2);
							bool foundsides = false;
							foreach(Linedef l in other.Linedefs)
							{
								// Should we go along the front or back side?
								if(l.Start == other)
								{
									// Front side of line connected to sector?
									if((l.Front != null) && (l.Front.Sector == s))
									{
										// Add this sidedef
										foundsides = true;
										if(!donesides.ContainsKey(l.Front)) nextsides.Add(l.Front);
									}
								}
								else
								{
									// Back side of line connected to sector?
									if((l.Back != null) && (l.Back.Sector == s))
									{
										// Add this sidedef
										foundsides = true;
										if(!donesides.ContainsKey(l.Back)) nextsides.Add(l.Back);
									}
								}
							}
							
							// Check if we can't go any further
							if(!foundsides)
							{
								// This is where the sector is broken
								foundholes.Add(other);
								break;
							}
							else if(nextsides.Count > 1)
							{
								// This is done to ensure the tracing works along vertices that are shared by
								// more than 2 lines/sides of the same sector. We must continue tracing along
								// the first next smallest delta angle! This sorts the smallest delta angle to
								// the top of the list.
								SidedefAngleSorter sorter = new SidedefAngleSorter(traceside, other);
								nextsides.Sort(sorter);
							}
							
							if(nextsides.Count > 0)
							{
								// Set next sidedef to trace
								traceside = nextsides[0];
								startvertex = other;
							}
							else
							{
								// Nothing more to trace from here, leave
								traceside = null;
							}
						}
					}
				}
				
				// Add report when holes have been found
				if(foundholes.Count > 0)
					SubmitResult(new ResultSectorUnclosed(s, foundholes));
				
				// Handle thread interruption
				try { Thread.Sleep(0); }
				catch(ThreadInterruptedException) { return; }

				// We are making progress!
				if((++progress / PROGRESS_STEP) > stepprogress)
				{
					stepprogress = (progress / PROGRESS_STEP);
					AddProgress(1);
				}
			}
		}
		
		#endregion
	}
}
