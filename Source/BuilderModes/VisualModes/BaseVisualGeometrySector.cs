
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
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal abstract class BaseVisualGeometrySector : BaseVisualGeometry
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public BaseVisualGeometrySector(VisualSector vs) : base(vs)
		{
		}

		#endregion

		#region ================== Methods

		// This changes the height
		public abstract void ChangeHeight(int amount);

		#endregion

		#region ================== Events
		
		// Edit button released
		public override void OnEditEnd()
		{
			List<Sector> sectors = new List<Sector>();
			sectors.Add(this.Sector.Sector);
			DialogResult result = General.Interface.ShowEditSectors(sectors);
			if(result == DialogResult.OK) (this.Sector as BaseVisualSector).Rebuild();
		}
		
		#endregion
	}
}
