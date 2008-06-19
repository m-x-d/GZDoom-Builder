
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
using System.Drawing;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class BuilderPlug : Plug
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Static instance
		private static BuilderPlug me;
		
		// Main objects
		private MenusForm menusform;
		private CurveLinedefsForm curvelinedefsform;

		#endregion

		#region ================== Properties

		public static BuilderPlug Me { get { return me; } }

		public MenusForm MenusForm { get { return menusform; } }
		public CurveLinedefsForm CurveLinedefsForm { get { return curvelinedefsform; } }

		#endregion

		#region ================== Initialize / Dispose

		// When plugin is initialized
		public override void OnInitialize()
		{
			// Setup
			me = this;

			// Load menus form and register it
			menusform = new MenusForm();
			menusform.Register();

			// Load curve linedefs form
			curvelinedefsform = new CurveLinedefsForm();
		}
		
		// Disposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!IsDisposed)
			{
				// Clean up
				menusform.Unregister();
				menusform.Dispose();
				menusform = null;
				curvelinedefsform.Dispose();
				curvelinedefsform = null;
				
				// Done
				me = null;
				base.Dispose();
			}
		}

		#endregion

		#region ================== Events

		// When the editing mode changes
		public override void OnModeChange(EditMode oldmode, EditMode newmode)
		{
			// Show the correct menu for the new mode
			menusform.ShowEditingModeMenu(newmode);
		}
		
		#endregion
		
		#region ================== Tools

		// This renders the associated sectors/linedefs with the indication color
		public void PlotAssociations(IRenderer2D renderer, Association asso)
		{
			// Tag must be above zero
			if(asso.tag <= 0) return;
			
			// Sectors?
			if(asso.type == UniversalType.SectorTag)
			{
				foreach(Sector s in General.Map.Map.Sectors)
					if(s.Tag == asso.tag) renderer.PlotSector(s, General.Colors.Indication);
			}
			// Linedefs?
			else if(asso.type == UniversalType.LinedefTag)
			{
				foreach(Linedef l in General.Map.Map.Linedefs)
				{
					if(l.Tag == asso.tag) renderer.PlotLinedef(l, General.Colors.Indication);
				}
			}
		}

		// This renders the associated things with the indication color
		public void RenderAssociations(IRenderer2D renderer, Association asso)
		{
			// Tag must be above zero
			if(asso.tag <= 0) return;

			// Things?
			if(asso.type == UniversalType.ThingTag)
			{
				foreach(Thing t in General.Map.Map.Things)
				{
					if(t.Tag == asso.tag) renderer.RenderThing(t, General.Colors.Indication, 1.0f);
				}
			}
		}

		// This renders the associated sectors/linedefs with the indication color
		public void PlotReverseAssociations(IRenderer2D renderer, Association asso)
		{
			// Tag must be above zero
			if(asso.tag <= 0) return;

			// Linedefs
			foreach(Linedef l in General.Map.Map.Linedefs)
			{
				// Known action on this line?
				if((l.Action > 0) && General.Map.Config.LinedefActions.ContainsKey(l.Action))
				{
					LinedefActionInfo action = General.Map.Config.LinedefActions[l.Action];
					if((action.Args[0].Type == (int)asso.type) && (l.Args[0] == asso.tag)) renderer.PlotLinedef(l, General.Colors.Indication);
					if((action.Args[1].Type == (int)asso.type) && (l.Args[1] == asso.tag)) renderer.PlotLinedef(l, General.Colors.Indication);
					if((action.Args[2].Type == (int)asso.type) && (l.Args[2] == asso.tag)) renderer.PlotLinedef(l, General.Colors.Indication);
					if((action.Args[3].Type == (int)asso.type) && (l.Args[3] == asso.tag)) renderer.PlotLinedef(l, General.Colors.Indication);
					if((action.Args[4].Type == (int)asso.type) && (l.Args[4] == asso.tag)) renderer.PlotLinedef(l, General.Colors.Indication);
				}
			}
		}

		// This renders the associated things with the indication color
		public void RenderReverseAssociations(IRenderer2D renderer, Association asso)
		{
			// Tag must be above zero
			if(asso.tag <= 0) return;

			// Things
			foreach(Thing t in General.Map.Map.Things)
			{
				// Known action on this thing?
				if((t.Action > 0) && General.Map.Config.LinedefActions.ContainsKey(t.Action))
				{
					LinedefActionInfo action = General.Map.Config.LinedefActions[t.Action];
					if((action.Args[0].Type == (int)asso.type) && (t.Args[0] == asso.tag)) renderer.RenderThing(t, General.Colors.Indication, 1.0f);
					if((action.Args[1].Type == (int)asso.type) && (t.Args[1] == asso.tag)) renderer.RenderThing(t, General.Colors.Indication, 1.0f);
					if((action.Args[2].Type == (int)asso.type) && (t.Args[2] == asso.tag)) renderer.RenderThing(t, General.Colors.Indication, 1.0f);
					if((action.Args[3].Type == (int)asso.type) && (t.Args[3] == asso.tag)) renderer.RenderThing(t, General.Colors.Indication, 1.0f);
					if((action.Args[4].Type == (int)asso.type) && (t.Args[4] == asso.tag)) renderer.RenderThing(t, General.Colors.Indication, 1.0f);
				}
			}
		}

		#endregion
	}
}
