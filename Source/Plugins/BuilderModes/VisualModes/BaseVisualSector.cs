
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
		
		protected BaseVisualMode mode;

		protected VisualFloor floor;
		protected VisualCeiling ceiling;
		protected Dictionary<Sidedef, VisualSidedefParts> sides;
		
		// If this is set to true, the sector will be rebuilt after the action is performed.
		protected bool changed;

		#endregion

		#region ================== Properties

		public VisualFloor Floor { get { return floor; } }
		public VisualCeiling Ceiling { get { return ceiling; } }
		public bool Changed { get { return changed; } set { changed |= value; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public BaseVisualSector(BaseVisualMode mode, Sector s) : base(s)
		{
			this.mode = mode;
			
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
				sides = null;
				floor = null;
				ceiling = null;
				
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
			if(floor == null) floor = new VisualFloor(mode, this);
			if(floor.Setup()) base.AddGeometry(floor);

			// Create ceiling
			if(ceiling == null) ceiling = new VisualCeiling(mode, this);
			if(ceiling.Setup()) base.AddGeometry(ceiling);

			// Go for all sidedefs
			Dictionary<Sidedef, VisualSidedefParts> oldsides = sides ?? new Dictionary<Sidedef, VisualSidedefParts>(1);
			sides = new Dictionary<Sidedef, VisualSidedefParts>(base.Sector.Sidedefs.Count);
			foreach(Sidedef sd in base.Sector.Sidedefs)
			{
				// VisualSidedef already exists?
				VisualSidedefParts parts = oldsides.ContainsKey(sd) ? oldsides[sd] : new VisualSidedefParts();
				
				// Doublesided or singlesided?
				if(sd.Other != null)
				{
					// Create upper part
					VisualUpper vu = parts.upper ?? new VisualUpper(mode, this, sd);
					if(vu.Setup()) base.AddGeometry(vu);
					
					// Create lower part
					VisualLower vl = parts.lower ?? new VisualLower(mode, this, sd);
					if(vl.Setup()) base.AddGeometry(vl);
					
					// Create middle part
					VisualMiddleDouble vm = parts.middledouble ?? new VisualMiddleDouble(mode, this, sd);
					if(vm.Setup()) base.AddGeometry(vm);

					// Store
					sides.Add(sd, new VisualSidedefParts(vu, vl, vm));
				}
				else
				{
					// Create middle part
					VisualMiddleSingle vm = parts.middlesingle ?? new VisualMiddleSingle(mode, this, sd);
					if(vm.Setup()) base.AddGeometry(vm);
					
					// Store
					sides.Add(sd, new VisualSidedefParts(vm));
				}
			}
			
			// Done
			changed = false;
		}
		
		// This returns the visual sidedef parts for a given sidedef
		public VisualSidedefParts GetSidedefParts(Sidedef sd)
		{
			if(sides.ContainsKey(sd))
				return sides[sd];
			else
				return new VisualSidedefParts();
		}
		
		#endregion
	}
}
