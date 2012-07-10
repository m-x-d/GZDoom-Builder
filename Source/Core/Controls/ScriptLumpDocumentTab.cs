
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
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.IO;
using System.IO;
using CodeImp.DoomBuilder.Compilers;
//mxd
using CodeImp.DoomBuilder.GZBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal sealed class ScriptLumpDocumentTab : ScriptDocumentTab
	{
		#region ================== Constants
		
		#endregion

		#region ================== Variables

		private string lumpname;
		private bool ismapheader;
		
		#endregion
		
		#region ================== Properties
		
		public override bool ExplicitSave { get { return false; } }
		public override bool IsSaveAsRequired { get { return false; } }
		public override bool IsClosable { get { return false; } }
		public override bool IsReconfigurable { get { return false; } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		public ScriptLumpDocumentTab(ScriptEditorPanel panel, string lumpname, ScriptConfiguration config) : base(panel)
		{
			// Initialize
			if(lumpname == MapManager.CONFIG_MAP_HEADER)
			{
				this.lumpname = MapManager.TEMP_MAP_HEADER;
				this.ismapheader = true;
			}
			else
			{
				this.lumpname = lumpname;
				this.ismapheader = false;
			}
			
			this.config = config;
			editor.SetupStyles(config);
			
			// Load the lump data
			MemoryStream stream = General.Map.GetLumpData(this.lumpname);
			if(stream != null)
			{
				editor.SetText(stream.ToArray());
				editor.ClearUndoRedo();
			}
			
			// Done
			if(ismapheader)
				SetTitle(General.Map.Options.CurrentName);
			else
				SetTitle(this.lumpname.ToUpper());

            //mxd
            if (this.Text == "SCRIPTS") {
                updateNavigator();
                navigator.SelectedIndexChanged += new EventHandler(navigator_SelectedIndexChanged);
            }else{
                navigator.Enabled = false;
            }
		}
		
		// Disposer
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
		
		#endregion
		
		#region ================== Methods

        //mxd
        private void updateNavigator() {
            string selectedItem = "";
            int selectedIndex = 0;
            if (navigator.SelectedIndex != -1) selectedItem = navigator.Text;
            
            navigator.Items.Clear();
            
            //add named scripts
            int i = 0;
            if (General.Map.UDMF) {
                ScriptItem[] namedScripts = new ScriptItem[General.Map.NamedScripts.Count];
                foreach (ScriptItem si in General.Map.NamedScripts) {
                    namedScripts[i++] = si;
                    if (si.Name == selectedItem) selectedIndex = i - 1;
                }
                navigator.Items.AddRange(namedScripts);
            }

            //add numbered scripts
            ScriptItem[] numberedScripts = new ScriptItem[General.Map.NumberedScripts.Count];
            int c = 0;
            foreach (ScriptItem si in General.Map.NumberedScripts) {
                numberedScripts[c++] = si;
                if (si.Name == selectedItem) selectedIndex = i - 1 + c;
            }
            navigator.Items.AddRange(numberedScripts);

            if (navigator.Items.Count > 0) navigator.SelectedIndex = selectedIndex;
        }
		
		// Compile script
		public override void Compile()
		{
			// Compile
			if(ismapheader)
				General.Map.CompileLump(MapManager.CONFIG_MAP_HEADER, true);
			else
				General.Map.CompileLump(lumpname, true);

			// Feed errors to panel
			panel.ShowErrors(General.Map.Errors);

            //mxd
            if (General.Map.Errors.Count == 0) {
                General.Map.UpdateScriptNames();
                navigator.SelectedIndexChanged -= navigator_SelectedIndexChanged;
                updateNavigator();
                navigator.SelectedIndexChanged += new EventHandler(navigator_SelectedIndexChanged);
            }
		}
		
		// Implicit save
		public override bool Save()
		{
			// Store the lump data
			MemoryStream stream = new MemoryStream(editor.GetText());
			General.Map.SetLumpData(lumpname, stream);
			editor.IsChanged = false;
			return true;
		}

		// This checks if a script error applies to this script
		public override bool VerifyErrorForScript(CompilerError e)
		{
			return (string.Compare(e.filename, "?" + lumpname, true) == 0);
		}
		
		#endregion
		
		#region ================== Events

        //mxd
        private void navigator_SelectedIndexChanged(object sender, EventArgs e) {
            if (navigator.SelectedItem is ScriptItem) {
                ScriptItem si = navigator.SelectedItem as ScriptItem;
                editor.EnsureLineVisible(editor.LineFromPosition(si.SelectionStart));
                editor.SelectionStart = si.SelectionStart;
                editor.SelectionEnd = si.SelectionEnd;
            }
        }

		#endregion
	}
}
