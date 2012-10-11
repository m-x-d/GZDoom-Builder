using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;

namespace CodeImp.DoomBuilder.PropertiesDock {
    public class CustomPropertyDescriptor : PropertyDescriptor {
        CustomProperty m_Property;
        public CustomPropertyDescriptor(ref CustomProperty myProperty, Attribute[] attrs)
            : base(myProperty.Name, attrs) {
            m_Property = myProperty;
        }
        /*public CustomPropertyDescriptor(ref CustomProperty myProperty, Attribute[] attrs)
            : base(myProperty.Name, combineAttributes(attrs, myProperty.Attributes)) {
            m_Property = myProperty;
        }*/

        /*private static Attribute[] combineAttributes(Attribute[] attrs, Attribute[] attribute) {
            List<Attribute> l = new List<Attribute>();
            l.AddRange(attrs);
            l.AddRange(attribute);
            return l.ToArray();
        }*/

        #region PropertyDescriptor specific

        public override bool CanResetValue(object component) {
            return false;
        }

        public override Type ComponentType {
            get {
                return null;
            }
        }

        public override object GetValue(object component) {
            return m_Property.Value;
        }

        public override string Description {
            get {
                return m_Property.Name;
            }
        }

        public override string Category {
            get {
                return m_Property.Category;
            }
        }

        public override string DisplayName {
            get {
                return m_Property.Name;
            }

        }

        public override bool IsReadOnly {
            get {
                return m_Property.ReadOnly;
            }
        }

        public override void ResetValue(object component) {
            //Have to implement
        }

        public override bool ShouldSerializeValue(object component) {
            return false;
        }

        public override void SetValue(object component, object value) {
            m_Property.Value = value;
        }

        public override Type PropertyType {
            get { return m_Property.Value.GetType(); }
        }

        #endregion
    }
}
