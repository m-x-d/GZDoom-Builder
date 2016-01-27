
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

using System.Collections.Generic;
using System.IO;
using CodeImp.DoomBuilder.Compilers;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal sealed class ScriptLumpDocumentTab : ScriptDocumentTab
	{
		#region ================== Constants
		
		#endregion

		#region ================== Variables

		private readonly string lumpname;
		private readonly bool ismapheader;
		
		#endregion
		
		#region ================== Properties
		
		public override bool ExplicitSave { get { return false; } }
		public override bool IsSaveAsRequired { get { return false; } }
		public override bool IsClosable { get { return false; } }
		public override bool IsReconfigurable { get { return false; } }
		public override string Filename { get { return lumpname; } } //mxd
		
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
				editor.SetText(stream.ToArray()); //mxd
				editor.ClearUndoRedo();
				UpdateNavigator(); //mxd
			}

			// Set title
			SetTitle(ismapheader ? General.Map.Options.CurrentName : this.lumpname.ToUpper());
		}
		
		#endregion
		
		#region ================== Methods
		
		// Compile script
		public override void Compile()
		{
			//mxd. List of errors. UpdateScriptNames can return errors and also updates acs includes list
			List<CompilerError> errors = (config.ScriptType == ScriptType.ACS ? General.Map.UpdateScriptNames() : new List<CompilerError>());

			//mxd. Errors already?..
			if(errors.Count > 0)
			{
				// Feed errors to panel
				panel.ShowErrors(errors);
				return;
			}

			// Compile
			General.Map.CompileLump((ismapheader ? MapManager.CONFIG_MAP_HEADER : lumpname), true);
			
			//mxd. Update script navigator
			UpdateNavigator();

			// Feed errors to panel
			panel.ShowErrors(General.Map.Errors);
		}
		
		// Implicit save
		public override bool Save()
		{
			// Store the lump data
			MemoryStream stream = new MemoryStream(editor.GetText());
			General.Map.SetLumpData(lumpname, stream);
			editor.SetSavePoint(); //mxd
			UpdateTitle(); //mxd
			return true;
		}

		// This checks if a script error applies to this script
		public override bool VerifyErrorForScript(CompilerError e)
		{
			return (string.Compare(e.filename, "?" + lumpname, true) == 0);
		}
		
		#endregion
		
		#region ================== Events

		#endregion
	}
}
