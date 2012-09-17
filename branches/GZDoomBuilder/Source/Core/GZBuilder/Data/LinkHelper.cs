using System;
using System.Collections.Generic;
using System.Text;
//using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.GZBuilder.Geometry;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Config;

namespace CodeImp.DoomBuilder.GZBuilder.Data {
    
    public static class LinksCollector {
        private struct ThingsCheckResult {
            public bool ProcessPathNodes;
            public bool ProcessInterpolationPoints;
            public bool ProcessThingsWithGoal;
            public bool ProcessCameras;
        }
        
        public static List<Line3D> GetThingLinks(ICollection<VisualThing> visualThings) {
            List<Thing> things = new List<Thing>();
            foreach (VisualThing vt in visualThings) things.Add(vt.Thing);

            ThingsCheckResult result = checkThings(things);
            if (result.ProcessPathNodes || result.ProcessInterpolationPoints || result.ProcessThingsWithGoal || result.ProcessCameras)
                return getThingLinks(result, true);
            return new List<Line3D>();
        }
        
        public static List<Line3D> GetThingLinks(ICollection<Thing> things) {
            ThingsCheckResult result = checkThings(things);
            if (result.ProcessPathNodes || result.ProcessInterpolationPoints || result.ProcessThingsWithGoal || result.ProcessCameras)
                return getThingLinks(result, false);
            return new List<Line3D>();
        }

        private static ThingsCheckResult checkThings(ICollection<Thing> things) {
            ThingsCheckResult result = new ThingsCheckResult();

            foreach (Thing t in things) {
                if (t.Type == 9024) //zdoom path node
                    result.ProcessPathNodes = true;
                else if (t.Type == 9070) //zdoom camera interpolation point
                    result.ProcessInterpolationPoints = true;
                else if (t.Action == 229 && t.Args[1] != 0) //Thing_SetGoal
                    result.ProcessThingsWithGoal = true;
                else if (t.Type == 9072 && (t.Args[0] != 0 || t.Args[1] != 0)) //camera with a path
                    result.ProcessCameras = true;
            }

            return result;
        }

        private static List<Line3D> getThingLinks(ThingsCheckResult result, bool correctHeight) {
            List<Line3D> lines = new List<Line3D>();
            Dictionary<int, Thing> pathNodes = new Dictionary<int, Thing>();
            Dictionary<int, Thing> interpolationPoints = new Dictionary<int, Thing>();
            List<Thing> thingsWithGoal = new List<Thing>();
            List<Thing> cameras = new List<Thing>();

            bool getPathNodes = result.ProcessPathNodes || result.ProcessThingsWithGoal;
            bool getInterpolationPoints = result.ProcessInterpolationPoints || result.ProcessCameras;

            //collect relevant things
            foreach (Thing t in General.Map.Map.Things) {
                if (getPathNodes && t.Type == 9024) {
                    pathNodes[t.Tag] = t;
                }
                if (getInterpolationPoints && t.Type == 9070) {
                    interpolationPoints[t.Tag] = t;
                }
                if (result.ProcessThingsWithGoal && t.Action == 229 && t.Args[1] != 0) {
                    thingsWithGoal.Add(t);
                }
                if (result.ProcessCameras && t.Type == 9072 && (t.Args[0] != 0 || t.Args[1] != 0)) {
                    cameras.Add(t);
                }
            }

            Vector3D start = new Vector3D();
            Vector3D end = new Vector3D();

            //process path nodes
            if (result.ProcessPathNodes) {
                foreach (KeyValuePair<int, Thing> group in pathNodes) {
                    if (group.Value.Args[0] == 0) continue; //no goal
                    if (pathNodes.ContainsKey(group.Value.Args[0])) {
                        start = group.Value.Position;
                        if (correctHeight) start.z += getCorrectHeight(group.Value);

                        end = pathNodes[group.Value.Args[0]].Position;
                        if (correctHeight) end.z += getCorrectHeight(pathNodes[group.Value.Args[0]]);

                        lines.Add(new Line3D(start, end));
                    }
                }
            }

            //process things with Thing_SetGoal
            if (result.ProcessThingsWithGoal) {
                foreach (Thing t in thingsWithGoal) {
                    if (pathNodes.ContainsKey(t.Args[1])) {
                        if (t.Args[0] == 0 || t.Args[0] == t.Tag) {
                            start = t.Position;
                            if (correctHeight) start.z += getCorrectHeight(t);

                            end = pathNodes[t.Args[1]].Position;
                            if (correctHeight) end.z += getCorrectHeight(pathNodes[t.Args[1]]);

                            lines.Add(new Line3D(start, end));
                        }
                    }
                }
            }

            //process interpolation points
            if (result.ProcessInterpolationPoints) {
                foreach (KeyValuePair<int, Thing> group in interpolationPoints) {
                    int targetTag = group.Value.Args[3] + group.Value.Args[4] * 256;
                    if (targetTag == 0) continue; //no goal

                    if (interpolationPoints.ContainsKey(targetTag)) {
                        start = group.Value.Position;
                        if (correctHeight) start.z += getCorrectHeight(group.Value);

                        end = interpolationPoints[targetTag].Position;
                        if (correctHeight) end.z += getCorrectHeight(interpolationPoints[targetTag]);

                        lines.Add(new Line3D(start, end));
                    }
                }
            }

            //process cameras
            if (result.ProcessCameras) {
                foreach (Thing t in cameras) {
                    int targetTag = t.Args[0] + t.Args[1] * 256;
                    if (targetTag == 0) continue; //no goal

                    if (interpolationPoints.ContainsKey(targetTag)) {
                            start = t.Position;
                            if (correctHeight) start.z += getCorrectHeight(t);

                            end = interpolationPoints[targetTag].Position;
                            if (correctHeight) end.z += getCorrectHeight(interpolationPoints[targetTag]);

                        lines.Add(new Line3D(start, end));
                    }
                }
            }

            return lines;
        }

        private static float getCorrectHeight(Thing thing) {
            ThingTypeInfo tti = General.Map.Data.GetThingInfo(thing.Type);
            float height = tti.Height / 2f;
            if (thing.Sector != null) height += thing.Sector.FloorHeight;
            return height;
        }
    }
}
