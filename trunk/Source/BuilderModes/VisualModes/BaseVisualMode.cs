
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

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Visual Mode",
			  SwitchAction = "visualmode",		// Action name used to switch to this mode
			  ButtonDesc = "Visual Mode",		// Description on the button in toolbar/menu
			  ButtonImage = "VisualMode.png",	// Image resource name for the button
			  ButtonOrder = 0)]					// Position of the button (lower is more to the left)

	public class BaseVisualMode : VisualMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public BaseVisualMode()
		{
			// Initialize
			
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
				
				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This creates a visual sector
		protected override VisualSector CreateVisualSector(Sector s)
		{
			return new BaseVisualSector(s);
		}
		
		// This draws a frame
		public override void OnRedrawDisplay()
		{
			// Start drawing
			if(renderer.Start())
			{
				// Begin with geometry
				renderer.StartGeometry();

				// This renders all visible sectors
				base.OnRedrawDisplay();

				// Done rendering geometry
				renderer.FinishGeometry();

				// Present!
				renderer.Finish();
			}
		}
		
		#endregion
	}
}
