
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
	public partial class ButtonsNumericTextbox : UserControl
	{
		#region ================== Events

		public event EventHandler TextChanged;

		#endregion

		#region ================== Variables
		
		private bool ignorebuttonchange = false;
		
		#endregion

		#region ================== Properties
		
		public bool AllowNegative { get { return textbox.AllowNegative; } set { textbox.AllowNegative = value; } }
		public bool AllowRelative { get { return textbox.AllowRelative; } set { textbox.AllowRelative = value; } }
		public string Text { get { return textbox.Text; } set { textbox.Text = value; } }
		
		#endregion
		
		#region ================== Constructor / Disposer

		// Constructor
		public ButtonsNumericTextbox()
		{
			InitializeComponent();
			buttons.Value = 0;
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
		
		// Text in textbox changes
		private void textbox_TextChanged(object sender, EventArgs e)
		{
			if(TextChanged != null) TextChanged(sender, e);
			buttons.Enabled = !textbox.CheckIsRelative();
		}
		
		// Buttons changed
		private void buttons_ValueChanged(object sender, EventArgs e)
		{
			if(!ignorebuttonchange)
			{
				ignorebuttonchange = true;
				if(!textbox.CheckIsRelative())
				{
					int newvalue = textbox.GetResult(0) - buttons.Value;
					textbox.Text = newvalue.ToString();
				}
				buttons.Value = 0;
				ignorebuttonchange = false;
			}
		}

		#endregion
		
		#region ================== Methods
		
		// This checks if the number is relative
		public bool CheckIsRelative()
		{
			return textbox.CheckIsRelative();
		}
		
		// This determines the result value
		public int GetResult(int original)
		{
			return textbox.GetResult(original);
		}
		
		// This determines the result value
		public float GetResultFloat(float original)
		{
			return textbox.GetResultFloat(original);
		}
		
		#endregion
	}
}
