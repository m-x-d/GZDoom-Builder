using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.PropertiesDock {
    
    public class VertexInfo : PropertyBag, IMapElementInfo {
        /*[CategoryAttribute("Position"), DefaultValueAttribute(0f)]
        public float X { get { return x; } set { x = value; } }*/
        //private float x;

        /*[CategoryAttribute("Position"), DefaultValueAttribute(0f)]
        public float Y { get { return y; } set { y = value; } }*/
        //private float y;

        //public PropertyBag Properties { get { return properties; } }
        //private PropertyBag properties;

        private Vertex vertex;

        public VertexInfo(Vertex v) : base() {
            vertex = v;
            //x = v.Position.x;
            //y = v.Position.y;
            //properties = new PropertyBag();
            properties.Add(new PropertySpec("X:", typeof(float), "Position:", null, v.Position.x));
            properties.Add(new PropertySpec("Y:", typeof(float), "Position:", null, v.Position.y));
        }

        public void ApplyChanges() {
            float min = (float)General.Map.FormatInterface.MinCoordinate;
            float max = (float)General.Map.FormatInterface.MaxCoordinate;
            vertex.Move(new CodeImp.DoomBuilder.Geometry.Vector2D(General.Clamp((float)properties[0].Value, min, max), General.Clamp((float)properties[1].Value, min, max)));
            
            //todo: add custom fields support
        }

        public void AddCustomProperty(string name, Type type) {
            properties.Add(new PropertySpec(name + ":", type, "Custom properties:"));
        }

        public void RemoveCustomProperty(string name){
            string n = name.ToUpperInvariant().Trim();
            foreach (PropertySpec ps in properties) {
                string cn = ps.Name.ToUpperInvariant();
                if (cn.IndexOf(n) == 0 && cn.Length == n.Length + 1) {
                    properties.Remove(name);
                    return;
                }
            }
        }
    }
}
