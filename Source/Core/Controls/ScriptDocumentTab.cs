
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
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Compilers;
//mxd
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.GZBuilder.GZDoom;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal abstract class ScriptDocumentTab : TabPage
	{
		#region ================== Constants

		private const int NAVIGATOR_BORDER_TOP = 8; //mxd
		private const int EDITOR_BORDER_TOP = 33;
		private const int EDITOR_BORDER_BOTTOM = 4;
		private const int EDITOR_BORDER_LEFT = 4;
		private const int EDITOR_BORDER_RIGHT = 4;
		
		#endregion
		
		#region ================== Variables
		
		// The script edit control
		protected ScriptEditorControl editor;
		//mxd
		protected ComboBox navigator;

		// Derived classes must set this!
		protected ScriptConfiguration config;
		
		// The panel we're on
		protected ScriptEditorPanel panel;
		
		#endregion

		#region ================== Properties

		public virtual bool ExplicitSave { get { return true; } }
		public virtual bool IsSaveAsRequired { get { return true; } }
		public virtual bool IsClosable { get { return true; } }
		public virtual bool IsReconfigurable { get { return true; } }
		public virtual string Filename { get { return null; } }
		public ScriptEditorPanel Panel { get { return panel; } }
		public bool IsChanged { get { return editor.IsChanged; } }
		public int SelectionStart { get { return editor.SelectionStart; } set { editor.SelectionStart = value; } }
		public int SelectionEnd { get { return editor.SelectionEnd; } set { editor.SelectionEnd = value; } }
		public ScriptConfiguration Config { get { return config; } }
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public ScriptDocumentTab(ScriptEditorPanel panel)
		{
			// Keep panel
			this.panel = panel;

			//mxd
			navigator = new ComboBox();
			navigator.Location = new Point(EDITOR_BORDER_LEFT, NAVIGATOR_BORDER_TOP);
			navigator.Width = this.ClientSize.Width - EDITOR_BORDER_LEFT - EDITOR_BORDER_RIGHT;
			navigator.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			navigator.DropDownStyle = ComboBoxStyle.DropDownList;
			navigator.Name = "navigator";
			navigator.TabStop = true;
			navigator.TabIndex = 0;
			navigator.DropDown += navigator_DropDown;
			this.Controls.Add(navigator);
			
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
		}
		
		// Disposer
		protected override void Dispose(bool disposing)
		{
			// Remove events
			editor.OnExplicitSaveTab -= panel.ExplicitSaveCurrentTab;
			editor.OnOpenScriptBrowser -= panel.OpenBrowseScript;
			editor.OnOpenFindAndReplace -= panel.OpenFindAndReplace;
			editor.OnFindNext -= panel.FindNext;
			
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
			updateNavigator(); //mxd
		}

		// Call this to set the tab title
		protected void SetTitle(string title)
		{
			this.Text = title;
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
		
		// Find next result
		public bool FindNext(FindReplaceOptions options)
		{
			byte[] data = editor.GetText();
			string text = Encoding.GetEncoding(config.CodePage).GetString(data);
			StringComparison mode = options.CaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
			int startpos = Math.Max(editor.SelectionStart, editor.SelectionEnd);
			bool wrapped = false;
			
			while(true)
			{
				int result = text.IndexOf(options.FindText, startpos, mode);
				if(result > -1)
				{
					// Check to see if it is the whole word
					if(options.WholeWord)
					{
						// Veryfy that we have found a whole word
						string foundword = editor.GetWordAt(result + 1);
						if(foundword.Length != options.FindText.Length)
						{
							startpos = result + 1;
							result = -1;
						}
					}
					
					// Still ok?
					if(result > -1)
					{
						// Select the result
						editor.SelectionStart = result;
						editor.SelectionEnd = result + options.FindText.Length;
						editor.EnsureLineVisible(editor.LineFromPosition(editor.SelectionEnd));
						return true;
					}
				}
				else
				{
					// If we haven't tried from the start, try from the start now
					if((startpos > 0) && !wrapped)
					{
						startpos = 0;
						wrapped = true;
					}
					else
					{
						// Can't find it
						return false;
					}
				}
			}
		}
		
		// This replaces the selection with the given text
		public void ReplaceSelection(string replacement)
		{
			editor.ReplaceSelection(replacement);
		}
		
		// This returns the selected text
		public string GetSelectedText()
		{
			byte[] data = editor.GetText();
			string text = Encoding.GetEncoding(config.CodePage).GetString(data);
			if(editor.SelectionStart < editor.SelectionEnd)
				return text.Substring(editor.SelectionStart, editor.SelectionEnd - editor.SelectionStart);
			else
				return "";
		}

		//mxd
		protected void updateNavigator() {
			//mxd. known script type?
			if (Array.IndexOf(ScriptTypes.TYPES, config.Description) != -1) {
				updateNavigator(new MemoryStream(editor.GetText()), config.Description);
				navigator.Enabled = true;
				navigator.SelectedIndexChanged += navigator_SelectedIndexChanged;
			}else{
				navigator.Items.Clear();
				navigator.Enabled = false;
			}
		}

		//mxd
		private void updateNavigator(MemoryStream stream, string scriptType) {
			if (scriptType == ScriptTypes.TYPES[(int)ScriptType.ACS]) {
				updateNavigatorAcs(stream);
			} else if (scriptType == ScriptTypes.TYPES[(int)ScriptType.MODELDEF]) {
				updateNavigatorModeldef(stream);
			} else if (scriptType == ScriptTypes.TYPES[(int)ScriptType.DECORATE]) {
				updateNavigatorDecorate(stream);
			}
		}

		//mxd
		private void updateNavigatorDecorate(MemoryStream stream) {
			if (stream == null) return;

			navigator.Items.Clear();

			DecorateParserSE parser = new DecorateParserSE();
			parser.Parse(stream, "DECORATE");

			if (parser.Actors.Count == 0) return; 

			navigator.Items.AddRange(parser.Actors.ToArray());
		}

		//mxd
		private void updateNavigatorModeldef(MemoryStream stream) {
			if (stream == null) return;

			navigator.Items.Clear();

			ModeldefParserSE parser = new ModeldefParserSE();
			parser.Parse(stream, "MODELDEF");

			if (parser.Models.Count == 0) return;

			navigator.Items.AddRange(parser.Models.ToArray());
		}

		//mxd
		private void updateNavigatorAcs(MemoryStream stream) {
			if (stream == null) return;
			
			navigator.Items.Clear();

			AcsParserSE parser = new AcsParserSE();
			parser.Parse(stream, "ACS");

			if (parser.NamedScripts.Count == 0 && parser.NumberedScripts.Count == 0) return;

			if(General.Map.UDMF)
				navigator.Items.AddRange(parser.NamedScripts.ToArray());

			navigator.Items.AddRange(parser.NumberedScripts.ToArray());
		}
		
		//mxd
		internal ScriptType VerifyScriptType() {
			ScriptTypeParserSE parser = new ScriptTypeParserSE();
			if (parser.Parse(new MemoryStream(editor.GetText()), config.Description)) {
				if (parser.ScriptType != (int)ScriptType.UNKNOWN && config.Description != ScriptTypes.TYPES[(int)parser.ScriptType])
					return parser.ScriptType;
			}
			return ScriptType.UNKNOWN;
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
		protected void navigator_SelectedIndexChanged(object sender, EventArgs e) {
			if (navigator.SelectedItem is ScriptItem) {
				ScriptItem si = navigator.SelectedItem as ScriptItem;
				editor.EnsureLineVisible(editor.LineFromPosition(si.SelectionStart));
				editor.SelectionStart = si.SelectionStart;
				editor.SelectionEnd = si.SelectionEnd;
				
				// Focus to the editor!
				editor.Focus();
				editor.GrabFocus();
			}
		}

		//mxd
		protected void navigator_DropDown(object sender, EventArgs e) {
			if(editor.IsChanged) updateNavigator();
		}
		
		#endregion
	}
}
