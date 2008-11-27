
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
using CodeImp.DoomBuilder.Data;

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
		private FindReplaceForm findreplaceform;
		private ErrorCheckForm errorcheckform;

		#endregion

		#region ================== Properties
		
		public override string Name { get { return "Doom Builder"; } }
		public static BuilderPlug Me { get { return me; } }
		
		public MenusForm MenusForm { get { return menusform; } }
		public CurveLinedefsForm CurveLinedefsForm { get { return curvelinedefsform; } }
		public FindReplaceForm FindReplaceForm { get { return findreplaceform; } }
		public ErrorCheckForm ErrorCheckForm { get { return errorcheckform; } }
		
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

			// Load find/replace form
			findreplaceform = new FindReplaceForm();

			// Load error checking form
			errorcheckform = new ErrorCheckForm();
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
				findreplaceform.Dispose();
				findreplaceform = null;
				errorcheckform.Dispose();
				errorcheckform = null;
				
				// Done
				me = null;
				base.Dispose();
			}
		}

		#endregion

		#region ================== Events
		
		// When floor surface geometry is created for classic modes
		public override void OnSectorFloorSurfaceUpdate(Sector s, ref FlatVertex[] vertices)
		{
			ImageData img = General.Map.Data.GetFlatImage(s.LongFloorTexture);
			if(img != null)
			{
				// Make scalars
				float sw = 1.0f / img.ScaledWidth;
				float sh = 1.0f / img.ScaledHeight;
				
				// Make proper texture coordinates
				for(int i = 0; i < vertices.Length; i++)
				{
					vertices[i].u = vertices[i].u * sw;
					vertices[i].v = vertices[i].v * sw;
				}
			}
		}

		// When ceiling surface geometry is created for classic modes
		public override void OnSectorCeilingSurfaceUpdate(Sector s, ref FlatVertex[] vertices)
		{
			ImageData img = General.Map.Data.GetFlatImage(s.LongCeilTexture);
			if(img != null)
			{
				// Make scalars
				float sw = 1.0f / img.ScaledWidth;
				float sh = 1.0f / img.ScaledHeight;

				// Make proper texture coordinates
				for(int i = 0; i < vertices.Length; i++)
				{
					vertices[i].u = vertices[i].u * sw;
					vertices[i].v = vertices[i].v * sw;
				}
			}
		}

		// When the editing mode changes
		public override bool OnModeChange(EditMode oldmode, EditMode newmode)
		{
			// Show the correct menu for the new mode
			menusform.ShowEditingModeMenu(newmode);
			
			return base.OnModeChange(oldmode, newmode);
		}
		
		#endregion
		
		#region ================== Tools
		
		// This finds all class types that inherits from the given type
		public Type[] FindClasses(Type t)
		{
			List<Type> found = new List<Type>();
			Type[] types;

			// Get all exported types
			types = Assembly.GetExecutingAssembly().GetTypes();
			foreach(Type it in types)
			{
				// Compare types
				if(t.IsAssignableFrom(it)) found.Add(it);
			}

			// Return list
			return found.ToArray();
		}
		
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
