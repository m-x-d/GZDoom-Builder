
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
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Data;
using System.IO;
using System.Diagnostics;

#endregion

namespace CodeImp.DoomBuilder
{
	internal class NodebuilderInfo : IComparable<NodebuilderInfo>
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private string name;
		private string title;
		private ProcessStartInfo process;
		
		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public string Title { get { return title; } }
		public ProcessStartInfo Process { get { return process; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public NodebuilderInfo(string filename, string name, Configuration cfg)
		{
			General.WriteLogLine("Registered nodebuilder configuration '" + name + "' from '" + filename + "'");

			// Initialize
			this.name = name;
			this.title = cfg.ReadSetting(name + ".title", "<untitled configuration>");

			// Setup save map process
			process = new ProcessStartInfo();
			process.Arguments = cfg.ReadSetting(name + ".parameters", "");
			process.FileName = Path.Combine(General.CompilersPath, cfg.ReadSetting(name + ".compiler", ""));
			process.CreateNoWindow = false;
			process.ErrorDialog = false;
			process.UseShellExecute = true;
			process.WindowStyle = ProcessWindowStyle.Hidden;
			process.WorkingDirectory = General.TempPath;
		}

		
		#endregion

		#region ================== Methods

		// This compares it to other ConfigurationInfo objects
		public int CompareTo(NodebuilderInfo other)
		{
			// Compare
			return name.CompareTo(other.name);
		}

		// String representation
		public override string ToString()
		{
			return title;
		}
		
		#endregion
	}
}
