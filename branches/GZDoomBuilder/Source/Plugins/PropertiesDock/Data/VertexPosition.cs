using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace CodeImp.DoomBuilder.PropertiesDock {

    /*[TypeConverterAttribute(typeof(VertexPositionConverter)), DescriptionAttribute("Vertex position.")]
    public class VertexPosition {
        [DefaultValueAttribute(0f)]
        public float X { get { return x; } set { x = value; } }
        private float x;

        [DefaultValueAttribute(0f)]
        public float Y { get { return y; } set { y = value; } }
        private float y;

        public VertexPosition(float x, float y) {
            this.x = x;
            this.y = y;
        }
    }

    public class VertexPositionConverter : ExpandableObjectConverter {
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType) {
            if (destinationType == typeof(VertexPosition))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType) {
            if (destinationType == typeof(System.String) && value is VertexPosition) {
                VertexPosition tp = (VertexPosition)value;
                return "X:" + tp.X + ", Y: " + tp.Y;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType) {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value is string) {
                try {
                    string s = (string)value;
                    int colon = s.IndexOf(':');
                    int comma = s.IndexOf(',');

                    if (colon != -1 && comma != -1) {
                        string px = s.Substring(colon + 1, (comma - colon - 1));
                        colon = s.IndexOf(':', comma + 1);

                        if (colon != -1) {
                            string py = s.Substring(colon + 1, s.Length - (colon + 1));
                            VertexPosition tp = new VertexPosition(float.Parse(px), float.Parse(py));
                            return tp;
                        }
                    }
                } catch {
                    throw new ArgumentException("Can not convert '" + (string)value + "' to type TestPosition");
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }*/
}
