#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.Plugins
{
	/// <summary>
	/// This is the key link between the Doom Builder core and the plugin.
	/// Every plugin must expose a single class that inherits this class.
	/// </summary>
	public class Plug : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Internals
		private Plugin plugin;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Internals
		internal Plugin Plugin { get { return plugin; } set { plugin = value; } }
		
		/// <summary>
		/// Indicates if the plugin has been disposed.
		/// </summary>
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		/// <summary>
		/// This is the key link between the Doom Builder core and the plugin.
		/// Every plugin must expose a single class that inherits this class.
		/// <para>
		/// NOTE: Some methods cannot be used in this constructor, because the plugin
		/// is not yet fully initialized. Instead, use the Initialize method to do
		/// your initializations.
		/// </para>
		/// </summary>
		public Plug()
		{
			// Initialize

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// This is called by the Doom Builder core when the plugin is being disposed.
		/// </summary>
		public virtual void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				plugin = null;
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		/// <summary>
		/// This finds the embedded resource with the specified name in the plugin and creates
		/// a Stream from it. Returns null when the embedded resource cannot be found.
		/// </summary>
		/// <param name="resourcename">Name of the resource in the plugin.</param>
		/// <returns>Returns a Stream of the embedded resource,
		/// or null when the resource cannot be found.</returns>
		public Stream GetResourceStream(string resourcename)
		{
			return plugin.GetResourceStream(resourcename);
		}

		#endregion

		#region ================== Events

		/// <summary>
		/// Occurs before a map is opened.
		/// </summary>
		public virtual void OnMapOpenBegin()
		{
		}

		/// <summary>
		/// Occurs after a map is opened.
		/// </summary>
		public virtual void OnMapOpenEnd()
		{
		}

		/// <summary>
		/// Occurs before a new map is created.
		/// </summary>
		public virtual void OnMapNewBegin()
		{
		}

		/// <summary>
		/// Occurs after a new map is created.
		/// </summary>
		public virtual void OnMapNewEnd()
		{
		}

		/// <summary>
		/// This is called after the constructor to allow a plugin to initialize.
		/// </summary>
		public virtual void OnInitialize()
		{
		}

		/// <summary>
		/// This is called when the user chose to reload the resources.
		/// </summary>
		public virtual void OnReloadResources()
		{
		}

		/// <summary>
		/// This is called by the Doom Builder core when the editing mode changes.
		/// Return false to abort the mode change.
		/// </summary>
		/// <param name="oldmode">The previous editing mode</param>
		/// <param name="newmode">The new editing mode</param>
		public virtual bool OnModeChange(EditMode oldmode, EditMode newmode)
		{
			return true;
		}

		/// <summary>
		/// Called by the Doom Builder core when the user changes the program configuration (F5).
		/// </summary>
		public virtual void OnProgramReconfigure()
		{
		}

		/// <summary>
		/// Called by the Doom Builder core when the user changes the map settings (F2).
		/// </summary>
		public virtual void OnMapReconfigure()
		{
		}

		/// <summary>
		/// Called by the Doom Builder core when the user wants to copy selected geometry.
		/// Return false to abort the copy operation.
		/// The result parameter is false when the operation was already aborted by another plugin.
		/// </summary>
		public virtual bool OnCopyBegin(bool result)
		{
			return true;
		}

		/// <summary>
		/// Called by the Doom Builder core when the user has copied geometry.
		/// </summary>
		public virtual void OnCopyEnd()
		{
		}

		/// <summary>
		/// Called by the Doom Builder core when the user wants to paste geometry into the map.
		/// Return false to abort the paste operation.
		/// The result parameter is false when the operation was already aborted by another plugin.
		/// </summary>
		public virtual bool OnPasteBegin(bool result)
		{
			return true;
		}

		/// <summary>
		/// Called by the Doom Builder core when the user pastes geometry into the map. The new geometry is created and marked before this method is called.
		/// </summary>
		public virtual void OnPasteEnd()
		{
		}

		/// <summary>
		/// Called by the Doom Builder core when the user wants to undo the previous action.
		/// Return false to abort the operation.
		/// The result parameter is false when the operation was already aborted by another plugin.
		/// </summary>
		public virtual bool OnUndoBegin(bool result)
		{
			return true;
		}

		/// <summary>
		/// Called by the Doom Builder core when the user has undone the previous action.
		/// </summary>
		public virtual void OnUndoEnd()
		{
		}

		/// <summary>
		/// Called by the Doom Builder core when the user wants to redo the previously undone action.
		/// Return false to abort the operation.
		/// The result parameter is false when the operation was already aborted by another plugin.
		/// </summary>
		public virtual bool OnRedoBegin(bool result)
		{
			return true;
		}

		/// <summary>
		/// Called by the Doom Builder core when the user has redone the action.
		/// </summary>
		public virtual void OnRedoEnd()
		{
		}

		#endregion
	}
}
