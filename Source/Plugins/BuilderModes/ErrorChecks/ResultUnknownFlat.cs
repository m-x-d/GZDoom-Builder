
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
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class ResultUnknownFlat : ErrorResult
	{
		#region ================== Variables
		
		private readonly Sector sector;
		private readonly bool ceiling;
		
		#endregion
		
		#region ================== Properties

		public override int Buttons { get { return 1; } }
		public override string Button1Text { get { return "Add Default Flat"; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public ResultUnknownFlat(Sector s, bool ceiling)
		{
			// Initialize
			this.sector = s;
			this.ceiling = ceiling;
			this.viewobjects.Add(s);
			this.hidden = s.IgnoredErrorChecks.Contains(this.GetType()); //mxd
			string objname = ceiling ? "ceiling" : "floor";
			this.description = "This sector's " + objname + " uses an unknown flat. This could be the result of missing resources, or a mistyped flat name. Click the 'Add Default Flat' button to use a known flat instead.";
		}
		
		#endregion
		
		#region ================== Methods

		// This sets if this result is displayed in ErrorCheckForm (mxd)
		internal override void Hide(bool hide) 
		{
			hidden = hide;
			Type t = this.GetType();
			if(hide) sector.IgnoredErrorChecks.Add(t);
			else if(sector.IgnoredErrorChecks.Contains(t)) sector.IgnoredErrorChecks.Remove(t);
		}
		
		// This must return the string that is displayed in the listbox
		public override string ToString()
		{
			if(ceiling)
				return "Sector " + sector.Index + " has unknown ceiling flat \"" + sector.CeilTexture + "\"";
			else
				return "Sector " + sector.Index + " has unknown floor flat \"" + sector.FloorTexture + "\"";
		}
		
		// Rendering
		public override void PlotSelection(IRenderer2D renderer)
		{
			renderer.PlotSector(sector, General.Colors.Selection);
		}

		//mxd. More rendering
		public override void RenderOverlaySelection(IRenderer2D renderer) 
		{
			if(!BuilderPlug.Me.UseHighlight) return;
			renderer.RenderHighlight(sector.FlatVertices, General.Colors.Selection.WithAlpha(64).ToInt());
		}
		
		// Fix by setting default flat
		public override bool Button1Click(bool batchMode)
		{
			if(!batchMode) General.Map.UndoRedo.CreateUndo("Unknown flat correction");
			General.Settings.FindDefaultDrawSettings();
			
			if(ceiling)
				sector.SetCeilTexture(General.Map.Options.DefaultCeilingTexture);
			else
				sector.SetFloorTexture(General.Map.Options.DefaultFloorTexture);
			
			General.Map.Map.Update();
			return true;
		}
		
		#endregion
	}
}
