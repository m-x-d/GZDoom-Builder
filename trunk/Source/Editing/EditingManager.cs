
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
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using System.Diagnostics;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Plugins;

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	public sealed class EditingManager
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		// All editing modes available
		private List<EditModeInfo> allmodes;
		
		// Editing modes selected through configuration
		private List<EditModeInfo> usedmodes;
		
		// Status
		private EditMode mode;
		private EditMode newmode;
		private Type prevmode;
		private Type prevstablemode;

		// Disposing
		private bool isdisposed = false;
		
		#endregion
		
		#region ================== Properties
		
		public EditMode Mode { get { return mode; } }
		public EditMode NewMode { get { return newmode; } }
		public Type PreviousMode { get { return prevmode; } }
		public Type PreviousStableMode { get { return prevstablemode; } }
		public bool IsDisposed { get { return isdisposed; } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		internal EditingManager()
		{
			// Initialize
			allmodes = new List<EditModeInfo>();
			usedmodes = new List<EditModeInfo>();
			
			// Make list of all editing modes we can find
			foreach(Plugin p in General.Plugins.Plugins)
			{
				// For all classes that inherit from EditMode
				Type[] editclasses = p.FindClasses(typeof(EditMode));
				foreach(Type t in editclasses)
				{
					// For all defined EditMode attributes
					EditModeAttribute[]  emattrs = (EditModeAttribute[])t.GetCustomAttributes(typeof(EditModeAttribute), false);
					foreach(EditModeAttribute a in emattrs)
					{
						// Make edit mode information
						allmodes.Add(new EditModeInfo(p, t, a));
					}
				}
			}
			
			// Sort the modes in order for buttons
			allmodes.Sort();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// Disposer
		internal void Dispose()
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
		
		#region ================== Methods
		
		// This returns specific editing mode info by name
		internal EditModeInfo GetEditModeInfo(string editmodename)
		{
			// Find the edit mode
			foreach(EditModeInfo emi in usedmodes)
			{
				// Mode matches class name?
				if(emi.ToString() == editmodename) return emi;
			}
			
			// No such mode found
			return null;
		}
		
		// This is called when the editing modes must update
		internal void UpdateCurrentEditModes()
		{
			// For now we use all the modes we can find
			usedmodes.Clear();
			usedmodes.AddRange(allmodes);
			
			// Remove editing mode buttons from interface
			General.MainWindow.RemoveEditModeButtons();
			
			// Go for all used edit modes to add buttons
			foreach(EditModeInfo emi in usedmodes)
			{
				if((emi.ButtonImage != null) && (emi.ButtonDesc != null))
					General.MainWindow.AddEditModeButton(emi);
			}
		}
		
		//
		// This changes the editing mode.
		// Order in which events occur for the old and new modes:
		// 
		// - Constructor of new mode is called
		// - Disengage of old mode is called
		// ----- Mode switches -----
		// - Engage of new mode is called
		// - Dispose of old mode is called
		//
		// Returns false when cancelled
		public bool ChangeMode(EditMode nextmode)
		{
			EditMode oldmode = mode;

			// Log info
			if(newmode != null)
				General.WriteLogLine("Switching edit mode to " + newmode.GetType().Name + "...");
			else
				General.WriteLogLine("Stopping edit mode...");

			// Remember previous mode
			newmode = nextmode;
			if(mode != null)
			{
				prevmode = mode.GetType();
				if(!mode.Attributes.Volatile) prevstablemode = prevmode;
			}
			else
			{
				prevmode = null;
				prevstablemode = null;
			}

			// Let the plugins know beforehand and check if not cancelled
			if(General.Plugins.ModeChanges(oldmode, newmode))
			{
				// Disenagage old mode
				if(oldmode != null) oldmode.OnDisengage();

				// Reset cursor
				General.Interface.SetCursor(Cursors.Default);

				// Apply new mode
				mode = newmode;

				// Engage new mode
				if(newmode != null) newmode.OnEngage();

				// Update the interface
				General.MainWindow.EditModeChanged();

				// Dispose old mode
				if(oldmode != null) oldmode.Dispose();

				// Done switching
				newmode = null;

				// Redraw the display
				General.MainWindow.RedrawDisplay();
				return true;
			}
			else
			{
				// Cancelled
				General.WriteLogLine("Edit mode change cancelled.");
				return false;
			}
		}
		
		// This changes mode by class name and optionally with arguments
		public void ChangeMode(string classname, params object[] args)
		{
			EditModeInfo emi = GetEditModeInfo(classname);
			if(emi != null) emi.SwitchToMode(args);
		}
		
		#endregion
		
		#region ================== Actions
		
		/// <summary>
		/// This cancels the current mode.
		/// </summary>
		[BeginAction("cancelmode")]
		public void CancelMode()
		{
			// Let the mode know
			mode.OnCancel();
		}
		
		/// <summary>
		/// This accepts the changes in the current mode.
		/// </summary>
		[BeginAction("acceptmode")]
		public void AcceptMode()
		{
			// Let the mode know
			mode.OnAccept();
		}
		
		#endregion
	}
}
