
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
using System.Threading;
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
		
		// Background thread
		private volatile bool dobackgroundwork;
		private Thread backgroundthread;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public UndoSnapshot NextUndo { get { if(undos.Count > 0) return undos[0]; else return null; } }
		public UndoSnapshot NextRedo { get { if(redos.Count > 0) return redos[0]; else return null; } }
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

			// Start background thread
			backgroundthread = new Thread(new ThreadStart(BackgroundThread));
			backgroundthread.Name = "Snapshot Compressor";
			backgroundthread.Priority = ThreadPriority.Lowest;
			backgroundthread.IsBackground = true;
			backgroundthread.Start();
			
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

				// Stop the thread and wait for it to end
				backgroundthread.Interrupt();
				backgroundthread.Join();
				backgroundthread = null;
				
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
			lock(redos)
			{
				// Dispose all redos
				foreach(UndoSnapshot u in redos) u.Dispose();
				redos.Clear();
			}
		}

		// This clears the undos
		private void ClearUndos()
		{
			lock(undos)
			{
				// Dispose all undos
				foreach(UndoSnapshot u in undos) u.Dispose();
				undos.Clear();
			}
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
				u.Dispose();
				list.RemoveAt(list.Count - 1);
			}
		}

		// Background thread
		private void BackgroundThread()
		{
			while(true)
			{
				if(dobackgroundwork)
				{
					// First set dobackgroundwork to false before performing the work so
					// that it can be set to true again when another pass is needed
					dobackgroundwork = false;

					int undolevel = 0;
					UndoSnapshot snapshot;
					while(true)
					{
						// Get the next snapshot or leave
						lock(undos)
						{
							if(undolevel < undos.Count)
								snapshot = undos[undolevel];
							else
								break;
						}

						// Write to file or load from file, if needed
						if(snapshot.StoreOnDisk && !snapshot.IsOnDisk)
							snapshot.WriteToFile();
						else if(!snapshot.StoreOnDisk && snapshot.IsOnDisk)
							snapshot.RestoreFromFile();

						// Next
						undolevel++;
					}

					int redolevel = 0;
					while(true)
					{
						// Get the next snapshot or leave
						lock(redos)
						{
							if(redolevel < redos.Count)
								snapshot = redos[redolevel];
							else
								break;
						}

						// Write to file or load from file, if needed
						if(snapshot.StoreOnDisk && !snapshot.IsOnDisk)
							snapshot.WriteToFile();
						else if(!snapshot.StoreOnDisk && snapshot.IsOnDisk)
							snapshot.RestoreFromFile();

						// Next
						redolevel++;
					}
				}

				try { Thread.Sleep(30); }
				catch(ThreadInterruptedException) { break; }
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
		// Also automatically indicates that the map is changed
		public int CreateUndo(string description)
		{
			return CreateUndo(description, UndoGroup.None, 0);
		}
		
		// This makes an undo and returns the unique ticket id
		// Also automatically indicates that the map is changed
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
				u = new UndoSnapshot(description, General.Map.Map.Serialize(), ticketid);

				lock(undos)
				{
					// The current top of the stack can now be written to disk
					// because it is no longer the next immediate undo level
					if(undos.Count > 0) undos[0].StoreOnDisk = true;
					
					// Put it on the stack
					undos.Insert(0, u);
					LimitUndoRedoLevel(undos);
				}
				
				// Clear all redos
				ClearRedos();

				// Keep grouping info
				lastgroup = group;
				lastgrouptag = grouptag;
				
				// Map changes!
				General.Map.IsChanged = true;

				// Update
				dobackgroundwork = true;
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
				if(ticket == undos[0].TicketID)
				{
					General.WriteLogLine("Withdrawing undo snapshot \"" + undos[0].Description + "\", Ticket ID " + ticket + "...");

					lock(undos)
					{
						// Remove the last made undo
						undos[0].Dispose();
						undos.RemoveAt(0);
						
						// Make the current top of the stack load into memory
						// because it just became the next immediate undo level
						if(undos.Count > 0) undos[0].StoreOnDisk = false;
					}
					
					// Update
					dobackgroundwork = true;
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
			
			// Anything to undo?
			if(undos.Count > 0)
			{
				// Let the plugins know
				if(General.Plugins.OnUndoBegin())
				{
					// Call UndoBegin event
					if(General.Editing.Mode.OnUndoBegin())
					{
						// Cancel volatile mode, if any
						// This returns false when mode was not volatile
						if(!General.CancelVolatileMode())
						{
							lock(undos)
							{
								// Get undo snapshot
								u = undos[0];
								undos.RemoveAt(0);

								// Make the current top of the stack load into memory
								// because it just became the next immediate undo level
								if(undos.Count > 0) undos[0].StoreOnDisk = false;
							}

							General.WriteLogLine("Performing undo \"" + u.Description + "\", Ticket ID " + u.TicketID + "...");
							General.Interface.DisplayStatus(StatusType.Action, u.Description + " undone.");

							// Make a snapshot for redo
							r = new UndoSnapshot(u, General.Map.Map.Serialize());

							lock(redos)
							{
								// The current top of the stack can now be written to disk
								// because it is no longer the next immediate undo level
								if(redos.Count > 0) redos[0].StoreOnDisk = true;
								
								// Put it on the stack
								redos.Insert(0, r);
								LimitUndoRedoLevel(redos);
							}
							
							// Reset grouping
							lastgroup = UndoGroup.None;

							// Change map set
							MemoryStream data = u.GetMapData();
							General.Map.ChangeMapSet(new MapSet(data));
							data.Dispose();
							
							// Remove selection
							General.Map.Map.ClearAllMarks(false);
							General.Map.Map.ClearAllSelected();

							// Done
							General.Editing.Mode.OnUndoEnd();
							General.Plugins.OnUndoEnd();

							// Update
							dobackgroundwork = true;
							General.Map.Data.UpdateUsedTextures();
							General.MainWindow.RedrawDisplay();
							General.MainWindow.UpdateInterface();
						}
					}
				}
			}
			
			Cursor.Current = oldcursor;
		}
		
		// This performs a redo
		[BeginAction("redo")]
		public void PerformRedo()
		{
			UndoSnapshot u, r;
			Cursor oldcursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			
			// Anything to redo?
			if(redos.Count > 0)
			{
				// Let the plugins know
				if(General.Plugins.OnRedoBegin())
				{
					// Call RedoBegin event
					if(General.Editing.Mode.OnRedoBegin())
					{
						// Cancel volatile mode, if any
						General.CancelVolatileMode();

						lock(redos)
						{
							// Get redo snapshot
							r = redos[0];
							redos.RemoveAt(0);
							
							// Make the current top of the stack load into memory
							// because it just became the next immediate undo level
							if(redos.Count > 0) redos[0].StoreOnDisk = false;
						}

						General.WriteLogLine("Performing redo \"" + r.Description + "\", Ticket ID " + r.TicketID + "...");
						General.Interface.DisplayStatus(StatusType.Action, r.Description + " redone.");

						// Make a snapshot for undo
						u = new UndoSnapshot(r, General.Map.Map.Serialize());

						lock(undos)
						{
							// The current top of the stack can now be written to disk
							// because it is no longer the next immediate undo level
							if(undos.Count > 0) undos[0].StoreOnDisk = true;

							// Put it on the stack
							undos.Insert(0, u);
							LimitUndoRedoLevel(undos);
						}
						
						// Reset grouping
						lastgroup = UndoGroup.None;

						// Change map set
						MemoryStream data = r.GetMapData();
						General.Map.ChangeMapSet(new MapSet(data));
						data.Dispose();
						
						// Remove selection
						General.Map.Map.ClearAllMarks(false);
						General.Map.Map.ClearAllSelected();

						// Done
						General.Editing.Mode.OnRedoEnd();
						General.Plugins.OnRedoEnd();

						// Update
						dobackgroundwork = true;
						General.Map.Data.UpdateUsedTextures();
						General.MainWindow.RedrawDisplay();
						General.MainWindow.UpdateInterface();
					}
				}
			}
			
			Cursor.Current = oldcursor;
		}

		#endregion
	}
}
