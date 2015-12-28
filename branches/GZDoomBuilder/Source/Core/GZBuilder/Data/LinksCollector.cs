using System.Collections.Generic;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.GZBuilder.Geometry;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.VisualModes;

namespace CodeImp.DoomBuilder.GZBuilder.Data 
{
	public static class LinksCollector 
	{
		private class SpecialThings 
		{
			public readonly Dictionary<int, List<Thing>> PatrolPoints; // PatrolPoint tag, list of PatrolPoints
			public readonly Dictionary<int, List<PathNode>> InterpolationPoints; // InterpolationPoint tag, list of InterpolationPoints
			public readonly List<Thing> ThingsWithGoal;
			public readonly List<Thing> Cameras;
			public readonly Dictionary<int, List<Thing>> ActorMovers; // ActorMover target tag, list of ActorMovers
			public readonly List<Thing> PathFollowers;
			public readonly Dictionary<int, List<Thing>> PolyobjectAnchors; //angle, list of PolyobjectAnchors
			public readonly Dictionary<int, List<Thing>> PolyobjectStartSpots; //angle, list of PolyobjectStartSpots

			public SpecialThings()
			{
				PatrolPoints = new Dictionary<int, List<Thing>>();
				InterpolationPoints = new Dictionary<int, List<PathNode>>();
				ThingsWithGoal = new List<Thing>();
				Cameras = new List<Thing>();
				ActorMovers = new Dictionary<int, List<Thing>>();
				PathFollowers = new List<Thing>();
				PolyobjectAnchors = new Dictionary<int, List<Thing>>();
				PolyobjectStartSpots = new Dictionary<int, List<Thing>>();
			}
		}

		private class PathNode
		{
			private readonly Thing thing;
			private readonly Vector3D position;
			private readonly Dictionary<int, PathNode> nextnodes;
			private readonly Dictionary<int, PathNode> prevnodes;

			public Thing Thing { get { return thing; } }
			public Dictionary<int, PathNode> NextNodes { get { return nextnodes; } } // Thing index, PathNode
			public Dictionary<int, PathNode> PreviousNodes { get { return prevnodes; } } // Thing index, PathNode
			public Vector3D Position { get { return position; } }
			public bool IsCurved;

			public PathNode(Thing t, VisualBlockMap blockmap)
			{
				thing = t;
				position = new Vector3D(t.Position, (blockmap != null ? t.Position.z + GetCorrectHeight(t, blockmap) : t.Position.z));
				nextnodes = new Dictionary<int, PathNode>();
				prevnodes = new Dictionary<int, PathNode>();
			}

			internal void PropagateCurvedFlag()
			{
				if(!IsCurved) return;
				foreach(PathNode node in nextnodes.Values)
				{
					if(node.IsCurved) continue;
					node.IsCurved = true;
					node.PropagateCurvedFlag();
				}
				foreach(PathNode node in prevnodes.Values)
				{
					if(node.IsCurved) continue;
					node.IsCurved = true;
					node.PropagateCurvedFlag();
				}
			}
		}

		public static List<Line3D> GetThingLinks(IEnumerable<Thing> things) { return GetThingLinks(things, null); }
		public static List<Line3D> GetThingLinks(IEnumerable<Thing> things, VisualBlockMap blockmap) 
		{
			return GetThingLinks(GetSpecialThings(things, blockmap), blockmap);
		}

		private static SpecialThings GetSpecialThings(IEnumerable<Thing> things, VisualBlockMap blockmap) 
		{
			SpecialThings result = new SpecialThings();

			// Process oh so special things
			foreach(Thing t in things)
			{
				ThingTypeInfo info = General.Map.Data.GetThingInfoEx(t.Type);
				if(info == null) continue;
				switch(info.ClassName.ToLowerInvariant())
				{
					case "patrolpoint":
						if(t.Tag != 0 || t.Args[0] != 0)
						{
							if(!result.PatrolPoints.ContainsKey(t.Tag)) result.PatrolPoints.Add(t.Tag, new List<Thing>());
							result.PatrolPoints[t.Tag].Add(t);
						}
						break;

					case "$polyanchor":
						if(!result.PolyobjectAnchors.ContainsKey(t.AngleDoom)) result.PolyobjectAnchors[t.AngleDoom] = new List<Thing>();
						result.PolyobjectAnchors[t.AngleDoom].Add(t);
						break;

					case "$polyspawn":
					case "$polyspawncrush":
					case "$polyspawnhurt":
						if(!result.PolyobjectStartSpots.ContainsKey(t.AngleDoom)) result.PolyobjectStartSpots[t.AngleDoom] = new List<Thing>();
						result.PolyobjectStartSpots[t.AngleDoom].Add(t);
						break;
				}

				// Process Thing_SetGoal action
				if(t.Action != 0 
					&& General.Map.Config.LinedefActions.ContainsKey(t.Action) 
					&& General.Map.Config.LinedefActions[t.Action].Id.ToLowerInvariant() == "thing_setgoal"
					&& (t.Args[0] == 0 || t.Args[0] == t.Tag)
					&& t.Args[1] != 0)
				{
					result.ThingsWithGoal.Add(t);
				}
			}

			// We may need all of these actors...
			foreach(Thing t in General.Map.ThingsFilter.VisibleThings)
			{
				ThingTypeInfo info = General.Map.Data.GetThingInfoEx(t.Type);
				if(info == null) continue;
				switch(info.ClassName.ToLowerInvariant())
				{
					case "interpolationpoint":
						if(!result.InterpolationPoints.ContainsKey(t.Tag)) result.InterpolationPoints.Add(t.Tag, new List<PathNode>());
						result.InterpolationPoints[t.Tag].Add(new PathNode(t, blockmap));
						break;

					case "movingcamera":
						if(t.Args[0] != 0 || t.Args[1] != 0) result.Cameras.Add(t);
						break;

					case "pathfollower":
						if(t.Args[0] != 0 || t.Args[1] != 0) result.PathFollowers.Add(t);
						break;

					case "actormover":
						if((t.Args[0] != 0 || t.Args[1] != 0) && t.Args[3] != 0)
						{
							if(!result.ActorMovers.ContainsKey(t.Args[3])) result.ActorMovers.Add(t.Args[3], new List<Thing>());
							result.ActorMovers[t.Args[3]].Add(t);
						}
						break;
				}
			}

			return result;
		}

		private static List<Line3D> GetThingLinks(SpecialThings result, VisualBlockMap blockmap) 
		{
			var lines = new List<Line3D>();
			var actormovertargets = new Dictionary<int, List<Thing>>();

			// Get ActorMover targets
			if(result.ActorMovers.Count > 0)
			{
				foreach(Thing t in General.Map.Map.Things) 
				{
					if(t.Tag == 0 || !result.ActorMovers.ContainsKey(t.Tag)) continue;
					if(!actormovertargets.ContainsKey(t.Tag)) actormovertargets[t.Tag] = new List<Thing>();
					actormovertargets[t.Tag].Add(t);
				}
			}

			Vector3D start, end;

			// Process patrol points
			foreach(KeyValuePair<int, List<Thing>> group in result.PatrolPoints) 
			{
				foreach(Thing t in group.Value) 
				{
					if(!result.PatrolPoints.ContainsKey(t.Args[0])) continue;
					
					start = t.Position;
					start.z += GetCorrectHeight(t, blockmap);

					foreach(Thing tt in result.PatrolPoints[t.Args[0]])
					{
						end = tt.Position;
						end.z += GetCorrectHeight(tt, blockmap);
						lines.Add(new Line3D(start, end));
					}
				}
			}

			// Process things with Thing_SetGoal
			foreach(Thing t in result.ThingsWithGoal) 
			{
				if(!result.PatrolPoints.ContainsKey(t.Args[1])) continue;

				start = t.Position;
				start.z += GetCorrectHeight(t, blockmap);

				foreach(Thing tt in result.PatrolPoints[t.Args[1]]) 
				{
					end = tt.Position;
					end.z += GetCorrectHeight(tt, blockmap);

					lines.Add(new Line3D(start, end, General.Colors.Selection));
				}
			}

			// Process cameras [CAN USE INTERPOLATION]
			foreach(Thing t in result.Cameras)
			{
				int targettag = t.Args[0] + (t.Args[1] << 8);
				if(targettag == 0 || !result.InterpolationPoints.ContainsKey(targettag)) continue; //no target / target desn't exist
				bool interpolatepath = ((t.Args[2] & 1) != 1);

				start = t.Position;
				start.z += GetCorrectHeight(t, blockmap);

				foreach(PathNode node in result.InterpolationPoints[targettag]) 
				{
					node.IsCurved = interpolatepath;
					lines.Add(new Line3D(start, node.Position, General.Colors.Selection));
				}
			}

			//process actor movers [CAN USE INTERPOLATION]
			foreach(List<Thing> things in result.ActorMovers.Values) 
			{
				foreach(Thing t in things)
				{
					int targettag = t.Args[0] + (t.Args[1] << 8);

					// Add interpolation point targets
					if(targettag != 0 && result.InterpolationPoints.ContainsKey(targettag))
					{
						bool interpolatepath = ((t.Args[2] & 1) != 1);
						start = t.Position;
						start.z += GetCorrectHeight(t, blockmap);

						foreach(PathNode node in result.InterpolationPoints[targettag])
						{
							node.IsCurved = interpolatepath;
							lines.Add(new Line3D(start, node.Position, General.Colors.Selection));
						}
					}

					// Add thing-to-move targets
					if(actormovertargets.ContainsKey(t.Args[3]))
					{
						start = t.Position;
						start.z += GetCorrectHeight(t, blockmap);

						foreach(Thing tt in actormovertargets[t.Args[3]])
						{
							end = tt.Position;
							end.z += GetCorrectHeight(tt, blockmap);
							lines.Add(new Line3D(start, end, General.Colors.Selection));
						}
					}
				}
			}

			// Process path followers [CAN USE INTERPOLATION]
			foreach(Thing t in result.PathFollowers)
			{
				int targettag = t.Args[0] + (t.Args[1] << 8);
				if(targettag == 0 || !result.InterpolationPoints.ContainsKey(targettag)) continue; //no target / target desn't exist
				bool interpolatepath = (t.Args[2] & 1) != 1;

				start = t.Position;
				start.z += GetCorrectHeight(t, blockmap);

				foreach(PathNode node in result.InterpolationPoints[targettag])
				{
					node.IsCurved = interpolatepath;
					lines.Add(new Line3D(start, node.Position, General.Colors.Selection));
				}
			}

			// Process polyobjects
			foreach(KeyValuePair<int, List<Thing>> group in result.PolyobjectAnchors)
			{
				if(!result.PolyobjectStartSpots.ContainsKey(group.Key)) continue;
				foreach(Thing anchor in group.Value)
				{
					start = anchor.Position;
					start.z += GetCorrectHeight(anchor, blockmap);

					foreach(Thing startspot in result.PolyobjectStartSpots[group.Key]) 
					{
						end = startspot.Position;
						end.z += GetCorrectHeight(startspot, blockmap);
						lines.Add(new Line3D(start, end, General.Colors.Selection));
					}
				}
			}

			// Process interpolation points [CAN BE INTERPOLATED]
			// 1. Connect PathNodes
			foreach(KeyValuePair<int, List<PathNode>> group in result.InterpolationPoints)
			{
				foreach(PathNode node in group.Value)
				{
					int targettag = node.Thing.Args[3] + (node.Thing.Args[4] << 8);
					if(targettag == 0 || !result.InterpolationPoints.ContainsKey(targettag)) continue;

					foreach(PathNode targetnode in result.InterpolationPoints[targettag])
					{
						// Connect both ways
						if(!node.NextNodes.ContainsKey(targetnode.Thing.Index)) node.NextNodes.Add(targetnode.Thing.Index, targetnode);
						if(!targetnode.PreviousNodes.ContainsKey(node.Thing.Index)) targetnode.PreviousNodes.Add(node.Thing.Index, node);
					}
				}
			}

			// 2. Propagate IsCurved flag
			foreach(KeyValuePair<int, List<PathNode>> group in result.InterpolationPoints)
			{
				foreach(PathNode node in group.Value) node.PropagateCurvedFlag();
			}

			// 3. Make lines
			HashSet<int> processedindices = new HashSet<int>();
			foreach(KeyValuePair<int, List<PathNode>> group in result.InterpolationPoints)
			{
				foreach(PathNode node in group.Value)
				{
					// Draw as a curve?
					if(node.IsCurved && !processedindices.Contains(node.Thing.Index) && node.NextNodes.Count > 0 && node.PreviousNodes.Count > 0)
					{
						PathNode prev = General.GetByIndex(node.PreviousNodes, 0).Value;
						PathNode next = General.GetByIndex(node.NextNodes, 0).Value;
						if(next.NextNodes.Count > 0)
						{
							PathNode nextnext = General.GetByIndex(next.NextNodes, 0).Value;
								
								// Generate curve points
								List<Vector3D> points = new List<Vector3D>(11);
								for(int i = 0; i < 11; i++)
								{
									float u = i * 0.1f;
									points.Add(new Vector3D(
										SplineLerp(u, prev.Position.x, node.Position.x, next.Position.x, nextnext.Position.x),
										SplineLerp(u, prev.Position.y, node.Position.y, next.Position.y, nextnext.Position.y),
										(blockmap == null ? 0 : SplineLerp(u, prev.Position.z, node.Position.z, next.Position.z, nextnext.Position.z))
									));
								}

								// Add line segments
								for(int i = 1; i < 11; i++)
								{
									lines.Add(new Line3D(points[i - 1], points[i], i == 10));
								}

								continue;
						}
					}

					// Draw regular lines
					bool startnode = (node.IsCurved && node.PreviousNodes.Count == 0); // When using curves, this node won't be used by camera (the last node won't be used as well), so draw them using different color
					foreach(PathNode targetnode in node.NextNodes.Values)
					{
						bool isskipped = (startnode || (targetnode.IsCurved && targetnode.NextNodes.Count == 0));
						lines.Add(new Line3D(node.Position, targetnode.Position, (isskipped ? General.Colors.Highlight : General.Colors.InfoLine), !isskipped));
					}
				}
			}

			return lines;
		}

		// Taken from Xabis' "curved interpolation points paths" patch.
		private static float SplineLerp(float u, float p1, float p2, float p3, float p4)
		{
			float t2 = u;
			float res = 2 * p2;
			res += (p3 - p1) * u;
			t2 *= u;
			res += (2 * p1 - 5 * p2 + 4 * p3 - p4) * t2;
			t2 *= u;
			res += (3 * p2 - 3 * p3 + p4 - p1) * t2;
			return 0.5f * res;
		}

		// Required only when called from VisualMode
		private static float GetCorrectHeight(Thing thing, VisualBlockMap blockmap)
		{
			if(blockmap == null) return 0f;
			float height = thing.Height / 2f;
			if(thing.Sector == null) thing.DetermineSector(blockmap);
			if(thing.Sector != null) height += thing.Sector.FloorHeight;
			return height;
		}
	}
}
