using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.PropertiesDock {

    public class VertexInfo : CustomPropertiesCollection, IMapElementInfo {
        private Vertex vertex;

        public VertexInfo(Vertex v) : base() {
            vertex = v;
            Add(new CustomProperty("X:", v.Position.x, "Position:", false, true));
            Add(new CustomProperty("Y:", v.Position.y, "Position:", false, true));

            //todo: add custom fields
            if (v.Fields != null && v.Fields.Count > 0) {
                foreach (KeyValuePair<string, UniValue> group in v.Fields) {
                    Add(new CustomProperty(group.Key, group.Value.Value, "Custom properties:", false, true));
                }
            }
        }

        public void ApplyChanges() {
            float min = (float)General.Map.FormatInterface.MinCoordinate;
            float max = (float)General.Map.FormatInterface.MaxCoordinate;
            vertex.Move(new CodeImp.DoomBuilder.Geometry.Vector2D(General.Clamp((float)this[0].Value, min, max), General.Clamp((float)this[1].Value, min, max)));
            
            //todo: add custom fields support
        }

        public void AddCustomProperty(string name, Type type) {
            Add(new CustomProperty(name, Activator.CreateInstance(type), "Custom properties:", false, true));
        }

        public void RemoveCustomProperty(string name){
            string n = name.ToUpperInvariant().Trim();
            foreach (CustomProperty ps in this) {
                string cn = ps.Name.ToUpperInvariant();
                if (cn.IndexOf(n) == 0 && cn.Length == n.Length + 1) {
                    Remove(name);
                    return;
                }
            }
        }
    }
}
