
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.GZBuilder.Tools;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class VertexInfoPanel : UserControl
	{
		// Constructor
		public VertexInfoPanel()
		{
			// Initialize
			InitializeComponent();
		}

		// This shows the info
		public void ShowInfo(Vertex v)
		{
			// Vertex info
			vertexinfo.Text = " Vertex " + v.Index + " ";
			position.Text = v.Position.x.ToString("0.##") + ", " + v.Position.y.ToString("0.##");
			
			//mxd. Height offsets
			if(General.Map.UDMF) {
				if(v.Fields.ContainsKey("zceiling")) {
					float zc = v.Fields.GetValue("zceiling", 0f);
					zceiling.Text = zc.ToString("0.##");
					zceiling.Enabled = true;
					labelCeilingOffset.Enabled = true;
				} else {
					zceiling.Text = "--";
				}

				if(v.Fields.ContainsKey("zfloor")) {
					float zf = v.Fields.GetValue("zfloor", 0f);
					zfloor.Text = zf.ToString("0.##");
					zfloor.Enabled = true;
					labelFloorOffset.Enabled = true;
				} else {
					zfloor.Text = "--";
				}

				panelOffsets.Visible = true;
			} else {
				panelOffsets.Visible = false;
			}

			// Show the whole thing
			this.Show();
			this.Update();
		}
	}
}
