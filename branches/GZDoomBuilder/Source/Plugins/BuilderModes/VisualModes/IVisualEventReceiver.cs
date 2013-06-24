
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

using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal interface IVisualEventReceiver
	{
		// The events that must be handled
		void OnSelectBegin();
		void OnSelectEnd();
		void OnEditBegin();
		void OnEditEnd();
		void OnMouseMove(MouseEventArgs e);
		void OnChangeTargetHeight(int amount);
		void OnChangeTargetBrightness(bool up);
		void OnChangeTextureOffset(int horizontal, int vertical, bool doSurfaceAngleCorrection);
		void OnChangeTextureScale(float incrementX, float incrementY); //mxd
		void OnResetTextureOffset();
		void OnSelectTexture();
		void OnCopyTexture();
		void OnPasteTexture();
		void OnCopyTextureOffsets();
		void OnPasteTextureOffsets();
		void OnCopyProperties();
		void OnPasteProperties();
		void OnTextureAlign(bool alignx, bool aligny);
		void OnTextureFit(bool fitWidth, bool fitHeight); //mxd
		void OnTextureFloodfill();
		void OnToggleUpperUnpegged();
		void OnToggleLowerUnpegged();
		void OnProcess(float deltatime);
		void OnInsert();
		void OnDelete();

		// Assist functions
		void ApplyTexture(string texture);
		void ApplyUpperUnpegged(bool set);
		void ApplyLowerUnpegged(bool set);
		
		// Other methods
		string GetTextureName();
		void SelectNeighbours(bool select, bool withSameTexture, bool withSameHeight); //mxd
		bool IsSelected(); //mxd
	}
}
