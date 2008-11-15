
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
using System.IO;
using CodeImp.DoomBuilder.Compilers;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal sealed class ScriptLumpDocumentTab : ScriptDocumentTab
	{
		#region ================== Constants
		
		#endregion

		#region ================== Variables

		private string lumpname;
		
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
			this.lumpname = lumpname;
			this.config = config;
			editor.SetupStyles(config);
			
			// Load the lump data
			MemoryStream stream = General.Map.GetLumpData(lumpname);
			if(stream != null)
			{
				StreamReader reader = new StreamReader(stream);
				stream.Seek(0, SeekOrigin.Begin);
				editor.Text = reader.ReadToEnd();
				editor.ClearUndoRedo();
			}
			
			// Done
			SetTitle(lumpname.ToUpper());
		}
		
		// Disposer
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
		
		#endregion
		
		#region ================== Methods
		
		// Compile script
		public override void Compile()
		{
			// Compile
			General.Map.CompileLump(lumpname, true);

			// Feed errors to panel
			panel.ShowErrors(General.Map.Errors);
		}
		
		// Implicit save
		public override bool Save()
		{
			// Store the lump data
			byte[] data = Encoding.ASCII.GetBytes(editor.Text);
			MemoryStream stream = new MemoryStream(data);
			General.Map.SetLumpData(lumpname, stream);
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
