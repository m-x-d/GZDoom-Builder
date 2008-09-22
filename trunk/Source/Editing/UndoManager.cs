
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

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	public class UndoManager
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Undo and redo stacks
		private List<UndoSnapshot> undos;
		private List<UndoSnapshot> redos;

		// Grouping
		private UndoGroup lastgroup;
		private int lastgrouptag;
		
		// Unique tickets
		private int ticketid;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		internal UndoSnapshot NextUndo { get { if(undos.Count > 0) return undos[0]; else return null; } }
		internal UndoSnapshot NextRedo { get { if(redos.Count > 0) return redos[0]; else return null; } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal UndoManager()
		{
			// Initialize
			ticketid = 1;
			undos = new List<UndoSnapshot>(General.Settings.UndoLevels + 1);
			redos = new List<UndoSnapshot>(General.Settings.UndoLevels + 1);

			// Bind any methods
			General.Actions.BindMethods(this);

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Disposer
		internal void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Unbind any methods
				General.Actions.UnbindMethods(this);
				
				// Clean up
				ClearUndos();
				ClearRedos();
				General.WriteLogLine("All undo and redo levels cleared.");
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Private Methods

		// This clears the redos
		private void ClearRedos()
		{
			// Dispose all redos
			foreach(UndoSnapshot u in redos) u.map.Dispose();
			redos.Clear();
		}

		// This clears the undos
		private void ClearUndos()
		{
			// Dispose all undos
			foreach(UndoSnapshot u in undos) u.map.Dispose();
			undos.Clear();
		}

		// This checks and removes a level when the limit is reached
		private void LimitUndoRedoLevel(List<UndoSnapshot> list)
		{
			UndoSnapshot u;
			
			// Too many?
			if(list.Count > General.Settings.UndoLevels)
			{
				// Remove one and dispose map
				u = list[list.Count - 1];
				u.map.Dispose();
				list.RemoveAt(list.Count - 1);
			}
		}

		#endregion
		
		#region ================== Public Methods

		// This clears all redos
		public void ClearAllRedos()
		{
			ClearRedos();
			General.MainWindow.UpdateInterface();
		}
		
		// This makes an undo and returns the unique ticket id
		public int CreateUndo(string description, UndoGroup group, int grouptag)
		{
			UndoSnapshot u;
			
			// Not the same as previous group?
			if((group == UndoGroup.None) ||
			   (group != lastgroup) ||
			   (grouptag != lastgrouptag))
			{
				// Next ticket id
				if(++ticketid == int.MaxValue) ticketid = 1;

				General.WriteLogLine("Creating undo snapshot \"" + description + "\", Group " + group + ", Tag " + grouptag + ", Ticket ID " + ticketid + "...");
				
				// Make a snapshot
				u = new UndoSnapshot(description, General.Map.Map.Clone(), ticketid);

				// Put it on the stack
				undos.Insert(0, u);
				LimitUndoRedoLevel(undos);
				
				// Clear all redos
				ClearRedos();

				// Keep grouping info
				lastgroup = group;
				lastgrouptag = grouptag;

				// Update
				General.MainWindow.UpdateInterface();

				// Done
				return ticketid;
			}
			else
			{
				return -1;
			}
		}

		// This removes a previously made undo
		public void WithdrawUndo(int ticket)
		{
			// Anything to undo?
			if(undos.Count > 0)
			{
				// Check if the ticket id matches
				if(ticket == undos[0].ticketid)
				{
					General.WriteLogLine("Withdrawing undo snapshot \"" + undos[0].description + "\", Ticket ID " + ticket + "...");

					// Remove the last made undo
					undos.RemoveAt(0);
					
					// Update
					General.MainWindow.UpdateInterface();
				}
			}
		}

		// This performs an undo
		[BeginAction("undo")]
		public void PerformUndo()
		{
			UndoSnapshot u, r;
			Cursor oldcursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			
			// Call UndoBegin event
			if(General.Map.Mode.OnUndoBegin())
			{
				// Cancel volatile mode, if any
				// This returns false when mode was not volatile
				if(!General.CancelVolatileMode())
				{
					// Anything to undo?
					if(undos.Count > 0)
					{
						// Get undo snapshot
						u = undos[0];
						undos.RemoveAt(0);

						General.WriteLogLine("Performing undo \"" + u.description + "\", Ticket ID " + u.ticketid + "...");

						// Make a snapshot for redo
						r = new UndoSnapshot(u, General.Map.Map.Clone());

						// Put it on the stack
						redos.Insert(0, r);
						LimitUndoRedoLevel(redos);

						// Reset grouping
						lastgroup = UndoGroup.None;

						// Remove selection
						u.map.ClearAllMarks(false);
						u.map.ClearAllSelected();

						// Change map set
						General.Map.ChangeMapSet(u.map);

						// Update
						General.MainWindow.RedrawDisplay();
						General.MainWindow.UpdateInterface();
					}
				}
			}
			
			Cursor.Current = oldcursor;
		}
		
		// This performs a redo
		[BeginAction("redo")]
		internal void PerformRedo()
		{
			UndoSnapshot u, r;
			Cursor oldcursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			
			// Call RedoBegin event
			if(General.Map.Mode.OnRedoBegin())
			{
				// Cancel volatile mode, if any
				General.CancelVolatileMode();

				// Anything to redo?
				if(redos.Count > 0)
				{
					// Get redo snapshot
					r = redos[0];
					redos.RemoveAt(0);

					General.WriteLogLine("Performing redo \"" + r.description + "\", Ticket ID " + r.ticketid + "...");

					// Make a snapshot for undo
					u = new UndoSnapshot(r, General.Map.Map.Clone());

					// Put it on the stack
					undos.Insert(0, u);
					LimitUndoRedoLevel(undos);

					// Reset grouping
					lastgroup = UndoGroup.None;

					// Remove selection
					r.map.ClearAllMarks(false);
					r.map.ClearAllSelected();

					// Change map set
					General.Map.ChangeMapSet(r.map);

					// Update
					General.MainWindow.RedrawDisplay();
					General.MainWindow.UpdateInterface();
				}
			}
			
			Cursor.Current = oldcursor;
		}

		#endregion
	}
}
