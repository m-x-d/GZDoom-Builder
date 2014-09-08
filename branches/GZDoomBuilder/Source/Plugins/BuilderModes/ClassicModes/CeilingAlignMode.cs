
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

using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[EditMode(DisplayName = "Ceiling Align Mode",
			  SwitchAction = "ceilingalignmode",
			  ButtonImage = "CeilingAlign.png",
			  ButtonOrder = int.MinValue + 311,
			  ButtonGroup = "000_editing",
			  UseByDefault = true, //mxd
			  SupportedMapFormats = new[] { "UniversalMapSetIO" }, //mxd
			  Volatile = true)]

	public class CeilingAlignMode : FlatAlignMode
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private ViewMode prevviewmode;

		#endregion

		#region ================== Properties

		public override string XScaleName { get { return "xscaleceiling"; } }
		public override string YScaleName { get { return "yscaleceiling"; } }
		public override string XOffsetName { get { return "xpanningceiling"; } }
		public override string YOffsetName { get { return "ypanningceiling"; } }
		public override string RotationName { get { return "rotationceiling"; } }
		public override string UndoDescription { get { return "Ceiling Alignment"; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public CeilingAlignMode()
		{
		}

		#endregion

		#region ================== Methods

		// Get the texture data to align
		protected override ImageData GetTexture(Sector editsector)
		{
			return General.Map.Data.GetFlatImage(editsector.LongCeilTexture);
		}

		#endregion

		#region ================== Events

		// Mode engages
		public override void OnEngage()
		{
			prevviewmode = General.Map.Renderer2D.ViewMode;

			base.OnEngage();
			
			General.Actions.InvokeAction("builder_viewmodeceilings");
		}

		// Mode disengages
		public override void OnDisengage()
		{
			switch(prevviewmode)
			{
				case ViewMode.Normal: General.Actions.InvokeAction("builder_viewmodenormal"); break;
				case ViewMode.FloorTextures: General.Actions.InvokeAction("builder_viewmodefloors"); break;
				case ViewMode.CeilingTextures: General.Actions.InvokeAction("builder_viewmodeceilings"); break;
				case ViewMode.Brightness: General.Actions.InvokeAction("builder_viewmodebrightness"); break;
			}
			
			base.OnDisengage();
		}

		#endregion
	}
}
