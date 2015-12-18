
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
		protected readonly ScriptEditorControl editor;
		protected readonly ComboBox navigator; //mxd
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
		public new string Text { get { return title; } } //mxd
		public bool IsChanged { get { return editor.IsChanged; } internal set { editor.IsChanged = value; } } //mxd. Added setter
		public int SelectionStart { get { return editor.SelectionStart; } set { editor.SelectionStart = value; } }
		public int SelectionEnd { get { return editor.SelectionEnd; } set { editor.SelectionEnd = value; } }
		public ScriptConfiguration Config { get { return config; } }
		
		#endregion
		
		#region ================== Events (mxd)

		public new event EventHandler OnTextChanged; //mxd

		#endregion

		#region ================== Constructor
		
		// Constructor
		protected ScriptDocumentTab(ScriptEditorPanel panel)
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
			navigator.SelectedIndexChanged += navigator_SelectedIndexChanged;
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
			editor.OnFindPrevious += panel.FindPrevious; //mxd
			editor.OnTextChanged += editor_TextChanged; //mxd
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
			UpdateNavigator(); //mxd
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
			byte[] data = editor.GetText();
			string text = Encoding.GetEncoding(config.CodePage).GetString(data);
			StringComparison mode = options.CaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
			int startpos = (useselectionstart ? Math.Min(editor.SelectionStart, editor.SelectionEnd) : Math.Max(editor.SelectionStart, editor.SelectionEnd)); //mxd
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

		// Find previous result (mxd)
		public bool FindPrevious(FindReplaceOptions options) 
		{
			bool wrapped = false;
			byte[] data = editor.GetText();
			string text = Encoding.GetEncoding(config.CodePage).GetString(data);
			StringComparison mode = options.CaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
			int endpos = Math.Min(editor.SelectionStart, editor.SelectionEnd) - 1;
			if(endpos < 0)
			{
				endpos = text.Length - 1;
				wrapped = true;
			}

			while(true) 
			{
				int result = text.LastIndexOf(options.FindText, endpos, mode);
				if(result > -1) 
				{
					// Check to see if it is the whole word
					if(options.WholeWord) 
					{
						// Veryfy that we have found a whole word
						string foundword = editor.GetWordAt(result + 1);
						if(foundword.Length != options.FindText.Length) 
						{
							endpos = result - 1;
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
					// If we haven't tried from the end, try from the end now
					if(!wrapped) 
					{
						endpos = Math.Max(0, text.Length - 2);
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
			return "";
		}

		//mxd
		protected void UpdateNavigator() 
		{
			switch(config.ScriptType)
			{
				case ScriptType.ACS:
					UpdateNavigatorAcs(new MemoryStream(editor.GetText()));
					break;

				case ScriptType.DECORATE:
					UpdateNavigatorDecorate(new MemoryStream(editor.GetText()));
					break;

				case ScriptType.MODELDEF:
					UpdateNavigatorModeldef(new MemoryStream(editor.GetText()));
					break;

				default: // Unsupported script type. Just clear the items
					navigator.Items.Clear();
					break;
			}

			// Put some text in the navigator (but don't actually trigger selection event)
			navigator.Enabled = (navigator.Items.Count > 0);
			if(navigator.Items.Count > 0)
			{
				preventchanges = true;
				navigator.Text = navigator.Items[0].ToString();
				preventchanges = false;
			}
		}

		//mxd
		private void UpdateNavigatorDecorate(MemoryStream stream) 
		{
			if(stream == null) return;
			navigator.Items.Clear();

			DecorateParserSE parser = new DecorateParserSE();
			if(parser.Parse(stream, "DECORATE", false))
			{
				navigator.Items.AddRange(parser.Actors.ToArray());
			}

			if(parser.HasError)
			{
				panel.ShowErrors(new List<CompilerError> { new CompilerError(parser.ErrorDescription, parser.ErrorSource, parser.ErrorLine) });
			}
		}

		//mxd
		private void UpdateNavigatorModeldef(MemoryStream stream) 
		{
			if(stream == null) return;
			navigator.Items.Clear();

			ModeldefParserSE parser = new ModeldefParserSE();
			if(parser.Parse(stream, "MODELDEF", false))
			{
				navigator.Items.AddRange(parser.Models.ToArray());
			}

			if(parser.HasError)
			{
				panel.ShowErrors(new List<CompilerError> { new CompilerError(parser.ErrorDescription, parser.ErrorSource, parser.ErrorLine) });
			}
		}

		//mxd
		private void UpdateNavigatorAcs(MemoryStream stream) 
		{
			if(stream == null) return;
			navigator.Items.Clear();

			AcsParserSE parser = new AcsParserSE { AddArgumentsToScriptNames = true, IsMapScriptsLump = this is ScriptLumpDocumentTab };
			if(parser.Parse(stream, "SCRIPTS", false))
			{
				navigator.Items.AddRange(parser.NamedScripts.ToArray());
				navigator.Items.AddRange(parser.NumberedScripts.ToArray());
				navigator.Items.AddRange(parser.Functions.ToArray());
			}

			if(parser.HasError)
			{
				panel.ShowErrors(new List<CompilerError> { new CompilerError(parser.ErrorDescription, parser.ErrorSource, parser.ErrorLine) });
			}
		}
		
		//mxd
		internal ScriptType VerifyScriptType() 
		{
			ScriptTypeParserSE parser = new ScriptTypeParserSE();
			if(parser.Parse(new MemoryStream(editor.GetText()), config.Description, false)) 
			{
				if(parser.ScriptType != ScriptType.UNKNOWN && config.ScriptType != parser.ScriptType)
					return parser.ScriptType;
			}

			if(parser.HasError)
			{
				panel.ShowErrors(new List<CompilerError> { new CompilerError(parser.ErrorDescription, parser.ErrorSource, parser.ErrorLine) });
			}

			return ScriptType.UNKNOWN;
		}

		//mxd
		internal void InsertSnippet(string[] lines) 
		{
			editor.InsertSnippet(lines);
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
		private void navigator_SelectedIndexChanged(object sender, EventArgs e) 
		{
			if(!preventchanges && navigator.SelectedItem is ScriptItem) 
			{
				ScriptItem si = navigator.SelectedItem as ScriptItem;
				editor.EnsureLineVisible(editor.LineFromPosition(si.CursorPosition));
				editor.SelectionStart = si.CursorPosition;
				editor.SelectionEnd = si.CursorPosition;
				
				// Focus to the editor!
				editor.Focus();
				editor.GrabFocus();
			}
		}

		//mxd
		private void navigator_DropDown(object sender, EventArgs e) 
		{
			if(!preventchanges && editor.IsChanged) UpdateNavigator();
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
