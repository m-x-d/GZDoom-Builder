
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Reflection;
using System.Drawing;
using SlimDX.Direct3D9;
using SlimDX;

#endregion

namespace CodeImp.DoomBuilder.Map
{
    public struct MacroData
    {
        #region ================== Constants

        #endregion

        #region ================== Variables

        public int type;
        public int tag;
        public int batch;
        public int indexref;

        #endregion

        #region ================== Constructors

        // Constructor
        public MacroData(int type, int tag, int batch, int index)
        {
            // Initialize
            this.type = type;
            this.tag = tag;
            this.batch = batch;
            this.indexref = index;
        }

        #endregion

        #region ================== Methods

        public string SetName()
        {
            return "(" + type + ") " + General.Map.Config.LinedefActions[type].Name + " : " + tag;
        }

        #endregion
    }
}
