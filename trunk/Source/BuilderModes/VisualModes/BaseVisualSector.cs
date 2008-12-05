
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
	internal class BaseVisualSector : VisualSector
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public BaseVisualSector(Sector s) : base(s)
		{
			// Initialize
			Rebuild();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!IsDisposed)
			{
				// Clean up

				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This (re)builds the visual sector, calculating all geometry from scratch
		public void Rebuild()
		{
			// Forget old geometry
			base.ClearGeometry();
			
			// Create floor
			VisualFloor vf = new VisualFloor(this);
			if(vf.Setup()) base.AddGeometry(vf);

			// Create ceiling
			VisualCeiling vc = new VisualCeiling(this);
			if(vc.Setup()) base.AddGeometry(vc);

			// Go for all sidedefs
			foreach(Sidedef sd in base.Sector.Sidedefs)
			{
				// Doublesided or singlesided?
				if(sd.Other != null)
				{
					// Create upper part
					VisualUpper vu = new VisualUpper(this, sd);
					if(vu.Setup()) base.AddGeometry(vu);
					
					// Create lower part
					VisualLower vl = new VisualLower(this, sd);
					if(vl.Setup()) base.AddGeometry(vl);
					
					// Create middle part
					VisualMiddleDouble vm = new VisualMiddleDouble(this, sd);
					if(vm.Setup()) base.AddGeometry(vm);
				}
				else
				{
					// Create middle part
					VisualMiddleSingle vm = new VisualMiddleSingle(this, sd);
					if(vm.Setup()) base.AddGeometry(vm);
				}
			}
		}
		
		#endregion
	}
}
