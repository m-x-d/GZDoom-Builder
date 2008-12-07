
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
	[ErrorChecker("Check for stucked things", true, 1000)]
	public class CheckStuckedThings : ErrorChecker
	{
		#region ================== Constants

		private const float ALLOWED_STUCK_DISTANCE = 6.0f;

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public CheckStuckedThings()
		{
			// Total progress is done when all things are checked
			SetTotalProgress(General.Map.Map.Things.Count);
		}
		
		#endregion
		
		#region ================== Methods
		
		// This runs the check
		public override void Run()
		{
			// Go for all the things
			foreach(Thing t in General.Map.Map.Things)
			{
				ThingTypeInfo info = General.Map.Config.GetThingInfo(t.Type);
				bool stucked = false;
				
				// Check this thing for getting stucked?
				if( (info.ErrorCheck == ThingTypeInfo.THING_ERROR_INSIDE_STUCKED) &&
					(info.Blocking > ThingTypeInfo.THING_BLOCKING_NONE))
				{
					// Make square coordinates from thing
					float blockingsize = t.Size - ALLOWED_STUCK_DISTANCE;
					Vector2D lt = new Vector2D(t.Position.x - blockingsize, t.Position.y - blockingsize);
					Vector2D rb = new Vector2D(t.Position.x + blockingsize, t.Position.y + blockingsize);
					
					// Go for all the lines to see if this thing is stucked
					foreach(Linedef l in General.Map.Map.Linedefs)
					{
						// Test only single-sided lines
						if(l.Back == null)
						{
							// Test if line ends are inside the thing
							if(PointInRect(lt, rb, l.Start.Position) ||
							   PointInRect(lt, rb, l.End.Position))
							{
								// Thing stucked in line!
								stucked = true;
							}
							// Test if the line intersects the square
							else if(Line2D.GetIntersection(l.Start.Position, l.End.Position, lt.x, lt.y, rb.x, lt.y) ||
									Line2D.GetIntersection(l.Start.Position, l.End.Position, rb.x, lt.y, rb.x, rb.y) ||
									Line2D.GetIntersection(l.Start.Position, l.End.Position, rb.x, rb.y, lt.x, rb.y) ||
									Line2D.GetIntersection(l.Start.Position, l.End.Position, lt.x, rb.y, lt.x, lt.y))
							{
								// Thing stucked in line!
								stucked = true;
							}
						}
					}
				}
				
				// Stucked?
				if(stucked)
				{
					// Make result
					SubmitResult(new ResultStuckedThing(t));
				}
				else
				{
					// Check this thing for being outside the map?
					if(info.ErrorCheck >= ThingTypeInfo.THING_ERROR_INSIDE)
					{
						// Get the nearest line to see if the thing is outside the map
						bool outside = false;
						Linedef l = General.Map.Map.NearestLinedef(t.Position);
						if(l.SideOfLine(t.Position) <= 0)
						{
							outside = (l.Front == null);
						}
						else
						{
							outside = (l.Back == null);
						}

						// Outside the map?
						if(outside)
						{
							// Make result
							SubmitResult(new ResultThingOutside(t));
						}
					}
				}
				
				// Handle thread interruption
				try { Thread.Sleep(0); }
				catch(ThreadInterruptedException) { return; }
				
				// We are making progress!
				AddProgress(1);
			}
		}
		
		// Point in rect?
		private bool PointInRect(Vector2D lt, Vector2D rb, Vector2D p)
		{
			return (p.x >= lt.x) && (p.x <= rb.x) && (p.y >= lt.y) && (p.y <= rb.y);
		}
		
		#endregion
	}
}
