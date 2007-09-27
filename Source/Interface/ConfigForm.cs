
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

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	public partial class ConfigForm : DelayedForm
	{
		// Constructor
		public ConfigForm()
		{
			// Initialize
			InitializeComponent();

			// Fill list of configurations
			foreach(ConfigurationInfo ci in General.Configs)
			{
				// Add a copy
				listconfigs.Items.Add(ci.Clone());
			}
		}
	}
}