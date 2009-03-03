
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
using CodeImp.DoomBuilder.Map;
using System.IO;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Controls;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	public partial class ScriptFindReplaceForm : DelayedForm
	{
		#region ================== Variables

		#endregion

		#region ================== Properties
		
		#endregion

		#region ================== Constructor

		// Constructor
		public ScriptFindReplaceForm()
		{
			InitializeComponent();
		}

		#endregion

		#region ================== Events

		// Find Next
		private void findnextbutton_Click(object sender, EventArgs e)
		{
			FindReplaceOptions options = new FindReplaceOptions();
			options.FindText = findtext.Text;
			options.CaseSensitive = casesensitive.Checked;
			options.WholeWord = wordonly.Checked;
			options.ReplaceWith = replacetext.Text;
			
			if(!General.Map.ScriptEditor.Editor.ActiveTab.FindNext(options))
			{
				// No such thing
				General.MainWindow.DisplayStatus(StatusType.Warning, "Can't find any occurence of \"" + findtext.Text + "\".");
			}
		}

		#endregion
	}
}