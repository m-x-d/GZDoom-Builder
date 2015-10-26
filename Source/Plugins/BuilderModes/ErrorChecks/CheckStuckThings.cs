
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Config;
using System.Threading;
using System.Drawing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[ErrorChecker("Check stuck things", true, 1000)]
	public class CheckStuckThings : ErrorChecker
	{
		#region ================== Constants

		private const int PROGRESS_STEP = 10;
		private const float ALLOWED_STUCK_DISTANCE = 6.0f;

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public CheckStuckThings()
		{
			// Total progress is done when all things are checked
			SetTotalProgress(General.Map.Map.Things.Count / PROGRESS_STEP);
		}
		
		#endregion
		
		#region ================== Methods
		
		// This runs the check
		public override void Run()
		{
			BlockMap<BlockEntry> blockmap = BuilderPlug.Me.ErrorCheckForm.BlockMap;
			int progress = 0;
			int stepprogress = 0;
			float maxradius = 0;
			Dictionary<int, Dictionary<int, bool>> processedthingpairs = new Dictionary<int, Dictionary<int, bool>>(); //mxd

			foreach (ThingTypeInfo tti in General.Map.Data.ThingTypes)
			{
				if (tti.Radius > maxradius) maxradius = tti.Radius;
			}

			// Go for all the things
			foreach(Thing t in General.Map.Map.Things)
			{
				ThingTypeInfo info = General.Map.Data.GetThingInfo(t.Type);
				bool stuck = false;

				// Check this thing for getting stuck?
				if( (info.ErrorCheck == ThingTypeInfo.THING_ERROR_INSIDE_STUCK) &&
					(info.Blocking > ThingTypeInfo.THING_BLOCKING_NONE))
				{
					// Make square coordinates from thing
					float blockingsize = t.Size - ALLOWED_STUCK_DISTANCE;
					Vector2D lt = new Vector2D(t.Position.x - blockingsize, t.Position.y - blockingsize);
					Vector2D rb = new Vector2D(t.Position.x + blockingsize, t.Position.y + blockingsize);
					Vector2D bmlt = new Vector2D(t.Position.x - maxradius, t.Position.y - maxradius);
					Vector2D bmrb = new Vector2D(t.Position.x + maxradius, t.Position.y + maxradius);

					// Go for all the lines to see if this thing is stuck
					List<BlockEntry> blocks = blockmap.GetSquareRange(new RectangleF(bmlt.x, bmlt.y, (bmrb.x - bmlt.x), (bmrb.y - bmlt.y)));
					Dictionary<Linedef, Linedef> doneblocklines = new Dictionary<Linedef, Linedef>(blocks.Count * 3);

					foreach(BlockEntry b in blocks)
					{
						foreach(Linedef l in b.Lines)
						{
							// Only test when sinlge-sided, two-sided + impassable and not already checked
							if(((l.Back == null) || l.IsFlagSet(General.Map.Config.ImpassableFlag)) && !doneblocklines.ContainsKey(l))
							{
								// Test if line ends are inside the thing
								if(PointInRect(lt, rb, l.Start.Position) || PointInRect(lt, rb, l.End.Position))
								{
									// Thing stuck in line!
									stuck = true;
									SubmitResult(new ResultStuckThingInLine(t, l));
								}
								// Test if the line intersects the square
								else if(Line2D.GetIntersection(l.Start.Position, l.End.Position, lt.x, lt.y, rb.x, lt.y) ||
										Line2D.GetIntersection(l.Start.Position, l.End.Position, rb.x, lt.y, rb.x, rb.y) ||
										Line2D.GetIntersection(l.Start.Position, l.End.Position, rb.x, rb.y, lt.x, rb.y) ||
										Line2D.GetIntersection(l.Start.Position, l.End.Position, lt.x, rb.y, lt.x, lt.y))
								{
									// Thing stuck in line!
									stuck = true;
									SubmitResult(new ResultStuckThingInLine(t, l));
								}
								
								// Checked
								doneblocklines.Add(l, l);
							}
						}

						// Check if thing is stuck in other things
						if(info.Blocking != ThingTypeInfo.THING_BLOCKING_NONE) 
						{
							foreach (Thing ot in b.Things)
							{
								// Don't compare the thing with itself
								if (t.Index == ot.Index) continue;

								// mxd. Don't compare already processed stuff
								if (processedthingpairs.ContainsKey(t.Index) && processedthingpairs[t.Index].ContainsKey(ot.Index)) continue;

								// Only check of items that can block
								if (General.Map.Data.GetThingInfo(ot.Type).Blocking == ThingTypeInfo.THING_BLOCKING_NONE) continue;

								// need to compare the flags
								if (FlagsOverlap(t, ot) && ThingsOverlap(t, ot))
								{
									stuck = true;
									
									//mxd. Prepare collection
									if(!processedthingpairs.ContainsKey(t.Index)) processedthingpairs.Add(t.Index, new Dictionary<int, bool>());
									if(!processedthingpairs.ContainsKey(ot.Index)) processedthingpairs.Add(ot.Index, new Dictionary<int, bool>());

									//mxd. Add both ways
									processedthingpairs[t.Index].Add(ot.Index, false);
									processedthingpairs[ot.Index].Add(t.Index, false);

									SubmitResult(new ResultStuckThingInThing(t, ot));
								}
							}
						}
					}
				}

				// Check this thing for being outside the map?
				if(!stuck && info.ErrorCheck >= ThingTypeInfo.THING_ERROR_INSIDE) 
				{
					// Get the nearest line to see if the thing is outside the map
					bool outside;
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
		
		// Point in rect?
		private static bool PointInRect(Vector2D lt, Vector2D rb, Vector2D p)
		{
			return (p.x >= lt.x) && (p.x <= rb.x) && (p.y <= lt.y) && (p.y >= rb.y);
		}

		// Checks if two things overlap
		private static bool ThingsOverlap(Thing t1, Thing t2)
		{
			Vector3D p1 = t1.Position;
			Vector3D p2 = t2.Position;
			ThingTypeInfo t1info = General.Map.Data.GetThingInfo(t1.Type);
			ThingTypeInfo t2info = General.Map.Data.GetThingInfo(t2.Type);
	
			// simple bounding box collision detection
			if (	p1.x + t1.Size - ALLOWED_STUCK_DISTANCE < p2.x - t2.Size + ALLOWED_STUCK_DISTANCE ||
					p1.x - t1.Size + ALLOWED_STUCK_DISTANCE > p2.x + t2.Size - ALLOWED_STUCK_DISTANCE ||
					p1.y - t1.Size + ALLOWED_STUCK_DISTANCE > p2.y + t2.Size - ALLOWED_STUCK_DISTANCE ||
					p1.y + t1.Size - ALLOWED_STUCK_DISTANCE < p2.y - t2.Size + ALLOWED_STUCK_DISTANCE)
				return false;

			// if either thing blocks full height there's no need to check the z-axis
			if (t1info.Blocking == ThingTypeInfo.THING_BLOCKING_FULL || t2info.Blocking == ThingTypeInfo.THING_BLOCKING_FULL)
				return true;

			// check z-axis
			if (p1.z > p2.z + t2info.Height || p1.z + t1info.Height < p2.z)
				return false;

			return true;
		}
		
		// Checks if the flags of two things overlap (i.e. if they show up at the same time)
		private static bool FlagsOverlap(Thing t1, Thing t2) 
		{
			if(General.Map.Config.ThingFlagsCompare.Count < 1) return true; //mxd. Otherwise, no things will ever overlap when ThingFlagsCompare is empty
			int overlappinggroups = 0;
			int totalgroupscount = General.Map.Config.ThingFlagsCompare.Count; //mxd. Some groups can be ignored when unset...

			// Go through all flags in all groups and check if they overlap
			foreach(KeyValuePair<string, Dictionary<string, ThingFlagsCompare>> group in General.Map.Config.ThingFlagsCompare) 
			{
				foreach(ThingFlagsCompare tfc in group.Value.Values)
				{
					int compareresult = tfc.Compare(t1, t2); //mxd
					if(compareresult > 0) 
					{
						overlappinggroups++;
						break;
					}

					//mxd. Some groups can be ignored when unset...
					if(compareresult == 0 && tfc.IgnoreGroupWhenUnset)
					{
						totalgroupscount--;
					}
				}
			}

			// All groups have to overlap for the things to show up at the same time
			return (totalgroupscount > 0 && overlappinggroups == totalgroupscount);
		}
		
		#endregion
	}
}
