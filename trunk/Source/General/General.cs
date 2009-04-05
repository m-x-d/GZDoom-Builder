
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
using CodeImp.DoomBuilder.Geometry;
using System.Runtime.InteropServices;
using CodeImp.DoomBuilder.Actions;
using System.Diagnostics;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Config;
using Microsoft.Win32;
using SlimDX.Direct3D9;
using System.Drawing;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.Types;
using System.Collections.ObjectModel;
using System.Threading;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder
{
	public static class General
	{
		#region ================== API Declarations

		[DllImport("user32.dll")]
		internal static extern bool LockWindowUpdate(IntPtr hwnd);

		[DllImport("kernel32.dll", EntryPoint = "RtlZeroMemory", SetLastError = false)]
		internal static extern void ZeroMemory(IntPtr dest, int size);

		[DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
		internal static extern unsafe void CopyMemory(void* dst, void* src, uint length);

		[DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern int SendMessage(IntPtr hwnd, uint Msg, int wParam, int lParam);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool MessageBeep(MessageBeepType type);

		[DllImport("kernel32.dll")]
		internal extern static IntPtr LoadLibrary(string filename);

		[DllImport("kernel32.dll")]
		internal extern static bool FreeLibrary(IntPtr moduleptr);

		[DllImport("user32.dll")]
		internal static extern IntPtr CreateWindowEx(uint exstyle, string classname, string windowname, uint style,
												   int x, int y, int width, int height, IntPtr parentptr, int menu,
												   IntPtr instanceptr, string param);

		[DllImport("user32.dll")]
		internal static extern bool DestroyWindow(IntPtr windowptr);

		[DllImport("user32.dll")]
		internal static extern int SetWindowPos(IntPtr windowptr, int insertafterptr, int x, int y, int cx, int cy, int flags);
		
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern uint GetShortPathName([MarshalAs(UnmanagedType.LPTStr)] string longpath, [MarshalAs(UnmanagedType.LPTStr)]StringBuilder shortpath, uint buffersize);

		[DllImport("user32.dll")]
		internal static extern int SetScrollInfo(IntPtr windowptr, int bar, IntPtr scrollinfo, bool redraw);

		[DllImport("user32.dll")]
		internal static extern int GetScrollInfo(IntPtr windowptr, int bar, IntPtr scrollinfo);

		#endregion

		#region ================== Constants

		// SendMessage API
		internal const int WM_USER = 0x400;
		internal const int WM_SYSCOMMAND = 0x112;
		internal const int SC_KEYMENU = 0xF100;
		internal const int CB_SETITEMHEIGHT = 0x153;
		internal const int CB_SHOWDROPDOWN = 0x14F;
		internal const int EM_GETSCROLLPOS = WM_USER + 221;
		internal const int EM_SETSCROLLPOS = WM_USER + 222;
		internal const int SB_HORZ = 0;
		internal const int SB_VERT = 1;
		internal const int SB_CTL = 2;
		internal const int SIF_RANGE = 0x1;
		internal const int SIF_PAGE = 0x2;
		internal const int SIF_POS = 0x4;
		internal const int SIF_DISABLENOSCROLL = 0x8;
		internal const int SIF_TRACKPOS = 0x16;
		internal const int SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS;
		
		// Files and Folders
		private const string SETTINGS_FILE = "Builder.cfg";
		private const string SETTINGS_DIR = "Doom Builder";
		private const string LOG_FILE = "Builder.log";
		private const string GAME_CONFIGS_DIR = "Configurations";
		private const string COMPILERS_DIR = "Compilers";
		private const string PLUGINS_DIR = "Plugins";
		private const string SCRIPTS_DIR = "Scripting";
		private const string SETUP_DIR = "Setup";
		private const string SPRITES_DIR = "Sprites";
		private const string HELP_FILE = "Refmanual.chm";

		// SCROLLINFO structure
		internal struct ScrollInfo
		{
			public int size;		// size of this structure
			public uint mask;		// combination of SIF_ constants
			public int min;			// minimum scrolling position
			public int max;			// maximum scrolling position
			public uint page;		// page size (scroll bar uses this value to determine the appropriate size of the proportional scroll box)
			public int pos;			// position of the scroll box
			public int trackpos;	// immediate position of a scroll box that the user is dragging
		}

		#endregion

		#region ================== Variables

		// Files and Folders
		private static string apppath;
		private static string setuppath;
		private static string settingspath;
		private static string logfile;
		private static string temppath;
		private static string configspath;
		private static string compilerspath;
		private static string scriptspath;
		private static string pluginspath;
		private static string spritespath;
		
		// Main objects
		private static Assembly thisasm;
		private static MainForm mainwindow;
		private static ProgramConfiguration settings;
		private static MapManager map;
		private static EditingManager editing;
		private static ActionManager actions;
		private static PluginManager plugins;
		private static ColorCollection colors;
		private static TypesManager types;
		private static Clock clock;
		private static ErrorLogger errorlogger;
		
		// Configurations
		private static List<ConfigurationInfo> configs;
		private static List<CompilerInfo> compilers;
		private static List<NodebuilderInfo> nodebuilders;
		private static Dictionary<string, ScriptConfiguration> scriptconfigs;
		
		// States
		private static bool debugbuild;
		
		// Command line arguments
		private static string[] cmdargs;
		private static string autoloadfile = null;
		private static string autoloadmap = null;
		private static string autoloadconfig = null;
		private static bool delaymainwindow;

		#endregion

		#region ================== Properties

		public static Assembly ThisAssembly { get { return thisasm; } }
		public static string AppPath { get { return apppath; } }
		public static string TempPath { get { return temppath; } }
		public static string ConfigsPath { get { return configspath; } }
		public static string CompilersPath { get { return compilerspath; } }
		public static string PluginsPath { get { return pluginspath; } }
		public static string SpritesPath { get { return spritespath; } }
		public static ICollection<string> CommandArgs { get { return Array.AsReadOnly<string>(cmdargs); } }
		internal static MainForm MainWindow { get { return mainwindow; } }
		public static IMainForm Interface { get { return mainwindow; } }
		public static ProgramConfiguration Settings { get { return settings; } }
		public static ColorCollection Colors { get { return colors; } }
		internal static List<ConfigurationInfo> Configs { get { return configs; } }
		internal static List<NodebuilderInfo> Nodebuilders { get { return nodebuilders; } }
		internal static List<CompilerInfo> Compilers { get { return compilers; } }
		internal static Dictionary<string, ScriptConfiguration> ScriptConfigs { get { return scriptconfigs; } }
		public static MapManager Map { get { return map; } }
		public static ActionManager Actions { get { return actions; } }
		internal static PluginManager Plugins { get { return plugins; } }
		public static Clock Clock { get { return clock; } }
		public static bool DebugBuild { get { return debugbuild; } }
		internal static TypesManager Types { get { return types; } }
		public static string AutoLoadFile { get { return autoloadfile; } }
		public static string AutoLoadMap { get { return autoloadmap; } }
		public static string AutoLoadConfig { get { return autoloadconfig; } }
		public static bool DelayMainWindow { get { return delaymainwindow; } }
		public static EditingManager Editing { get { return editing; } }
		public static ErrorLogger ErrorLogger { get { return errorlogger; } }
		
		#endregion

		#region ================== Configurations

		// This returns the game configuration info by filename
		internal static ConfigurationInfo GetConfigurationInfo(string filename)
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
		internal static Configuration LoadGameConfiguration(string filename)
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
					// The following code was used to convert the linedef types of DB1 type
					// configurations into proper categorized structures for DB2.
					// I keep this code here in the repository because if it failed, I might
					// need this again to convert from scratch.
					/*
					GameConfiguration gcfg = new GameConfiguration(cfg);
					Configuration newcfg = new Configuration();
					newcfg.NewConfiguration(true);
					bool doommap = (gcfg.FormatInterface == "DoomMapSetIO");

					foreach(LinedefActionInfo a in gcfg.SortedLinedefActions)
					{
						string catkey = a.Category.ToLowerInvariant().Trim();
						string cattitle = a.Category;
						string linekey = a.Index.ToString(CultureInfo.InvariantCulture);
						string linetitle = a.Name;
						string lineprefix = a.Prefix;
						if(catkey.Length == 0) { catkey = "misc"; cattitle = ""; }
						if(cattitle.Length > 0) newcfg.WriteSetting("linedeftypes." + catkey + ".title", cattitle);
						newcfg.WriteSetting("linedeftypes." + catkey + "." + linekey + ".title", linetitle);
						if(doommap) newcfg.WriteSetting("linedeftypes." + catkey + "." + linekey + ".prefix", lineprefix);

						if(!doommap)
						{
							for(int i = 0; i < 5; i++)
							{
								if(a.ArgUsed[i])
								{
									newcfg.WriteSetting("linedeftypes." + catkey + "." + linekey + ".arg" + i.ToString(CultureInfo.InvariantCulture) + ".title", a.ArgTitle[i]);
									if(a.ArgTagType[i] != TagType.None) newcfg.WriteSetting("linedeftypes." + catkey + "." + linekey + ".arg" + i.ToString(CultureInfo.InvariantCulture) + ".tag", (int)a.ArgTagType[i]);
								}
							}
						}
					}
					newcfg.SaveConfiguration(Path.Combine(configspath, "_" + filename));
					*/
					
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
			mainwindow.DisplayStatus(StatusType.Busy, "Loading game configurations...");

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
					fullfilename = Path.GetFileName(filepath);
					ConfigurationInfo cfginfo = new ConfigurationInfo(cfg, fullfilename);
					
					// Add to lists
					General.WriteLogLine("Registered game configuration '" + cfginfo.Name + "' from '" + fullfilename + "'");
					configs.Add(cfginfo);
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
			mainwindow.DisplayStatus(StatusType.Busy, "Loading nodebuilder configurations...");

			// Make array
			nodebuilders = new List<NodebuilderInfo>();

			// Go for all cfg files in the compilers directory
			filenames = Directory.GetFiles(compilerspath, "*.cfg", SearchOption.AllDirectories);
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

		// This loads all script configurations
		private static void LoadAllScriptConfigurations()
		{
			Configuration cfg;
			string[] filenames;
			
			// Display status
			mainwindow.DisplayStatus(StatusType.Busy, "Loading script configurations...");
			
			// Make collection
			scriptconfigs = new Dictionary<string, ScriptConfiguration>();
			
			// Go for all cfg files in the scripts directory
			filenames = Directory.GetFiles(scriptspath, "*.cfg", SearchOption.TopDirectoryOnly);
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
						ShowErrorMessage("Unable to load the script configuration file \"" + Path.GetFileName(filepath) + "\".\n" +
										 "Error near line " + cfg.ErrorLine + ": " + cfg.ErrorDescription, MessageBoxButtons.OK);
					}
					else
					{
						try
						{
							// Make script configuration
							ScriptConfiguration scfg = new ScriptConfiguration(cfg);
							string filename = Path.GetFileName(filepath);
							scriptconfigs.Add(filename.ToLowerInvariant(), scfg);
						}
						catch(Exception e)
						{
							// Unable to load configuration
							ShowErrorMessage("Unable to load the script configuration \"" + Path.GetFileName(filepath) + "\". Error: " + e.Message, MessageBoxButtons.OK);
						}
					}
				}
				catch(Exception e)
				{
					// Unable to load configuration
					ShowErrorMessage("Unable to load the script configuration file \"" + Path.GetFileName(filepath) + "\". Error: " + e.Message, MessageBoxButtons.OK);
				}
			}
		}

		// This loads all compiler configurations
		private static void LoadAllCompilerConfigurations()
		{
			Configuration cfg;
			Dictionary<string, CompilerInfo> addedcompilers = new Dictionary<string,CompilerInfo>();
			IDictionary compilerslist;
			string[] filenames;

			// Display status
			mainwindow.DisplayStatus(StatusType.Busy, "Loading compiler configurations...");

			// Make array
			compilers = new List<CompilerInfo>();

			// Go for all cfg files in the compilers directory
			filenames = Directory.GetFiles(compilerspath, "*.cfg", SearchOption.AllDirectories);
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
								CompilerInfo info = new CompilerInfo(Path.GetFileName(filepath), de.Key.ToString(), Path.GetDirectoryName(filepath), cfg);
								if(!addedcompilers.ContainsKey(info.Name))
								{
									compilers.Add(info);
									addedcompilers.Add(info.Name, info);
								}
								else
								{
									errorlogger.Add(ErrorType.Error, "Compiler \"" + info.Name + "\" is defined more than once. The first definition in " + addedcompilers[info.Name].FileName + " will be used.");
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
		}
		
		// This returns a nodebuilder by name
		internal static NodebuilderInfo GetNodebuilderByName(string name)
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
		internal static void Main(string[] args)
		{
			Uri localpath;
			Version thisversion;
			
			// Determine states
			#if DEBUG
				debugbuild = true;
			#else
				debugbuild = false;
			#endif
			
			// Enable OS visual styles
			Application.EnableVisualStyles();
			Application.DoEvents();		// This must be here to work around a .NET bug

			// Hook to DLL loading failure event
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
			
			// Set current thread name
			Thread.CurrentThread.Name = "Main Application";
			
			// Get a reference to this assembly
			thisasm = Assembly.GetExecutingAssembly();
			thisversion = thisasm.GetName().Version;
			
			// Find application path
			localpath = new Uri(Path.GetDirectoryName(thisasm.GetName().CodeBase));
			apppath = Uri.UnescapeDataString(localpath.AbsolutePath);
			
			// Setup directories
			temppath = Path.GetTempPath();
			setuppath = Path.Combine(apppath, SETUP_DIR);
			settingspath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SETTINGS_DIR);
			configspath = Path.Combine(apppath, GAME_CONFIGS_DIR);
			compilerspath = Path.Combine(apppath, COMPILERS_DIR);
			pluginspath = Path.Combine(apppath, PLUGINS_DIR);
			scriptspath = Path.Combine(apppath, SCRIPTS_DIR);
			spritespath = Path.Combine(apppath, SPRITES_DIR);
			logfile = Path.Combine(settingspath, LOG_FILE);
			
			// Make program settings directory if missing
			if(!Directory.Exists(settingspath)) Directory.CreateDirectory(settingspath);
			
			// Remove the previous log file and start logging
			if(File.Exists(logfile)) File.Delete(logfile);
			General.WriteLogLine("Doom Builder " + thisversion.Major + "." + thisversion.Minor + " startup");
			General.WriteLogLine("Application path:        " + apppath);
			General.WriteLogLine("Temporary path:          " + temppath);
			General.WriteLogLine("Local settings path:     " + settingspath);
			General.WriteLogLine("Command-line arguments:  " + args.Length);
			for(int i = 0; i < args.Length; i++)
				General.WriteLogLine("Argument " + i + ":   \"" + args[i] + "\"");
			
			// Parse command-line arguments
			ParseCommandLineArgs(args);
			
			// Load configuration
			General.WriteLogLine("Loading program configuration...");
			settings = new ProgramConfiguration();
			if(settings.Load(Path.Combine(settingspath, SETTINGS_FILE),
							 Path.Combine(apppath, SETTINGS_FILE)))
			{
				// Create error logger
				errorlogger = new ErrorLogger();
				
				// Create action manager
				actions = new ActionManager();
				
				// Bind static methods to actions
				General.Actions.BindMethods(typeof(General));

				// Initialize static classes
				MapSet.Initialize();

				// Create main window
				General.WriteLogLine("Loading main interface window...");
				mainwindow = new MainForm();
				mainwindow.UpdateInterface();

				if(!delaymainwindow)
				{
					// Show main window
					General.WriteLogLine("Showing main interface window...");
					mainwindow.Show();
					mainwindow.Update();
				}
				
				// Start Direct3D
				General.WriteLogLine("Starting Direct3D graphics driver...");
				try { D3DDevice.Startup(); }
				catch(Direct3D9NotFoundException) { AskDownloadDirectX(); return; }
				catch(Direct3DX9NotFoundException) { AskDownloadDirectX(); return; }
				
				// Load plugin manager
				General.WriteLogLine("Loading plugins...");
				plugins = new PluginManager();
				plugins.LoadAllPlugins();
				
				// Load game configurations
				General.WriteLogLine("Loading game configurations...");
				LoadAllGameConfigurations();

				// Create editing modes
				General.WriteLogLine("Creating editing modes manager...");
				editing = new EditingManager();
				
				// Now that all settings have been combined (core & plugins) apply the defaults
				General.WriteLogLine("Applying configuration settings...");
				actions.ApplyDefaultShortcutKeys();
				mainwindow.ApplyShortcutKeys();
				foreach(ConfigurationInfo info in configs) info.ApplyDefaults(null);
				
				// Load compiler configurations
				General.WriteLogLine("Loading compiler configurations...");
				LoadAllCompilerConfigurations();

				// Load nodebuilder configurations
				General.WriteLogLine("Loading nodebuilder configurations...");
				LoadAllNodebuilderConfigurations();
				
				// Load script configurations
				General.WriteLogLine("Loading script configurations...");
				LoadAllScriptConfigurations();
				
				// Load color settings
				General.WriteLogLine("Loading color settings...");
				colors = new ColorCollection(settings.Config);
				
				// Create application clock
				General.WriteLogLine("Creating application clock...");
				clock = new Clock();
				
				// Create types manager
				General.WriteLogLine("Creating types manager...");
				types = new TypesManager();
				
				// Do auto map loading when window is delayed
				if(delaymainwindow)
					mainwindow.PerformAutoMapLoading();
				
				// All done
				General.WriteLogLine("Startup done");
				mainwindow.DisplayReady();
				
				// Show any errors if preferred
				if(errorlogger.IsErrorAdded)
				{
					mainwindow.DisplayStatus(StatusType.Warning, "There were errors during program statup!");
					if(!delaymainwindow && General.Settings.ShowErrorsWindow) mainwindow.ShowErrors();
				}
				
				// Run application from the main window
				Application.Run(mainwindow);
			}
			else
			{
				// Terminate
				Terminate(false);
			}
		}

		// This handles DLL linking errors
		private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			// Check if SlimDX failed loading
			if(args.Name.Contains("SlimDX")) AskDownloadDirectX();

			// Return null
			return null;
		}
		
		// This asks the user to download DirectX
		private static void AskDownloadDirectX()
		{
			// Cancel loading map from command-line parameters, if any.
			// This causes problems, because when the window is shown, the map will
			// be loaded and DirectX is initialized (which we seem to be missing)
			CancelAutoMapLoad();
			
			// Ask the user to download DirectX
			if(MessageBox.Show("This application requires the latest version of Microsoft DirectX installed on your computer." + Environment.NewLine +
				"Do you want to install and/or update Microsoft DirectX now?", "DirectX Error", System.Windows.Forms.MessageBoxButtons.YesNo,
				System.Windows.Forms.MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
			{
				// Open DX web setup
				//System.Diagnostics.Process.Start("http://www.microsoft.com/downloads/details.aspx?FamilyId=2DA43D38-DB71-4C1B-BC6A-9B6652CD92A3").WaitForExit(1000);
				System.Diagnostics.Process.Start(Path.Combine(setuppath, "dxwebsetup.exe")).WaitForExit(1000);
			}

			// End program here
			Terminate(false);
		}

		// This parses the command line arguments
		private static void ParseCommandLineArgs(string[] args)
		{
			// Keep a copy
			cmdargs = args;
			
			// Make a queue so we can parse the values from left to right
			Queue<string> argslist = new Queue<string>(args);
			
			// Parse list
			while(argslist.Count > 0)
			{
				// Get next arg
				string curarg = argslist.Dequeue();
				
				// Delay window?
				if(string.Compare(curarg, "-DELAYWINDOW", true) == 0)
				{
					// Delay showing the main window
					delaymainwindow = true;
				}
				// Map name info?
				else if(string.Compare(curarg, "-MAP", true) == 0)
				{
					// Store next arg as map name information
					autoloadmap = argslist.Dequeue();
				}
				// Config name info?
				else if((string.Compare(curarg, "-CFG", true) == 0) ||
					    (string.Compare(curarg, "-CONFIG", true) == 0))
				{
					// Store next arg as config filename information
					autoloadconfig = argslist.Dequeue();
				}
				// Every other arg
				else
				{
					// No command to load file yet?
					if(autoloadfile == null)
					{
						// Check if this is a file we can load
						if(File.Exists(curarg))
						{
							// Load this file!
							autoloadfile = curarg.Trim();
						}
						else
						{
							// Note in the log that we cannot find this file
							General.ErrorLogger.Add(ErrorType.Warning, "Cannot find the specified file \"" + curarg + "\"");
						}
					}
				}
			}
		}
		
		// This cancels automatic map loading
		internal static void CancelAutoMapLoad()
		{
			autoloadfile = null;
		}
		
		#endregion
		
		#region ================== Terminate

		// This is for plugins to use
		public static void Exit(bool properexit)
		{
			// Plugin wants to exit nicely?
			if(properexit)
			{
				// Close dialog forms first
				while((Form.ActiveForm != mainwindow) && (Form.ActiveForm != null))
					Form.ActiveForm.Close();

				// Close main window
				mainwindow.Close();
			}
			else
			{
				// Terminate, no questions asked
				Terminate(true);
			}
		}
		
		// This terminates the program
		internal static void Terminate(bool properexit)
		{
			// Terminate properly?
			if(properexit)
			{
				General.WriteLogLine("Termination requested");
				
				// Unbind static methods from actions
				General.Actions.UnbindMethods(typeof(General));
				
				// Save colors
				colors.SaveColors(settings.Config);
				
				// Save action controls
				actions.SaveSettings();
				
				// Save game configuration settings
				foreach(ConfigurationInfo ci in configs) ci.SaveSettings();
				
				// Save settings configuration
				General.WriteLogLine("Saving program configuration...");
				settings.Save(Path.Combine(settingspath, SETTINGS_FILE));
				
				// Clean up
				if(map != null) map.Dispose(); map = null;
				if(editing != null) editing.Dispose(); editing = null;
				if(mainwindow != null) mainwindow.Dispose();
				if(actions != null) actions.Dispose();
				if(clock != null) clock.Dispose();
				if(plugins != null) plugins.Dispose();
				if(types != null) types.Dispose();
				try { D3DDevice.Terminate(); } catch(Exception) { }

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

			// Die.
			Process.GetCurrentProcess().Kill();
		}
		
		#endregion
		
		#region ================== Management

		// This cancels a volatile mode, as if the user presses cancel
		public static bool CancelVolatileMode()
		{
			// Volatile mode?
			if((map != null) & (editing.Mode != null) && editing.Mode.Attributes.Volatile)
			{
				// Cancel
				editing.Mode.OnCancel();
				return true;
			}
			else
			{
				// Mode is not volatile
				return false;
			}
		}

		// This disengages a volatile mode, leaving the choice to cancel or accept to the editing mode
		public static bool DisengageVolatileMode()
		{
			// Volatile mode?
			if((map != null) && (editing.Mode != null) && editing.Mode.Attributes.Volatile)
			{
				// Change back to normal mode
				editing.ChangeMode(editing.PreviousStableMode.Name);
				return true;
			}
			else
			{
				// Mode is not volatile
				return false;
			}
		}
		
		// This creates a new map
		[BeginAction("newmap")]
		internal static void NewMap()
		{
			MapOptions newoptions = new MapOptions();
			MapOptionsForm optionswindow;
			
			// Cancel volatile mode, if any
			General.DisengageVolatileMode();
			
			// Ask the user to save changes (if any)
			if(General.AskSaveMap())
			{
				// Open map options dialog
				optionswindow = new MapOptionsForm(newoptions);
				optionswindow.IsForNewMap = true;
				if(optionswindow.ShowDialog(mainwindow) == DialogResult.OK)
				{
					// Display status
					mainwindow.DisplayStatus(StatusType.Busy, "Creating new map...");
					Cursor.Current = Cursors.WaitCursor;
					
					// Let the plugins know
					plugins.OnMapNewBegin();
					
					// Clear the display
					mainwindow.ClearDisplay();

					// Trash the current map, if any
					if(map != null) map.Dispose();

					// Set this to false so we can see if errors are added
					General.ErrorLogger.IsErrorAdded = false;
					
					// Create map manager with given options
					map = new MapManager();
					if(map.InitializeNewMap(newoptions))
					{
						// Done
					}
					else
					{
						// Unable to create map manager
						map.Dispose();
						map = null;

						// Show splash logo on display
						mainwindow.ShowSplashDisplay();
					}

					// Let the plugins know
					plugins.OnMapNewEnd();

					// All done
					mainwindow.RedrawDisplay();
					mainwindow.UpdateInterface();
					mainwindow.HideInfo();

					if(errorlogger.IsErrorAdded)
					{
						// Show any errors if preferred
						mainwindow.DisplayStatus(StatusType.Warning, "There were errors during loading!");
						if(!delaymainwindow && General.Settings.ShowErrorsWindow) mainwindow.ShowErrors();
					}
					else
						mainwindow.DisplayReady();
					
					Cursor.Current = Cursors.Default;
				}
			}
		}

		// This closes the current map
		[BeginAction("closemap")]
		internal static void ActionCloseMap() { CloseMap(); }
		internal static bool CloseMap()
		{
			// Cancel volatile mode, if any
			General.DisengageVolatileMode();

			// Ask the user to save changes (if any)
			if(General.AskSaveMap())
			{
				// Display status
				mainwindow.DisplayStatus(StatusType.Busy, "Closing map...");
				General.WriteLogLine("Unloading map...");
				Cursor.Current = Cursors.WaitCursor;
				
				// Trash the current map
				if(map != null) map.Dispose();
				map = null;
				
				// Clear errors
				General.ErrorLogger.Clear();
				
				// Show splash logo on display
				mainwindow.ShowSplashDisplay();
				
				// Done
				Cursor.Current = Cursors.Default;
				editing.UpdateCurrentEditModes();
				mainwindow.RedrawDisplay();
				mainwindow.HideInfo();
				mainwindow.UpdateInterface();
				mainwindow.DisplayReady();
				General.WriteLogLine("Map unload done");
				return true;
			}
			else
			{
				// User cancelled
				return false;
			}
		}

		// This loads a map from file
		[BeginAction("openmap")]
		internal static void OpenMap()
		{
			OpenFileDialog openfile;

			// Cancel volatile mode, if any
			General.DisengageVolatileMode();

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
		internal static void OpenMapFile(string filename)
		{
			OpenMapOptionsForm openmapwindow;

			// Cancel volatile mode, if any
			General.DisengageVolatileMode();
			
			// Ask the user to save changes (if any)
			if(General.AskSaveMap())
			{
				// Open map options dialog
				openmapwindow = new OpenMapOptionsForm(filename);
				if(openmapwindow.ShowDialog(mainwindow) == DialogResult.OK)
					OpenMapFileWithOptions(filename, openmapwindow.Options);
			}
		}
		
		// This opens the specified file without dialog
		internal static void OpenMapFileWithOptions(string filename, MapOptions options)
		{
			// Display status
			mainwindow.DisplayStatus(StatusType.Busy, "Opening map file...");
			Cursor.Current = Cursors.WaitCursor;

			// Let the plugins know
			plugins.OnMapOpenBegin();

			// Clear the display
			mainwindow.ClearDisplay();

			// Trash the current map, if any
			if(map != null) map.Dispose();

			// Set this to false so we can see if errors are added
			General.ErrorLogger.IsErrorAdded = false;

			// Create map manager with given options
			map = new MapManager();
			if(map.InitializeOpenMap(filename, options))
			{
				// Add recent file
				mainwindow.AddRecentFile(filename);
			}
			else
			{
				// Unable to create map manager
				map.Dispose();
				map = null;

				// Show splash logo on display
				mainwindow.ShowSplashDisplay();
			}

			// Let the plugins know
			plugins.OnMapOpenEnd();

			// All done
			mainwindow.RedrawDisplay();
			mainwindow.UpdateInterface();
			mainwindow.HideInfo();

			if(errorlogger.IsErrorAdded)
			{
				// Show any errors if preferred
				mainwindow.DisplayStatus(StatusType.Warning, "There were errors during loading!");
				if(!delaymainwindow && General.Settings.ShowErrorsWindow) mainwindow.ShowErrors();
			}
			else
				mainwindow.DisplayReady();
			
			Cursor.Current = Cursors.Default;
		}
		
		// This saves the current map
		// Returns tre when saved, false when cancelled or failed
		[BeginAction("savemap")]
		internal static void ActionSaveMap() { SaveMap(); }
		internal static bool SaveMap()
		{
			bool result = false;
			
			// Cancel volatile mode, if any
			General.DisengageVolatileMode();
			
			// Check if a wad file is known
			if(map.FilePathName == "")
			{
				// Call to SaveMapAs
				result = SaveMapAs();
			}
			else
			{
				// Display status
				mainwindow.DisplayStatus(StatusType.Busy, "Saving map file...");
				Cursor.Current = Cursors.WaitCursor;

				// Set this to false so we can see if errors are added
				General.ErrorLogger.IsErrorAdded = false;
				
				// Save the map
				if(map.SaveMap(map.FilePathName, MapManager.SAVE_NORMAL))
				{
					// Add recent file
					mainwindow.AddRecentFile(map.FilePathName);
					result = true;
				}

				// All done
				mainwindow.UpdateInterface();

				if(errorlogger.IsErrorAdded)
				{
					// Show any errors if preferred
					mainwindow.DisplayStatus(StatusType.Warning, "There were errors during saving!");
					if(!delaymainwindow && General.Settings.ShowErrorsWindow) mainwindow.ShowErrors();
				}
				else
					mainwindow.DisplayStatus(StatusType.Info, "Map saved in " + map.FileTitle + ".");

				Cursor.Current = Cursors.Default;
			}

			return result;
		}


		// This saves the current map as a different file
		// Returns tre when saved, false when cancelled or failed
		[BeginAction("savemapas")]
		internal static void ActionSaveMapAs() { SaveMapAs(); }
		internal static bool SaveMapAs()
		{
			SaveFileDialog savefile;
			bool result = false;
			
			// Cancel volatile mode, if any
			General.DisengageVolatileMode();

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
				// Display status
				mainwindow.DisplayStatus(StatusType.Busy, "Saving map file...");
				Cursor.Current = Cursors.WaitCursor;

				// Set this to false so we can see if errors are added
				General.ErrorLogger.IsErrorAdded = false;
				
				// Save the map
				if(map.SaveMap(savefile.FileName, MapManager.SAVE_AS))
				{
					// Add recent file
					mainwindow.AddRecentFile(map.FilePathName);
					result = true;
				}

				// All done
				mainwindow.UpdateInterface();

				if(errorlogger.IsErrorAdded)
				{
					// Show any errors if preferred
					mainwindow.DisplayStatus(StatusType.Warning, "There were errors during saving!");
					if(!delaymainwindow && General.Settings.ShowErrorsWindow) mainwindow.ShowErrors();
				}
				else
					mainwindow.DisplayStatus(StatusType.Info, "Map saved in " + map.FileTitle + ".");

				Cursor.Current = Cursors.Default;
			}

			return result;
		}


		// This saves the current map as a different file
		// Returns tre when saved, false when cancelled or failed
		[BeginAction("savemapinto")]
		internal static void ActionSaveMapInto() { SaveMapInto(); }
		internal static bool SaveMapInto()
		{
			SaveFileDialog savefile;
			bool result = false;

			// Cancel volatile mode, if any
			General.DisengageVolatileMode();

			// Show save as dialog
			savefile = new SaveFileDialog();
			savefile.Filter = "Doom WAD Files (*.wad)|*.wad";
			savefile.Title = "Save Map Into";
			savefile.AddExtension = true;
			savefile.CheckPathExists = true;
			savefile.OverwritePrompt = false;
			savefile.ValidateNames = true;
			if(savefile.ShowDialog(mainwindow) == DialogResult.OK)
			{
				// Display status
				mainwindow.DisplayStatus(StatusType.Busy, "Saving map file...");
				Cursor.Current = Cursors.WaitCursor;

				// Set this to false so we can see if errors are added
				General.ErrorLogger.IsErrorAdded = false;
				
				// Save the map
				if(map.SaveMap(savefile.FileName, MapManager.SAVE_INTO))
				{
					// Add recent file
					mainwindow.AddRecentFile(map.FilePathName);
					result = true;
				}

				// All done
				mainwindow.UpdateInterface();

				if(errorlogger.IsErrorAdded)
				{
					// Show any errors if preferred
					mainwindow.DisplayStatus(StatusType.Warning, "There were errors during saving!");
					if(!delaymainwindow && General.Settings.ShowErrorsWindow) mainwindow.ShowErrors();
				}
				else
					mainwindow.DisplayStatus(StatusType.Info, "Map saved into " + map.FileTitle + ".");

				Cursor.Current = Cursors.Default;
			}

			return result;
		}
		
		// This asks to save the map if needed
		// Returns false when action was cancelled
		internal static bool AskSaveMap()
		{
			DialogResult result;
			
			// Map open and not saved?
			if(map != null)
			{
				if(map.IsChanged)
				{
					// Ask to save changes
					result = MessageBox.Show(mainwindow, "Do you want to save changes to " + map.FileTitle + " (" + map.Options.CurrentName + ")?", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
					if(result == DialogResult.Yes)
					{
						// Save map
						if(SaveMap())
						{
							// Ask to save changes to scripts
							return map.AskSaveScriptChanges();
						}
						else
						{
							// Failed to save map
							return false;
						}
					}
					else if(result == DialogResult.Cancel)
					{
						// Abort
						return false;
					}
					else
					{
						// Ask to save changes to scripts
						return map.AskSaveScriptChanges();
					}
				}
				else
				{
					// Ask to save changes to scripts
					return map.AskSaveScriptChanges();
				}
			}
			else
			{
				return true;
			}
		}
		
		#endregion

		#region ================== Debug
		
		// This shows a major failure
		public static void Fail(string message)
		{
			General.WriteLogLine("FAIL: " + message);
			Debug.Fail(message);
			Terminate(false);
		}
		
		// This outputs log information
		public static void WriteLogLine(string line)
		{
			// Output to console
			Console.WriteLine(line);
			
			// Write to log file
			try { File.AppendAllText(logfile, line + Environment.NewLine); }
			catch(Exception) { }
		}

		// This outputs log information
		public static void WriteLog(string text)
		{
			// Output to console
			Console.Write(text);

			// Write to log file
			try { File.AppendAllText(logfile, text); }
			catch(Exception) { }
		}
		
		#endregion

		#region ================== Tools
		
		// This swaps two pointers
		public static void Swap<T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}
		
		// This clamps a value
		public static float Clamp(float value, float min, float max)
		{
			return Math.Min(Math.Max(min, value), max);
		}

		// This clamps a value
		public static int Clamp(int value, int min, int max)
		{
			return Math.Min(Math.Max(min, value), max);
		}

		// This clamps a value
		public static byte Clamp(byte value, byte min, byte max)
		{
			return Math.Min(Math.Max(min, value), max);
		}
		
		// This returns an element from a collection by index
		public static T GetByIndex<T>(ICollection<T> collection, int index)
		{
			IEnumerator<T> e = collection.GetEnumerator();
			for(int i = -1; i < index; i++) e.MoveNext();
			return e.Current;
		}

		// This returns the next power of 2
		public static int NextPowerOf2(int v)
		{
			int p = 0;

			// Continue increasing until higher than v
			while(Math.Pow(2, p) < v) p++;

			// Return power
			return (int)Math.Pow(2, p);
		}
		
		// Convert bool to integer
		internal static int Bool2Int(bool v)
		{
			return v ? 1 : 0;
		}

		// Convert integer to bool
		internal static bool Int2Bool(int v)
		{
			return (v != 0);
		}

		// This shows a message and logs the message
		public static DialogResult ShowErrorMessage(string message, MessageBoxButtons buttons)
		{
			Cursor oldcursor;
			DialogResult result;
			
			// Log the message
			WriteLogLine(message);
			
			// Use normal cursor
			oldcursor = Cursor.Current;
			Cursor.Current = Cursors.Default;
			
			// Show message
			IWin32Window window = null;
			if((Form.ActiveForm != null) && Form.ActiveForm.Visible) window = Form.ActiveForm;
			result = MessageBox.Show(window, message, Application.ProductName, buttons, MessageBoxIcon.Error);

			// Restore old cursor
			Cursor.Current = oldcursor;
			
			// Return result
			return result;
		}

		// This shows a message and logs the message
		public static DialogResult ShowWarningMessage(string message, MessageBoxButtons buttons)
		{
			return ShowWarningMessage(message, buttons, MessageBoxDefaultButton.Button1);
		}

		// This shows a message and logs the message
		public static DialogResult ShowWarningMessage(string message, MessageBoxButtons buttons, MessageBoxDefaultButton defaultbutton)
		{
			Cursor oldcursor;
			DialogResult result;

			// Log the message
			WriteLogLine(message);

			// Use normal cursor
			oldcursor = Cursor.Current;
			Cursor.Current = Cursors.Default;

			// Show message
			IWin32Window window = null;
			if((Form.ActiveForm != null) && Form.ActiveForm.Visible) window = Form.ActiveForm;
			result = MessageBox.Show(window, message, Application.ProductName, buttons, MessageBoxIcon.Warning, defaultbutton);

			// Restore old cursor
			Cursor.Current = oldcursor;

			// Return result
			return result;
		}

		// This shows the reference manual
		public static void ShowHelp(string pagefile)
		{
			Help.ShowHelp(mainwindow, Path.Combine(apppath, HELP_FILE), HelpNavigator.Topic, pagefile);
		}
		
		// This returns a unique temp filename
		internal static string MakeTempFilename(string tempdir)
		{
			return MakeTempFilename(tempdir, "tmp");
		}

		// This returns a unique temp filename
		internal static string MakeTempFilename(string tempdir, string extension)
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
				filename = Path.Combine(tempdir, filename + "." + extension);
			}
			// Continue while file is not unique
			while(File.Exists(filename) || Directory.Exists(filename));

			// Return the filename
			return filename;
		}

		// This returns a unique temp directory name
		internal static string MakeTempDirname()
		{
			string dirname;
			const string chars = "abcdefghijklmnopqrstuvwxyz1234567890";
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

		// This shows an image in a panel either zoomed or centered depending on size
		public static void DisplayZoomedImage(Panel panel, Image image)
		{
			// Set the image
			panel.BackgroundImage = image;
			
			// Image not null?
			if(image != null)
			{
				// Small enough to fit in panel?
				if((image.Size.Width < panel.ClientRectangle.Width) &&
				   (image.Size.Height < panel.ClientRectangle.Height))
				{
					// Display centered
					panel.BackgroundImageLayout = ImageLayout.Center;
				}
				else
				{
					// Display zoomed
					panel.BackgroundImageLayout = ImageLayout.Zoom;
				}
			}
		}

		// This calculates the new rectangle when one is scaled into another keeping aspect ratio
		public static RectangleF MakeZoomedRect(Size source, RectangleF target)
		{
			return MakeZoomedRect(new SizeF((int)source.Width, (int)source.Height), target);
		}

		// This calculates the new rectangle when one is scaled into another keeping aspect ratio
		public static RectangleF MakeZoomedRect(Size source, Rectangle target)
		{
			return MakeZoomedRect(new SizeF((int)source.Width, (int)source.Height),
								  new RectangleF((int)target.Left, (int)target.Top, (int)target.Width, (int)target.Height));
		}
		
		// This calculates the new rectangle when one is scaled into another keeping aspect ratio
		public static RectangleF MakeZoomedRect(SizeF source, RectangleF target)
		{
			float scale;
			
			// Image fits?
			if((source.Width <= target.Width) &&
			   (source.Height <= target.Height))
			{
				// Just center
				scale = 1.0f;
			}
			// Image is wider than tall?
			else if((source.Width - target.Width) > (source.Height - target.Height))
			{
				// Scale down by width
				scale = target.Width / source.Width;
			}
			else
			{
				// Scale down by height
				scale = target.Height / source.Height;
			}
			
			// Return centered and scaled
			return new RectangleF(target.Left + (target.Width - source.Width * scale) * 0.5f,
								  target.Top + (target.Height - source.Height * scale) * 0.5f,
								  source.Width * scale, source.Height * scale);
		}

		// This opens a URL in the default browser
		public static void OpenWebsite(string url)
		{
			RegistryKey key = null;
			Process p = null;
			string browser;

			try
			{
				// Get the registry key where default browser is stored
				key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

				// Trim off quotes
				browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");

				// String doesnt end in EXE?
				if(!browser.EndsWith("exe"))
				{
					// Get rid of everything after the ".exe"
					browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4);
				}
			}
			finally
			{
				// Clean up
				if(key != null) key.Close();
			}

			try
			{
				// Fork a process
				p = new Process();
				p.StartInfo.FileName = browser;
				p.StartInfo.Arguments = url;
				p.Start();
			}
			catch(Exception) { }

			// Clean up
			if(p != null) p.Dispose();
		}
		
		// This returns the short path name for a file
		public static string GetShortFilePath(string longpath)
		{
			int maxlen = 256;
			StringBuilder shortname = new StringBuilder(maxlen);
			uint len = GetShortPathName(longpath, shortname, (uint)maxlen);
			return shortname.ToString();
		}
		
		#endregion
		
		/*
		[BeginAction("testaction")]
		internal static void TestAction()
		{
			ScriptEditorForm t = new ScriptEditorForm();
			t.ShowDialog(mainwindow);
			t.Dispose();
		}
		*/
	}
}

