
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
	public class CopyPasteManager
	{
		#region ================== Constants
		
		private const string CLIPBOARD_DATA_FORMAT = "UDMF_GEOMETRTY";

		#endregion
		
		#region ================== Variables
		
		// Disposing
		private bool isdisposed = false;
		
		#endregion
		
		#region ================== Properties
		
		public bool IsDisposed { get { return isdisposed; } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		internal CopyPasteManager()
		{
			// Initialize
			
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
				
				// Done
				isdisposed = true;
			}
		}
		
		#endregion
		
		#region ================== Private Methods
		
		// This performs the copy. Returns false when copy was cancelled.
		private bool DoCopySelection()
		{
			// Ask the editing mode to prepare selection for copying.
			// The edit mode should mark all vertices, lines and sectors
			// that need to be copied.
			if(General.Map.Mode.OnCopyBegin())
			{
				MapSet copyset = new MapSet();
				
				// TODO: Do the copy

				// Create temp wadfile
				string tempfile = General.MakeTempFilename(General.TempPath);
				General.WriteLogLine("Creating temporary file: " + tempfile);
				WAD tempwad = new WAD(tempfile);
				
				// Create writer interface
				//MapSetIO io = new PrefabMapSetIO(tempwad, General.Map);
				
				// Write to temporary file
				General.WriteLogLine("Writing map data structures to file...");
				int index = tempwad.FindLumpIndex(MapManager.TEMP_MAP_HEADER);
				if(index == -1) index = 0;
				//io.Write(copyset, MapManager.TEMP_MAP_HEADER, index);
				
				//Clipboard.SetData(CLIPBOARD_DATA_FORMAT, io.
				return true;
			}
			else
			{
				General.MessageBeep(MessageBeepType.Warning);
				return false;
			}
		}
		
		// This performs the paste. Returns false when paste was cancelled.
		private bool DoPasteSelection()
		{
			// Ask the editing mode to prepare selection for pasting.
			if(General.Map.Mode.OnPasteBegin())
			{
				// TODO: Do the paste

				return true;
			}
			else
			{
				General.MessageBeep(MessageBeepType.Warning);
				return false;
			}
		}
		
		#endregion
		
		#region ================== Public Methods
		
		// This copies the current selection
		[BeginAction("copyselection")]
		public void CopySelection()
		{
			DoCopySelection();
		}
		
		// This cuts the current selection
		[BeginAction("cutselection")]
		public void CutSelection()
		{
			// Copy selected geometry
			if(DoCopySelection())
			{
				// Get the delete action and check if it's bound
				Action deleteitem = General.Actions["builder_deleteitem"];
				if(deleteitem.BeginBound)
				{
					// Perform delete action
					deleteitem.Begin();
					deleteitem.End();
				}
				else
				{
					// Action not bound
					General.Interface.DisplayWarning("Cannot remove that in this mode.");
				}
			}
		}
		
		// This pastes what is on the clipboard and marks the new geometry
		[BeginAction("pasteselection")]
		public void PasteSelection()
		{
			DoPasteSelection();
		}
		
		#endregion
	}
}
