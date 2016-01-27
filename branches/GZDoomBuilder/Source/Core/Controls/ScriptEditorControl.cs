
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Properties;
using CodeImp.DoomBuilder.Windows;
using ScintillaNET;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class ScriptEditorControl : UserControl
	{
		#region ================== Enums

		private enum ScriptStyleType
		{
			PlainText = 0,
			Keyword = 1,
			Constant = 2,
			Comment = 3,
			Literal = 4,
			LineNumber = 5,
			String = 6, //mxd
			Include = 7, //mxd
		}

		// Index for registered images
		private enum ImageIndex
		{
			ScriptConstant = 0,
			ScriptKeyword = 1,
			ScriptError = 2,
			ScriptSnippet = 3, //mxd
		}

		#endregion

		#region ================== Constants

		private const string LEXERS_RESOURCE = "Lexers.cfg";
		private const int MAX_BACKTRACK_LENGTH = 200;
		private const int HIGHLIGHT_INDICATOR = 8; //mxd. Indicators 0-7 could be in use by a lexer so we'll use indicator 8 to highlight words.
		
		#endregion

		#region ================== Delegates / Events

		public delegate void ExplicitSaveTabDelegate();
		public delegate void OpenScriptBrowserDelegate();
		public delegate void OpenFindReplaceDelegate();
		public delegate void FindNextDelegate();
		public delegate void FindPreviousDelegate(); //mxd

		public event ExplicitSaveTabDelegate OnExplicitSaveTab;
		public event OpenScriptBrowserDelegate OnOpenScriptBrowser;
		public event OpenFindReplaceDelegate OnOpenFindAndReplace;
		public event FindNextDelegate OnFindNext;
		public event FindPreviousDelegate OnFindPrevious; //mxd
		public new event EventHandler OnTextChanged; //mxd

		#endregion

		#region ================== Variables
		
		// Script configuration
		private ScriptConfiguration scriptconfig;
		
		// List of keywords and constants
		private List<string> autocompletelist;

		// Style translation from Scintilla style to ScriptStyleType
		private Dictionary<int, ScriptStyleType> stylelookup;
		
		// Current position information
		private string curfunctionname = "";
		private int curargumentindex;
		private int curfunctionstartpos;
		private int linenumbercharlength; //mxd. Current max number of chars in the line number
		private int lastcaretpos; //mxd. Used in brace matching
		private int caretoffset; //mxd. Used to modify caret position after autogenerating stuff
		private bool skiptextinsert; //mxd. Gross hacks
		private bool expandcodeblock; //mxd. More gross hacks
		private string highlightedword; //mxd
		
		#endregion

		#region ================== Properties

		public bool IsChanged { get { return scriptedit.Modified; } }
		public int SelectionStart { get { return scriptedit.SelectionStart; } set { scriptedit.SelectionStart = value; } }
		public int SelectionEnd { get { return scriptedit.SelectionEnd; } set { scriptedit.SelectionEnd = value; } }
		public new string Text { get { return scriptedit.Text; } set { scriptedit.Text = value; } } //mxd
		public string SelectedText { get { return scriptedit.SelectedText; } } //mxd
		public bool ShowWhitespace { get { return scriptedit.ViewWhitespace != WhitespaceMode.Invisible; } set { scriptedit.ViewWhitespace = value ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible; } }
		public bool WrapLongLines { get { return scriptedit.WrapMode != WrapMode.None; } set { scriptedit.WrapMode = (value ? WrapMode.Char : WrapMode.None); } }
		public ComboBox FunctionBar { get { return functionbar; } } //mxd
		public Scintilla Scintilla { get { return scriptedit; } } //mxd

		#endregion

		#region ================== Contructor / Disposer

		// Constructor
		public ScriptEditorControl()
		{
			// Initialize
			InitializeComponent();
			
			// Script editor properties
			//TODO: use ScintillaNET properties instead when they become available
			scriptedit.DirectMessage(NativeMethods.SCI_SETBACKSPACEUNINDENTS, new IntPtr(1));
			scriptedit.DirectMessage(NativeMethods.SCI_SETMOUSEDOWNCAPTURES, new IntPtr(1));
			scriptedit.DirectMessage(NativeMethods.SCI_SETTABINDENTS, new IntPtr(1));

			// Symbol margin
			scriptedit.Margins[0].Type = MarginType.Symbol;
			scriptedit.Margins[0].Width = 20;
			scriptedit.Margins[0].Mask = 1 << (int)ImageIndex.ScriptError; // Error marker only
			scriptedit.Margins[0].Cursor = MarginCursor.Arrow;
			scriptedit.Margins[0].Sensitive = true;
			
			// Line numbers margin
			scriptedit.Margins[1].Type = MarginType.Number;
			scriptedit.Margins[1].Width = 16;
			scriptedit.Margins[1].Mask = 0; // No markers here

			// Spacing margin
			scriptedit.Margins[2].Type = MarginType.Symbol;
			scriptedit.Margins[2].Width = 5;
			scriptedit.Margins[2].Cursor = MarginCursor.Arrow;
			scriptedit.Margins[2].Mask = 0; // No markers here

			// Images
			RegisterAutoCompleteImage(ImageIndex.ScriptConstant, Resources.ScriptConstant);
			RegisterAutoCompleteImage(ImageIndex.ScriptKeyword, Resources.ScriptKeyword);
			RegisterAutoCompleteImage(ImageIndex.ScriptSnippet, Resources.ScriptSnippet); //mxd
			RegisterMarkerImage(ImageIndex.ScriptError, Resources.ScriptError);

			//mxd. These key combinations put odd characters in the script. Let's disable them
			scriptedit.AssignCmdKey(Keys.Control | Keys.Q, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.W, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.E, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.R, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Y, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.U, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.I, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.P, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.A, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.D, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.G, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.H, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.J, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.K, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.L, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.Z, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.X, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.C, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.Shift | Keys.V, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.B, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.N, Command.Null);
			scriptedit.AssignCmdKey(Keys.Control | Keys.M, Command.Null);

			//mxd. These key combinations are used to perform special actions. Let's disable them
			scriptedit.AssignCmdKey(Keys.F3, Command.Null); // F3 for Find Next
			scriptedit.AssignCmdKey(Keys.F2, Command.Null); // F2 for Find Previous
			scriptedit.AssignCmdKey(Keys.Control | Keys.F, Command.Null); // CTRL+F for find & replace
			scriptedit.AssignCmdKey(Keys.Control | Keys.S, Command.Null); // CTRL+S for save
			scriptedit.AssignCmdKey(Keys.Control | Keys.O, Command.Null); // CTRL+O for open
			scriptedit.AssignCmdKey(Keys.Control | Keys.Space, Command.Null); // CTRL+Space to autocomplete <- TODO: this doesn't seem to work...
		}
		
		#endregion
		
		#region ================== Public methods
		
		// This launches keyword help website
		public bool LaunchKeywordHelp()
		{
			string helpsite = scriptconfig.KeywordHelp;
			string currentword = GetCurrentWord();
			if(!string.IsNullOrEmpty(currentword) && (currentword.Length > 1) && !string.IsNullOrEmpty(helpsite))
			{
				currentword = scriptconfig.GetKeywordCase(currentword);
				helpsite = helpsite.Replace("%K", currentword);
				General.OpenWebsite(helpsite);
				return true;
			}

			return !string.IsNullOrEmpty(helpsite); //mxd
		}
		
		// This replaces the selection with the given text
		public void ReplaceSelection(string replacement)
		{
			scriptedit.ReplaceSelection(replacement); //mxd TODO: encoding check/conversion?
		}
		
		// This moves the caret to a given line and ensures the line is visible
		public void MoveToLine(int linenumber)
		{
			scriptedit.Lines[linenumber].Goto();
			EnsureLineVisible(linenumber);
		}

		// This makes sure a line is visible
		public void EnsureLineVisible(int linenumber)
		{
			// Determine target lines range
			int startline = Math.Max(0, linenumber - 4);
			int endline = Math.Min(scriptedit.Lines.Count, Math.Max(linenumber, linenumber + scriptedit.LinesOnScreen - 6));

			// Go to target line
			scriptedit.ShowLines(startline, endline);

			// We may want to do some scrolling...
			if(scriptedit.FirstVisibleLine >= startline)
				scriptedit.Lines[startline].Goto();
			else if(scriptedit.FirstVisibleLine + scriptedit.LinesOnScreen <= endline)
				scriptedit.Lines[endline].Goto();
		}

		// This returns the line for a position
		public int LineFromPosition(int position)
		{
			return scriptedit.LineFromPosition(position);
		}
		
		// This clears all marks
		public void ClearMarks()
		{
			scriptedit.MarkerDeleteAll((int)ImageIndex.ScriptError);
		}

		// This adds a mark on the given line
		public void AddMark(int linenumber)
		{
			scriptedit.Lines[linenumber].MarkerAdd((int)ImageIndex.ScriptError);
		}

		// This refreshes the style setup
		public void RefreshStyle()
		{
			// Re-setup with the same config
			SetupStyles(scriptconfig);
		}

		// This sets up the script editor with a script configuration
		public void SetupStyles(ScriptConfiguration config)
		{
			Configuration lexercfg = new Configuration();

			// Make collections
			stylelookup = new Dictionary<int, ScriptStyleType>();
			Dictionary<string, string> autocompletedict = new Dictionary<string, string>(StringComparer.Ordinal);
			
			// Keep script configuration
			scriptconfig = config;
			
			// Find a resource named Lexers.cfg
			string[] resnames = General.ThisAssembly.GetManifestResourceNames();
			foreach(string rn in resnames)
			{
				// Found one?
				if(rn.EndsWith(LEXERS_RESOURCE, StringComparison.InvariantCultureIgnoreCase))
				{
					// Get a stream from the resource
					Stream lexersdata = General.ThisAssembly.GetManifestResourceStream(rn);
					if(lexersdata != null)
					{
						StreamReader lexersreader = new StreamReader(lexersdata, Encoding.ASCII);

						// Load configuration from stream
						lexercfg.InputConfiguration(lexersreader.ReadToEnd());

						// Done with the resource
						lexersreader.Dispose();
						lexersdata.Dispose();
					}

					//mxd. We are done here
					break;
				}
			}
			
			// Check if specified lexer exists and set the lexer to use
			string lexername = "lexer" + (int)scriptconfig.Lexer;
			if(!lexercfg.SettingExists(lexername)) throw new InvalidOperationException("Unknown lexer " + scriptconfig.Lexer + " specified in script configuration!");
			scriptedit.Lexer = scriptconfig.Lexer;
			
			// Set the default style and settings
			scriptedit.Styles[Style.Default].Font = General.Settings.ScriptFontName;
			scriptedit.Styles[Style.Default].Size = General.Settings.ScriptFontSize;
			scriptedit.Styles[Style.Default].Bold = General.Settings.ScriptFontBold;
			scriptedit.Styles[Style.Default].Italic = false;
			scriptedit.Styles[Style.Default].Underline = false;
			scriptedit.Styles[Style.Default].Case = StyleCase.Mixed;
			scriptedit.Styles[Style.Default].ForeColor = General.Colors.PlainText.ToColor();
			scriptedit.Styles[Style.Default].BackColor = General.Colors.ScriptBackground.ToColor();
			scriptedit.CaretPeriod = SystemInformation.CaretBlinkTime;
			scriptedit.CaretForeColor = General.Colors.ScriptBackground.Inverse().ToColor();
			
			// Set tabulation settings
			scriptedit.UseTabs = General.Settings.ScriptUseTabs;
			scriptedit.TabWidth = General.Settings.ScriptTabWidth;
			//scriptedit.IndentWidth = General.Settings.ScriptTabWidth; // Equals to TabWidth by default
			//TODO: use ScintillaNET properties instead when they become available
			scriptedit.DirectMessage(NativeMethods.SCI_SETTABINDENTS, new IntPtr(1));
			scriptedit.DirectMessage(NativeMethods.SCI_SETBACKSPACEUNINDENTS, new IntPtr(1));
			
			// This applies the default style to all styles
			scriptedit.StyleClearAll();

			// Set the code page to use. [mxd] No longer needed?
			//scriptedit.CodePage = scriptconfig.CodePage;

			//mxd. We can't change Font or Size here because this will screw displayed tab width (because it's based on character width)...
			// Set the default to something normal (this is used by the autocomplete list)
			//scriptedit.Styles[Style.Default].Font = this.Font.Name;
			scriptedit.Styles[Style.Default].Bold = this.Font.Bold;
			scriptedit.Styles[Style.Default].Italic = this.Font.Italic;
			scriptedit.Styles[Style.Default].Underline = this.Font.Underline;
			//scriptedit.Styles[Style.Default].Size = (int)Math.Round(this.Font.SizeInPoints);

			// Set style for linenumbers and margins
			scriptedit.Styles[Style.LineNumber].BackColor = General.Colors.ScriptBackground.ToColor();
 
			//mxd. Set style for (mis)matching braces
			scriptedit.Styles[Style.BraceLight].BackColor = General.Colors.ScriptBraceHighlight.ToColor(); //Color.Cyan;
			scriptedit.Styles[Style.BraceBad].BackColor = General.Colors.ScriptBadBraceHighlight.ToColor(); //Color.Red;

			//mxd. Set whitespace color
			scriptedit.SetWhitespaceForeColor(true, General.Colors.ScriptWhitespace.ToColor());

			//mxd. Set selection colors
			scriptedit.SetSelectionForeColor(true, General.Colors.ScriptSelectionForeColor.ToColor());
			scriptedit.SetSelectionBackColor(true, General.Colors.ScriptSelectionBackColor.ToColor());

			// Clear all keywords
			for(int i = 0; i < 9; i++) scriptedit.SetKeywords(i, null);
			
			// Now go for all elements in the lexer configuration
			// We are looking for the numeric keys, because these are the
			// style index to set and the value is our ScriptStyleType
			IDictionary dic = lexercfg.ReadSetting(lexername, new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Check if this is a numeric key
				int stylenum;
				if(int.TryParse(de.Key.ToString(), out stylenum))
				{
					// Add style to lookup table
					stylelookup.Add(stylenum, (ScriptStyleType)(int)de.Value);
					
					// Apply color to style
					int colorindex;
					ScriptStyleType type = (ScriptStyleType)(int)de.Value;
					switch(type)
					{
						case ScriptStyleType.PlainText: colorindex = ColorCollection.PLAINTEXT; break;
						case ScriptStyleType.Comment: colorindex = ColorCollection.COMMENTS; break;
						case ScriptStyleType.Constant: colorindex = ColorCollection.CONSTANTS; break;
						case ScriptStyleType.Keyword: colorindex = ColorCollection.KEYWORDS; break;
						case ScriptStyleType.LineNumber: colorindex = ColorCollection.LINENUMBERS; break;
						case ScriptStyleType.Literal: colorindex = ColorCollection.LITERALS; break;
						case ScriptStyleType.String: colorindex = ColorCollection.STRINGS; break;
						case ScriptStyleType.Include: colorindex = ColorCollection.INCLUDES; break;
						default: colorindex = ColorCollection.PLAINTEXT; break;
					}

					scriptedit.Styles[stylenum].ForeColor = General.Colors.Colors[colorindex].ToColor();
					
					//mxd. Display constants as uppercase
					if(type == ScriptStyleType.Constant) scriptedit.Styles[stylenum].Case = StyleCase.Upper;
				}
			}
			
			// Create the keywords list and apply it
			string imageindex = ((int)ImageIndex.ScriptKeyword).ToString(CultureInfo.InvariantCulture);
			int keywordsindex = lexercfg.ReadSetting(lexername + ".keywordsindex", -1);
			if(keywordsindex > -1)
			{
				StringBuilder keywordslist = new StringBuilder("");
				foreach(string k in scriptconfig.Keywords)
				{
					if(keywordslist.Length > 0) keywordslist.Append(" ");
					keywordslist.Append(k);

					//mxd. Skip adding the keyword if we have a snippet with the same name
					if(!scriptconfig.Snippets.Contains(k))
						autocompletedict.Add(k.ToUpperInvariant(), k + "?" + imageindex);
				}
				string words = keywordslist.ToString();
				scriptedit.SetKeywords(keywordsindex, (scriptconfig.CaseSensitive ? words : words.ToLowerInvariant()));
			}

			// Create the constants list and apply it
			imageindex = ((int)ImageIndex.ScriptConstant).ToString(CultureInfo.InvariantCulture);
			int constantsindex = lexercfg.ReadSetting(lexername + ".constantsindex", -1);
			if(constantsindex > -1)
			{
				StringBuilder constantslist = new StringBuilder("");
				foreach(string c in scriptconfig.Constants)
				{
					if(autocompletedict.ContainsKey(c.ToUpperInvariant())) //mxd. This happens when there's a keyword and a constant with the same name...
					{
						General.ErrorLogger.Add(ErrorType.Error, "Constant '" + c + "' is double-defined in '" + scriptconfig.Description + "' script configuration!");
						continue;
					}
					
					if(constantslist.Length > 0) constantslist.Append(" ");
					constantslist.Append(c);

					//mxd. Skip adding the constant if we have a snippet with the same name
					if(!scriptconfig.Snippets.Contains(c))
						autocompletedict.Add(c.ToUpperInvariant(), c + "?" + imageindex);
				}
				string words = constantslist.ToString();
				scriptedit.SetKeywords(constantsindex, (scriptconfig.CaseSensitive ? words : words.ToLowerInvariant()));
			}

			//mxd. Create the snippets list and apply it
			imageindex = ((int)ImageIndex.ScriptSnippet).ToString(CultureInfo.InvariantCulture);
			int snippetindex = lexercfg.ReadSetting(lexername + ".snippetindex", -1);
			if(snippetindex > -1 && scriptconfig.Snippets.Count > 0) 
			{
				StringBuilder snippetslist = new StringBuilder("");
				foreach(string c in scriptconfig.Snippets) 
				{
					if(autocompletedict.ContainsKey(c.ToUpperInvariant())) continue;
					if(snippetslist.Length > 0) snippetslist.Append(" ");
					snippetslist.Append(c);
					autocompletedict.Add(c.ToUpperInvariant(), c + "?" + imageindex);
				}
				string words = snippetslist.ToString();
				scriptedit.SetKeywords(snippetindex, (scriptconfig.CaseSensitive ? words : words.ToLowerInvariant()));
			}
			
			// Make autocomplete list
			autocompletelist = new List<string>(autocompletedict.Values);

			// Setup folding (https://github.com/jacobslusser/ScintillaNET/wiki/Automatic-Code-Folding)
			if(scriptconfig.Lexer == Lexer.Cpp || scriptconfig.Lexer == Lexer.CppNoCase)
			{
				// Instruct the lexer to calculate folding
				scriptedit.SetProperty("fold", "1");
				scriptedit.SetProperty("fold.compact", "1");

				// Configure a margin to display folding symbols
				scriptedit.Margins[2].Type = MarginType.Symbol;
				scriptedit.Margins[2].Mask = Marker.MaskFolders;
				scriptedit.Margins[2].Sensitive = true;
				scriptedit.Margins[2].Width = 12;

				// Set colors for all folding markers
				for(int i = 25; i < 32; i++)
				{
					scriptedit.Markers[i].SetForeColor(SystemColors.ControlLightLight);
					scriptedit.Markers[i].SetBackColor(SystemColors.ControlDark);
				}

				// Configure folding markers with respective symbols
				scriptedit.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
				scriptedit.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
				scriptedit.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
				scriptedit.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
				scriptedit.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
				scriptedit.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
				scriptedit.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

				// Enable automatic folding
				scriptedit.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
			}
			else
			{
				// Disable folding
				scriptedit.SetProperty("fold", "0");
				scriptedit.SetProperty("fold.compact", "0");

				scriptedit.Margins[2].Type = MarginType.Symbol;
				scriptedit.Margins[2].Mask = 0; // No markers here
				scriptedit.Margins[2].Sensitive = false;
				scriptedit.Margins[2].Width = 5;

				scriptedit.AutomaticFold = AutomaticFold.None;
			}

			// Rearrange the layout
			scriptedit.ClearDocumentStyle();
			this.PerformLayout();
		}
		
		// This returns the current word (where the caret is at)
		public string GetCurrentWord()
		{
			return GetWordAt(scriptedit.CurrentPosition);
		}

		// This returns the word at the given position
		public string GetWordAt(int position)
		{
			return scriptedit.GetWordFromPosition(position);
		}

		// Perform undo
		public void Undo()
		{
			scriptedit.Undo();
		}

		// Perform redo
		public void Redo()
		{
			scriptedit.Redo();
		}

		// This clears all undo levels
		public void ClearUndoRedo()
		{
			scriptedit.EmptyUndoBuffer();
		}

		//mxd. This marks the current document as unmodified
		public void SetSavePoint()
		{
			scriptedit.SetSavePoint();
		}

		// Perform cut
		public void Cut()
		{
			scriptedit.Cut();
		}

		// Perform copy
		public void Copy()
		{
			scriptedit.Copy();
		}

		// Perform paste
		public void Paste()
		{
			scriptedit.Paste();
		}
		
		// This steals the focus (use with care!)
		public void GrabFocus()
		{
			scriptedit.Focus();
		}

		public byte[] GetText()
		{
			return Encoding.ASCII.GetBytes(scriptedit.Text); //mxd TODO: other encodings?..
		}

		public void SetText(byte[] text)
		{
			scriptedit.Text = Encoding.ASCII.GetString(text); //mxd TODO: other encodings?..
		}

		//mxd
		public void InsertSnippet(string[] lines) 
		{
			// Insert the snippet
			int curline = scriptedit.LineFromPosition(scriptedit.SelectionStart);
			int indent = scriptedit.Lines[scriptedit.CurrentLine].Indentation;
			string tabs = Environment.NewLine + GetIndentationString(indent);
			string spaces = new String(' ', General.Settings.ScriptTabWidth);
			int entrypos = -1;
			int entryline = -1;
			string[] processedlines = ProcessLineBreaks(lines);

			// Process special chars, try to find entry position marker
			for(int i = 0; i < lines.Length; i++) 
			{
				if(!scriptedit.UseTabs) processedlines[i] = processedlines[i].Replace("\t", spaces);

				// Check if we have the [EP] marker
				if(entrypos == -1) 
				{
					int pos = processedlines[i].IndexOf("[EP]", StringComparison.Ordinal);
					if(pos != -1) 
					{
						processedlines[i] = processedlines[i].Remove(pos, 4);
						entryline = curline + i;
						entrypos = processedlines[i].Length - pos;
					}
				}
			}

			// Replace the text
			string text = string.Join(tabs, processedlines);
			scriptedit.SelectionStart = scriptedit.WordStartPosition(scriptedit.CurrentPosition, true);
			scriptedit.SelectionEnd = scriptedit.WordEndPosition(scriptedit.CurrentPosition, true);
			scriptedit.ReplaceSelection(text);

			// Move the cursor if we had the [EP] marker
			if(entrypos != -1) 
			{
				scriptedit.SetEmptySelection(scriptedit.Lines[entryline].EndPosition - entrypos - 2);
			}
		}

		//mxd. Find next result
		public bool FindNext(FindReplaceOptions options, bool useselectionstart)
		{
			int startpos = (useselectionstart ? Math.Min(scriptedit.SelectionStart, scriptedit.SelectionEnd) : Math.Max(scriptedit.SelectionStart, scriptedit.SelectionEnd));

			// Search the document
			scriptedit.TargetStart = startpos;
			scriptedit.TargetEnd = scriptedit.TextLength;
			scriptedit.SearchFlags = options.CaseSensitive ? SearchFlags.MatchCase : SearchFlags.None;
			if(options.WholeWord) scriptedit.SearchFlags |= SearchFlags.WholeWord;

			int result = scriptedit.SearchInTarget(options.FindText);
			
			// Wrap around?
			if(result == -1)
			{
				scriptedit.TargetStart = 0;
				scriptedit.TargetEnd = startpos;
				result = scriptedit.SearchInTarget(options.FindText);
			}

			// Found something
			if(result != -1)
			{
				// Select the result
				SelectAndShow(result, result + options.FindText.Length);

				// Update extra highlights
				HighlightWord(options.FindText);

				// All done
				return true;
			}

			// Nothing found...
			return false;
		}

		//mxd. Find previous result
		public bool FindPrevious(FindReplaceOptions options)
		{
			int endpos = Math.Max(0, Math.Min(scriptedit.SelectionStart, scriptedit.SelectionEnd) - 1);

			// Search the document
			scriptedit.TargetStart = endpos;
			scriptedit.TargetEnd = 0;
			scriptedit.SearchFlags = options.CaseSensitive ? SearchFlags.MatchCase : SearchFlags.None;
			if(options.WholeWord) scriptedit.SearchFlags |= SearchFlags.WholeWord;

			int result = scriptedit.SearchInTarget(options.FindText);

			// Wrap around?
			if(result == -1)
			{
				scriptedit.TargetStart = scriptedit.TextLength;
				scriptedit.TargetEnd = endpos;
				result = scriptedit.SearchInTarget(options.FindText);
			}

			// Found something
			if(result != -1)
			{
				// Select the result
				SelectAndShow(result, result + options.FindText.Length);

				// Update extra highlights
				HighlightWord(options.FindText);

				// All done
				return true;
			}

			// Nothing found...
			return false;
		}

		//mxd
		private void SelectAndShow(int startpos, int endpos)
		{
			// Select the result
			int startline = scriptedit.LineFromPosition(startpos);
			int endline = scriptedit.LineFromPosition(endpos);

			// Go to target line
			scriptedit.ShowLines(startline, endline);
			scriptedit.GotoPosition(startpos);

			// We may want to do some extra scrolling...
			if(startline > 1 && scriptedit.FirstVisibleLine >= startline - 1)
			{
				scriptedit.Lines[startline - 1].Goto();
			}
			else if(endline < scriptedit.Lines.Count - 1 && scriptedit.FirstVisibleLine + scriptedit.LinesOnScreen <= endline + 1)
			{
				scriptedit.Lines[endline + 1].Goto();
			}

			// Update selection
			scriptedit.SelectionStart = startpos;
			scriptedit.SelectionEnd = endpos;
		}

		//mxd. (Un)indents selection
		public void IndentSelection(bool indent)
		{
			// Get selected range of lines
			int startline = scriptedit.LineFromPosition(scriptedit.SelectionStart);
			int endline = scriptedit.LineFromPosition(scriptedit.SelectionEnd);

			for(int i = startline; i < endline + 1; i++)
			{
				scriptedit.Lines[i].Indentation += (indent ? General.Settings.ScriptTabWidth : -General.Settings.ScriptTabWidth);
			}
		}

		#endregion

		#region ================== Utility methods

		// This returns the ScriptStyleType for a given Scintilla style
		private ScriptStyleType GetScriptStyle(int scintillastyle)
		{
			return (stylelookup.ContainsKey(scintillastyle) ? stylelookup[scintillastyle] : ScriptStyleType.PlainText);
		}
		
		// This gathers information about the current caret position
		private void UpdatePositionInfo()
		{
			int bracketlevel = 0;			// bracket level counting
			int argindex = 0;				// function argument counting
			int pos = scriptedit.CurrentPosition;

			// Get the text
			string scripttext = scriptedit.Text;

			// Reset position info
			curfunctionname = "";
			curargumentindex = 0;
			curfunctionstartpos = 0;
			
			// Determine lowest backtrack position
			int limitpos = scriptedit.CurrentPosition - MAX_BACKTRACK_LENGTH;
			if(limitpos < 0) limitpos = 0;
			
			// We can only do this when we have function syntax information
			if((scriptconfig.ArgumentDelimiter.Length == 0) || (scriptconfig.FunctionClose.Length == 0) ||
			   (scriptconfig.FunctionOpen.Length == 0) || (scriptconfig.Terminator.Length == 0)) return;
			
			// Get int versions of the function syntax informantion
			int argumentdelimiter = scriptconfig.ArgumentDelimiter[0];
			int functionclose = scriptconfig.FunctionClose[0];
			int functionopen = scriptconfig.FunctionOpen[0];
			int terminator = scriptconfig.Terminator[0];
			
			// Continue backtracking until we reached the limitpos
			while(pos >= limitpos)
			{
				// Backtrack 1 character
				pos--;
				
				// Get the style and character at this position
				ScriptStyleType curstyle = GetScriptStyle(scriptedit.GetStyleAt(pos));
				int curchar = scriptedit.GetCharAt(pos);
				
				// Then meeting ) then increase bracket level
				// When meeting ( then decrease bracket level
				// When bracket level goes -1, then the next word should be the function name
				// Only when at bracket level 0, count the comma's for argument index
				
				// TODO:
				// Original code checked for scope character here and breaks if found
				
				// Check if in plain text or keyword
				if((curstyle == ScriptStyleType.PlainText) || (curstyle == ScriptStyleType.Keyword))
				{
					// Closing bracket
					if(curchar == functionclose)
					{
						bracketlevel++;
					}
					// Opening bracket
					else if(curchar == functionopen)
					{
						bracketlevel--;
						
						// Out of the brackets?
						if(bracketlevel < 0)
						{
							// Skip any whitespace before this bracket
							do
							{
								// Backtrack 1 character
								curchar = scriptedit.GetCharAt(--pos);
							}
							while((pos >= limitpos) && ((curchar == ' ') || (curchar == '\t') ||
														(curchar == '\r') || (curchar == '\n')));
							
							// NOTE: We may need to set onlyWordCharacters argument in the
							// following calls to false to get any argument delimiter included,
							// but this may also cause a valid keyword to be combined with other
							// surrounding characters that do not belong to the keyword.
							
							// Find the word before this bracket
							int wordstart = scriptedit.WordStartPosition(pos, true);
							int wordend = scriptedit.WordEndPosition(pos, true);
							string word = scripttext.Substring(wordstart, wordend - wordstart);
							if(word.Length > 0)
							{
								// Check if this is an argument delimiter
								// I can't remember why I did this, but I'll probably stumble
								// upon the problem if this doesn't work right (see note above)
								if(word[0] == argumentdelimiter)
								{
									// We are now in the parent function
									bracketlevel++;
									argindex = 0;
								}
								// Now check if this is a keyword
								else if(scriptconfig.IsKeyword(word))
								{
									// Found it!
									curfunctionname = scriptconfig.GetKeywordCase(word);
									curargumentindex = argindex;
									curfunctionstartpos = wordstart;
									break;
								}
								else
								{
									// Don't know this word
									break;
								}
							}
						}
					}
					// Argument delimiter
					else if(curchar == argumentdelimiter)
					{
						// Only count these at brackt level 0
						if(bracketlevel == 0) argindex++;
					}
					// Terminator
					else if(curchar == terminator)
					{
						// Can't find anything, break now
						break;
					}
				}
			}
		}
		
		// This registers an image for the autocomplete list
		private void RegisterAutoCompleteImage(ImageIndex index, Bitmap image)
		{
			// Register image
			scriptedit.RegisterRgbaImage((int)index, image);
		}

		// This registers an image for the markes list
		private void RegisterMarkerImage(ImageIndex index, Bitmap image)
		{
			// Register image
			scriptedit.Markers[(int)index].DefineRgbaImage(image);
			scriptedit.Markers[(int)index].Symbol = MarkerSymbol.RgbaImage;
		}

		//mxd. This converts [LB] markers to line breaks if necessary
		private static string[] ProcessLineBreaks(string[] lines) 
		{
			List<string> result = new List<string>(lines.Length);
			string[] separator = new[] { "[LB]" };

			foreach(string line in lines) 
			{
				if(line.IndexOf(separator[0], StringComparison.Ordinal) != -1) 
				{
					if(General.Settings.SnippetsAllmanStyle)
						result.AddRange(line.Split(separator, StringSplitOptions.RemoveEmptyEntries));
					else
						result.Add(line.Replace(separator[0], " "));
				} 
				else 
				{
					result.Add(line);
				}
			}

			return result.ToArray();
		}

		//mxd. Autocompletion handling (https://github.com/jacobslusser/ScintillaNET/wiki/Basic-Autocompletion)
		private bool ShowAutoCompletionList()
		{
			int currentpos = scriptedit.CurrentPosition;
			int wordstartpos = scriptedit.WordStartPosition(currentpos, true);

			if(wordstartpos >= currentpos)
			{
				// Hide the list
				scriptedit.AutoCCancel();
				return false;
			}

			// Get entered text
			string start = scriptedit.GetTextRange(wordstartpos, currentpos - wordstartpos);
			if(string.IsNullOrEmpty(start))
			{
				// Hide the list
				scriptedit.AutoCCancel();
				return false;
			}

			// Filter the list
			List<string> filtered = new List<string>();
			foreach(string s in autocompletelist)
				if(s.IndexOf(start, StringComparison.OrdinalIgnoreCase) != -1) filtered.Add(s);

			// Any matches?
			if(filtered.Count > 0)
			{
				// Show the list
				scriptedit.AutoCShow(currentpos - wordstartpos, string.Join(" ", filtered.ToArray()));
				return true;
			}

			// Hide the list
			scriptedit.AutoCCancel();
			return false;
		}

		//mxd
		private string GetIndentationString(int indent)
		{
			if(scriptedit.UseTabs)
			{
				string indentstr = string.Empty;
				int numtabs = indent / scriptedit.TabWidth;
				if(numtabs > 0) indentstr = new string('\t', numtabs);

				// Mixed padding? Add spaces
				if(numtabs * scriptedit.TabWidth < indent)
				{
					int numspaces = indent - numtabs * scriptedit.TabWidth;
					indentstr += new string(' ', numspaces);
				}

				return indentstr;
			}
			else
			{
				return new string(' ', indent);
			}
		}

		//mxd. https://github.com/jacobslusser/ScintillaNET/wiki/Find-and-Highlight-Words
		private void HighlightWord(string text)
		{
			// Remove all uses of our indicator
			scriptedit.IndicatorCurrent = HIGHLIGHT_INDICATOR;
			scriptedit.IndicatorClearRange(0, scriptedit.TextLength);

			// Update indicator appearance
			scriptedit.Indicators[HIGHLIGHT_INDICATOR].Style = IndicatorStyle.RoundBox;
			scriptedit.Indicators[HIGHLIGHT_INDICATOR].Under = true;
			scriptedit.Indicators[HIGHLIGHT_INDICATOR].ForeColor = General.Colors.ScriptIndicator.ToColor();
			scriptedit.Indicators[HIGHLIGHT_INDICATOR].OutlineAlpha = 50;
			scriptedit.Indicators[HIGHLIGHT_INDICATOR].Alpha = 30;

			// Search the document
			scriptedit.TargetStart = 0;
			scriptedit.TargetEnd = scriptedit.TextLength;
			scriptedit.SearchFlags = SearchFlags.WholeWord;

			while(scriptedit.SearchInTarget(text) != -1)
			{
				//mxd. Don't mark currently selected word
				if(scriptedit.SelectionStart != scriptedit.TargetStart && scriptedit.SelectionEnd != scriptedit.TargetEnd)
				{
					// Mark the search results with the current indicator
					scriptedit.IndicatorFillRange(scriptedit.TargetStart, scriptedit.TargetEnd - scriptedit.TargetStart);
				}

				// Search the remainder of the document
				scriptedit.TargetStart = scriptedit.TargetEnd;
				scriptedit.TargetEnd = scriptedit.TextLength;
			}
		}

		#endregion
		
		#region ================== Events
		
		// Layout needs to be re-organized
		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);

			// With or without functions bar?
			if(functionbar.Visible)
			{
				scriptpanel.Top = functionbar.Bottom + 6;
				scriptpanel.Height = this.ClientSize.Height - scriptpanel.Top;
			}
			else
			{
				scriptpanel.Top = 0;
				scriptpanel.Height = this.ClientSize.Height;
			}
		}

		//mxd. Script text changed
		private void scriptedit_TextChanged(object sender, EventArgs e)
		{
			// Line number margin width needs changing?
			int curlinenumbercharlength = scriptedit.Lines.Count.ToString().Length;

			// Calculate the width required to display the last line number
			// and include some padding for good measure.
			if(curlinenumbercharlength != linenumbercharlength)
			{
				const int padding = 2;
				scriptedit.Margins[1].Width = scriptedit.TextWidth(Style.LineNumber, new string('9', curlinenumbercharlength + 1)) + padding;
				linenumbercharlength = curlinenumbercharlength;
			}
			
			if(OnTextChanged != null) OnTextChanged(this, EventArgs.Empty);
		}

		//mxd
		private void scriptedit_CharAdded(object sender, CharAddedEventArgs e)
		{
			// Hide call tip if any
			scriptedit.CallTipCancel();

			// Offset caret if needed
			if(caretoffset != 0)
			{
				scriptedit.SetEmptySelection(scriptedit.SelectionStart + caretoffset);
				caretoffset = 0;
				if(!expandcodeblock) return;
			}

			// Move CodeBlockOpen to the new line?
			if(expandcodeblock)
			{
				if(scriptedit.CurrentLine > 0)
				{
					string linetext = scriptedit.Lines[scriptedit.CurrentLine - 1].Text;
					int blockopenpos = (string.IsNullOrEmpty(scriptconfig.CodeBlockOpen) ? -1 : linetext.LastIndexOf(scriptconfig.CodeBlockOpen, StringComparison.Ordinal));
					if(blockopenpos != -1)
					{
						// Do it only if initial line doesn't start with CodeBlockOpen
						string linestart = linetext.Substring(0, blockopenpos).Trim();
						if(linestart.Length > 0)
						{
							scriptedit.InsertText(scriptedit.Lines[scriptedit.CurrentLine - 1].Position + blockopenpos, 
								Environment.NewLine + GetIndentationString(scriptedit.Lines[scriptedit.CurrentLine - 1].Indentation));
						}
					}
				}

				expandcodeblock = false;
				return;
			}

			// Auto-match braces
			//TODO: Auto-match quotes
			bool endpos = (scriptedit.CurrentPosition == scriptedit.TextLength);
			if(!string.IsNullOrEmpty(scriptconfig.CodeBlockOpen) && e.Char == scriptconfig.CodeBlockOpen[0] && !string.IsNullOrEmpty(scriptconfig.CodeBlockClose) &&
				(endpos || (char)scriptedit.GetCharAt(scriptedit.CurrentPosition + 1) != scriptconfig.CodeBlockClose[0]))
			{
				scriptedit.InsertText(scriptedit.CurrentPosition, scriptconfig.CodeBlockClose);
			}
			else if(!string.IsNullOrEmpty(scriptconfig.FunctionOpen) && e.Char == scriptconfig.FunctionOpen[0] && !string.IsNullOrEmpty(scriptconfig.FunctionClose) &&
				(endpos || (char)scriptedit.GetCharAt(scriptedit.CurrentPosition + 1) != scriptconfig.FunctionClose[0]))
			{
				scriptedit.InsertText(scriptedit.CurrentPosition, scriptconfig.FunctionClose);
			}
			else if(!string.IsNullOrEmpty(scriptconfig.ArrayOpen) && e.Char == scriptconfig.ArrayOpen[0] && !string.IsNullOrEmpty(scriptconfig.ArrayClose) &&
				(endpos || (char)scriptedit.GetCharAt(scriptedit.CurrentPosition + 1) != scriptconfig.ArrayClose[0]))
			{
				scriptedit.InsertText(scriptedit.CurrentPosition, scriptconfig.ArrayClose);
			}
			else
			{
				// Display the autocompletion list
				// TODO: make this behaviour optional?
				ShowAutoCompletionList();
			}
		}

		//mxd
		private void scriptedit_UpdateUI(object sender, UpdateUIEventArgs e)
		{
			// If a word is selected, highlight the same words
			if(scriptedit.SelectedText != highlightedword)
			{
				// Highlight only when whole word is selected
				if(!string.IsNullOrEmpty(scriptedit.SelectedText) && scriptedit.GetWordFromPosition(scriptedit.SelectionStart) == scriptedit.SelectedText)
				{
					HighlightWord(scriptedit.SelectedText);
				}
				else
				{
					// Clear highlight
					scriptedit.IndicatorCurrent = HIGHLIGHT_INDICATOR;
					scriptedit.IndicatorClearRange(0, scriptedit.TextLength);
				}

				highlightedword = scriptedit.SelectedText;
			}
			
			// Has the caret changed position?
			int caretpos = scriptedit.CurrentPosition;
			if(lastcaretpos != caretpos && scriptconfig.BraceChars.Count > 0)
			{
				// Perform brace matching (https://github.com/jacobslusser/ScintillaNET/wiki/Brace-Matching)
				lastcaretpos = caretpos;
				int bracepos1 = -1;

				// Is there a brace to the left or right?
				if(caretpos > 0 && scriptconfig.BraceChars.Contains((char)scriptedit.GetCharAt(caretpos - 1)))
					bracepos1 = (caretpos - 1);
				else if(scriptconfig.BraceChars.Contains((char)(scriptedit.GetCharAt(caretpos))))
					bracepos1 = caretpos;

				if(bracepos1 > -1)
				{
					// Find the matching brace
					int bracepos2 = scriptedit.BraceMatch(bracepos1);
					if(bracepos2 == Scintilla.InvalidPosition)
						scriptedit.BraceBadLight(bracepos1);
					else
						scriptedit.BraceHighlight(bracepos1, bracepos2);
				}
				else
				{
					// Turn off brace matching
					scriptedit.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition);
				}
			}
		}

		//mxd
		private void scriptedit_InsertCheck(object sender, InsertCheckEventArgs e)
		{
			// Gross hacks...
			if(skiptextinsert)
			{
				e.Text = string.Empty;
				skiptextinsert = false;
			}
			// Do we want auto-indentation?
			else if(!expandcodeblock && General.Settings.ScriptAutoIndent && e.Text == "\r\n")
			{
				// Get current line indentation up to the cursor position
				string linetext = scriptedit.Lines[scriptedit.CurrentLine].Text;
				int selectionpos = scriptedit.SelectionStart - scriptedit.Lines[scriptedit.CurrentLine].Position;
				int indent = 0;
				for(int i = 0; i < selectionpos; i++)
				{
					switch(linetext[i])
					{
						case  ' ': indent++; break;
						case '\t': indent += scriptedit.TabWidth; break;
						default: i = selectionpos; break; // break the loop
					}
				}

				// Store initial indentation
				int initialindent = indent;

				// Need to increase indentation? We do this when:
				// 1. Line contains '{' and '}' and the cursor is between them
				// 2. Line either doesn't contain '}', or it's before '{', it contains '{' and the cursor is after it 
				// 3. Line doesn't contain ';', line contains ')' and the cursor is after it
				int blockopenpos = (string.IsNullOrEmpty(scriptconfig.CodeBlockOpen) ? -1 : linetext.LastIndexOf(scriptconfig.CodeBlockOpen, selectionpos, StringComparison.Ordinal));
				int blockclosepos = (string.IsNullOrEmpty(scriptconfig.CodeBlockOpen) ? -1 : linetext.IndexOf(scriptconfig.CodeBlockClose, selectionpos, StringComparison.Ordinal));
				int funcclosepos = (string.IsNullOrEmpty(scriptconfig.FunctionClose) ? -1 : linetext.LastIndexOf(scriptconfig.FunctionClose, StringComparison.Ordinal));

				// Add indentation when the cursor is between { and }
				bool addindent = (blockopenpos != -1 && blockopenpos < selectionpos) && (blockclosepos == -1 || (blockopenpos < blockclosepos && blockclosepos >= selectionpos));
				bool isblockindent = addindent;
				addindent |= funcclosepos != -1 && blockopenpos == -1 && funcclosepos < selectionpos && !linetext.Contains(scriptconfig.Terminator);
				if(addindent) indent += scriptedit.TabWidth;

				// Calculate indentation
				string indentstr = GetIndentationString(indent);

				// Move CodeBlockOpen to the new line (will be applied in scriptedit_CharAdded)?
				expandcodeblock = (isblockindent && General.Settings.SnippetsAllmanStyle);

				// Offset closing block char?
				if(addindent && blockclosepos != -1)
				{
					string initialindentstr = GetIndentationString(initialindent);
					indentstr += Environment.NewLine + initialindentstr;
					
					// Offset cursor position (will be performed in scriptedit_CharAdded)
					caretoffset = -(initialindentstr.Length + Environment.NewLine.Length);
				}

				// Apply new indentation
				e.Text += indentstr;
			}
		}

		//mxd
		private void scriptedit_AutoCCompleted(object sender, AutoCSelectionEventArgs e)
		{
			// Expand snippet?
			string[] lines = scriptconfig.GetSnippet(e.Text);
			if(lines != null) InsertSnippet(lines);
		}
		
		// Key pressed down
		private void scriptedit_KeyDown(object sender, KeyEventArgs e)
		{
			// F3 for Find Next
			if((e.KeyCode == Keys.F3) && (e.Modifiers == Keys.None))
			{
				if(OnFindNext != null) OnFindNext();
			}
			// F2 for Find Previous (mxd)
			else if((e.KeyCode == Keys.F2) && (e.Modifiers == Keys.None))
			{
				if(OnFindPrevious != null) OnFindPrevious();
			}
			// CTRL+F for find & replace
			else if((e.KeyCode == Keys.F) && ((e.Modifiers & Keys.Control) == Keys.Control))
			{
				if(OnOpenFindAndReplace != null) OnOpenFindAndReplace();
			}
			// CTRL+S for save
			else if((e.KeyCode == Keys.S) && ((e.Modifiers & Keys.Control) == Keys.Control))
			{
				if(OnExplicitSaveTab != null) OnExplicitSaveTab();
			}
			// CTRL+O for open
			else if((e.KeyCode == Keys.O) && ((e.Modifiers & Keys.Control) == Keys.Control))
			{
				if(OnOpenScriptBrowser != null) OnOpenScriptBrowser();
			}
			// CTRL+Space to autocomplete
			else if((e.KeyCode == Keys.Space) && (e.Modifiers == Keys.Control))
			{
				// Hide call tip if any
				scriptedit.CallTipCancel();
				
				// Show autocomplete
				if(ShowAutoCompletionList()) skiptextinsert = true;
			}
			//mxd. Tab to expand code snippet. Do it only when the text cursor is at the end of a keyword.
			else if(e.KeyCode == Keys.Tab)
			{
				if(!scriptedit.AutoCActive)
				{
					string curword = GetCurrentWord().ToLowerInvariant();
					if(scriptconfig.Snippets.Contains(curword) && scriptedit.CurrentPosition == scriptedit.WordEndPosition(scriptedit.CurrentPosition, true))
					{
						InsertSnippet(scriptconfig.GetSnippet(curword));
						skiptextinsert = true;
					}
				}
			}
			//mxd. Handle screenshot saving
			else if(DelayedForm.ProcessSaveScreenshotAction((int)e.KeyData))
			{
				skiptextinsert = true;
			}
		}
		
		// Key released
		private void scriptedit_KeyUp(object sender, KeyEventArgs e)
		{
			bool showcalltip = false;
			int highlightstart = 0;
			int highlightend = 0;
			
			UpdatePositionInfo();
			
			// Call tip shown
			if(scriptedit.CallTipActive)
			{
				// Should we hide the call tip?
				if(curfunctionname.Length == 0)
				{
					// Hide the call tip
					scriptedit.CallTipCancel();
				}
				else
				{
					// Update the call tip
					showcalltip = true;
				}
			}
			// No call tip
			else
			{
				// Should we show a call tip?
				showcalltip = (curfunctionname.Length > 0) && !scriptedit.AutoCActive;
			}
			
			// Show or update call tip
			if(showcalltip)
			{
				string functiondef = scriptconfig.GetFunctionDefinition(curfunctionname);
				if(functiondef != null)
				{
					// Determine the range to highlight
					int argsopenpos = functiondef.IndexOf(scriptconfig.FunctionOpen, StringComparison.Ordinal);
					int argsclosepos = functiondef.LastIndexOf(scriptconfig.FunctionClose, StringComparison.Ordinal);
					if((argsopenpos > -1) && (argsclosepos > -1))
					{
						string argsstr = functiondef.Substring(argsopenpos + 1, argsclosepos - argsopenpos - 1);
						string[] args = argsstr.Split(scriptconfig.ArgumentDelimiter[0]);
						if((curargumentindex >= 0) && (curargumentindex < args.Length))
						{
							int argoffset = 0;
							for(int i = 0; i < curargumentindex; i++) argoffset += args[i].Length + 1;
							highlightstart = argsopenpos + argoffset + 1;
							highlightend = highlightstart + args[curargumentindex].Length;
						}
					}

					//mxd. If the tip obscures the view, move it down
					int tippos;
					int funcline = scriptedit.LineFromPosition(curfunctionstartpos);
					if(scriptedit.CurrentLine > funcline)
						tippos = scriptedit.Lines[scriptedit.CurrentLine].Position + scriptedit.Lines[scriptedit.CurrentLine].Indentation; //scriptedit.PositionFromLine(curline) /*+ (curfunctionstartpos - scriptedit.PositionFromLine(funcline))*/;
					else
						tippos = curfunctionstartpos;
					
					// Show tip
					scriptedit.CallTipShow(tippos, functiondef);
					scriptedit.CallTipSetHlt(highlightstart, highlightend);
				}
			}
		}
		
		#endregion
	}
}
