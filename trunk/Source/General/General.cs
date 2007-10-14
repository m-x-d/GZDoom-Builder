
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

		//[DllImport("user32.dll")]
		//public static extern bool LockWindowUpdate(IntPtr hwnd);

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
		private const string SETTINGS_FILE = "Builder.cfg";
		private const string SETTINGS_DIR = "Doom Builder";
		private const string LOG_FILE = "Builder.log";
		private const string GAME_CONFIGS_DIR = "Configurations";
		private const string COMPILERS_DIR = "Compilers";

		#endregion

		#region ================== Variables

		// Files and Folders
		private static string apppath;
		private static string settingspath;
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
		private static List<CompilerInfo> compilers;
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
		public static List<CompilerInfo> Compilers { get { return compilers; } }
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
					ShowErrorMessage("Unable to load the game configuration file \"" + filename + "\".\n" +
									 "Error near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription, MessageBoxButtons.OK);
					return null;
				}
				// Check if this is a Doom Builder 2 config
				else if(cfg.ReadSetting("type", "") != "Doom Builder 2 Game Configuration")
				{
					// Old configuration
					ShowErrorMessage("Unable to load the game configuration file \"" + filename + "\".\n" +
									 "This configuration is not a Doom Builder 2 game configuration.", MessageBoxButtons.OK);
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
				ShowErrorMessage("Unable to load the game configuration file \"" + filename + "\".", MessageBoxButtons.OK);
				return null;
			}
		}

		// This loads all game configurations
		private static void LoadAllGameConfigurations()
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
					General.WriteLogLine("Registered game configuration '" + name + "' from '" + fullfilename + "'");
					configs.Add(new ConfigurationInfo(name, fullfilename));
				}
			}

			// Sort the list
			configs.Sort();
		}

		// This loads all nodebuilder configurations
		private static void LoadAllNodebuilderConfigurations()
		{
			Configuration cfg;
			IDictionary builderslist;
			string[] filenames;
			
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
						ShowErrorMessage("Unable to load the compiler configuration file \"" + Path.GetFileName(filepath) + "\".\n" +
										 "Error near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription, MessageBoxButtons.OK);
					}
					else
					{
						// Get structures
						builderslist = cfg.ReadSetting("nodebuilders", new Hashtable());
						foreach(DictionaryEntry de in builderslist)
						{
							// Check if this is a structure
							if(de.Value is IDictionary)
							{
								try
								{
									// Make nodebuilder info
									nodebuilders.Add(new NodebuilderInfo(Path.GetFileName(filepath), de.Key.ToString(), cfg));
								}
								catch(Exception e)
								{
									// Unable to load configuration
									ShowErrorMessage("Unable to load the nodebuilder configuration '" + de.Key.ToString() + "' from \"" + Path.GetFileName(filepath) + "\". Error: " + e.Message, MessageBoxButtons.OK);
								}
							}
						}
					}
				}
				catch(Exception)
				{
					// Unable to load configuration
					ShowErrorMessage("Unable to load the compiler configuration file \"" + Path.GetFileName(filepath) + "\".", MessageBoxButtons.OK);
				}
			}

			// Sort the list
			nodebuilders.Sort();
		}

		// This loads all compiler configurations
		private static void LoadAllCompilerConfigurations()
		{
			Configuration cfg;
			IDictionary compilerslist;
			string[] filenames;

			// Display status
			mainwindow.DisplayStatus("Loading compiler configurations...");

			// Make array
			compilers = new List<CompilerInfo>();

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
						ShowErrorMessage("Unable to load the compiler configuration file \"" + Path.GetFileName(filepath) + "\".\n" +
										 "Error near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription, MessageBoxButtons.OK);
					}
					else
					{
						// Get structures
						compilerslist = cfg.ReadSetting("compilers", new Hashtable());
						foreach(DictionaryEntry de in compilerslist)
						{
							// Check if this is a structure
							if(de.Value is IDictionary)
							{
								// Make compiler info
								compilers.Add(new CompilerInfo(Path.GetFileName(filepath), de.Key.ToString(), cfg));
							}
						}
					}
				}
				catch(Exception)
				{
					// Unable to load configuration
					ShowErrorMessage("Unable to load the compiler configuration file \"" + Path.GetFileName(filepath) + "\".", MessageBoxButtons.OK);
				}
			}
		}
		
		// This returns a nodebuilder by name
		public static NodebuilderInfo GetNodebuilderByName(string name)
		{
			// Go for all nodebuilders
			foreach(NodebuilderInfo n in nodebuilders)
			{
				// Name matches?
				if(n.Name == name) return n;
			}

			// Cannot find that nodebuilder
			return null;
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
			temppath = Path.GetTempPath();
			settingspath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SETTINGS_DIR);
			configspath = Path.Combine(apppath, GAME_CONFIGS_DIR);
			compilerspath = Path.Combine(apppath, COMPILERS_DIR);
			logfile = Path.Combine(settingspath, LOG_FILE);
			
			// Make program settings directory if missing
			if(!Directory.Exists(settingspath)) Directory.CreateDirectory(settingspath);
			
			// Remove the previous log file and start logging
			if(File.Exists(logfile)) File.Delete(logfile);
			General.WriteLogLine("Doom Builder " + thisversion.Major + "." + thisversion.Minor + " startup");
			General.WriteLogLine("Application path:     " + apppath);
			General.WriteLogLine("Temporary path:       " + temppath);
			General.WriteLogLine("Local settings path:  " + settingspath);
			General.WriteLogLine("Configurations path:  " + configspath);
			General.WriteLogLine("Compilers path:       " + compilerspath);
			
			// Load configuration
			General.WriteLogLine("Loading program configuration...");
			if(LoadProgramConfiguration())
			{
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
				LoadAllGameConfigurations();

				// Load compiler configurations
				General.WriteLogLine("Loading compiler configurations...");
				LoadAllCompilerConfigurations();

				// Load nodebuilder configurations
				General.WriteLogLine("Loading nodebuilder configurations...");
				LoadAllNodebuilderConfigurations();

				// Run application from the main window
				General.WriteLogLine("Startup done");
				mainwindow.DisplayReady();
				Application.Run(mainwindow);
			}
			else
			{
				// Terminate
				Terminate(false);
			}
		}
		
		// Program configuration
		private static bool LoadProgramConfiguration()
		{
			DialogResult result;
			
			// Check if no config for this user exists yet
			if(!File.Exists(Path.Combine(settingspath, SETTINGS_FILE)))
			{
				// Copy new configuration
				General.WriteLogLine("Local user program configuration is missing!");
				File.Copy(Path.Combine(apppath, SETTINGS_FILE), Path.Combine(settingspath, SETTINGS_FILE));
				General.WriteLogLine("New program configuration copied for local user");
			}

			// Load it
			settings = new Configuration(Path.Combine(settingspath, SETTINGS_FILE), true);
			if(settings.ErrorResult != 0)
			{
				// Error in configuration
				// Ask user for a new copy
				result = ShowErrorMessage("Error in program configuration near line " + settings.ErrorLine + ": " + settings.ErrorDescription, MessageBoxButtons.YesNoCancel);
				if(result == DialogResult.Yes)
				{
					// Remove old configuration and make a new copy
					General.WriteLogLine("User requested a new copy of the program configuration");
					File.Delete(Path.Combine(settingspath, SETTINGS_FILE));
					File.Copy(Path.Combine(apppath, SETTINGS_FILE), Path.Combine(settingspath, SETTINGS_FILE));
					General.WriteLogLine("New program configuration copied for local user");
					
					// Load it
					settings = new Configuration(Path.Combine(settingspath, SETTINGS_FILE), true);
					if(settings.ErrorResult != 0)
					{
						// Error in configuration
						General.WriteLogLine("Error in program configuration near line " + settings.ErrorLine + ": " + settings.ErrorDescription);
						ShowErrorMessage("Default program configuration is corrupted. Please re-install Doom Builder.", MessageBoxButtons.OK);
						return false;
					}
				}
				else if(result == DialogResult.Cancel)
				{
					// User requested to cancel startup
					General.WriteLogLine("User cancelled startup");
					return false;
				}
			}

			// Done
			return true;
		}
		
		#endregion
		
		#region ================== Terminate
		
		// This terminates the program
		public static void Terminate(bool properexit)
		{
			// Terminate properly?
			if(properexit)
			{
				General.WriteLogLine("Termination requested");

				// Unbind static methods from actions
				ActionAttribute.UnbindMethods(typeof(General));
				
				// Clean up
				if(map != null) map.Dispose();
				mainwindow.Dispose();
				actions.Dispose();

				// Save action controls
				actions.SaveSettings();

				// Save game configuration settings
				foreach(ConfigurationInfo ci in configs) ci.SaveSettings();

				// Save settings configuration
				General.WriteLogLine("Saving program configuration...");
				settings.SaveConfiguration(Path.Combine(settingspath, SETTINGS_FILE));

				// Application ends here and now
				General.WriteLogLine("Termination done");
				Application.Exit();
			}
			else
			{
				// Just end now
				General.WriteLogLine("Immediate program termination");
				Application.Exit();
			}
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
				General.WriteLogLine("Unloading map...");
				
				// Trash the current map
				if(map != null) map.Dispose();
				map = null;

				// Show splash logo on display
				mainwindow.ShowSplashDisplay();

				// Done
				mainwindow.UpdateMenus();
				mainwindow.DisplayReady();
				General.WriteLogLine("Map unload done");
			}
		}

		// This loads a map from file
		[Action(Action.OPENMAP)]
		public static void OpenMap()
		{
			OpenFileDialog openfile;
			
			// Open map file dialog
			openfile = new OpenFileDialog();
			openfile.Filter = "Doom WAD Files (*.wad)|*.wad";
			openfile.Title = "Open Map";
			openfile.AddExtension = false;
			openfile.CheckFileExists = true;
			openfile.Multiselect = false;
			openfile.ValidateNames = true;
			if(openfile.ShowDialog(mainwindow) == DialogResult.OK)
			{
				// Update main window
				mainwindow.Update();

				// Open map file
				OpenMapFile(openfile.FileName);
			}
		}
		
		// This opens the specified file
		public static void OpenMapFile(string filename)
		{
			OpenMapOptionsForm openmapwindow;

			// Ask the user to save changes (if any)
			if(General.AskSaveMap())
			{
				// Open map options dialog
				openmapwindow = new OpenMapOptionsForm(filename);
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
					if(map.InitializeOpenMap(filename, openmapwindow.Options))
					{
						// Done
						mainwindow.AddRecentFile(filename);
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
		
		// This saves the current map
		[Action(Action.SAVEMAP)]
		public static void SaveMap()
		{
			// Check if a wad file is known
			if(map.FilePathName == "")
			{
				// Call to SaveMapAs
				SaveMapAs();
			}
			else
			{
				// Save the map
				map.SaveMap(map.FilePathName, MapManager.SAVE_NORMAL);
			}
		}

		// This saves the current map as a different file
		[Action(Action.SAVEMAPAS)]
		public static void SaveMapAs()
		{
			SaveFileDialog savefile;
			
			// Show save as dialog
			savefile = new SaveFileDialog();
			savefile.Filter = "Doom WAD Files (*.wad)|*.wad";
			savefile.Title = "Save Map As";
			savefile.AddExtension = true;
			savefile.CheckPathExists = true;
			savefile.OverwritePrompt = true;
			savefile.ValidateNames = true;
			if(savefile.ShowDialog(mainwindow) == DialogResult.OK)
			{
				// Save the map
				map.SaveMap(savefile.FileName, MapManager.SAVE_AS);
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
		
		// This shows a message and logs the message
		public static DialogResult ShowErrorMessage(string message, MessageBoxButtons buttons)
		{
			// Log the message
			WriteLogLine(message);
			
			// Show message
			return MessageBox.Show(Form.ActiveForm, message, Application.ProductName, buttons, MessageBoxIcon.Error);
		}

		// This shows a message and logs the message
		public static DialogResult ShowWarningMessage(string message, MessageBoxButtons buttons)
		{
			// Log the message
			WriteLogLine(message);

			// Show message
			return MessageBox.Show(Form.ActiveForm, message, Application.ProductName, buttons, MessageBoxIcon.Warning);
		}
		
		// This returns a unique temp filename
		public static string MakeTempFilename(string tempdir)
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
				filename = Path.Combine(tempdir, filename + ".tmp");
			}
			// Continue while file is not unique
			while(File.Exists(filename) || Directory.Exists(filename));

			// Return the filename
			return filename;
		}

		// This returns a unique temp directory name
		public static string MakeTempDirname()
		{
			string dirname;
			string chars = "abcdefghijklmnopqrstuvwxyz1234567890";
			Random rnd = new Random();
			int i;

			do
			{
				// Generate a filename
				dirname = "";
				for(i = 0; i < 8; i++) dirname += chars[rnd.Next(chars.Length)];
				dirname = Path.Combine(temppath, dirname);
			}
			// Continue while file is not unique
			while(File.Exists(dirname) || Directory.Exists(dirname));

			// Return the filename
			return dirname;
		}

		#endregion
	}
}

