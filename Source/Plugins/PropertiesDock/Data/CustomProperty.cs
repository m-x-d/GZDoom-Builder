using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;

namespace CodeImp.DoomBuilder.PropertiesDock {
    public class CustomProperty {
        public bool ReadOnly { get { return bReadOnly; } }
        public string Name { get { return sName; } }
        public string Category { get { return category; } }
        public bool Visible { get { return bVisible; } }
        public object Value { get { return objValue; } set { objValue = value; } }
        
        private string sName = string.Empty;
        private string category = string.Empty;
        private bool bReadOnly = false;
        private bool bVisible = true;
        private object objValue = null;
        //private Attribute[] attributes;

        public CustomProperty(string sName, object value, bool bReadOnly, bool bVisible) {
            this.sName = sName;
            this.objValue = value;
            this.bReadOnly = bReadOnly;
            this.bVisible = bVisible;
            //this.attributes = new Attribute[0];
        }

        //mxd
        public CustomProperty(string sName, object value, string category, bool bReadOnly, bool bVisible) : this(sName, value, bReadOnly, bVisible) {
            //attributes = attrs;
            this.category = category;
        }

        /*public Attribute[] Attributes {
            get {
                return attributes;
            }
        }*/
    }
}
