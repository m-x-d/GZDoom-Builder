
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
using System.Drawing;
using System.ComponentModel;
using CodeImp.DoomBuilder.Map;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Interface
{
	public interface IMainForm : IWin32Window
	{
		// Properties
		bool AltState { get; }
		bool CtrlState { get; }
		bool ShiftState { get; }
		bool MouseInDisplay { get; }
		bool AutoMerge { get; }
		bool SnapToGrid { get; }
		bool MouseExclusive { get; }

		// Methods
		void DisplayReady();
		void DisplayStatus(string status);
		void RedrawDisplay();
		void ShowEditLinedefs(ICollection<Linedef> lines);
		void ShowEditSectors(ICollection<Sector> sectors);
		void ShowLinedefInfo(Linedef l);
		void ShowSectorInfo(Sector s);
		void ShowThingInfo(Thing t);
		void ShowVertexInfo(Vertex v);
		void HideInfo();
		void UpdateCoordinates(Vector2D coords);
		bool Focus();
		void SetProcessorState(bool on);
		void StartExclusiveMouseInput();
		void StopExclusiveMouseInput();
		void BreakExclusiveMouseInput();
		void ResumeExclusiveMouseInput();
		bool CheckActionActive(Assembly assembly, string actionname);
		
		/// <summary>
		/// This adds a menu to the Doom Builder menu strip.
		/// <para>
		/// NOTE: When the Tag property of menu items is set with a string, this changes the
		/// tag to a fully qualified action name by prefixing it with the assembly name.
		/// </para>
		/// </summary>
		/// <param name="menu">The menu to add to Doom Builder.</param>
		void AddMenu(ToolStripMenuItem menu);
		
		/// <summary>
		/// This removes a menu from the Doom Builder menu strip.
		/// </summary>
		/// <param name="menu">The menu to remove.</param>
		void RemoveMenu(ToolStripMenuItem menu);
		
		/// <summary>
		/// This method invokes the action specified on the Tag property of the given menu item.
		/// </summary>
		/// <param name="sender">Menu item with Tag property set to the name of an action
		/// that you want to invoke.</param>
		/// <param name="e">Unused.</param>
		void InvokeTaggedAction(object sender, EventArgs e);
	}
}
