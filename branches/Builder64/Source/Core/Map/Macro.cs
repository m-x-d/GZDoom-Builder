
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
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using SlimDX.Direct3D9;
using System.Drawing;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Map
{
    public class Macro
    {
        #region ================== Constants

        #endregion

        #region ================== Variables

        private MacroData[] data;
        private int count;

        #endregion

        #region ================== Properties

        public MacroData[] MacroData { get { return data; } }
        public int Count { get { return count; } }

        #endregion

        #region ================== Constructor / Disposer

        public Macro(int count)
        {
            int i;

            data = new MacroData[count];

            for (i = 0; i < count; i++)
            {
                data[i].batch = 0;
                data[i].tag = 0;
                data[i].type = 0;
                data[i].indexref = -1;
            }

            this.count = count;

            GC.SuppressFinalize(this);
        }

        #endregion

        #region ================== Methods

        public void SetDataFromTreeNode(TreeView tree)
        {
            int i = 0;
            int batchid = 10;

            count = tree.GetNodeCount(true);
            Array.Resize(ref data, count);

            foreach (TreeNode n in tree.Nodes)
            {
                foreach (TreeNode nn in n.Nodes)
                {
                    data[i] = (MacroData)nn.Tag;
                    data[i].batch = batchid;
                    i++;
                }
                batchid += 10;
            }

            Array.Resize(ref data, i);
            count = i;
        }

        public void SetNodeFromData(int macroid, TreeView tree)
        {
            int batch = -1;
            TreeNode n = tree.TopNode;

            foreach (MacroData m in data)
            {
                if (batch != m.batch)
                {
                    batch = m.batch;
                    n = tree.Nodes.Add("Batch " + m.batch);
                }

                TreeNode nn = n.Nodes.Add("Action");
                nn.Text = m.SetName();
                nn.Tag = m;
            }
            tree.ExpandAll();
        }

        public void Set(int index, int type, int batch, int tag)
        {
            data[index].type = type;
            data[index].tag = tag;
            data[index].batch = batch;
        }

        #endregion
    }
}
