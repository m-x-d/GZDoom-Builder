
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal abstract class ScriptDocumentTab : TabPage
	{
		#region ================== Constants
		
		private const int EDITOR_BORDER_TOP = 8;
		private const int EDITOR_BORDER_BOTTOM = 4;
		private const int EDITOR_BORDER_LEFT = 4;
		private const int EDITOR_BORDER_RIGHT = 4;
		
		#endregion
		
		#region ================== Variables
		
		protected ScriptEditorControl editor;
		
		#endregion
		
		#region ================== Properties
		
		public virtual bool ExplicitSave { get { return true; } }
		public virtual bool IsSaveAsRequired { get { return true; } }
		public virtual bool IsClosable { get { return true; } }
		public bool IsChanged { get { return editor.IsChanged; } }
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public ScriptDocumentTab()
		{
			// Make the script control
			editor = new ScriptEditorControl();
			editor.Location = new Point(EDITOR_BORDER_LEFT, EDITOR_BORDER_TOP);
			editor.Size = new Size(this.ClientSize.Width - EDITOR_BORDER_LEFT - EDITOR_BORDER_RIGHT,
								   this.ClientSize.Height - EDITOR_BORDER_TOP - EDITOR_BORDER_BOTTOM);
			editor.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			editor.Name = "editor";
			editor.TabIndex = 0;
			this.Controls.Add(editor);
		}
		
		// Disposer
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
		
		#endregion
		
		#region ================== Methods
		
		// This saves the document (used for both explicit and implicit)
		// Return true when successfully saved
		public virtual bool Save()
		{
			return false;
		}
		
		// This saves the document to a new file
		// Return true when successfully saved
		public virtual bool SaveAs(string filename)
		{
			return false;
		}
		
		// Call this to set the tab title
		protected void SetTitle(string title)
		{
			this.Text = title;
		}
		
		#endregion
		
		#region ================== Events
		
		#endregion
	}
}
