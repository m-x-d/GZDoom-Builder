
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
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Rendering;
using SlimDX.Direct3D9;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using CodeImp.DoomBuilder.Map;
using System.Globalization;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal partial class DockersControl : UserControl
	{
		#region ================== Constants
		
		#endregion

		#region ================== Delegates
		
		public event EventHandler MouseContainerEnter;
		public event EventHandler MouseContainerLeave;
		public event EventHandler Collapsed;
		public event EventHandler Expanded;
		
		#endregion
		
		#region ================== Variables

		private bool rightalign;
		private bool iscollapsed;
		
		private int expandedwidth;		// width when expanded
		private int expandedtab;		// selected tab index when expanded
		
		#endregion

		#region ================== Properties
		
		public bool IsCollpased { get { return iscollapsed; } }
		
		#endregion
		
		#region ================== Constructor

		// Constructor
		public DockersControl()
		{
			InitializeComponent();
			expandedwidth = (int)((float)this.Width * (this.CurrentAutoScaleDimensions.Width / this.AutoScaleDimensions.Width));
		}
		
		#endregion
		
		#region ================== Methods
		
		// This sets up the controls for left or right alignment
		public void Setup(bool right)
		{
			rightalign = right;
			if(rightalign)
			{
				splitter.Dock = DockStyle.Left;
				tabs.Alignment = TabAlignment.Right;
				tabs.Location = new Point(0, 0);
				tabs.Size = new Size(this.ClientRectangle.Width + 2, this.ClientRectangle.Height);
			}
			else
			{
				splitter.Dock = DockStyle.Right;
				tabs.Alignment = TabAlignment.Left;
				tabs.Location = new Point(-2, 0);
				tabs.Size = new Size(this.ClientRectangle.Width + 2, this.ClientRectangle.Height);
			}
			
			tabs.SendToBack();
		}
		
		// This collapses the docker
		public void Collapse()
		{
			if(iscollapsed) return;
			
			splitter.Visible = false;
			expandedtab = tabs.SelectedIndex;
			expandedwidth = this.Width;
			tabs.SelectedIndex = -1;
			General.LockWindowUpdate(Parent.Handle);
			if(rightalign) this.Left = this.Right - GetCollapsedWidth();
			this.Width = GetCollapsedWidth();
			General.LockWindowUpdate(IntPtr.Zero);
			
			iscollapsed = true;
			
			if(Collapsed != null) Collapsed(this, EventArgs.Empty);
		}
		
		// This expands the docker
		public void Expand()
		{
			if(!iscollapsed) return;
			
			splitter.Visible = true;
			General.LockWindowUpdate(Parent.Handle);
			if(rightalign) this.Left = this.Right - expandedwidth;
			this.Width = expandedwidth;
			General.LockWindowUpdate(IntPtr.Zero);
			tabs.SelectedIndex = expandedtab;
			tabs.Invalidate();
			
			iscollapsed = false;
			
			if(Expanded != null) Expanded(this, EventArgs.Empty);
		}
		
		// This calculates the collapsed width
		public int GetCollapsedWidth()
		{
			// Downside of this function is that we need a tab :\
			Rectangle r = tabs.GetTabRect(0);
			return r.Width + (int)(2.0f * (this.CurrentAutoScaleDimensions.Width / this.AutoScaleDimensions.Width));
		}
		
		// This adds a docker
		public void Add(Docker d)
		{
			TabPage page = new TabPage(d.Title);
			page.Font = this.Font;
			page.Tag = d;
			page.UseVisualStyleBackColor = false;
			page.Controls.Add(d.Panel);
			tabs.TabPages.Add(page);
		}
		
		// This removes a docker
		public bool Remove(Docker d)
		{
			foreach(TabPage page in tabs.TabPages)
			{
				if((page.Tag as Docker) == d)
				{
					page.Controls.Clear();
					tabs.TabPages.Remove(page);
					return true;
				}
			}
			
			return false;
		}
		
		// This sorts tabs by their full name
		public void SortTabs(IEnumerable<string> fullnames)
		{
			Dictionary<string, TabPage> pages = new Dictionary<string, TabPage>(tabs.TabPages.Count);
			foreach(TabPage p in tabs.TabPages) pages.Add((p.Tag as Docker).FullName, p);
			tabs.TabPages.Clear();
			
			// Add tabs in order as in fullnames
			foreach(string name in fullnames)
			{
				if(pages.ContainsKey(name))
				{
					tabs.TabPages.Add(pages[name]);
					pages.Remove(name);
				}
			}
			
			// Add remaining tabs
			foreach(KeyValuePair<string, TabPage> p in pages)
				tabs.TabPages.Add(p.Value);
		}
		
		#endregion
		
		#region ================== Events
		
		// This raises the MouseContainerEnter event
		private void RaiseMouseContainerEnter(object sender, EventArgs e)
		{
			if(MouseContainerEnter != null)
				MouseContainerEnter(sender, e);
		}

		// This raises the MouseContainerLeave event
		private void RaiseMouseContainerLeave(object sender, EventArgs e)
		{
			if(MouseContainerLeave != null)
				MouseContainerLeave(sender, e);
		}
		
		// We don't want the focus
		private void tabs_Enter(object sender, EventArgs e) { General.MainWindow.FocusDisplay(); }
		private void tabs_MouseUp(object sender, MouseEventArgs e) { General.MainWindow.FocusDisplay(); }
		
		#endregion

		private void tabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			General.MainWindow.FocusDisplay();
		}

		private void tabs_Selected(object sender, TabControlEventArgs e)
		{
			General.MainWindow.FocusDisplay();
		}

		protected override void OnEnter(EventArgs e)
		{
			General.MainWindow.FocusDisplay();
		}
	}
}
