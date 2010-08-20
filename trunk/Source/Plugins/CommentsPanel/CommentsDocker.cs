#region ================== Copyright (c) 2010 Pascal vd Heiden

/*
 * Copyright (c) 2010 Pascal vd Heiden, www.codeimp.com
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Windows;
using System.Reflection;
using System.Globalization;
using System.Threading;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.CommentsPanel
{
	public partial class CommentsDocker : UserControl
	{
		#region ================== Variables
		
		#endregion
		
		#region ================== Constructor

		// Constructor
		public CommentsDocker()
		{
			InitializeComponent();
		}

		// Disposer
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			
			base.Dispose(disposing);
		}
		
		#endregion

		#region ================== Methods

		// When attached to the docker
		public void Setup()
		{
			if(this.ParentForm != null)
			{
				this.ParentForm.Activated += ParentForm_Activated;
			}

			updatetimer.Start();
		}

		// Before detached from the docker
		public void Terminate()
		{
			if(this.ParentForm != null)
			{
				this.ParentForm.Activated -= ParentForm_Activated;
			}

			updatetimer.Stop();
		}

		// This finds all comments and updates the list
		public void UpdateList()
		{
			// Build a dictionary out of all comments
			Dictionary<string, CommentInfo> comments = new Dictionary<string, CommentInfo>();
			foreach(Vertex v in General.Map.Map.Vertices) AddComments(v, comments);
			foreach(Linedef l in General.Map.Map.Linedefs) AddComments(l, comments);
			foreach(Sidedef sd in General.Map.Map.Sidedefs) AddComments(sd, comments);
			foreach(Sector s in General.Map.Map.Sectors) AddComments(s, comments);
			foreach(Thing t in General.Map.Map.Things) AddComments(t, comments);

			// Fill the list with comments
			grid.Rows.Clear();
			foreach(KeyValuePair<string, CommentInfo> c in comments)
			{
				//Image icon = (e.type == ErrorType.Error) ? Properties.Resources.ErrorLarge : Properties.Resources.WarningLarge;
				int index = grid.Rows.Add();
				DataGridViewRow row = grid.Rows[index];
				//row.Cells[0].Value = icon;
				row.Cells[0].Style.Alignment = DataGridViewContentAlignment.TopCenter;
				row.Cells[0].Style.Padding = new Padding(0, 5, 0, 0);
				row.Cells[1].Value = c.Key;
				row.Cells[1].Style.WrapMode = DataGridViewTriState.True;
			}
		}

		// This adds comments from a MapElement
		private void AddComments(MapElement e, Dictionary<string, CommentInfo> comments)
		{
			if(e.Fields.ContainsKey("comment"))
			{
				string c = e.Fields["comment"].Value.ToString();
				if(comments.ContainsKey(c))
					comments[c].Elements.Add(e);
				else
					comments[c] = new CommentInfo(c, e);
			}
		}

		#endregion

		#region ================== Events

		// This gives a good idea when comments could have been changed
		// as it is called every time a dialog window closes.
		private void ParentForm_Activated(object sender, EventArgs e)
		{
			UpdateList();
		}

		// Update regulary
		private void updatetimer_Tick(object sender, EventArgs e)
		{
			UpdateList();
		}

		#endregion
	}
}
