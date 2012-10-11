using System;
using System.Collections.Generic;
using System.Text;

using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;

namespace CodeImp.DoomBuilder.PropertiesDock {
    internal static class MapElementsData {

        internal static Dictionary<int, string> ThingTypeDescriptions { get { return thingTypeDescriptions; } }
        private static Dictionary<int, string> thingTypeDescriptions;

        internal static void Init() {
            //thing types
            thingTypeDescriptions = new Dictionary<int, string>();

            foreach (ThingCategory tc in General.Map.Data.ThingCategories) {
                foreach (ThingTypeInfo ti in tc.Things) {
                    thingTypeDescriptions.Add(ti.Index, ti.Title);
                }
            }
        }

        internal static void InitTypes(TypeHandlerAttribute[] types) {

        }
    }
}
