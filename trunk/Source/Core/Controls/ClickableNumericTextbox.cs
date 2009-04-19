
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
using System.Globalization;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public partial class ClickableNumericTextbox : UserControl
	{
		#region ================== Events

		public event EventHandler ValueChanged;

		#endregion
		
		#region ================== Properties

		public int Minimum { get { return buttons.Minimum; } set { buttons.Minimum = value; } }
		public int Maximum { get { return buttons.Maximum; } set { buttons.Maximum = value; } }
		public bool AllowNegative { get { return textbox.AllowNegative; } set { textbox.AllowNegative = value; } }
		public int Value { get { return buttons.Value; } set { buttons.Value = value; } }
		
		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		public ClickableNumericTextbox()
		{
			InitializeComponent();
		}

		#endregion

		#region ================== Interface

		// Client size changes
		protected override void OnClientSizeChanged(EventArgs e)
		{
			base.OnClientSizeChanged(e);
			ClickableNumericTextbox_Resize(this, e);
		}
		
		// Layout changes
		private void ClickableNumericTextbox_Layout(object sender, LayoutEventArgs e)
		{
			ClickableNumericTextbox_Resize(sender, e);
		}

		// Control resizes
		private void ClickableNumericTextbox_Resize(object sender, EventArgs e)
		{
			buttons.Height = textbox.Height + 4;
			textbox.Width = ClientRectangle.Width - buttons.Width - 2;
			buttons.Left = textbox.Width + 2;
			this.Height = buttons.Height;
		}

		#endregion
		
		#region ================== Control

		// Text in textbox changes
		private void textbox_TextChanged(object sender, EventArgs e)
		{
			int result = textbox.GetResult(buttons.Value);
			if((result >= buttons.Minimum) && (result <= buttons.Maximum)) buttons.Value = result;
		}

		// Textbox loses focus
		private void textbox_Leave(object sender, EventArgs e)
		{
			textbox.Text = buttons.Value.ToString();
		}

		// Buttons changed
		private void buttons_ValueChanged(object sender, EventArgs e)
		{
			textbox.Text = buttons.Value.ToString();
			if(ValueChanged != null) ValueChanged(this, e);
		}

		#endregion
	}
}
