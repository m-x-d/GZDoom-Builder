
#region ================== Copyright (c) 2009 Pascal vd Heiden

/*
 * Copyright (c) 2009 Pascal vd Heiden, www.codeimp.com
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.MassUndoRedo
{
	public partial class UndoRedoForm : DelayedForm
	{
		#region ================== Variables
		
		#endregion
		
		#region ================== Constructor
		
		// Constructor
		public UndoRedoForm()
		{
			InitializeComponent();
		}
		
		#endregion
		
		#region ================== Methods
		
		// This sets up the form for display
		public void Setup(Form owner)
		{
			List<UndoSnapshot> levels;
			
			// Position window on the left side of the main window
			SizeF scalefactor = new SizeF(owner.CurrentAutoScaleDimensions.Width / owner.AutoScaleDimensions.Width,
										  owner.CurrentAutoScaleDimensions.Height / owner.AutoScaleDimensions.Height);
			int topoffset = SystemInformation.CaptionHeight + (int)(80 * scalefactor.Height);
			this.Location = new Point(owner.Location.X + (int)(20 * scalefactor.Width), owner.Location.Y + topoffset);
			this.Height = owner.Height - (topoffset + (int)(50 * scalefactor.Height));
			
			// Fill the list
			list.Items.Clear();
			
			levels = General.Map.UndoRedo.GetUndoList();
			levels.Reverse();
			foreach(UndoSnapshot u in levels)
			{
				list.Items.Add(u.Description);
			}
			
			levels = General.Map.UndoRedo.GetRedoList();
			foreach(UndoSnapshot r in levels)
			{
				ListViewItem item = list.Items.Add(r.Description);
				item.BackColor = SystemColors.Control;
				item.ForeColor = SystemColors.GrayText;
			}
		}
		
		#endregion
		
		#region ================== Events
		
		#endregion
	}
}