
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
using CodeImp.DoomBuilder.Geometry;

#endregion

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
		private static Assembly thisasm;
		private static MainForm mainwindow;
		private static Configuration settings;
		private static MapManager map;
		
		// Configurations
		private static List<ConfigurationInfo> configs;
		
		#endregion

		#region ================== Properties

		public static Assembly ThisAssembly { get { return thisasm; } }
		public static string AppPath { get { return apppath; } }
		public static string TempPath { get { return temppath; } }
		public static string ConfigsPath { get { return configspath; } }
		public static MainForm MainWindow { get { return mainwindow; } }
		public static Configuration Settings { get { return settings; } }
		public static List<ConfigurationInfo> Configs { get { return configs; } }
		public static MapManager Map { get { return map; } }
		
		#endregion

		#region ================== Configurations

		// This loads and returns a game configuration
		public static Configuration LoadGameConfiguration(string filename)
		{
			Configuration cfg;
			
			// Make the full filepathname
			string filepathname = Path.Combine(configspath, filename);
			
			// Load configuration
			try
			{
				// Try loading the configuration
				cfg = new Configuration(filepathname, true);

				// Check for erors
				if(cfg.ErrorResult != 0)
				{
					// Error in configuration
					MessageBox.Show(mainwindow, "Unable to load the game configuration file \"" + filename + "\".\n" +
						"Error near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription,
						Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return null;
				}
				// Check if this is a Doom Builder 1 config
				else if(cfg.ReadSetting("type", "") != "Doom Builder 2 Game Configuration")
				{
					// Old configuration
					MessageBox.Show(mainwindow, "Unable to load the game configuration file \"" + filename + "\".\n" +
						"This configuration is not a Doom Builder 2 game configuration.",
						Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return null;
				}
				else
				{
					// Return config
					return cfg;
				}
			}
			catch(Exception)
			{
				// Unable to load configuration
				MessageBox.Show(mainwindow, "Unable to load the game configuration file \"" + filename + "\".",
					Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}
		}

		// This finds all game configurations
		private static void FindGameConfigurations()
		{
			Configuration cfg;
			string[] filenames;
			string name, fullfilename;
			
			// Display status
			mainwindow.DisplayStatus("Loading game configurations...");

			// Make array
			configs = new List<ConfigurationInfo>();
			
			// Go for all files in the configurations directory
			filenames = Directory.GetFiles(configspath, "*.cfg", SearchOption.TopDirectoryOnly);
			foreach(string filepath in filenames)
			{
				// Check if it can be loaded
				cfg = LoadGameConfiguration(Path.GetFileName(filepath));
				if(cfg != null)
				{
					// Get name and filename
					name = cfg.ReadSetting("game", "<unnamed game>");
					fullfilename = Path.GetFileName(filepath);
					
					// Add to lists
					configs.Add(new ConfigurationInfo(name, fullfilename));
				}
			}

			// Sort the configurations list
			configs.Sort();
		}
		
		#endregion

		#region ================== Startup

		// Main program entry
		[STAThread]
		public static void Main(string[] args)
		{
			Uri localpath;
			
			// Get a reference to this assembly
			thisasm = Assembly.GetExecutingAssembly();
			
			// Find application path
			localpath = new Uri(Path.GetDirectoryName(thisasm.GetName().CodeBase));
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
			mainwindow.UpdateMenus();
			
			// Show main window
			mainwindow.Show();
			mainwindow.Update();
			
			// Load game configurations
			FindGameConfigurations();
			
			// Run application from the main window
			mainwindow.DisplayReady();
			Application.Run(mainwindow);
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
		
		#region ================== Management

		// This creates a new map
		public static bool NewMap()
		{
			MapOptions newoptions;
			MapOptionsForm optionswindow;

			// Empty options
			newoptions = new MapOptions();

			// Open map options dialog
			optionswindow = new MapOptionsForm(newoptions);
			if(optionswindow.ShowDialog(mainwindow) == DialogResult.OK)
			{
				// Display status
				mainwindow.DisplayStatus("Creating new map...");
				
				// Clear the display
				mainwindow.ClearDisplay();
				
				// Trash the current map, if any
				if(map != null) map.Dispose();

				// Create map manager with given options
				map = new MapManager();
				if(!map.InitializeNewMap(newoptions))
				{
					// Unable to create map manager
					map.Dispose();
					map = null;

					// Show splash logo on display
					mainwindow.ShowSplashDisplay();

					// Failed
					mainwindow.UpdateMenus();
					mainwindow.DisplayReady();
					return false;
				}

				// Done
				mainwindow.UpdateMenus();
				mainwindow.DisplayReady();
				return true;
			}
			
			// Cancelled
			return false;
		}

		// This closes the current map
		public static bool CloseMap()
		{
			// Display status
			mainwindow.DisplayStatus("Closing map...");

			// Trash the current map
			if(map != null) map.Dispose();
			map = null;

			// Show splash logo on display
			mainwindow.ShowSplashDisplay();
			
			// Done
			mainwindow.UpdateMenus();
			mainwindow.DisplayReady();
			return true;
		}

		// This loads a map from file
		public static bool OpenMap()
		{
			OpenFileDialog openfile;
			OpenMapOptionsForm openmapwindow;
			
			// Open map file dialog
			openfile = new OpenFileDialog();
			openfile.Filter = "Doom WAD Files (*.wad)|*.wad";
			openfile.Title = "Open Map";
			if(openfile.ShowDialog(mainwindow) == DialogResult.OK)
			{
				// Update main window
				mainwindow.Update();
				
				// Open map options dialog
				openmapwindow = new OpenMapOptionsForm(openfile.FileName);
				if(openmapwindow.ShowDialog(mainwindow) == DialogResult.OK)
				{
					// Display status
					mainwindow.DisplayStatus("Opening map file...");

					// Clear the display
					mainwindow.ClearDisplay();

					// Trash the current map, if any
					if(map != null) map.Dispose();

					// Create map manager with given options
					map = new MapManager();
					if(!map.InitializeOpenMap(openfile.FileName, openmapwindow.Options))
					{
						// Unable to create map manager
						map.Dispose();
						map = null;

						// Show splash logo on display
						mainwindow.ShowSplashDisplay();

						// Failed
						mainwindow.UpdateMenus();
						mainwindow.DisplayReady();
						return false;
					}

					// Done
					mainwindow.UpdateMenus();
					mainwindow.DisplayReady();
					return true;
				}
			}

			// Cancelled
			return false;
		}
		
		// This asks to save the map if needed
		// Returns false when action was cancelled
		public static bool AskSaveMap()
		{
			DialogResult result;
			
			// Map open and not saved?
			if((map != null) && map.IsChanged)
			{
				// Ask to save changes
				result = MessageBox.Show(mainwindow, "Do you want to save changes to " + map.FileTitle + " (" + map.Options.CurrentName + ")?", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if(result == DialogResult.Yes)
				{
					// TODO: Save map

				}
				else if(result == DialogResult.Cancel)
				{
					// Abort
					return false;
				}
			}
			
			// Continue
			return true;
		}
		
		#endregion
	}
}
