
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

namespace CodeImp.DoomBuilder
{
	internal static class General
	{
		#region ================== Constants

		// Files and Folders
		private const string SETTINGS_CONFIG_FILE = "Builder.cfg";

		#endregion

		#region ================== Variables

		// Files and Folders
		private static string apppath;
		private static string temppath;
		
		// Main objects
		private static MainForm mainwindow;
		private static Configuration settings;

		#endregion

		#region ================== Properties

		public static string AppPath { get { return apppath; } }
		public static string TempPath { get { return temppath; } }
		public static MainForm MainWindow { get { return mainwindow; } }
		public static Configuration Settings { get { return settings; } }

		#endregion

		#region ================== Methods

		// Main program entry
		public static void Main(string[] args)
		{
			// Find application path
			string dirpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
			Uri localpath = new Uri(dirpath);
			apppath = Uri.UnescapeDataString(localpath.AbsolutePath);

			// Temporary directory
			temppath = Path.GetTempPath();

			// Load configuration
			if(!File.Exists(Path.Combine(apppath, SETTINGS_CONFIG_FILE))) throw (new FileNotFoundException("Unable to find the program configuration \"" + SETTINGS_CONFIG_FILE + "\"."));
			settings = new Configuration(Path.Combine(apppath, SETTINGS_CONFIG_FILE), false);
			
			// Create main window
			mainwindow = new MainForm();

			// Show main window
			mainwindow.Show();
			
			// Run application from the main window
			Application.Run(mainwindow);
		}
		
		#endregion
	}
}
