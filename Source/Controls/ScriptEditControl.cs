
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

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public partial class ScriptEditControl : RichTextBox
	{
		#region ================== Constants

		// Update delay in milliseconds
		private const int UPDATE_DELAY = 100;

		#endregion
		
		#region ================== Variables

		// Range in which updating is required
		// Only the timer may reset this range, the key press methods 
		private int updatestartline;
		private int updateendline;
		
		// Line information
		private ScriptLineInfo[] lineinfo;

		// Update timer
		private Timer updatetimer;

		#endregion

		#region ================== Properties
		
		// These prevent changing these properties
		public new bool AcceptsTab { get { return base.AcceptsTab; } }
		public new bool AllowDrop { get { return base.AllowDrop; } }
		public new bool AutoWordSelection { get { return base.AutoWordSelection; } }
		public new bool EnableAutoDragDrop { get { return base.EnableAutoDragDrop; } }
		public new bool Multiline { get { return base.Multiline; } }
		public new bool ShortcutsEnabled { get { return base.ShortcutsEnabled; } }
		public new bool WordWrap { get { return base.WordWrap; } }
		public new float ZoomFactor { get { return base.ZoomFactor; } }

		#endregion

		#region ================== Contructor

		// Constructor
		public ScriptEditControl()
		{
			// Properties that we need to have set this way.
			// No, you cannot choose these yourself.
			base.AcceptsTab = true;
			base.AllowDrop = false;
			base.AutoWordSelection = false;
			base.EnableAutoDragDrop = false;
			base.Multiline = true;
			base.ShortcutsEnabled = false;
			base.WordWrap = false;
			base.ZoomFactor = 1.0f;

			// Initialize
			if(!this.DesignMode)
			{
				lineinfo = new ScriptLineInfo[1];
				lineinfo[0] = new ScriptLineInfo(ScriptMarking.None);
				updatetimer = new Timer();
				updatetimer.Interval = UPDATE_DELAY;
				updatetimer.Tick += new EventHandler(OnUpdateTimerTick);
			}
		}

		// Disposer
		protected override void Dispose(bool disposing)
		{
			// Disposing?
			if(!base.IsDisposed)
			{
				// Dispose managed resources
				if(disposing)
				{
					if(!this.DesignMode)
					{
						updatetimer.Stop();
						updatetimer.Dispose();
						lineinfo = null;
					}
				}
			}
			
			base.Dispose(disposing);
		}
		
		#endregion

		#region ================== Events

		// Key pressed
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			
			// Check in which range this keypress must update the highlighting.
			// The range may only be increased, not decreased!
			int startline = base.GetLineFromCharIndex(base.SelectionStart);
			int endline = base.GetLineFromCharIndex(base.SelectionStart + base.SelectionLength);
			if(startline < updatestartline) updatestartline = startline;
			if(endline < updatestartline) updatestartline = endline;
			if(startline > updateendline) updateendline = startline;
			if(endline > updateendline) updateendline = endline;
		}

		// Key presses (repeating)
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			
			// (Re)start timer!
			updatetimer.Stop();
			updatetimer.Start();
		}
		
		// Time to update!
		private void OnUpdateTimerTick(object sender, EventArgs e)
		{
			updatetimer.Stop();
			UpdateRange();
		}

		#endregion
		
		#region ================== Methods

		// This updates the range
		private void UpdateRange()
		{
			bool continueupdate = false;
			int updateline = updatestartline;
			int selstart = base.SelectionStart;
			int sellength = base.SelectionLength;
			General.LockWindowUpdate(base.Handle);
			
			if(updateendline > base.Lines.Length) updateendline = base.Lines.Length;
			
			// First make sure the lineinfo array is big enough
			if(base.Lines.Length >= lineinfo.Length)
			{
				// Resize array
				ScriptLineInfo[] oldlineinfo = lineinfo;
				lineinfo = new ScriptLineInfo[base.Lines.Length + 1];
				oldlineinfo.CopyTo(lineinfo, 0);
			}
			
			// Start updating the range
			// Or go beyond the range when the result has influence on the next line
			while((updateline <= updateendline) || (continueupdate && (updateline < base.Lines.Length)))
			{
				continueupdate = UpdateLine(updateline++);
			}

			// Reset the range to current position/selection
			int startline = base.GetLineFromCharIndex(selstart);
			int endline = base.GetLineFromCharIndex(selstart + sellength);
			updatestartline = Math.Min(startline, endline);
			updateendline = Math.Max(startline, endline);

			// Restore selection
			base.SelectionStart = selstart;
			base.SelectionLength = sellength;
			General.LockWindowUpdate(IntPtr.Zero);
		}
		
		// This parses a single line to update the syntax highlighting
		// NOTE: Before calling this make sure the lineinfo array is resized correctly!
		// NOTE: This function changes the selection and does not prevent any redrawing!
		// Returns true if the change continues on the next line (multi-line comment changes)
		private bool UpdateLine(int lineindex)
		{
			// Get the start information
			ScriptLineInfo info = lineinfo[lineindex];
			ScriptLineInfo endinfo = lineinfo[lineindex + 1];
			string text = base.Lines[lineindex];
			int lineoffset = base.GetFirstCharIndexFromLine(lineindex);
			int curpos = 0;
			int prevpos = 0;
			
			// TODO: Scan the line
			// TODO: Use regexes for this?

			// TEST: Block comment only
			int startcurpos;
			do
			{
				startcurpos = curpos;
				
				// If we're in a block comment, we first have to find the block ending
				if(info.startmarking == ScriptMarking.BlockComment)
				{
					int endpos = text.IndexOf("*/");
					if(endpos > -1)
					{
						// Block comment ends here
						curpos = endpos + 2;
						base.SelectionStart = lineoffset + prevpos;
						base.SelectionLength = lineoffset + curpos;
						base.SelectionColor = General.Colors.Comments.ToColor();
						info.startmarking = ScriptMarking.None;
						prevpos = curpos;
					}
				}

				// Out of the block comment?
				if(info.startmarking != ScriptMarking.BlockComment)
				{
					int endpos = text.IndexOf("/*");
					if(endpos > -1)
					{
						// Block comment starts here
						curpos = endpos;
						base.SelectionStart = lineoffset + prevpos;
						base.SelectionLength = lineoffset + curpos;
						base.SelectionColor = General.Colors.PlainText.ToColor();
						info.startmarking = ScriptMarking.BlockComment;
						prevpos = curpos;
					}
				}
			}
			while(startcurpos < curpos);
			
			// More to mark?
			if(prevpos < text.Length)
			{
				if(info.startmarking == ScriptMarking.BlockComment)
				{
					base.SelectionStart = lineoffset + prevpos;
					base.SelectionLength = lineoffset + text.Length;
					base.SelectionColor = General.Colors.Comments.ToColor();
				}
				else
				{
					base.SelectionStart = lineoffset + prevpos;
					base.SelectionLength = lineoffset + text.Length;
					base.SelectionColor = General.Colors.PlainText.ToColor();
				}
			}
			
			// Update next line info
			lineinfo[lineindex + 1] = info;
			
			// Check if this changes anything on the next line
			return ((info.startmarking == ScriptMarking.BlockComment) &&
					(endinfo.startmarking != ScriptMarking.BlockComment)) ||
				   ((info.startmarking != ScriptMarking.BlockComment) &&
					(endinfo.startmarking == ScriptMarking.BlockComment));
		}
		
		#endregion
	}
}
