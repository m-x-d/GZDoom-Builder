
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
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class ResultMissingTexture : ErrorResult
	{
		#region ================== Variables

		private readonly Sidedef side;
		private readonly SidedefPart part;

		#endregion

		#region ================== Properties

		public override int Buttons { get { return 1; } }
		public override string Button1Text { get { return "Add Default Texture"; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public ResultMissingTexture(Sidedef sd, SidedefPart part)
		{
			// Initialize
			this.side = sd;
			this.part = part;
			this.viewobjects.Add(sd);
			this.hidden = sd.IgnoredErrorChecks.Contains(this.GetType()); //mxd
			this.description = "This sidedef is missing a texture where it is required and could cause a 'Hall Of Mirrors' visual problem in the map. Click the 'Add Default Texture' button to add a texture to the line.";
		}

		#endregion

		#region ================== Methods

		// This sets if this result is displayed in ErrorCheckForm (mxd)
		internal override void Hide(bool hide) 
		{
			hidden = hide;
			Type t = this.GetType();
			if(hide) side.IgnoredErrorChecks.Add(t);
			else if(side.IgnoredErrorChecks.Contains(t)) side.IgnoredErrorChecks.Remove(t);
		}

		// This must return the string that is displayed in the listbox
		public override string ToString()
		{
			string sidestr = side.IsFront ? "front" : "back";

			switch (part)
			{
				case SidedefPart.Upper:
					return "Sidedef " + side.Index + " has missing upper texture (" + sidestr + " side)";

				case SidedefPart.Middle:
					return "Sidedef " + side.Index + " has missing middle texture (" + sidestr + " side)";

				case SidedefPart.Lower:
					return "Sidedef " + side.Index + " has missing lower texture (" + sidestr + " side)";

				default:
					return "ERROR";
			}
		}

		// Rendering
		public override void PlotSelection(IRenderer2D renderer)
		{
			renderer.PlotLinedef(side.Line, General.Colors.Selection);
			renderer.PlotVertex(side.Line.Start, ColorCollection.VERTICES);
			renderer.PlotVertex(side.Line.End, ColorCollection.VERTICES);
		}

		// Fix by setting default texture
		public override bool Button1Click(bool batchMode)
		{
			if(!batchMode) General.Map.UndoRedo.CreateUndo("Missing texture correction");
			General.Settings.FindDefaultDrawSettings();
			switch (part)
			{
				case SidedefPart.Upper: side.SetTextureHigh(General.Map.Options.DefaultTopTexture); break;
				case SidedefPart.Middle: side.SetTextureMid(General.Map.Options.DefaultWallTexture); break;
				case SidedefPart.Lower: side.SetTextureLow(General.Map.Options.DefaultBottomTexture); break;
			}

			General.Map.Map.Update();
			return true;
		}

		#endregion
	}
}
