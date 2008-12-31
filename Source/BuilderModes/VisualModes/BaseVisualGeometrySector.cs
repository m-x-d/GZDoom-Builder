
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
	internal abstract class BaseVisualGeometrySector : VisualGeometry, IVisualEventReceiver
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		protected BaseVisualMode mode;

		#endregion

		#region ================== Properties
		
		new public BaseVisualSector Sector { get { return (BaseVisualSector)base.Sector; } }
		
		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public BaseVisualGeometrySector(BaseVisualMode mode, VisualSector vs) : base(vs)
		{
			this.mode = mode;
		}

		#endregion

		#region ================== Methods

		// This changes the height
		protected abstract void ChangeHeight(int amount);

		#endregion

		#region ================== Events

		// Unused
		public virtual void OnSelectBegin() { }
		public virtual void OnSelectEnd() { }
		public virtual void OnEditBegin() { }
		public virtual void OnMouseMove(MouseEventArgs e) { }
		public virtual void OnChangeTextureOffset(int horizontal, int vertical) { }
		public virtual void OnTextureAlign(bool alignx, bool aligny) { }
		public virtual void OnToggleUpperUnpegged() { }
		public virtual void OnToggleLowerUnpegged() { }
		protected virtual void SetTexture(string texturename) { }

		// Select texture
		public virtual void OnSelectTexture()
		{
			string oldtexture = GetTextureName();
			string newtexture = General.Interface.BrowseFlat(General.Interface, oldtexture);
			if(newtexture != oldtexture)
			{
				General.Map.UndoRedo.CreateUndo("Change flat " + newtexture);
				SetTexture(newtexture);
			}
		}
		
		// Copy texture
		public virtual void OnCopyTexture()
		{
			mode.CopiedFlat = GetTextureName();
			if(General.Map.Config.MixTexturesFlats) mode.CopiedTexture = GetTextureName();
		}
		
		public virtual void OnPasteTexture() { }

		// Return texture name
		public virtual string GetTextureName() { return ""; }
		
		// Edit button released
		public virtual void OnEditEnd()
		{
			// Not using any modifier buttons
			if(!General.Interface.ShiftState && !General.Interface.CtrlState && !General.Interface.AltState)
			{
				List<Sector> sectors = new List<Sector>();
				sectors.Add(this.Sector.Sector);
				DialogResult result = General.Interface.ShowEditSectors(sectors);
				if(result == DialogResult.OK) (this.Sector as BaseVisualSector).Rebuild();
			}
		}

		// Sector height change
		public virtual void OnChangeTargetHeight(int amount)
		{
			ChangeHeight(amount);
			
			// Rebuild sector
			Sector.Rebuild();
			
			// Go for all things in this sector
			foreach(Thing t in General.Map.Map.Things)
			{
				if(t.Sector == Sector.Sector)
				{
					if(mode.VisualThingExists(t))
					{
						// Update thing
						BaseVisualThing vt = (mode.GetVisualThing(t) as BaseVisualThing);
						vt.Setup();
					}
				}
			}

			// Also rebuild surrounding sectors, because outside sidedefs may need to be adjusted
			foreach(Sidedef sd in Sector.Sector.Sidedefs)
			{
				if((sd.Other != null) && mode.VisualSectorExists(sd.Other.Sector))
				{
					BaseVisualSector bvs = (BaseVisualSector)mode.GetVisualSector(sd.Other.Sector);
					bvs.Rebuild();
				}
			}
		}
		
		// Sector brightness change
		public virtual void OnChangeTargetBrightness(int amount)
		{
			// Change brightness
			General.Map.UndoRedo.CreateUndo("Change sector brightness", UndoGroup.SectorBrightnessChange, Sector.Sector.Index);
			Sector.Sector.Brightness = General.Clamp(Sector.Sector.Brightness + amount, 0, 255);
			
			// Rebuild sector
			Sector.Rebuild();

			// Go for all things in this sector
			foreach(Thing t in General.Map.Map.Things)
			{
				if(t.Sector == Sector.Sector)
				{
					if(mode.VisualThingExists(t))
					{
						// Update thing
						BaseVisualThing vt = (mode.GetVisualThing(t) as BaseVisualThing);
						vt.Setup();
					}
				}
			}
		}
		
		#endregion
	}
}
