using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using CodeImp.DoomBuilder.Map;
using System.Globalization;

namespace CodeImp.DoomBuilder.PropertiesDock {
    public class ThingInfo : IMapElementInfo {

        [TypeConverterAttribute(typeof(ThingTypeConverter)), CategoryAttribute("General"), DefaultValueAttribute(0)]
        public int Type { get { return type; } set { type = value; } }
        private int type;

        private Thing thing;

        public ThingInfo(Thing t) {
            thing = t;
            type = t.Type;
        }

        public void ApplyChanges() {

        }

        public void AddCustomProperty(string name, Type type) {
            //properties.Add(new PropertySpec(name + ":", value.GetType(), "Custom properties:"));
        }

        public void RemoveCustomProperty(string name) {
            /*string n = name.ToUpperInvariant().Trim();
            foreach (PropertySpec ps in properties) {
                string cn = ps.Name.ToUpperInvariant();
                if (cn.IndexOf(n) == 0 && cn.Length == n.Length + 1) {
                    properties.Remove(name);
                    return;
                }
            }*/
        }
    }

    public class ThingTypeConverter : TypeConverter {

        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType) {
            if (destinationType == typeof(int))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType) {
            if (destinationType == typeof(System.String) && value is int) {
                int type = (int)value;
                if (MapElementsData.ThingTypeDescriptions.ContainsKey(type)) {
                    return type + " - " + MapElementsData.ThingTypeDescriptions[type];
                }

                return type + " - Unknown Thing";
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
                int type = 0;
                if (!int.TryParse((string)value, out type)) {
                    //throw new ArgumentException("'" + (string)value + "' is not a valid Thing type");
                    General.ShowErrorMessage("'" + (string)value + "' is not a valid Thing type", System.Windows.Forms.MessageBoxButtons.OK);
                }
                return type;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
