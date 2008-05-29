
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
using System.Drawing.Drawing2D;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public class BlaBlaBla : ComboBox
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private EnumList enums;
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public BlaBlaBla()
		{
			this.ImeMode = ImeMode.Off;
		}

		#endregion

		#region ================== Methods

		// This sets the value
		public void SetValue(int v)
		{
			this.SelectedItem = null;
			this.Text = v.ToString();
		}
		
		// This fills the box with the given enum
		public void SetupEnums(EnumList enumslist)
		{
			// Keep old value
			int value = this.GetResult(0);

			// Find the enums list
			if(enumslist != null)
				this.DropDownStyle = ComboBoxStyle.DropDown;
			else
				this.DropDownStyle = ComboBoxStyle.Simple;
			
			// Fill list
			this.enums = enumslist;
			this.Items.Clear();
			if(enumslist != null) this.Items.AddRange(enumslist.ToArray());

			// Re-apply value
			this.Text = value.ToString();
			OnValidating(new CancelEventArgs());
		}
		
		// This returns the selected value
		public int GetResult(int original)
		{
			// Strip prefixes
			string str = this.Text.Trim().ToLowerInvariant();
			str = str.TrimStart('+', '-');
			int num = original;

			// Anything in the box?
			if(str.Length > 0)
			{
				// Enum selected?
				if(this.SelectedItem != null)
				{
					return (this.SelectedItem as EnumItem).GetIntValue();
				}
				else
				{
					// Prefixed with ++?
					if(this.Text.StartsWith("++"))
					{
						// Add number to original
						if(!int.TryParse(str, out num)) num = 0;
						return original + num;
					}
					// Prefixed with --?
					else if(this.Text.StartsWith("--"))
					{
						// Subtract number from original
						if(!int.TryParse(str, out num)) num = 0;
						return original - num;
					}
					else
					{
						// Return the new value
						if(int.TryParse(this.Text, out num)) return num; else return 0;
					}
				}
			}
			else
			{
				// Just return the original
				return original;
			}
		}
		
		// This finds the matching enum by number or title and selects it
		// Otherwise, if it is a number, keeps the number or sets it to 0
		protected override void OnValidating(CancelEventArgs e)
		{
			// Strip prefixes
			string str = this.Text.Trim().ToLowerInvariant();
			str = str.TrimStart('+', '-');
			int num = 0;

			// Prefixed?
			if(this.Text.Trim().StartsWith("++") || this.Text.Trim().StartsWith("--"))
			{
				// Try parsing to number
				if(!int.TryParse(str, NumberStyles.Integer, CultureInfo.CurrentCulture, out num))
				{
					// Invalid relative number
					this.SelectedItem = null;
					this.Text = "";
				}
			}
			else
			{
				// Try parsing to number
				if(int.TryParse(str, NumberStyles.Integer, CultureInfo.CurrentCulture, out num))
				{
					// Try selecting this enum
					EnumItem item = null;
					if(enums != null) item = enums.GetByEnumIndex(num.ToString());
					if(item != null)
					{
						// Select enum
						this.SelectedItem = item;
					}
				}
				// Enums available to check?
				else if(enums != null)
				{
					// Try finding the enum by comparing the left part of the string
					bool foundmatch = false;
					foreach(EnumItem item in enums)
					{
						// Enum matches?
						if(item.ToString().ToLowerInvariant().StartsWith(str))
						{
							// Select enum
							this.SelectedItem = item;
							foundmatch = true;
							break;
						}
					}

					// Not found anything?
					if(!foundmatch)
					{
						// Null the value
						this.SelectedItem = null;
						this.Text = "";
					}
				}
				else
				{
					// Invalid, just zero it
					this.SelectedItem = null;
					this.Text = "";
				}
			}
			
			// Validate base
			base.OnValidating(e);
		}
		
		#endregion
	}
}
