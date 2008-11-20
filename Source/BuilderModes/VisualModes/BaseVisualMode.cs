
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.Editing
{
	[EditMode(SwitchAction = "visualmode",		// Action name used to switch to this mode
			  ButtonDesc = "Visual Mode",		// Description on the button in toolbar/menu
			  ButtonImage = "VisualMode.png",	// Image resource name for the button
			  ButtonOrder = 0)]					// Position of the button (lower is more to the left)

	public class BaseVisualMode : VisualMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// All constructed visual sectors
		private Dictionary<Sector, BaseVisualSector> allsectors;

		// List of visible sectors
		private Dictionary<Sector, BaseVisualSector> visiblesectors;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public BaseVisualMode()
		{
			// Initialize
			allsectors = new Dictionary<Sector, BaseVisualSector>(General.Map.Map.Sectors.Count);
			visiblesectors = new Dictionary<Sector, BaseVisualSector>();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				foreach(KeyValuePair<Sector, BaseVisualSector> s in allsectors) s.Value.Dispose();
				visiblesectors = null;
				allsectors = null;
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Private Tools

		// This finds the nearest sector to the camera
		private Sector FindStartSector(Vector2D campos)
		{
			float side;
			Linedef l;
			
			// Get nearest linedef
			l = General.Map.Map.NearestLinedef(campos);
			if(l != null)
			{
				// Check if we are on front or back side
				side = l.SideOfLine(campos);
				if(side > 0)
				{
					// Is there a sidedef here?
					if(l.Back != null)
						return l.Back.Sector;
					else if(l.Front != null)
						return l.Front.Sector;
					else
						return null;
				}
				else
				{
					// Is there a sidedef here?
					if(l.Front != null)
						return l.Front.Sector;
					else if(l.Back != null)
						return l.Back.Sector;
					else
						return null;
				}
			}
			else
				return null;
		}

		// This recursively finds and adds visible sectors
		private void ProcessVisibleSectors(Sector start, Vector2D campos)
		{
			// Add the start sector
			AddVisibleSector(start);
			
			// Get a range of blocks
			List<VisualBlockEntry> blocks = blockmap.GetSquareRange(campos, General.Settings.ViewDistance);
			foreach(VisualBlockEntry b in blocks)
			{
				// Go for all the linedefs in this block
				foreach(Linedef ld in b.Lines)
				{
					// Add sectors from both sides of the line
					if(ld.Front != null) AddVisibleSector(ld.Front.Sector);
					if(ld.Back != null) AddVisibleSector(ld.Back.Sector);
				}
			}
		}

		// This adds (and creates if needed) the BaseVisualSector for
		// the given sector to the visible sectors list
		private void AddVisibleSector(Sector s)
		{
			BaseVisualSector vs;

			// Find the basesector and make it if needed
			if(allsectors.ContainsKey(s))
			{
				// Take existing visualsector
				vs = allsectors[s];
			}
			else
			{
				// Make new visualsector
				vs = new BaseVisualSector(s);
				allsectors.Add(s, vs);
			}

			// Add sector to visibility list
			visiblesectors[s] = vs;
		}
		
		#endregion

		#region ================== Methods

		[EndAction("reloadresources", BaseAction = true)]
		public void ReloadResources()
		{
			foreach(KeyValuePair<Sector, BaseVisualSector> s in allsectors) s.Value.Dispose();
			allsectors.Clear();
			visiblesectors.Clear();
		}
		
		// Mode engages
		public override void OnEngage()
		{
			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			
			base.OnEngage();
		}
		
		// This draws a frame
		public override void OnRedrawDisplay()
		{
			// Start drawing
			if(renderer.Start())
			{
				// Begin with geometry
				renderer.StartGeometry();

				// Render all visible sectors
				foreach(KeyValuePair<Sector, BaseVisualSector> vs in visiblesectors)
					renderer.RenderGeometry(vs.Value);

				// Done rendering geometry
				renderer.FinishGeometry();

				// Present!
				renderer.Finish();
			}
			
			// Call base
			base.OnRedrawDisplay();
		}

		// This processes a frame
		public override void OnProcess()
		{
			Vector2D campos;
			
			// Process base class first
			base.OnProcess();
			
			// Get the 2D camera position
			campos = new Vector2D(base.CameraPosition.x, base.CameraPosition.y);
			
			// Make new visibility list
			visiblesectors = new Dictionary<Sector, BaseVisualSector>(General.Map.Map.Sectors.Count);
			
			// Process all visible sectors starting with the nearest
			ProcessVisibleSectors(FindStartSector(campos), campos);
		}
		
		#endregion
	}
}
