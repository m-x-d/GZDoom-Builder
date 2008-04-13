
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using CodeImp.DoomBuilder.Interface;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using System.Diagnostics;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	/// <summary>
	/// Provides basic user input interface functionality for a Doom Builder editing mode.
	/// </summary>
	public abstract class EditMode
	{
		#region ================== Constants

		public const int DRAG_START_MOVE_PIXELS = 5;
		public const MouseButtons EDIT_BUTTON = MouseButtons.Right;
		public const MouseButtons SELECT_BUTTON = MouseButtons.Left;

		#endregion

		#region ================== Variables
		
		// Disposing
		protected bool isdisposed = false;

		#endregion

		#region ================== Properties

		public bool IsDisposed { get { return isdisposed; } }

		// Unless overriden, this returns the name of this mode
		// for checking the appropriate button on the toolbar.
		public virtual string EditModeButtonName { get { return GetType().Name; } }

		#endregion

		#region ================== Constructor / Disposer

		/// <summary>
		/// Provides basic user input interface functionality for a Doom Builder editing mode.
		/// </summary>
		public EditMode()
		{
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Static Methods

		// This creates an instance of a specific mode
		public static EditMode Create(Type modetype, object[] args)
		{
			try
			{
				// Create new mode
				return (EditMode)General.ThisAssembly.CreateInstance(modetype.FullName, false,
					BindingFlags.Default, null, args, CultureInfo.CurrentCulture, new object[0]);
			}
			// Catch errors
			catch(TargetInvocationException e)
			{
				// Throw the actual exception
				Debug.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
				Debug.WriteLine(e.InnerException.Source + " throws " + e.InnerException.GetType().Name + ":");
				Debug.WriteLine(e.InnerException.Message);
				Debug.WriteLine(e.InnerException.StackTrace);
				throw e.InnerException;
			}
		}
		
		#endregion

		#region ================== Methods

		//
		// Order in which events occur for the old and new modes:
		// 
		// - Constructor of new mode is called
		// - Disengage of old mode is called
		// ----- Mode switches -----
		// - Engage of new mode is called
		// - Dispose of old mode is called
		//
		
		// Mode engages
		public virtual void Engage()
		{
			// Bind any methods
			ActionAttribute.BindMethods(this);
		}

		// Mode disengages
		public virtual void Disengage()
		{
			// Unbind any methods
			ActionAttribute.UnbindMethods(this);
		}

		// This forces the mode to cancel and return to the "parent" mode
		[Action("cancelmode", BaseAction = true)]
		public virtual void Cancel() { }
		
		// Optional interface methods
		public virtual void MouseClick(MouseEventArgs e) { }
		public virtual void MouseDoubleClick(MouseEventArgs e) { }
		public virtual void MouseDown(MouseEventArgs e) { }
		public virtual void MouseEnter(EventArgs e) { }
		public virtual void MouseLeave(EventArgs e) { }
		public virtual void MouseMove(MouseEventArgs e) { }
		public virtual void MouseUp(MouseEventArgs e) { }
		public virtual void KeyDown(KeyEventArgs e) { }
		public virtual void KeyUp(KeyEventArgs e) { }
		public virtual void MouseInput(Vector2D delta) { }

		/// <summary>
		/// NOTE: Do not call directly! Only for overriding. Call General.Interface.RedrawDisplay() instead.
		/// </summary>
		public virtual void RedrawDisplay() { }
		public virtual void RefreshDisplay() { }

		public virtual void Process() { }
		
		#endregion
	}
}
