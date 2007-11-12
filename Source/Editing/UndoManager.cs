
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

#endregion

namespace CodeImp.DoomBuilder.Editing
{
	public class UndoManager : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Undo and redo stacks
		private Stack<UndoSnapshot> undos;
		private Stack<UndoSnapshot> redos;

		// Grouping
		private UndoGroup lastgroup;
		private int lastgrouptag;
		
		// Unique tickets
		private int ticketid;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public UndoSnapshot NextUndo { get { if(undos.Count > 0) return undos.Peek(); else return null; } }
		public UndoSnapshot NextRedo { get { if(redos.Count > 0) return redos.Peek(); else return null; } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public UndoManager()
		{
			// Initialize
			ticketid = 1;
			undos = new Stack<UndoSnapshot>();
			redos = new Stack<UndoSnapshot>();

			// Bind any methods
			ActionAttribute.BindMethods(this);

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Unbind any methods
				ActionAttribute.UnbindMethods(this);
				
				// Clean up
				ClearUndos();
				ClearRedos();
				
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

		#endregion
		
		#region ================== Public Methods

		// This makes an undo and returns the unique ticket id
		public int CreateUndo(string description, UndoGroup group, int grouptag, bool allow3dchange)
		{
			UndoSnapshot u;
			
			// Not the same as previous group?
			if((group == UndoGroup.None) ||
			   (group != lastgroup) ||
			   (grouptag != lastgrouptag))
			{
				// Next ticket id
				if(++ticketid == int.MaxValue) ticketid = 1;

				// Make a snapshot
				u = new UndoSnapshot(description, allow3dchange, General.Map.Map.Clone(), ticketid);

				// Put it on the stack
				undos.Push(u);

				// Clear all redos
				redos.Clear();

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
				if(ticket == undos.Peek().ticketid)
				{
					// Remove the last made undo
					undos.Pop();
					
					// Update
					General.MainWindow.UpdateInterface();
				}
			}
		}

		// This performs an undo
		[Action("undo")]
		public void PerformUndo()
		{
			UndoSnapshot u, r;

			// Anything to undo?
			if(undos.Count > 0)
			{
				// Get undo snapshot
				u = undos.Pop();

				// Make a snapshot for redo
				r = new UndoSnapshot(u, General.Map.Map.Clone());

				// Put it on the stack
				redos.Push(r);

				// Change map set
				General.Map.ChangeMapSet(u.map);

				// Update
				General.MainWindow.RedrawDisplay();
				General.MainWindow.UpdateInterface();
			}
		}
		
		// This performs a redo
		[Action("redo")]
		public void PerformRedo()
		{
			UndoSnapshot u, r;

			// Anything to redo?
			if(redos.Count > 0)
			{
				// Get redo snapshot
				r = redos.Pop();

				// Make a snapshot for undo
				u = new UndoSnapshot(r, General.Map.Map.Clone());

				// Put it on the stack
				undos.Push(u);

				// Change map set
				General.Map.ChangeMapSet(r.map);

				// Update
				General.MainWindow.RedrawDisplay();
				General.MainWindow.UpdateInterface();
			}
		}

		#endregion
	}
}
