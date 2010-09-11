
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

namespace CodeImp.DoomBuilder.GZDoomEditing
{
	internal class BaseVisualSector : VisualSector
	{
		#region ================== Constants

		#endregion

		#region ================== Variables
		
		protected BaseVisualMode mode;
		
		protected SectorData data;
		protected VisualFloor floor;
		protected VisualCeiling ceiling;
		protected Dictionary<Effect3DFloor, VisualFloor> extrafloors;
		protected Dictionary<Effect3DFloor, VisualCeiling> extraceilings;
		protected Dictionary<Sidedef, VisualSidedefParts> sides;
		
		// If this is set to true, the sector will be rebuilt after the action is performed.
		protected bool changed;

		#endregion

		#region ================== Properties
		
		public SectorData Data { get { return data; } }
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
				extrafloors = null;
				extraceilings = null;
				
				// Dispose base
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods
		
		// This updates this virtual the sector and neightbours if needed
		public void UpdateSectorGeometry(bool includeneighbours)
		{
			// Rebuild sector
			this.Changed = true;
			
			data.Reset();

			// Update sectors that rely on this sector
			foreach(KeyValuePair<Sector, bool> s in data.UpdateAlso)
			{
				if(mode.VisualSectorExists(s.Key))
				{
					BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(s.Key);
					vs.UpdateSectorGeometry(s.Value);
				}
			}

			// Go for all things in this sector
			foreach(Thing t in General.Map.Map.Things)
			{
				if(t.Sector == this.Sector)
				{
					if(mode.VisualThingExists(t))
					{
						// Update thing
						BaseVisualThing vt = (mode.GetVisualThing(t) as BaseVisualThing);
						vt.Changed = true;
					}
				}
			}

			if(includeneighbours)
			{
				// Also rebuild surrounding sectors, because outside sidedefs may need to be adjusted
				foreach(Sidedef sd in this.Sector.Sidedefs)
				{
					if(sd.Other != null)
					{
						if(mode.VisualSectorExists(sd.Other.Sector))
						{
							BaseVisualSector bvs = (BaseVisualSector)mode.GetVisualSector(sd.Other.Sector);
							bvs.Changed = true;
						}
					}
				}
			}
		}
		
		// This (re)builds the visual sector, calculating all geometry from scratch
		public void Rebuild()
		{
			// Forget old geometry
			base.ClearGeometry();
			
			// Get sector data
			data = mode.GetSectorData(this.Sector);
			if(!data.Updated) data.Update();
			
			// Create floor
			floor = floor ?? new VisualFloor(mode, this, data.Floor);
			if(floor.Setup())
				base.AddGeometry(floor);
			
			// Create ceiling
			ceiling = ceiling ?? new VisualCeiling(mode, this, data.Ceiling);
			if(ceiling.Setup())
				base.AddGeometry(ceiling);
			
			// Create 3D floors
			Dictionary<Effect3DFloor, VisualFloor> oldextrafloors = extrafloors ?? new Dictionary<Effect3DFloor, VisualFloor>(1);
			extrafloors = new Dictionary<Effect3DFloor, VisualFloor>(data.ExtraFloors.Count);
			Dictionary<Effect3DFloor, VisualCeiling> oldextraceilings = extraceilings ?? new Dictionary<Effect3DFloor, VisualCeiling>(1);
			extraceilings = new Dictionary<Effect3DFloor, VisualCeiling>(data.ExtraFloors.Count);
			foreach(Effect3DFloor ef in data.ExtraFloors)
			{
				// Create a floor
				VisualFloor vf = oldextrafloors.ContainsKey(ef) ? oldextrafloors[ef] : new VisualFloor(mode, this, ef.Floor);
				if(vf.Setup())
					base.AddGeometry(vf);
				extrafloors.Add(ef, vf);
				
				// Create a ceiling
				VisualCeiling vc = oldextraceilings.ContainsKey(ef) ? oldextraceilings[ef] : new VisualCeiling(mode, this, ef.Ceiling);
				if(vc.Setup())
					base.AddGeometry(vc);
				extraceilings.Add(ef, vc);
			}
			
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
					if(vu.Setup())
						base.AddGeometry(vu);
					
					// Create lower part
					VisualLower vl = parts.lower ?? new VisualLower(mode, this, sd);
					if(vl.Setup())
						base.AddGeometry(vl);
					
					// Create middle part
					VisualMiddleDouble vm = parts.middledouble ?? new VisualMiddleDouble(mode, this, sd);
					if(vm.Setup())
						base.AddGeometry(vm);
					
					// Create 3D wall parts
					SectorData osd = mode.GetSectorData(sd.Other.Sector);
					if(!osd.Updated) osd.Update();
					Dictionary<Effect3DFloor, VisualMiddle3D> oldfloors = parts.middle3d ?? new Dictionary<Effect3DFloor, VisualMiddle3D>(2);
					Dictionary<Effect3DFloor, VisualMiddle3D> newfloors = new Dictionary<Effect3DFloor, VisualMiddle3D>(2);
					foreach(Effect3DFloor ef in osd.ExtraFloors)
					{
						VisualMiddle3D vm3 = oldfloors.ContainsKey(ef) ? oldfloors[ef] : new VisualMiddle3D(mode, this, sd, ef);
						if(vm3.Setup())
							base.AddGeometry(vm3);
						newfloors.Add(ef, vm3);
					}
					
					// Store
					sides.Add(sd, new VisualSidedefParts(vu, vl, vm, newfloors));
				}
				else
				{
					// Create middle part
					VisualMiddleSingle vm = parts.middlesingle ?? new VisualMiddleSingle(mode, this, sd);
					if(vm.Setup())
						base.AddGeometry(vm);
					
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
