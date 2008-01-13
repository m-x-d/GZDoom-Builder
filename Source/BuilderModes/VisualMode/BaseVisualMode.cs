
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
using CodeImp.DoomBuilder.Interface;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;

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

		// Diposer
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
			BaseVisualSector vs;
			Sector os;
			
			// Find the basesector and make it if needed
			if(allsectors.ContainsKey(start))
			{
				// Take existing visualsector
				vs = allsectors[start];
			}
			else
			{
				// Make new visualsector
				vs = new BaseVisualSector(start);
				allsectors.Add(start, vs);
			}
			
			// Add sector to visibility list
			visiblesectors.Add(start, vs);
			
			// Go for all sidedefs in the sector
			foreach(Sidedef sd in start.Sidedefs)
			{
				// Doublesided and not referring to same sector?
				if((sd.Other != null) && (sd.Other.Sector != sd.Sector))
				{
					// Get the other sector
					os = sd.Other.Sector;

					// Sector not yet added?
					if(!visiblesectors.ContainsKey(os))
					{
						// TODO: Visiblity checking!
						{
							// Process this sector as well
							ProcessVisibleSectors(os, campos);
						}
					}
				}
			}
		}
		
		#endregion

		#region ================== Methods

		// This draws a frame
		public override void RedrawDisplay()
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
			base.RedrawDisplay();
		}

		// This processes a frame
		public override void Process()
		{
			Vector2D campos;
			
			// Process base class first
			base.Process();
			
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
