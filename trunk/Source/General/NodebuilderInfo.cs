
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
using CodeImp.DoomBuilder.Images;
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
		private string filename;
		private ProcessStartInfo savemapprocess;
		private ProcessStartInfo testmapprocess;
		private ProcessStartInfo mode3dbuildprocess;
		
		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public string Filename { get { return filename; } }
		public ProcessStartInfo SaveMapProcess { get { return savemapprocess; } }
		public ProcessStartInfo TestMapProcess { get { return testmapprocess; } }
		public ProcessStartInfo Mode3DBuildProcess { get { return mode3dbuildprocess; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public NodebuilderInfo(Configuration cfg, string filename)
		{
			// Initialize
			this.filename = Path.GetFileName(filename);
			this.name = cfg.ReadSetting("title", "");
			
			// Setup save map process
			SetupProcess(out savemapprocess,
				cfg.ReadSetting("savemap.compiler", ""),
				cfg.ReadSetting("savemap.parameters", ""));

			// Setup test map process
			SetupProcess(out testmapprocess,
				cfg.ReadSetting("testmap.compiler", ""),
				cfg.ReadSetting("testmap.parameters", ""));

			// Setup 3d build process
			SetupProcess(out mode3dbuildprocess,
				cfg.ReadSetting("mode3dbuild.compiler", ""),
				cfg.ReadSetting("mode3dbuild.parameters", ""));
		}

		// This sets up a process
		private void SetupProcess(out ProcessStartInfo processinfo, string compiler, string parameters)
		{
			processinfo = new ProcessStartInfo();
			processinfo.Arguments = parameters;
			processinfo.FileName = Path.Combine(General.CompilersPath, compiler);
			processinfo.CreateNoWindow = false;
			processinfo.ErrorDialog = false;
			processinfo.UseShellExecute = true;
			processinfo.WindowStyle = ProcessWindowStyle.Hidden;
			processinfo.WorkingDirectory = General.TempPath;
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
			return name;
		}
		
		#endregion
	}
}
