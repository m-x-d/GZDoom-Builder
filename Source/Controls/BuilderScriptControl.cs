
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
using System.Drawing.Drawing2D;
using CodeImp.DoomBuilder.Config;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.IO;
using System.Collections;
using System.Globalization;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class BuilderScriptControl : UserControl
	{
		#region ================== Constants

		private const string LEXERS_RESOURCE = "Lexers.cfg";
		private const int DEFAULT_STYLE = (int)ScriptStylesCommon.Default;
		
		#endregion

		#region ================== Delegates / Events

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Variables
		
		// List of keywords and constants, sorted as uppercase
		private string autocompletestring;
		
		#endregion

		#region ================== Contructor / Disposer

		// Constructor
		public BuilderScriptControl()
		{
			// Initialize
			InitializeComponent();
			
			// Script editor properties
			// Unfortunately, these cannot be set using the designer
			// because the control is not really loaded in design mode
			scriptedit.AutoCMaximumHeight = 8;
			scriptedit.AutoCSeparator = ' ';
			scriptedit.AutoCTypeSeparator = ',';
			scriptedit.CaretWidth = 2;
			scriptedit.EndAtLastLine = 1;
			scriptedit.EndOfLineMode = ScriptEndOfLine.CRLF;
			scriptedit.IsAutoCGetChooseSingle = true;
			scriptedit.IsAutoCGetIgnoreCase = true;
			scriptedit.IsBackSpaceUnIndents = true;
			scriptedit.IsBufferedDraw = true;
			scriptedit.IsCaretLineVisible = false;
			scriptedit.IsHScrollBar = true;
			scriptedit.IsIndentationGuides = true;
			scriptedit.IsMouseDownCaptures = true;
			scriptedit.IsTabIndents = true;
			scriptedit.IsUndoCollection = true;
			scriptedit.IsUseTabs = true;
			scriptedit.IsViewEOL = false;
			scriptedit.IsVScrollBar = true;
			scriptedit.SetFoldFlags((int)ScriptFoldFlag.Box);
			scriptedit.SetMarginTypeN(0, (int)ScriptMarginType.Symbol);
			scriptedit.SetMarginTypeN(1, (int)ScriptMarginType.Number);
			scriptedit.SetMarginTypeN(2, (int)ScriptMarginType.Symbol);
			scriptedit.SetMarginWidthN(0, 16);
			scriptedit.SetMarginWidthN(1, 40);
			scriptedit.SetMarginWidthN(2, 5);
			//scriptedit.AddIgnoredKey(Keys.ControlKey, Keys.None);
			//scriptedit.AddIgnoredKey(Keys.Space, Keys.None);
			//scriptedit.AddIgnoredKey(Keys.Space, Keys.Control);
		}
		
		#endregion
		
		#region ================== Methods
		
		// This sets up the script editor with a script configuration
		public void SetupStyles(ScriptConfiguration config)
		{
			Stream lexersdata;
			StreamReader lexersreader;
			Configuration lexercfg = new Configuration();
			List<string> autocompletelist = new List<string>();
			string[] resnames;
			
			// Find a resource named Actions.cfg
			resnames = General.ThisAssembly.GetManifestResourceNames();
			foreach(string rn in resnames)
			{
				// Found one?
				if(rn.EndsWith(LEXERS_RESOURCE, StringComparison.InvariantCultureIgnoreCase))
				{
					// Get a stream from the resource
					lexersdata = General.ThisAssembly.GetManifestResourceStream(rn);
					lexersreader = new StreamReader(lexersdata, Encoding.ASCII);

					// Load configuration from stream
					lexercfg.InputConfiguration(lexersreader.ReadToEnd());

					// Done with the resource
					lexersreader.Dispose();
					lexersdata.Dispose();
				}
			}
			
			// Check if specified lexer exists and set the lexer to use
			string lexername = "lexer" + config.Lexer.ToString(CultureInfo.InvariantCulture);
			if(!lexercfg.SettingExists(lexername)) throw new InvalidOperationException("Unknown lexer " + config.Lexer + " specified in script configuration!");
			scriptedit.Lexer = config.Lexer;
			
			// Set the default style and settings
			scriptedit.StyleSetFont(DEFAULT_STYLE, "Lucida Console");
			scriptedit.StyleSetBack(DEFAULT_STYLE, General.Colors.ScriptBackground.ToColorRef());
			scriptedit.StyleSetBold(DEFAULT_STYLE, true);
			scriptedit.StyleSetCase(DEFAULT_STYLE, ScriptCaseVisible.Mixed);
			scriptedit.StyleSetFore(DEFAULT_STYLE, General.Colors.PlainText.ToColorRef());
			scriptedit.StyleSetItalic(DEFAULT_STYLE, false);
			scriptedit.StyleSetSize(DEFAULT_STYLE, 10);
			scriptedit.StyleSetUnderline(DEFAULT_STYLE, false);
			scriptedit.CaretPeriod = SystemInformation.CaretBlinkTime;
			scriptedit.CaretFore = General.Colors.ScriptBackground.Inverse().ToColorRef();
			scriptedit.StyleBits = 7;
			
			// This applies the default style to all styles
			scriptedit.StyleClearAll();
			
			// Set the default to something normal (this is used by the autocomplete list)
			scriptedit.StyleSetFont(DEFAULT_STYLE, this.Font.Name);
			scriptedit.StyleSetBold(DEFAULT_STYLE, this.Font.Bold);
			scriptedit.StyleSetItalic(DEFAULT_STYLE, this.Font.Italic);
			scriptedit.StyleSetUnderline(DEFAULT_STYLE, this.Font.Underline);
			scriptedit.StyleSetSize(DEFAULT_STYLE, (int)Math.Round(this.Font.SizeInPoints));
			
			// Set style for linenumbers and margins
			scriptedit.StyleSetBack((int)ScriptStylesCommon.LineNumber, General.Colors.ScriptBackground.ToColorRef());
			
			// Clear all keywords
			for(int i = 0; i < 9; i++) scriptedit.KeyWords(i, null);
			
			// Now go for all elements in the lexer configuration
			// We are looking for the numeric keys, because these are the
			// style index to set and the value is the color index to apply
			IDictionary dic = lexercfg.ReadSetting(lexername, new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Check if this is a numeric key
				int stylenum = -1;
				if(int.TryParse(de.Key.ToString(), out stylenum))
				{
					// Apply color to style
					scriptedit.StyleSetFore(stylenum, General.Colors.Colors[(int)de.Value].ToColorRef());
				}
			}
			
			// Create the keywords list and apply it
			int keywordsindex = lexercfg.ReadSetting(lexername + ".keywordsindex", -1);
			if(keywordsindex > -1)
			{
				StringBuilder keywordslist = new StringBuilder("");
				foreach(string k in config.Keywords)
				{
					if(keywordslist.Length > 0) keywordslist.Append(" ");
					keywordslist.Append(k);
					autocompletelist.Add(k);
				}
				string words = keywordslist.ToString();
				scriptedit.KeyWords(keywordsindex, words.ToLowerInvariant());
			}

			// Create the constants list and apply it
			int constantsindex = lexercfg.ReadSetting(lexername + ".constantsindex", -1);
			if(constantsindex > -1)
			{
				StringBuilder constantslist = new StringBuilder("");
				foreach(string c in config.Constants)
				{
					if(constantslist.Length > 0) constantslist.Append(" ");
					constantslist.Append(c);
					autocompletelist.Add(c);
				}
				string words = constantslist.ToString();
				scriptedit.KeyWords(constantsindex, words.ToLowerInvariant());
			}
			
			// Sort the autocomplete list
			autocompletelist.Sort(StringComparer.CurrentCultureIgnoreCase);
			autocompletestring = string.Join(" ", autocompletelist.ToArray());
		}
		
		#endregion
		
		#region ================== Events
		
		// Key pressed down
		private void scriptedit_KeyDown(object sender, KeyEventArgs e)
		{
			// CTRL+Space to autocomplete
			if((e.KeyCode == Keys.Space) && (e.Modifiers == Keys.Control))
			{
				// Hide call tip if any
				scriptedit.CallTipCancel();
				
				// Show autocomplete
				int currentpos = scriptedit.CurrentPos;
				int wordstartpos = scriptedit.WordStartPosition(currentpos, true);
				scriptedit.AutoCShow(currentpos - wordstartpos, autocompletestring);
				
				e.Handled = true;
			}
		}
		
		#endregion
	}
}
