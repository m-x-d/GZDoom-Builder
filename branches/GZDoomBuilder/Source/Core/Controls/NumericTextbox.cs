
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
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public class NumericTextbox : AutoSelectTextbox
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private bool allownegative;		// Allow negative numbers
		private bool allowrelative;		// Allow ++, --, * and / prefix for relative changes
		private bool allowdecimal;		// Allow decimal (float) numbers
		private bool controlpressed;
		private bool shiftpressed; //mxd
		private readonly ToolTip tooltip; //mxd
		
		#endregion

		#region ================== Properties

		public bool AllowNegative { get { return allownegative; } set { allownegative = value; } }
		public bool AllowRelative { get { return allowrelative; } set { allowrelative = value; UpdateTextboxStyle(); } }
		public bool AllowDecimal  { get { return allowdecimal; } set { allowdecimal = value; } }

		public bool ControlPressed { get { return controlpressed; } internal set { controlpressed = value; } } //mxd
		public bool ShiftPressed { get { return shiftpressed; } internal set { shiftpressed = value; } } //mxd

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public NumericTextbox()
		{
			this.ImeMode = ImeMode.Off;

			//mxd. Setup tooltip
			this.tooltip = new ToolTip { AutomaticDelay = 100, AutoPopDelay = 4000, InitialDelay = 100, ReshowDelay = 100 };
		}

		#endregion

		#region ================== Methods

		// Key pressed
		protected override void OnKeyDown(KeyEventArgs e)
		{
			controlpressed = e.Control;
			shiftpressed = e.Shift; //mxd
			base.OnKeyDown(e);
		}

		// Key released
		protected override void OnKeyUp(KeyEventArgs e)
		{
			controlpressed = e.Control;
			shiftpressed = e.Shift; //mxd
			base.OnKeyUp(e);
		}
		
		// When a key is pressed
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			string allowedchars = "0123456789\b";
			
			// Determine allowed chars
			if(allownegative) allowedchars += CultureInfo.CurrentUICulture.NumberFormat.NegativeSign;
			if(allowrelative) allowedchars += "+-*/"; //mxd
			if(controlpressed) allowedchars += "\u0018\u0003\u0016";
			if(allowdecimal || this.Text.StartsWith("*") || this.Text.StartsWith("/")) //mxd
				allowedchars += CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
			
			// Check if key is not allowed
			if(allowedchars.IndexOf(e.KeyChar) == -1)
			{
				// Cancel this
				e.Handled = true;
			}
			else
			{
				//mxd. Check if * or / is pressed
				if(e.KeyChar == '*' || e.KeyChar == '/') 
				{
					if (this.SelectionStart - 1 > -1) e.Handled = true; //only valid when at the start of the text
				}
				// Check if + or - is pressed
				else if((e.KeyChar == '+') || (e.KeyChar == '-'))
				{
					string nonselectedtext;
					
					// Determine non-selected text
					if(this.SelectionLength > 0)
					{
						nonselectedtext = this.Text.Substring(0, this.SelectionStart) +
							this.Text.Substring(this.SelectionStart + this.SelectionLength);
					}
					else if(this.SelectionLength < 0)
					{
						nonselectedtext = this.Text.Substring(0, this.SelectionStart + this.SelectionLength) +
							this.Text.Substring(this.SelectionStart);
					}
					else
					{
						nonselectedtext = this.Text;
					}
					
					// Not at the start?
					int selectionpos = this.SelectionStart - 1;
					if(this.SelectionLength < 0) selectionpos = (this.SelectionStart + this.SelectionLength) - 1;
					if(selectionpos > -1)
					{
						// Find any other characters before the insert position
						string textpart = this.Text.Substring(0, selectionpos + 1);
						textpart = textpart.Replace("+", "");
						textpart = textpart.Replace("-", "");
						if(textpart.Length > 0)
						{
							// Cancel this
							e.Handled = true;
						}
					}

					// Determine other prefix
					char otherprefix = (e.KeyChar == '+' ? '-' : '+');
					
					// Limit the number of + and - allowed
					int numprefixes = nonselectedtext.Split(e.KeyChar, otherprefix).Length;
					if(numprefixes > 2)
					{
						// Can't have more than 2 prefixes
						e.Handled = true;
					}
					else if(numprefixes > 1)
					{
						// Must have 2 the same prefixes
						if(this.Text.IndexOf(e.KeyChar) == -1) e.Handled = true;

						// Double prefix must be allowed
						if(!allowrelative) e.Handled = true;
					}
				}
			}
			
			// Call base
			base.OnKeyPress(e);
		}

		// Validate contents
		protected override void OnValidating(CancelEventArgs e)
		{
			//mxd. We may want "++" and "--" on their own...
			if(allowrelative && (this.Text == "++" || this.Text == "--"))
			{
				// Call base and bail out
				base.OnValidating(e);
				return;
			}
			
			// Strip prefixes
			string textpart = this.Text.Replace("+", "").Replace("*", "").Replace("/", ""); //mxd
			if(!allownegative) textpart = textpart.Replace("-", "");
			
			// No numbers left?
			if(textpart.Length == 0)
			{
				// Make the textbox empty
				this.Text = "";
			} 
			else if(allowdecimal) //mxd
			{ 
				float value;
				if(float.TryParse(textpart, NumberStyles.Float, CultureInfo.CurrentCulture, out value)) 
				{
					if(value == Math.Round(value))
						this.Text = this.Text.Replace(textpart, value.ToString());
				}
			}
			
			// Call base
			base.OnValidating(e);
		}
		
		// This checks if the number is relative
		public bool CheckIsRelative()
		{
			// Prefixed with ++, --, * or /?
			return ( (this.Text.Length > 2 && (this.Text.StartsWith("++") || this.Text.StartsWith("--"))) || 
				(this.Text.Length > 1 && (this.Text.StartsWith("*") || this.Text.StartsWith("/"))) ); //mxd
		}
		
		// This determines the result value
		public int GetResult(int original)
		{
			string textpart = this.Text;
			
			// Strip prefixes
			textpart = textpart.Replace("+", "").Replace("-", "").Replace("*", "").Replace("/", ""); //mxd
			
			// Any numbers left?
			if(textpart.Length > 0)
			{
				int result;
				
				// Prefixed with ++?
				if(this.Text.StartsWith("++"))
				{
					// Add number to original
					int.TryParse(textpart, out result);
					return original + result;
				}
				// Prefixed with --?
				if(this.Text.StartsWith("--"))
				{
					// Subtract number from original
					int.TryParse(textpart, out result);
					int newvalue = original - result;
					if(!allownegative && (newvalue < 0)) newvalue = 0;
					return newvalue;
				}
				//mxd. Prefixed with *?
				if(this.Text.StartsWith("*")) 
				{
					// Multiply original by number
					float resultf;
					float.TryParse(textpart, NumberStyles.Float, CultureInfo.CurrentCulture, out resultf);
					int newvalue = (int)Math.Round(original * resultf);
					if(!allownegative && (newvalue < 0)) newvalue = 0;
					return newvalue;
				}
				//mxd. Prefixed with /?
				if(this.Text.StartsWith("/")) 
				{
					// Divide original by number
					float resultf;
					float.TryParse(textpart, NumberStyles.Float, CultureInfo.CurrentCulture, out resultf);
					if (resultf == 0) return original;
					int newvalue = (int)Math.Round(original / resultf);
					if(!allownegative && (newvalue < 0)) newvalue = 0;
					return newvalue;
				}

				//mxd. Return the new value
				if(!int.TryParse(this.Text, out result)) return original;
				if(!allownegative && (result < 0)) return 0;
				return result;
			}

			// Nothing given, keep original value
			return original;
		}

		// This determines the result value
		public float GetResultFloat(float original)
		{
			// Strip prefixes
			string textpart = this.Text.Replace("+", "").Replace("-", "").Replace("*", "").Replace("/", ""); //mxd;

			// Any numbers left?
			if(textpart.Length > 0)
			{
				float result;
				
				// Prefixed with ++?
				if(this.Text.StartsWith("++"))
				{
					// Add number to original
					if(!float.TryParse(textpart, NumberStyles.Float, CultureInfo.CurrentCulture, out result)) result = 0;
					return original + result;
				}
				// Prefixed with --?
				if(this.Text.StartsWith("--"))
				{
					// Subtract number from original
					if(!float.TryParse(textpart, NumberStyles.Float, CultureInfo.CurrentCulture, out result)) result = 0;
					float newvalue = original - result;
					if(!allownegative && (newvalue < 0)) newvalue = 0;
					return newvalue;
				}
				//mxd. Prefixed with *?
				if(this.Text.StartsWith("*")) 
				{
					// Multiply original by number
					if(!float.TryParse(textpart, NumberStyles.Float, CultureInfo.CurrentCulture, out result)) result = 0;
					float newvalue = original * result;
					if(!allownegative && (newvalue < 0)) newvalue = 0;
					return newvalue;
				}
				//mxd. Prefixed with /?
				if(this.Text.StartsWith("/")) 
				{
					// Divide original by number
					if(!float.TryParse(textpart, NumberStyles.Float, CultureInfo.CurrentCulture, out result)) return original;
					float newvalue = (float)Math.Round(original / result, 3);
					if(!allownegative && (newvalue < 0)) newvalue = 0;
					return newvalue;
				}

				//mxd. Return the new value
				if(!float.TryParse(this.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out result)) return original;
				if(!allownegative && (result < 0)) return 0;
				return result;
			}

			// Nothing given, keep original value
			return original;
		}

		//mxd
		public void UpdateTextboxStyle()
		{
			UpdateTextboxStyle(string.Empty);
		}

		//mxd
		public void UpdateTextboxStyle(string tip) 
		{
			this.ForeColor = (allowrelative ? SystemColors.HotTrack : SystemColors.WindowText);
			if (allowrelative)
			{
				tooltip.SetToolTip(this, "Use ++ or -- prefixes to change\r\nexisting values by given value.\r\nUse * or / prefixes to multiply\r\nor divide existing values by given value." + Environment.NewLine + tip);
			}
			else if(!string.IsNullOrEmpty(tip))
			{
				tooltip.SetToolTip(this, tip);
			}
			else
			{
				tooltip.RemoveAll();
			}
		}
		
		#endregion
	}
}
