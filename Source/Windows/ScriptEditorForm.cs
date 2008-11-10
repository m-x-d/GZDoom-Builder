
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
using CodeImp.DoomBuilder.Map;
using System.IO;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class ScriptEditorForm : DelayedForm
	{
		#region ================== Variables
		
		private bool appclose;
		
		#endregion
		
		#region ================== Properties
		
		public ScriptEditorPanel Editor { get { return editor; } }
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public ScriptEditorForm()
		{
			InitializeComponent();
			editor.Initialize();
		}
		
		#endregion
		
		#region ================== Methods
		
		// This asks to save files and returns the result
		// Also does implicit saves
		// Returns false when cancelled by the user
		public bool AskSaveAll()
		{
			// Implicit-save the script lumps
			editor.ImplicitSave();
			
			// Save other scripts
			return editor.AskSaveAll();
		}
		
		// Close the window
		new public void Close()
		{
			appclose = true;
			base.Close();
		}
		
		#endregion
		
		#region ================== Events
		
		// Window is shown
		private void ScriptEditorForm_Shown(object sender, EventArgs e)
		{
			// Focus to script editor
			editor.ForceFocus();
		}
		
		// Window is closing
		private void ScriptEditorForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Only when closed by the user
			if(!appclose)
			{
				// Ask to save scripts
				if(AskSaveAll())
				{
					// Let the general call close the editor
					General.Map.CloseScriptEditor(true);
				}
				else
				{
					// Cancel
					e.Cancel = true;
				}
			}
		}
		
		#endregion
	}
}