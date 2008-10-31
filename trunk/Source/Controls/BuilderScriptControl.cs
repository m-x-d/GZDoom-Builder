
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

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public partial class BuilderScriptControl : UserControl
	{
		#region ================== Delegates / Events

		#endregion

		#region ================== Constants

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Contructor / Disposer

		// Constructor
		public BuilderScriptControl()
		{
			// Initialize
			InitializeComponent();

			// Script editor properties
			scriptedit.IsBackSpaceUnIndents = true;
			scriptedit.IsBufferedDraw = true;
			scriptedit.SetFoldFlags((int)ScriptFoldFlag.Box);
			scriptedit.IsUseTabs = true;
			scriptedit.IsViewEOL = false;
			scriptedit.IsVScrollBar = true;
			scriptedit.IsHScrollBar = true;
			scriptedit.IsCaretLineVisible = false;
			scriptedit.IsIndentationGuides = true;
			scriptedit.IsMouseDownCaptures = true;
			scriptedit.IsTabIndents = true;
			scriptedit.IsUndoCollection = true;
			scriptedit.EndOfLineMode = ScriptEndOfLine.CRLF;
			scriptedit.EndAtLastLine = 1;
			scriptedit.SetMarginWidthN(0, 16);
			scriptedit.SetMarginTypeN(0, (int)ScriptMarginType.Symbol);
			scriptedit.SetMarginWidthN(1, 42);
			scriptedit.SetMarginTypeN(1, (int)ScriptMarginType.Number);
		}

		#endregion

		#region ================== Methods
		
		// This sets up the script editor with a script configuration
		
		
		#endregion

		#region ================== Events

		#endregion
	}
}
