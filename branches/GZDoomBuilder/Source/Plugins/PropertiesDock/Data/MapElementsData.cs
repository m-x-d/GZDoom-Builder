using System;
using System.Collections.Generic;
using System.Text;

using CodeImp.DoomBuilder.Config;

namespace CodeImp.DoomBuilder.PropertiesDock {
    public static class MapElementsData {

        public static Dictionary<int, string> ThingTypeDescriptions { get { return thingTypeDescriptions;}}
        private static Dictionary<int, string> thingTypeDescriptions;
        
        public static void Init() {
            //thing types
            thingTypeDescriptions = new Dictionary<int, string>();

            foreach (ThingCategory tc in General.Map.Data.ThingCategories) {
                foreach (ThingTypeInfo ti in tc.Things) {
                    thingTypeDescriptions.Add(ti.Index, ti.Title);
                }
            }
        }
    }
}
