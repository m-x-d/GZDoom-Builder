
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
using System.Runtime.InteropServices;
using CodeImp.DoomBuilder.Controls;
using System.Diagnostics;

#endregion

namespace CodeImp.DoomBuilder
{
	internal static class General
	{
		#region ================== API Declarations

		[DllImport("user32.dll")]
		public static extern int LockWindowUpdate(IntPtr hwnd);

		[DllImport("kernel32.dll", EntryPoint="RtlZeroMemory", SetLastError=false)]
		public static extern void ZeroMemory(IntPtr dest, int size);

		[DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
		internal static extern unsafe void CopyMemory(void* dst, void* src, UIntPtr length);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static unsafe extern void* VirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static unsafe extern bool VirtualFree(void* lpAddress, UIntPtr dwSize, uint dwFreeType);

		#endregion

		#region ================== Constants

		// Memory APIs
		public const uint MEM_COMMIT = 0x1000;
		public const uint MEM_RESERVE = 0x2000;
		public const uint MEM_DECOMMIT = 0x4000;
		public const uint MEM_RELEASE = 0x8000;
		public const uint MEM_RESET = 0x80000;
		public const uint MEM_TOP_DOWN = 0x100000;
		public const uint MEM_PHYSICAL = 0x400000;
		public const uint PAGE_NOACCESS = 0x01;
		public const uint PAGE_READONLY = 0x02;
		public const uint PAGE_READWRITE = 0x04;
		public const uint PAGE_WRITECOPY = 0x08;
		public const uint PAGE_EXECUTE = 0x10;
		public const uint PAGE_EXECUTE_READ = 0x20;
		public const uint PAGE_EXECUTE_READWRITE = 0x40;
		public const uint PAGE_EXECUTE_WRITECOPY = 0x80;
		public const uint PAGE_GUARD = 0x100;
		public const uint PAGE_NOCACHE = 0x200;
		public const uint PAGE_WRITECOMBINE = 0x400;
		
		// Files and Folders
		private const string SETTINGS_CONFIG_FILE = "Builder.cfg";
		private const string LOG_FILE = "Builder.log";
		private const string GAME_CONFIGS_DIR = "Configurations";
		private const string COMPILERS_DIR = "Compilers";

		#endregion

		#region ================== Variables

		// Files and Folders
		private static string apppath;
		private static string logfile;
		private static string temppath;
		private static string configspath;
		private static string compilerspath;
		
		// Main objects
		private static Assembly thisasm;
		private static MainForm mainwindow;
		private static Configuration settings;
		private static MapManager map;
		private static ActionManager actions;
		
		// Configurations
		private static List<ConfigurationInfo> configs;
		private static List<NodebuilderInfo> nodebuilders;
		
		#endregion

		#region ================== Properties

		public static Assembly ThisAssembly { get { return thisasm; } }
		public static string AppPath { get { return apppath; } }
		public static string TempPath { get { return temppath; } }
		public static string ConfigsPath { get { return configspath; } }
		public static string CompilersPath { get { return compilerspath; } }
		public static MainForm MainWindow { get { return mainwindow; } }
		public static Configuration Settings { get { return settings; } }
		public static List<ConfigurationInfo> Configs { get { return configs; } }
		public static List<NodebuilderInfo> Nodebuilders { get { return nodebuilders; } }
		public static MapManager Map { get { return map; } }
		public static ActionManager Actions { get { return actions; } }
		
		#endregion

		#region ================== Configurations

		// This returns the game configuration info by filename
		public static ConfigurationInfo GetConfigurationInfo(string filename)
		{
			// Go for all config infos
			foreach(ConfigurationInfo ci in configs)
			{
				// Check if filename matches
				if(string.Compare(Path.GetFileNameWithoutExtension(ci.Filename),
								  Path.GetFileNameWithoutExtension(filename), true) == 0)
				{
					// Return this info
					return ci;
				}
			}

			// None found
			return null;
		}

		// This loads and returns a game configuration
		public static Configuration LoadGameConfiguration(string filename)
		{
			Configuration cfg;
			string message;
			
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
					message = "Unable to load the game configuration file \"" + filename + "\".\n" +
							  "Error near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription;
					
					General.WriteLogLine(message);
					MessageBox.Show(mainwindow, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return null;
				}
				// Check if this is a Doom Builder 2 config
				else if(cfg.ReadSetting("type", "") != "Doom Builder 2 Game Configuration")
				{
					// Old configuration
					message = "Unable to load the game configuration file \"" + filename + "\".\n" +
							  "This configuration is not a Doom Builder 2 game configuration.";
					
					General.WriteLogLine(message);
					MessageBox.Show(mainwindow, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				message = "Unable to load the game configuration file \"" + filename + "\".";

				General.WriteLogLine(message);
				MessageBox.Show(mainwindow, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

			// Go for all cfg files in the configurations directory
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
		
		// This finds all nodebuilder configurations
		private static void FindNodebuilderConfigurations()
		{
			Configuration cfg;
			string[] filenames;
			string message;

			// Display status
			mainwindow.DisplayStatus("Loading nodebuilder configurations...");

			// Make array
			nodebuilders = new List<NodebuilderInfo>();

			// Go for all cfg files in the compilers directory
			filenames = Directory.GetFiles(compilerspath, "*.cfg", SearchOption.TopDirectoryOnly);
			foreach(string filepath in filenames)
			{
				try
				{
					// Try loading the configuration
					cfg = new Configuration(filepath, true);

					// Check for erors
					if(cfg.ErrorResult != 0)
					{
						// Error in configuration
						message = "Unable to load the nodebuilder configuration file \"" + filepath + "\".\n" +
								  "Error near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription;

						General.WriteLogLine(message);
						MessageBox.Show(mainwindow, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					else
					{
						// Make nodebuilder info
						nodebuilders.Add(new NodebuilderInfo(cfg, filepath));
					}
				}
				catch(Exception)
				{
					// Unable to load configuration
					message = "Unable to load the nodebuilder configuration file \"" + filepath + "\".";
					
					General.WriteLogLine(message);
					MessageBox.Show(mainwindow, message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
			Version thisversion;
			
			// Get a reference to this assembly
			thisasm = Assembly.GetExecutingAssembly();
			thisversion = thisasm.GetName().Version;
			
			// Find application path
			localpath = new Uri(Path.GetDirectoryName(thisasm.GetName().CodeBase));
			apppath = Uri.UnescapeDataString(localpath.AbsolutePath);
			
			// Setup directories
			logfile = Path.Combine(apppath, LOG_FILE);
			temppath = Path.GetTempPath();
			configspath = Path.Combine(apppath, GAME_CONFIGS_DIR);
			compilerspath = Path.Combine(apppath, COMPILERS_DIR);

			// Remove the previous log file and start logging
			File.Delete(logfile);
			General.WriteLogLine("Doom Builder " + thisversion.Major + "." + thisversion.Minor + " startup");
			General.WriteLogLine("Application path:     " + apppath);
			General.WriteLogLine("Temporary path:       " + temppath);
			General.WriteLogLine("Configurations path:  " + configspath);
			General.WriteLogLine("Compilers path:       " + compilerspath);
			
			// Load configuration
			General.WriteLogLine("Loading program configuration...");
			if(!File.Exists(Path.Combine(apppath, SETTINGS_CONFIG_FILE))) throw (new FileNotFoundException("Unable to find the program configuration \"" + SETTINGS_CONFIG_FILE + "\"."));
			settings = new Configuration(Path.Combine(apppath, SETTINGS_CONFIG_FILE), true);
			
			// Create action manager
			actions = new ActionManager();
			
			// Bind static methods to actions
			ActionAttribute.BindMethods(typeof(General));

			// Create main window
			General.WriteLogLine("Loading main interface window...");
			mainwindow = new MainForm();
			mainwindow.UpdateMenus();
			
			// Show main window
			General.WriteLogLine("Showing main interface window...");
			mainwindow.Show();
			mainwindow.Update();
			
			// Load game configurations
			General.WriteLogLine("Loading game configurations...");
			FindGameConfigurations();

			// Load nodebuilder configurations
			General.WriteLogLine("Loading nodebuilder configurations...");
			FindNodebuilderConfigurations();
			
			// Run application from the main window
			General.WriteLogLine("Startup done");
			mainwindow.DisplayReady();
			Application.Run(mainwindow);
		}
		
		#endregion
		
		#region ================== Terminate
		
		// This terminates the program
		public static void Terminate()
		{
			General.WriteLogLine("Termination requested");

			// Unbind static methods from actions
			ActionAttribute.UnbindMethods(typeof(General));
			
			// Clean up
			mainwindow.Dispose();
			actions.Dispose();

			// Save action controls
			actions.SaveSettings();
			
			// Save game configuration settings
			foreach(ConfigurationInfo ci in configs) ci.SaveSettings();
			
			// Save settings configuration
			General.WriteLogLine("Saving program configuration...");
			settings.SaveConfiguration(Path.Combine(apppath, SETTINGS_CONFIG_FILE));

			// Application ends here and now
			General.WriteLogLine("Termination done");
			Application.Exit();
		}
		
		#endregion
		
		#region ================== Management

		// This creates a new map
		[Action(Action.NEWMAP)]
		public static void NewMap()
		{
			MapOptions newoptions = new MapOptions();
			MapOptionsForm optionswindow;
			
			// Ask the user to save changes (if any)
			if(General.AskSaveMap())
			{
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
					if(map.InitializeNewMap(newoptions))
					{
						// Done
						mainwindow.UpdateMenus();
						mainwindow.DisplayReady();
					}
					else
					{
						// Unable to create map manager
						map.Dispose();
						map = null;

						// Show splash logo on display
						mainwindow.ShowSplashDisplay();

						// Failed
						mainwindow.UpdateMenus();
						mainwindow.DisplayReady();
					}
				}
			}
		}

		// This closes the current map
		[Action(Action.CLOSEMAP)]
		public static void CloseMap()
		{
			// Ask the user to save changes (if any)
			if(General.AskSaveMap())
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
			}
		}

		// This loads a map from file
		[Action(Action.OPENMAP)]
		public static void OpenMap()
		{
			OpenFileDialog openfile;
			OpenMapOptionsForm openmapwindow;
			
			// Ask the user to save changes (if any)
			if(General.AskSaveMap())
			{
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
						if(map.InitializeOpenMap(openfile.FileName, openmapwindow.Options))
						{
							// Done
							mainwindow.UpdateMenus();
							mainwindow.DisplayReady();
						}
						else
						{
							// Unable to create map manager
							map.Dispose();
							map = null;

							// Show splash logo on display
							mainwindow.ShowSplashDisplay();

							// Failed
							mainwindow.UpdateMenus();
							mainwindow.DisplayReady();
						}
					}
				}
			}
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

		#region ================== Debug
		
		// This outputs log information
		public static void WriteLogLine(string line)
		{
			// Output to console
			Console.WriteLine(line);
			
			// Write to log file
			File.AppendAllText(logfile, line + Environment.NewLine);
		}

		// This outputs log information
		public static void WriteLog(string text)
		{
			// Output to console
			Console.Write(text);

			// Write to log file
			File.AppendAllText(logfile, text);
		}

		#endregion

		#region ================== Tools

		// This returns a unique temp filename
		public static string MakeTempFilename()
		{
			string filename;
			string chars = "abcdefghijklmnopqrstuvwxyz1234567890";
			Random rnd = new Random();
			int i;

			do
			{
				// Generate a filename
				filename = "";
				for(i = 0; i < 8; i++) filename += chars[rnd.Next(chars.Length)];
				filename = Path.Combine(temppath, filename + ".tmp");
			}
			// Continue while file is not unique
			while(File.Exists(filename));

			// Return the filename
			return filename;
		}

		// This returns the long value for a 8 byte texture name
		public static unsafe long GetTextureLongName(string name)
		{
			long value = 0;
			byte[] namebytes = Encoding.ASCII.GetBytes(name);
			uint bytes = (uint)namebytes.Length;
			if(bytes > 8) bytes = 8;
			
			fixed(void* bp = namebytes)
			{
				CopyMemory(&value, bp, new UIntPtr(bytes));
			}
			
			return value;
		}

		#endregion
	}
}

