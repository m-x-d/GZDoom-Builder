
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
using System.Collections.Specialized;

namespace CodeImp.DoomBuilder
{
	internal static class General
	{
		#region ================== Constants

		// Files and Folders
		private const string SETTINGS_CONFIG_FILE = "Builder.cfg";
		private const string GAME_CONFIGS_DIR = "Configurations";

		#endregion

		#region ================== Variables

		// Files and Folders
		private static string apppath;
		private static string temppath;
		private static string configspath;
		
		// Main objects
		private static MainForm mainwindow;
		private static Configuration settings;

		// Configurations
		private static List<string> configfiles;
		private static List<string> confignames;
		
		#endregion

		#region ================== Properties

		public static string AppPath { get { return apppath; } }
		public static string TempPath { get { return temppath; } }
		public static string ConfigsPath { get { return configspath; } }
		public static MainForm MainWindow { get { return mainwindow; } }
		public static Configuration Settings { get { return settings; } }

		#endregion

		#region ================== Startup

		// Main program entry
		public static void Main(string[] args)
		{
			// Find application path
			string dirpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
			Uri localpath = new Uri(dirpath);
			apppath = Uri.UnescapeDataString(localpath.AbsolutePath);

			// Temporary directory
			temppath = Path.GetTempPath();

			// Configurations directory
			configspath = Path.Combine(apppath, GAME_CONFIGS_DIR);
			
			// Load configuration
			if(!File.Exists(Path.Combine(apppath, SETTINGS_CONFIG_FILE))) throw (new FileNotFoundException("Unable to find the program configuration \"" + SETTINGS_CONFIG_FILE + "\"."));
			settings = new Configuration(Path.Combine(apppath, SETTINGS_CONFIG_FILE), false);
			
			// Create main window
			mainwindow = new MainForm();
			
			// Show main window
			mainwindow.Show();
			mainwindow.Update();
			
			// Load game configurations
			LoadConfigurations();
			
			// Run application from the main window
			mainwindow.DisplayReady();
			Application.Run(mainwindow);
		}
		
		// This loads configurations
		private static void LoadConfigurations()
		{
			Configuration cfg;
			string[] filenames;
			string fn;
			
			// Display status
			mainwindow.DisplayStatus("Loading game configurations...");
			
			// Make arrays
			configfiles = new List<string>();
			confignames = new List<string>();

			// Go for all files in the configurations directory
			filenames = Directory.GetFiles(configspath, "*.cfg", SearchOption.TopDirectoryOnly);
			foreach(string filepath in filenames)
			{
				// Determine filename only
				fn = Path.GetFileName(filepath);
				
				try
				{
					// Try loading the configuration
					cfg = new Configuration(filepath, true);

					// Check for erors
					if(cfg.ErrorResult != 0)
					{
						// Error in configuration
						MessageBox.Show(mainwindow, "Unable to load the game configuration file \"" + fn + "\".\n" +
							"Error near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription,
							Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					else
					{
						// Add to lists
						configfiles.Add(fn);
						confignames.Add(cfg.ReadSetting("game", "<unnamed game>"));
					}
				}
				catch(Exception)
				{
					// Unable to load configuration
					MessageBox.Show(mainwindow, "Unable to load the game configuration file \"" + fn + "\".",
						Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
		
		#endregion
		
		#region ================== Terminate
		
		// This terminates the program
		public static void Terminate()
		{
			// Clean up
			mainwindow.Dispose();

			// Save settings configuration
			settings.SaveConfiguration(Path.Combine(apppath, SETTINGS_CONFIG_FILE));

			// Application ends here and now
			Application.Exit();
		}
		
		#endregion
	}
}
