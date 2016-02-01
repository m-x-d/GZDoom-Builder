
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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Compilers;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Windows;
using ScintillaNET;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class ScriptEditorPanel : UserControl
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		private List<ScriptConfiguration> scriptconfigs;
		private List<CompilerError> compilererrors;

		// Find/Replace
		private ScriptFindReplaceForm findreplaceform;
		private FindReplaceOptions findoptions;

		// Quick search bar settings (mxd)
		private static bool matchwholeword;
		private static bool matchcase;

		//mxd. Status update
		private ScriptStatusInfo status;
		private int statusflashcount;
		private bool statusflashicon;
		
		#endregion
		
		#region ================== Properties
		
		public ScriptDocumentTab ActiveTab { get { return (tabs.SelectedTab as ScriptDocumentTab); } }
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public ScriptEditorPanel()
		{
			InitializeComponent();
		}
		
		// This initializes the control
		public void Initialize()
		{
			// Make list of script configs
			scriptconfigs = new List<ScriptConfiguration>(General.ScriptConfigs.Values);
			scriptconfigs.Add(new ScriptConfiguration());
			scriptconfigs.Sort();
			
			// Fill the list of new document types
			foreach(ScriptConfiguration cfg in scriptconfigs)
			{
				// Button for new script menu
				ToolStripMenuItem item = new ToolStripMenuItem(cfg.Description);
				//item.Image = buttonnew.Image;
				item.Tag = cfg;
				item.Click += buttonnew_Click;
				buttonnew.DropDownItems.Add(item);
				
				// Button for script type menu
				item = new ToolStripMenuItem(cfg.Description);
				//item.Image = buttonnew.Image;
				item.Tag = cfg;
				item.Click += buttonscriptconfig_Click;
				buttonscriptconfig.DropDownItems.Add(item);
			}
			
			// Setup supported extensions
			string filterall = "";
			string filterseperate = "";
			foreach(ScriptConfiguration cfg in scriptconfigs)
			{
				if(cfg.Extensions.Length > 0)
				{
					string exts = "*." + string.Join(";*.", cfg.Extensions);
					if(filterseperate.Length > 0) filterseperate += "|";
					filterseperate += cfg.Description + "|" + exts;
					if(filterall.Length > 0) filterall += ";";
					filterall += exts;
				}
			}
			openfile.Filter = "Script files|" + filterall + "|" + filterseperate + "|All files|*.*";
			
			// Load the script lumps
			foreach(MapLumpInfo maplumpinfo in General.Map.Config.MapLumps.Values)
			{
				// Is this a script lump?
				if(maplumpinfo.ScriptBuild) //mxd
				{
					// Load this!
					ScriptLumpDocumentTab t = new ScriptLumpDocumentTab(this, maplumpinfo.Name, General.CompiledScriptConfigs[General.Map.Options.ScriptCompiler]);
					t.OnTextChanged += tabpage_OnLumpTextChanged; //mxd
					t.Scintilla.UpdateUI += scintilla_OnUpdateUI; //mxd
					tabs.TabPages.Add(t);
				} 
				else if(maplumpinfo.Script != null)
				{
					// Load this!
					ScriptLumpDocumentTab t = new ScriptLumpDocumentTab(this, maplumpinfo.Name, maplumpinfo.Script);
					t.OnTextChanged += tabpage_OnLumpTextChanged; //mxd
					t.Scintilla.UpdateUI += scintilla_OnUpdateUI; //mxd
					tabs.TabPages.Add(t);
				}
			}

			// Load the files that were previously opened for this map
			foreach(String filename in General.Map.Options.ScriptFiles)
			{
				// Does this file exist?
				if(File.Exists(filename))
				{
					// Load this!
					OpenFile(filename);
				}
			}

			//mxd. Select "Scripts" tab, because that's what user will want 99% of time
			int scriptsindex = GetTabPageIndex("SCRIPTS");
			tabs.SelectedIndex = (scriptsindex == -1 ? 0 : scriptsindex);

			//mxd. Apply quick search settings
			searchmatchcase.Checked = matchcase;
			searchwholeword.Checked = matchwholeword;
			searchbox_TextChanged(this, EventArgs.Empty);
			
			// If the map has remembered any compile errors, then show them
			ShowErrors(General.Map.Errors);
			
			// Done
			UpdateToolbar(true);
		}
		
		// This applies user preferences
		public void ApplySettings()
		{
			errorlist.Columns[0].Width = General.Settings.ReadSetting("scriptspanel.errorscolumn0width", errorlist.Columns[0].Width);
			errorlist.Columns[1].Width = General.Settings.ReadSetting("scriptspanel.errorscolumn1width", errorlist.Columns[1].Width);
			errorlist.Columns[2].Width = General.Settings.ReadSetting("scriptspanel.errorscolumn2width", errorlist.Columns[2].Width);
			buttonwhitespace.Checked = General.Settings.ReadSetting("scriptspanel.showwhitespace", false); //mxd
			buttonwordwrap.Checked = General.Settings.ReadSetting("scriptspanel.wraplonglines", false); //mxd
			ApplyTabSettings(); //mxd
		}
		
		// This saves user preferences
		public void SaveSettings()
		{
			General.Settings.WriteSetting("scriptspanel.errorscolumn0width", errorlist.Columns[0].Width);
			General.Settings.WriteSetting("scriptspanel.errorscolumn1width", errorlist.Columns[1].Width);
			General.Settings.WriteSetting("scriptspanel.showwhitespace", buttonwhitespace.Checked); //mxd
			General.Settings.WriteSetting("scriptspanel.wraplonglines", buttonwordwrap.Checked); //mxd
		}

		//mxd
		private void ApplyTabSettings()
		{
			foreach(var tp in tabs.TabPages)
			{
				ScriptDocumentTab scripttab = (tp as ScriptDocumentTab);
				if(scripttab != null)
				{
					scripttab.WrapLongLines = buttonwordwrap.Checked;
					scripttab.ShowWhitespace = buttonwhitespace.Checked;
				}
			}
		}
		
		#endregion
		
		#region ================== Methods

		// Find Next
		public void FindNext(FindReplaceOptions options)
		{
			// Save the options
			findoptions = options;
			FindNext();
		}

		// Find Next with saved options
		public void FindNext()
		{
			if(!string.IsNullOrEmpty(findoptions.FindText) && (ActiveTab != null))
			{
				if(!ActiveTab.FindNext(findoptions))
					DisplayStatus(ScriptStatusType.Warning, "Can't find any occurence of \"" + findoptions.FindText + "\".");
			}
			else
			{
				General.MessageBeep(MessageBeepType.Default);
			}
		}

		// Find Previous
		public void FindPrevious(FindReplaceOptions options) 
		{
			// Save the options
			findoptions = options;
			FindPrevious();
		}

		// Find Previous with saved options (mxd)
		public void FindPrevious() 
		{
			if(!string.IsNullOrEmpty(findoptions.FindText) && (ActiveTab != null)) 
			{
				if(!ActiveTab.FindPrevious(findoptions))
					DisplayStatus(ScriptStatusType.Warning, "Can't find any occurence of \"" + findoptions.FindText + "\".");
			} 
			else 
			{
				General.MessageBeep(MessageBeepType.Default);
			}
		}
		
		// Replace if possible
		public void Replace(FindReplaceOptions options)
		{
			if(!string.IsNullOrEmpty(findoptions.FindText) && (options.ReplaceWith != null) && (ActiveTab != null))
			{
				if(string.Compare(ActiveTab.SelectedText, options.FindText, !options.CaseSensitive) == 0)
				{
					// Replace selection
					ActiveTab.ReplaceSelection(options.ReplaceWith);
				}
			}
			else
			{
				General.MessageBeep(MessageBeepType.Default);
			}
		}
		
		// Replace all
		public void ReplaceAll(FindReplaceOptions options)
		{
			int replacements = 0;
			findoptions = options;
			if(!string.IsNullOrEmpty(findoptions.FindText) && (options.ReplaceWith != null) && (ActiveTab != null))
			{
				int firstfindpos = -1;
				int lastpos = -1;
				bool firstreplace = true;
				bool wrappedaround = false;
				int selectionstart = Math.Min(ActiveTab.SelectionStart, ActiveTab.SelectionEnd);
				
				// Continue finding and replacing until nothing more found
				while(ActiveTab.FindNext(findoptions))
				{
					int curpos = Math.Min(ActiveTab.SelectionStart, ActiveTab.SelectionEnd);
					if(curpos <= lastpos)
						wrappedaround = true;
					
					if(firstreplace)
					{
						// Remember where we started replacing
						firstfindpos = curpos;
					}
					else if(wrappedaround)
					{
						// Make sure we don't go past our start point, or we could be in an endless loop
						if(curpos >= firstfindpos)
							break;
					}
					
					Replace(findoptions);
					replacements++;
					firstreplace = false;

					lastpos = curpos;
				}

				// Restore selection
				ActiveTab.SelectionStart = selectionstart;
				ActiveTab.SelectionEnd = selectionstart;
				
				// Show result
				if(replacements == 0)
					DisplayStatus(ScriptStatusType.Warning, "Can't find any occurence of \"" + findoptions.FindText + "\".");
				else
					DisplayStatus(ScriptStatusType.Info, "Replaced " + replacements + " occurences of \"" + findoptions.FindText + "\" with \"" + findoptions.ReplaceWith + "\".");
			}
			else
			{
				General.MessageBeep(MessageBeepType.Default);
			}
		}
		
		// This closed the Find & Replace subwindow
		public void CloseFindReplace(bool closing)
		{
			if(findreplaceform != null)
			{
				if(!closing) findreplaceform.Close();
				findreplaceform = null;
			}
		}

		// This opens the Find & Replace subwindow
		public void OpenFindAndReplace()
		{
			if(findreplaceform == null)
				findreplaceform = new ScriptFindReplaceForm();

			try
			{
				if(findreplaceform.Visible)
					findreplaceform.Focus();
				else
					findreplaceform.Show(this.ParentForm);

				if(ActiveTab.SelectionEnd != ActiveTab.SelectionStart)
					findreplaceform.SetFindText(ActiveTab.SelectedText);
			}
			catch(Exception)
			{
				// If we can't pop up the find/replace form right now, thats just too bad.
			}
		}

		// This refreshes all settings
		public void RefreshSettings()
		{
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				t.RefreshSettings();
			}
		}

		// This clears all error marks and hides the errors list
		public void ClearErrors()
		{
			// Hide list
			splitter.Panel2Collapsed = true;
			errorlist.Items.Clear();

			// Clear marks
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				t.ClearMarks();
			}
		}
		
		// This shows the errors panel with the given errors
		// Also updates the scripts with markers for the given errors
		public void ShowErrors(IEnumerable<CompilerError> errors)
		{
			// Copy list
			if(errors != null)
				compilererrors = new List<CompilerError>(errors);
			else
				compilererrors = new List<CompilerError>();
			
			// Fill list
			errorlist.BeginUpdate();
			errorlist.Items.Clear();
			int listindex = 1;
			foreach(CompilerError e in compilererrors)
			{
				ListViewItem ei = new ListViewItem(listindex.ToString());
				ei.ImageIndex = 0;
				ei.SubItems.Add(e.description);
				string filename = (e.filename.StartsWith("?") ? e.filename.Replace("?", "") : Path.GetFileName(e.filename)); //mxd
				string linenumber = (e.linenumber != CompilerError.NO_LINE_NUMBER ? " (line " + (e.linenumber + 1) + ")" : String.Empty); //mxd
				ei.SubItems.Add(filename + linenumber);
				ei.Tag = e;
				errorlist.Items.Add(ei);
				listindex++;
			}
			errorlist.EndUpdate();
			
			// Show marks on scripts
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				t.MarkScriptErrors(compilererrors);
			}
			
			// Show/hide panel
			splitter.Panel2Collapsed = (errorlist.Items.Count == 0);
		}
		
		// This writes all explicitly opened files to the configuration
		public void WriteOpenFilesToConfiguration()
		{
			List<string> files = new List<string>();
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				if(t.ExplicitSave) files.Add(t.Filename);
			}
			General.Map.Options.ScriptFiles = files;
		}
		
		// This asks to save files and returns the result
		public bool AskSaveAll()
		{
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				if(t.ExplicitSave)
				{
					if(!CloseScript(t, true)) return false;
				}
			}
			
			return true;
		}
		
		// This closes a script and returns true when closed
		private bool CloseScript(ScriptDocumentTab t, bool saveonly)
		{
			if(t.IsChanged)
			{
				// Ask to save
				DialogResult result = MessageBox.Show(this.ParentForm, "Do you want to save changes to " + t.Title + "?", "Close File", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch(result)
				{
					case DialogResult.Yes:
						if(!SaveScript(t)) return false;
						break;
					case DialogResult.Cancel:
						return false;
				}
			}
			
			if(!saveonly)
			{
				//mxd. Select tab to the left of the one we are going to close
				if(t == tabs.SelectedTab && tabs.SelectedIndex > 0)
					tabs.SelectedIndex--;
				
				// Close file
				tabs.TabPages.Remove(t);
				t.Dispose();
			}
			return true;
		}
		
		// This returns true when any of the implicit-save scripts are changed
		public bool CheckImplicitChanges()
		{
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				if(!t.ExplicitSave && t.IsChanged) return true;
			}
			return false;
		}
		
		// This forces the focus to the script editor
		public void ForceFocus()
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			tabs.Focus();
			if(t != null) t.Focus();
		}
		
		// This does an implicit save on all documents that use implicit saving
		// Call this to save the lumps before disposing the panel!
		public void ImplicitSave()
		{
			// Save all scripts
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				if(!t.ExplicitSave) t.Save();
			}
			
			UpdateToolbar(false);
		}
		
		// This updates the toolbar for the current status
		private void UpdateToolbar(bool focuseditor)
		{
			int numscriptsopen = tabs.TabPages.Count;
			int explicitsavescripts = 0;
			ScriptDocumentTab t = null;
			
			// Any explicit save scripts?
			foreach(ScriptDocumentTab dt in tabs.TabPages)
				if(dt.ExplicitSave) explicitsavescripts++;
			
			// Get current script, if any are open
			if(numscriptsopen > 0)
				t = (tabs.SelectedTab as ScriptDocumentTab);
			
			// Enable/disable buttons
			buttonsave.Enabled = (t != null && t.ExplicitSave && t.IsChanged);
			buttonsaveall.Enabled = (explicitsavescripts > 0);
			buttoncompile.Enabled = (t != null && t.Config.Compiler != null);
			buttonsearch.Enabled = (t != null); //mxd
			buttonkeywordhelp.Enabled = (t != null && !string.IsNullOrEmpty(t.Config.KeywordHelp));
			buttonscriptconfig.Enabled = (t != null && t.IsReconfigurable);
			buttonundo.Enabled = (t != null && t.Scintilla.CanUndo);
			buttonredo.Enabled = (t != null && t.Scintilla.CanRedo);
			buttoncopy.Enabled = (t != null && t.Scintilla.SelectionStart < t.Scintilla.SelectionEnd);
			buttoncut.Enabled = (t != null && t.Scintilla.SelectionStart < t.Scintilla.SelectionEnd);
			buttonpaste.Enabled = (t != null && t.Scintilla.CanPaste);
			buttonclose.Enabled = (t != null && t.IsClosable);
			buttonsnippets.DropDownItems.Clear(); //mxd
			buttonsnippets.Enabled = (t != null && t.Config.Snippets.Count > 0); //mxd
			buttonindent.Enabled = (t != null); //mxd
			buttonunindent.Enabled = (t != null && t.Scintilla.Lines[t.Scintilla.CurrentLine].Indentation > 0); //mxd
			buttonwhitespace.Enabled = (t != null); //mxd
			buttonwordwrap.Enabled = (t != null); //mxd
			
			if(t != null)
			{
				// Check the according script config in menu
				foreach(ToolStripMenuItem item in buttonscriptconfig.DropDownItems)
				{
					ScriptConfiguration config = (item.Tag as ScriptConfiguration);
					item.Checked = (config == t.Config);
				}

				//mxd. Add snippets
				if(t.Config != null && t.Config.Snippets.Count > 0)
				{
					if(t.Config.Snippets.Count > 0)
						foreach(string snippetname in t.Config.Snippets) buttonsnippets.DropDownItems.Add(snippetname).Click += OnInsertSnippetClick;
				}
				
				// Focus to script editor
				if(focuseditor) ForceFocus();
			}

			//mxd. Update script type description
			scripttype.Text = ((t != null && t.Config != null) ? t.Config.Description : "Plain Text");
		}

		//mxd
		private int GetTabPageIndex(string title)
		{
			if(tabs.TabPages.Count == 0) return -1;

			for(int i = 0; i < tabs.TabPages.Count; i++)
			{
				if(tabs.TabPages[i].Text == title) return i;
			}

			return -1;
		}

		// This opens the given file, returns null when failed
		public ScriptFileDocumentTab OpenFile(string filename)
		{
			ScriptConfiguration foundconfig = new ScriptConfiguration();

			// Find the most suitable script configuration to use
			foreach(ScriptConfiguration cfg in scriptconfigs)
			{
				foreach(string ext in cfg.Extensions)
				{
					// Use this configuration if the extension matches
					if(filename.EndsWith("." + ext, StringComparison.OrdinalIgnoreCase))
					{
						foundconfig = cfg;
						break;
					}
				}
			}

			// Create new document
			ScriptFileDocumentTab t = new ScriptFileDocumentTab(this, foundconfig);
			if(t.Open(filename))
			{
				//mxd
				ScriptType st = t.VerifyScriptType();
				if(st != ScriptType.UNKNOWN) 
				{
					foreach(ScriptConfiguration cfg in scriptconfigs) 
					{
						if(cfg.ScriptType == st) 
						{
							t.ChangeScriptConfig(cfg);
							break;
						}
					}
				}
				
				// Mark any errors this script may have
				if(compilererrors != null) t.MarkScriptErrors(compilererrors);

				// Add to tabs
				tabs.TabPages.Add(t);
				tabs.SelectedTab = t;

				// Done
				t.OnTextChanged += tabpage_OnTextChanged; //mxd
				t.Scintilla.UpdateUI += scintilla_OnUpdateUI;
				UpdateToolbar(true);
				return t;
			}

			// Failed
			return null;
		}

		// This saves the current open script
		public void ExplicitSaveCurrentTab()
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			if((t != null))
			{
				if(t.ExplicitSave)
					buttonsave_Click(this, EventArgs.Empty);
				else if(t.Config.Compiler != null) //mxd
					buttoncompile_Click(this, EventArgs.Empty);
				else
					General.MessageBeep(MessageBeepType.Default);
			}
			else
			{
				General.MessageBeep(MessageBeepType.Default);
			}
		}
		
		// This opens a script
		public void OpenBrowseScript()
		{
			buttonopen_Click(this, EventArgs.Empty);
		}

		//mxd. This launches keyword help website
		public bool LaunchKeywordHelp() 
		{
			// Get script
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			return (t != null && t.LaunchKeywordHelp());
		}

		//mxd. This changes status text
		private void DisplayStatus(ScriptStatusType type, string message) { DisplayStatus(new ScriptStatusInfo(type, message)); }
		private void DisplayStatus(ScriptStatusInfo newstatus)
		{
			// Stop timers
			if(!newstatus.displayed)
			{
				statusresetter.Stop();
				statusflasher.Stop();
				statusflashicon = false;
			}

			// Determine what to do specifically for this status type
			switch(newstatus.type)
			{
				// Shows information without flashing the icon.
				case ScriptStatusType.Ready:
				case ScriptStatusType.Info:
					if(!newstatus.displayed)
					{
						statusresetter.Interval = MainForm.INFO_RESET_DELAY;
						statusresetter.Start();
					}
					break;

				// Shows a warning, makes a warning sound and flashes a warning icon.
				case ScriptStatusType.Warning:
					if(!newstatus.displayed)
					{
						General.MessageBeep(MessageBeepType.Warning);
						statusflasher.Interval = MainForm.WARNING_FLASH_INTERVAL;
						statusflashcount = MainForm.WARNING_FLASH_COUNT;
						statusflasher.Start();
						statusresetter.Interval = MainForm.WARNING_RESET_DELAY;
						statusresetter.Start();
					}
					break;
			}

			// Update status description
			status = newstatus;
			status.displayed = true;
			statuslabel.Text = status.message;

			// Update icon as well
			UpdateStatusIcon();

			// Refresh
			statusbar.Invalidate();
			this.Update();
		}

		// This updates the status icon
		private void UpdateStatusIcon()
		{
			int statusflashindex = (statusflashicon ? 1 : 0);

			// Status type
			switch(status.type)
			{
				case ScriptStatusType.Ready:
				case ScriptStatusType.Info:
					statuslabel.Image = General.MainWindow.STATUS_IMAGES[statusflashindex, 0];
					break;

				case ScriptStatusType.Busy:
					statuslabel.Image = General.MainWindow.STATUS_IMAGES[statusflashindex, 2];
					break;

				case ScriptStatusType.Warning:
					statuslabel.Image = General.MainWindow.STATUS_IMAGES[statusflashindex, 3];
					break;

				default:
					throw new NotImplementedException("Unsupported Script Status Type!");
			}
		}
		
		#endregion
		
		#region ================== Events

		// Called when the window that contains this panel closes
		public void OnClose()
		{
			//mxd. Store quick search settings
			matchcase = searchmatchcase.Checked;
			matchwholeword = searchwholeword.Checked;

			//mxd. Stop status timers
			statusresetter.Stop();
			statusflasher.Stop();
			
			// Close the sub windows now
			if(findreplaceform != null) findreplaceform.Dispose();
		}

		// Keyword help requested
		private void buttonkeywordhelp_Click(object sender, EventArgs e)
		{
			LaunchKeywordHelp();
		}

		// When the user changes the script configuration
		private void buttonscriptconfig_Click(object sender, EventArgs e)
		{
			// Get the tab and new script config
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			ScriptConfiguration scriptconfig = ((sender as ToolStripMenuItem).Tag as ScriptConfiguration);
			
			// Change script config
			t.ChangeScriptConfig(scriptconfig);

			//mxd. Update script type description
			scripttype.Text = scriptconfig.Description;

			// Done
			UpdateToolbar(true);
		}
		
		// When new script is clicked
		private void buttonnew_Click(object sender, EventArgs e)
		{
			// Get the script config to use
			ScriptConfiguration scriptconfig = ((sender as ToolStripMenuItem).Tag as ScriptConfiguration);
			
			// Create new document
			ScriptFileDocumentTab t = new ScriptFileDocumentTab(this, scriptconfig);
			tabs.TabPages.Add(t);
			tabs.SelectedTab = t;
			
			// Done
			UpdateToolbar(true);
		}
		
		// Open script clicked
		private void buttonopen_Click(object sender, EventArgs e)
		{
			// Show open file dialog
			if(openfile.ShowDialog(this.ParentForm) == DialogResult.OK)
			{
				//mxd. Gather already opened file names
				List<string> openedfiles = new List<string>();
				foreach(var page in tabs.TabPages)
				{
					var scriptpage = page as ScriptFileDocumentTab;
					if(scriptpage != null) openedfiles.Add(scriptpage.Filename);
				}

				//mxd. Add new tabs
				foreach(string name in openfile.FileNames)
				{
					if(!openedfiles.Contains(name)) OpenFile(name);
				}

				// Select the last new item
				foreach(var page in tabs.TabPages)
				{
					var scriptpage = page as ScriptFileDocumentTab;
					if(scriptpage != null && scriptpage.Filename == openfile.FileNames[openfile.FileNames.Length - 1])
					{
						tabs.SelectedTab = scriptpage;
						break;
					}
				}
			}
		}

		// Save script clicked
		private void buttonsave_Click(object sender, EventArgs e)
		{
			// Save the current script
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			SaveScript(t);
			UpdateToolbar(true);
		}

		// Save All clicked
		private void buttonsaveall_Click(object sender, EventArgs e)
		{
			// Save all scripts
			foreach(ScriptDocumentTab t in tabs.TabPages)
			{
				// Use explicit save for this script?
				if(t.ExplicitSave)
				{
					if(!SaveScript(t)) break;
				}
			}
			
			UpdateToolbar(true);
		}

		// This is called by Save and Save All to save a script
		// Returns false when cancelled by the user
		private bool SaveScript(ScriptDocumentTab t)
		{
			// Do we have to do a save as?
			if(t.IsSaveAsRequired)
			{
				// Setup save dialog
				string scriptfilter = t.Config.Description + "|*." + string.Join(";*.", t.Config.Extensions);
				savefile.Filter = scriptfilter + "|All files|*.*";
				if(savefile.ShowDialog(this.ParentForm) == DialogResult.OK)
				{
					// Save to new filename
					t.SaveAs(savefile.FileName);
					return true;
				}
				
				// Cancelled
				return false;
			}

			// Save to same filename
			t.Save();
			return true;
		}
		
		// A tab is selected
		private void tabs_Selecting(object sender, TabControlCancelEventArgs e)
		{
			UpdateToolbar(true);
		}
		
		// This closes the current file
		private void buttonclose_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			CloseScript(t, false);
			UpdateToolbar(true);
		}
		
		// Compile Script clicked
		private void buttoncompile_Click(object sender, EventArgs e)
		{
			// First save all implicit scripts to the temporary wad file
			ImplicitSave();
			
			// Get script
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);

			// Check if it must be saved as a new file
			if(t.ExplicitSave && t.IsSaveAsRequired)
			{
				// Save the script first!
				if(MessageBox.Show(this.ParentForm, "You must save your script before you can compile it. Do you want to save your script now?", "Compile Script", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					if(!SaveScript(t)) return;
				}
				else
				{
					return;
				}
			}
			else if(t.ExplicitSave && t.IsChanged)
			{
				// We can only compile when the script is saved
				if(!SaveScript(t)) return;
			}

			// Compile now
			DisplayStatus(ScriptStatusType.Busy, "Compiling script \"" + t.Title + "\"...");
			Cursor.Current = Cursors.WaitCursor;
			t.Compile();

			// Show warning
			if((compilererrors != null) && (compilererrors.Count > 0))
				DisplayStatus(ScriptStatusType.Warning, compilererrors.Count + " errors while compiling \"" + t.Title + "\"!");
			else
				DisplayStatus(ScriptStatusType.Info, "Script \"" + t.Title + "\" compiled without errors.");

			Cursor.Current = Cursors.Default;
			UpdateToolbar(true);
		}
		
		// Undo clicked
		private void buttonundo_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Undo();
			UpdateToolbar(true);
		}
		
		// Redo clicked
		private void buttonredo_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Redo();
			UpdateToolbar(true);
		}
		
		// Cut clicked
		private void buttoncut_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Cut();
			UpdateToolbar(true);
		}
		
		// Copy clicked
		private void buttoncopy_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Copy();
			UpdateToolbar(true);
		}

		// Paste clicked
		private void buttonpaste_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.Paste();
			UpdateToolbar(true);
		}

		//mxd
		private void buttonunindent_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.IndentSelection(false);
		}

		//mxd
		private void buttonindent_Click(object sender, EventArgs e)
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.IndentSelection(true);
		}

		//mxd
		private void buttonwhitespace_Click(object sender, EventArgs e)
		{
			ApplyTabSettings();
		}

		//mxd
		private void buttonwordwrap_Click(object sender, EventArgs e)
		{
			ApplyTabSettings();
		}

		//mxd. Search clicked
		private void buttonsearch_Click(object sender, EventArgs e) 
		{
			OpenFindAndReplace();
		}

		//mxd
		private void OnInsertSnippetClick(object sender, EventArgs eventArgs) 
		{
			ScriptDocumentTab t = (tabs.SelectedTab as ScriptDocumentTab);
			t.InsertSnippet( ((ToolStripItem)sender).Text );
		}
		
		// Mouse released on tabs
		private void tabs_MouseUp(object sender, MouseEventArgs e)
		{
			ForceFocus();
		}

		//mxd. Text in ScriptFileDocumentTab was changed
		private void tabpage_OnTextChanged(object sender, EventArgs eventArgs)
		{
			if(tabs.SelectedTab != null)
			{
				ScriptDocumentTab curtab = tabs.SelectedTab as ScriptDocumentTab;
				if(curtab != null)
				{
					buttonsave.Enabled = (curtab.ExplicitSave && curtab.IsChanged);
					buttonundo.Enabled = curtab.Scintilla.CanUndo;
					buttonredo.Enabled = curtab.Scintilla.CanRedo;
				}
			}
		}

		//mxd. Text in ScriptLumpDocumentTab was changed
		private void tabpage_OnLumpTextChanged(object sender, EventArgs e)
		{
			if(tabs.SelectedTab != null)
			{
				ScriptDocumentTab curtab = tabs.SelectedTab as ScriptDocumentTab;
				if(curtab != null)
				{
					buttonundo.Enabled = curtab.Scintilla.CanUndo;
					buttonredo.Enabled = curtab.Scintilla.CanRedo;
				}
			}
		}

		//mxd
		private void scintilla_OnUpdateUI(object sender, UpdateUIEventArgs e)
		{
			Scintilla s = sender as Scintilla;
			if(s != null)
			{
				// Update caret position info [line] : [caret pos start] OR [caret pos start x selection length] ([total lines])
				positionlabel.Text = (s.CurrentLine + 1) + " : " 
					+ (s.SelectionStart + 1 - s.Lines[s.LineFromPosition(s.SelectionStart)].Position) 
					+ (s.SelectionStart != s.SelectionEnd ? "x" + (s.SelectionEnd - s.SelectionStart) : "") 
					+ " (" + s.Lines.Count + ")";

				// Update copy-paste buttons
				buttoncut.Enabled = (s.SelectionEnd > s.SelectionStart);
				buttoncopy.Enabled = (s.SelectionEnd > s.SelectionStart);
				buttonpaste.Enabled = s.CanPaste;
				buttonunindent.Enabled = s.Lines[s.CurrentLine].Indentation > 0;
			}
		}
		
		// User double-clicks and error in the list
		private void errorlist_ItemActivate(object sender, EventArgs e)
		{
			// Anything selection?
			if(errorlist.SelectedItems.Count > 0)
			{
				// Get the compiler error
				CompilerError err = (CompilerError)errorlist.SelectedItems[0].Tag;
				
				// Show the tab with the script that matches
				bool foundscript = false;
				foreach(ScriptDocumentTab t in tabs.TabPages)
				{
					if(t.VerifyErrorForScript(err))
					{
						tabs.SelectedTab = t;
						t.MoveToLine(err.linenumber);
						foundscript = true;
						break;
					}
				}

				// If we don't have the script opened, see if we can find the file and open the script
				if(!foundscript && File.Exists(err.filename))
				{
					ScriptDocumentTab t = OpenFile(err.filename);
					if(t != null) t.MoveToLine(err.linenumber);
				}
				
				ForceFocus();
			}
		}
		
		#endregion

		#region ================== Quick Search (mxd)

		private FindReplaceOptions GetQuickSearchOptions()
		{
			return new FindReplaceOptions 
			{
				CaseSensitive = searchmatchcase.Checked,
				WholeWord = searchwholeword.Checked,
				FindText = searchbox.Text
			};
		}

		private void searchbox_TextChanged(object sender, EventArgs e)
		{
			bool success = (searchbox.Text.Length > 0 && ActiveTab.FindNext(GetQuickSearchOptions(), true));
			searchbox.BackColor = ((success || searchbox.Text.Length == 0) ? SystemColors.Window : Color.MistyRose);
			searchnext.Enabled = success;
			searchprev.Enabled = success;
		}

		private void searchnext_Click(object sender, EventArgs e)
		{
			ActiveTab.FindNext(GetQuickSearchOptions());
		}

		private void searchprev_Click(object sender, EventArgs e) 
		{
			ActiveTab.FindPrevious(GetQuickSearchOptions());
		}

		//mxd. This flashes the status icon
		private void statusflasher_Tick(object sender, EventArgs e)
		{
			statusflashicon = !statusflashicon;
			UpdateStatusIcon();
			statusflashcount--;
			if(statusflashcount == 0) statusflasher.Stop();
		}

		//mxd. This resets the status to ready
		private void statusresetter_Tick(object sender, EventArgs e)
		{
			DisplayStatus(ScriptStatusType.Ready, null);
		}

		#endregion
		
	}
}
