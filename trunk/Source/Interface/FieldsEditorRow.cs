
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
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Rendering;
using SlimDX.Direct3D9;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	internal class FieldsEditorRow : DataGridViewRow
	{
		#region ================== Constants

		#endregion

		#region ================== Variables
		
		// This is true when for a fixed field as defined in the game configuration
		// This means that the field cannot be deleted (delete will result in a reset)
		// and cannot change type.
		private bool isfixed;

		// Field information (only for fixed fields)
		private UniversalFieldInfo fieldinfo;

		// This is true when the field is defined. Cannot be false when this field
		// is not fixed, because non-fixed fields are deleted from the list when undefined.
		private bool isdefined;
		
		// Type
		private UniversalFieldType fieldtype;
		
		#endregion

		#region ================== Properties

		public bool IsFixed { get { return isfixed; } }
		public bool IsDefined { get { return isdefined; } }
		public UniversalFieldType Type { get { return fieldtype; } }
		public UniversalFieldInfo Info { get { return fieldinfo; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor for a fixed, undefined field
		public FieldsEditorRow(DataGridView view, UniversalFieldInfo fixedfield)
		{
			// Undefined
			this.DefaultCellStyle.ForeColor = SystemColors.GrayText;
			isdefined = false;
			
			// Fixed
			this.fieldinfo = fixedfield;
			isfixed = true;
			
			// Type
			this.fieldtype = fixedfield.Type;
			
			// Make all cells
			base.CreateCells(view);
			
			// Setup property cell
			this.Cells[0].Value = fixedfield.Name;
			this.Cells[0].ReadOnly = true;

			// Setup type cell
			this.Cells[1].Value = fixedfield.Type.ToString();
			this.Cells[1].ReadOnly = true;

			// Setup value cell
			this.Cells[2].Value = fixedfield.DefaultStr;
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Constructor for a non-fixed, defined field
		public FieldsEditorRow(DataGridView view, string name, UniversalFieldType type, object value)
		{
			// Defined
			this.DefaultCellStyle.ForeColor = SystemColors.WindowText;
			isdefined = true;

			// Non-fixed
			isfixed = false;

			// Type
			this.fieldtype = type;

			// Make all cells
			base.CreateCells(view);
			
			// Setup property cell
			this.Cells[0].Value = name;
			this.Cells[0].ReadOnly = true;

			// Setup type cell
			this.Cells[1].Value = type.ToString();
			this.Cells[1].ReadOnly = false;

			// Setup value cell
			this.Cells[2].Value = value;

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods
		
		// This is called when a cell is edited
		public void CellChanged()
		{
			// Update type from cell
			try { fieldtype = (UniversalFieldType)Enum.Parse(typeof(UniversalFieldType), this.Cells[1].Value.ToString(), true); }
			catch(Exception e) { this.Cells[1].Value = fieldtype.ToString(); }
		}
		
		// This undefines the field
		// ONLY VALID FOR FIXED FIELDS
		// You should just delete non-fixed fields
		public void Undefine()
		{
			// Must be fixed!
			if(!isfixed) throw new InvalidOperationException();
			
			// Now undefined
			this.Cells[2].Value = fieldinfo.DefaultStr;
			this.DefaultCellStyle.ForeColor = SystemColors.GrayText;
			isdefined = false;
		}

		// This defines the field
		public void Define(object value)
		{
			// Now defined
			this.Cells[2].Value = value;
			this.DefaultCellStyle.ForeColor = SystemColors.WindowText;
			isdefined = true;
		}
		
		#endregion
	}
}
