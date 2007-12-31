
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
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	internal partial class ThingInfoPanel : UserControl
	{
		// Constructor
		public ThingInfoPanel()
		{
			// Initialize
			InitializeComponent();
		}

		// This shows the info
		public void ShowInfo(Thing t)
		{
			ThingTypeInfo ti;
			int zvalue;
			string zinfo;
			
			// Lookup thing info
			ti = General.Map.Config.GetThingInfo(t.Type);

			// TODO: Lookup action description from config

			// Determine z info to show
			t.DetermineSector();
			if(t.Sector != null)
			{
				// Hangs from ceiling?
				if(ti.Hangs)
				{
					zvalue = t.Sector.CeilHeight + t.ZOffset;
					zinfo = zvalue.ToString();
				}
				else
				{
					zvalue = t.Sector.FloorHeight + t.ZOffset;
					zinfo = zvalue.ToString();
				}
			}
			else
			{
				zvalue = t.ZOffset;
				if(zvalue >= 0) zinfo = "+" + zvalue.ToString(); else zinfo = zvalue.ToString();
			}
			
			// Thing info
			type.Text = t.Type + " - " + ti.Title;
			action.Text = ""; // TODO
			position.Text = t.X.ToString() + ", " + t.Y.ToString() + ", " + zinfo;
			tag.Text = ""; // TODO
			angle.Text = t.AngleDeg.ToString() + "\u00B0";
			spritename.Text = ti.Sprite;
			General.DisplayZoomedImage(spritetex, General.Map.Data.GetSpriteBitmap(ti.Sprite));

			// Show the whole thing
			this.Show();
			this.Update();
		}

		// When visible changed
		protected override void OnVisibleChanged(EventArgs e)
		{
			// Hiding panels
			if(!this.Visible)
			{
				spritetex.BackgroundImage = null;
			}

			// Call base
			base.OnVisibleChanged(e);
		}
	}
}
