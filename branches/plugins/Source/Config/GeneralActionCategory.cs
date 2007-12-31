using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.IO;

namespace CodeImp.DoomBuilder.Config
{
	public class GeneralActionCategory : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Category properties
		private string title;
		private int offset;
		private int length;
		private List<GeneralActionOption> options;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public string Title { get { return title; } }
		public int Offset { get { return offset; } }
		public int Length { get { return length; } }
		public List<GeneralActionOption> Options { get { return options; } }
		public bool IsDisposed { get { return isdisposed; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public GeneralActionCategory(string name, Configuration cfg)
		{
			IDictionary opts;
			
			// Initialize
			this.options = new List<GeneralActionOption>();
			
			// Read properties
			this.title = cfg.ReadSetting("gen_linedeftypes." + name + ".title", "");
			this.offset = cfg.ReadSetting("gen_linedeftypes." + name + ".offset", 0);
			this.length = cfg.ReadSetting("gen_linedeftypes." + name + ".length", 0);
			
			// Read the options
			opts = cfg.ReadSetting("gen_linedeftypes." + name, new Hashtable());
			foreach(DictionaryEntry de in opts)
			{
				// Is this an option and not just some value?
				if(de.Value is IDictionary)
				{
					// Add the option
					this.options.Add(new GeneralActionOption(name, de.Key.ToString(), (IDictionary)de.Value));
				}
			}

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				options = null;
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// String representation
		public override string ToString()
		{
			return title;
		}
		
		#endregion
	}
}
