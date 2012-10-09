using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.PropertiesDock {
    public class VertexInfo : IMapElementInfo {
        [CategoryAttribute("Position"), DefaultValueAttribute(0f)]
        public float X { get { return x; } set { x = value; } }
        private float x;

        [CategoryAttribute("Position"), DefaultValueAttribute(0f)]
        public float Y { get { return y; } set { y = value; } }
        private float y;

        private Vertex vertex;

        public VertexInfo(Vertex v) {
            vertex = v;
            x = v.Position.x;
            y = v.Position.y;
        }

        public void ApplyChanges() {
            float min = (float)General.Map.FormatInterface.MinCoordinate;
            float max = (float)General.Map.FormatInterface.MaxCoordinate;
            vertex.Move(new CodeImp.DoomBuilder.Geometry.Vector2D(General.Clamp(x, min, max), General.Clamp(y, min, max)));
            
            //todo: add custom fields support
        }
    }
}
