
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
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Compilers;
using CodeImp.DoomBuilder.ZDoom.Scripting;
using ScintillaNET;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal abstract class ScriptDocumentTab : TabPage
	{
		#region ================== Constants

		private const int EDITOR_BORDER_TOP = 4;
		private const int EDITOR_BORDER_BOTTOM = 4;
		private const int EDITOR_BORDER_LEFT = 4;
		private const int EDITOR_BORDER_RIGHT = 4;
		
		#endregion
		
		#region ================== Variables
		
		// The script edit control
		protected readonly ScriptEditorControl editor;
		private bool preventchanges; //mxd
		private string title; //mxd

		// Derived classes must set this!
		protected ScriptConfiguration config;
		
		// The panel we're on
		protected readonly ScriptEditorPanel panel;
		
		#endregion

		#region ================== Properties

		public virtual bool ExplicitSave { get { return true; } }
		public virtual bool IsSaveAsRequired { get { return true; } }
		public virtual bool IsClosable { get { return true; } }
		public virtual bool IsReconfigurable { get { return true; } }
		public virtual string Filename { get { return null; } }
		public ScriptEditorPanel Panel { get { return panel; } }
		internal Scintilla Scintilla { get { return editor.Scintilla; } } //mxd
		public string Title { get { return title; } } //mxd
		public bool IsChanged { get { return editor.IsChanged; } }
		public int SelectionStart { get { return editor.SelectionStart; } set { editor.SelectionStart = value; } }
		public int SelectionEnd { get { return editor.SelectionEnd; } set { editor.SelectionEnd = value; } }
		public bool ShowWhitespace { get { return editor.ShowWhitespace; } set { editor.ShowWhitespace = value; } } //mxd
		public bool WrapLongLines { get { return editor.WrapLongLines; } set { editor.WrapLongLines = value; } } //mxd
		public string SelectedText { get { return editor.SelectedText; } } //mxd
		public ScriptConfiguration Config { get { return config; } }
		
		#endregion
		
		#region ================== Events (mxd)

		public new event EventHandler OnTextChanged; //mxd

		#endregion

		#region ================== Constructor
		
		// Constructor
		protected ScriptDocumentTab(ScriptEditorPanel panel, ScriptConfiguration config)
		{
			// Keep panel and config
			this.panel = panel;
			this.config = config; //mxd
			
			// Make the script control
			editor = new ScriptEditorControl();
			editor.Location = new Point(EDITOR_BORDER_LEFT, EDITOR_BORDER_TOP);
			editor.Size = new Size(this.ClientSize.Width - EDITOR_BORDER_LEFT - EDITOR_BORDER_RIGHT,
								   this.ClientSize.Height - EDITOR_BORDER_TOP - EDITOR_BORDER_BOTTOM);
			editor.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			editor.Name = "editor";
			editor.TabStop = true;
			editor.TabIndex = 1;
			this.Controls.Add(editor);

			// Bind events
			editor.OnExplicitSaveTab += panel.ExplicitSaveCurrentTab;
			editor.OnOpenScriptBrowser += panel.OpenBrowseScript;
			editor.OnOpenFindAndReplace += panel.OpenFindAndReplace;
			editor.OnFindNext += panel.FindNext;
			editor.OnFindPrevious += panel.FindPrevious; //mxd
			editor.OnTextChanged += editor_TextChanged; //mxd

			//mxd. Bind functionbar events
			editor.OnFunctionBarDropDown += functionbar_DropDown;

			//mxd. Setup styles
			editor.SetupStyles(config);
		}
		
		// Disposer
		protected override void Dispose(bool disposing)
		{
			// Remove events
			editor.OnExplicitSaveTab -= panel.ExplicitSaveCurrentTab;
			editor.OnOpenScriptBrowser -= panel.OpenBrowseScript;
			editor.OnOpenFindAndReplace -= panel.OpenFindAndReplace;
			editor.OnFindNext -= panel.FindNext;
			editor.OnFindPrevious -= panel.FindPrevious; //mxd
			editor.OnTextChanged -= editor_TextChanged; //mxd
			
			base.Dispose(disposing);
		}
		
		#endregion
		
		#region ================== Methods

		// This launches keyword help website
		public bool LaunchKeywordHelp()
		{
			return editor.LaunchKeywordHelp();
		}
		
		// This refreshes the style settings
		public virtual void RefreshSettings()
		{
			editor.RefreshStyle();
		}
		
		// This moves the caret to the given line
		public virtual void MoveToLine(int linenumber)
		{
			editor.MoveToLine(linenumber);
		}
		
		// This clears all marks
		public virtual void ClearMarks()
		{
			editor.ClearMarks();
		}
		
		// This creates error marks for errors that apply to this file
		public virtual void MarkScriptErrors(IEnumerable<CompilerError> errors)
		{
			// Clear all marks
			ClearMarks();
			
			// Go for all errors that apply to this script
			foreach(CompilerError e in errors)
			{
				if(VerifyErrorForScript(e))
				{
					// Add a mark on the line where this error occurred
					editor.AddMark(e.linenumber);
				}
			}
		}
		
		// This verifies if the specified error applies to this script
		public virtual bool VerifyErrorForScript(CompilerError e)
		{
			return false;
		}

		// This compiles the script
		public virtual void Compile() { }

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
		
		// This changes the script configurations
		public virtual void ChangeScriptConfig(ScriptConfiguration newconfig)
		{
			config = newconfig; //mxd
			editor.SetupStyles(newconfig); //mxd
			List<CompilerError> errors = UpdateNavigator(); //mxd
			if(panel.ActiveTab == this) panel.ShowErrors(errors); //mxd
		}

		// Call this to set the tab title
		protected void SetTitle(string title)
		{
			this.title = title; //mxd
			base.Text = (editor.IsChanged ? "\u25CF " + title : title); //mxd
		}

		//mxd
		protected void UpdateTitle()
		{
			SetTitle(title);
		}

		// Perform undo
		public void Undo()
		{
			editor.Undo();
		}

		// Perform redo
		public void Redo()
		{
			editor.Redo();
		}

		// Perform cut
		public void Cut()
		{
			editor.Cut();
		}

		// Perform copy
		public void Copy()
		{
			editor.Copy();
		}

		// Perform paste
		public void Paste()
		{
			editor.Paste();
		}

		// Find next result (mxd)
		public bool FindNext(FindReplaceOptions options)
		{
			return FindNext(options, false);
		}
		
		// Find next result
		public bool FindNext(FindReplaceOptions options, bool useselectionstart)
		{
			return editor.FindNext(options, useselectionstart);
		}

		// Find previous result (mxd)
		public bool FindPrevious(FindReplaceOptions options)
		{
			return editor.FindPrevious(options);
		}
		
		// This replaces the selection with the given text
		public void ReplaceSelection(string replacement)
		{
			editor.ReplaceSelection(replacement);
		}

		//mxd
		internal List<CompilerError> UpdateNavigator()
		{
			return editor.UpdateNavigator(this);
		}
		
		//mxd. TODO: remove this
		internal ScriptType VerifyScriptType() 
		{
			ScriptTypeParserSE parser = new ScriptTypeParserSE();
			TextResourceData data = new TextResourceData(new MemoryStream(editor.GetText()), new DataLocation(), config.Description, false);
			
			if(parser.Parse(data, false))
			{
				if(parser.ScriptType != ScriptType.UNKNOWN && config.ScriptType != parser.ScriptType)
					return parser.ScriptType;
			}

			if(parser.HasError)
				panel.ShowErrors(new List<CompilerError> { new CompilerError(parser.ErrorDescription, parser.ErrorSource, parser.ErrorLine) });

			return ScriptType.UNKNOWN;
		}

		//mxd
		internal void InsertSnippet(string name)
		{
			string[] lines = config.GetSnippet(name);
			if(lines != null) editor.InsertSnippet(lines);
		}

		//mxd
		internal void IndentSelection(bool indent)
		{
			editor.IndentSelection(indent);
		}

		//mxd
		internal void SetViewSettings(ScriptDocumentSettings settings)
		{
			// Text must be exactly the same
			long hash = MurmurHash2.Hash(Text);
			bool applyfolding = General.Settings.ScriptShowFolding && (Scintilla.Lexer == Lexer.Cpp || Scintilla.Lexer == Lexer.CppNoCase);
			if(hash == settings.Hash)
			{
				// Restore fold levels
				if(applyfolding) ApplyFolding(settings.FoldLevels ?? GetFoldLevels());

				// Restore scroll
				Scintilla.FirstVisibleLine = settings.FirstVisibleLine;

				// Restore caret position
				Scintilla.SetEmptySelection(settings.CaretPosition);
			}
			// Do what Visual Studio does: fold all #regions 
			else if(applyfolding)
			{
				ApplyFolding(GetFoldLevels());
			}
		}

		internal void SetDefaultViewSettings()
		{
			if(General.Settings.ScriptShowFolding && (Scintilla.Lexer == Lexer.Cpp || Scintilla.Lexer == Lexer.CppNoCase))
				ApplyFolding(GetFoldLevels());
		}

		private void ApplyFolding(Dictionary<int, HashSet<int>> foldlevelsarr)
		{
			// We'll want to fold deeper levels first...
			int[] fl = new int[foldlevelsarr.Keys.Count];
			foldlevelsarr.Keys.CopyTo(fl, 0);

			List<int> foldlevels = new List<int>(fl);
			foldlevels.Sort((a, b) => -1 * a.CompareTo(b)); // Sort in descending order

			foreach(int level in foldlevels)
			{
				foreach(int line in foldlevelsarr[level])
					Scintilla.Lines[line].FoldLine(FoldAction.Contract);
			}
		}

		private Dictionary<int, HashSet<int>> GetFoldLevels()
		{
			Dictionary<int, HashSet<int>> foldlevels = new Dictionary<int, HashSet<int>>();
			int foldlevel = NativeMethods.SC_FOLDLEVELBASE;

			for(int i = 0; i < Scintilla.Lines.Count; i++)
			{
				string line = Scintilla.Lines[i].Text.TrimStart();
				if(line.StartsWith("#region", true, CultureInfo.InvariantCulture))
				{
					foldlevel++;
					if(!foldlevels.ContainsKey(foldlevel)) foldlevels.Add(foldlevel, new HashSet<int>());
					foldlevels[foldlevel].Add(i);
				}
				else if(line.StartsWith("#endregion", true, CultureInfo.InvariantCulture) && foldlevel > NativeMethods.SC_FOLDLEVELBASE)
				{
					foldlevel--;
				}
			}

			return foldlevels;
		}

		//mxd
		internal ScriptDocumentSettings GetViewSettings()
		{
			Dictionary<int, HashSet<int>> foldlevels = new Dictionary<int, HashSet<int>>();

			for(int i = 0; i < Scintilla.Lines.Count; i++)
			{
				if(!Scintilla.Lines[i].Expanded)
				{
					if(!foldlevels.ContainsKey(Scintilla.Lines[i].FoldLevel))
						foldlevels.Add(Scintilla.Lines[i].FoldLevel, new HashSet<int>());
					foldlevels[Scintilla.Lines[i].FoldLevel].Add(i);
				}
			}

			return new ScriptDocumentSettings
			{
				Filename = this.Filename,
				FoldLevels = foldlevels,
				CaretPosition = Scintilla.SelectionStart,
				IsActiveTab = (this.Panel.ActiveTab == this),
				FirstVisibleLine = Scintilla.FirstVisibleLine,
				Hash = MurmurHash2.Hash(Text),
			};
		}

		#endregion
		
		#region ================== Events
		
		// Mouse released
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			// Focus to the editor!
			editor.Focus();
			editor.GrabFocus();
		}
		
		// Receiving focus?
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			
			// Focus to the editor!
			editor.Focus();
			editor.GrabFocus();
		}

		//mxd
		private void functionbar_DropDown(object sender, EventArgs e) 
		{
			if(!preventchanges && editor.IsChanged) panel.ShowErrors(UpdateNavigator());
		}

		//mxd
		private void editor_TextChanged(object sender, EventArgs eventArgs)
		{
			UpdateTitle();
			if(OnTextChanged != null) OnTextChanged(this, EventArgs.Empty);
		}
		
		#endregion
	}
}
