
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

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class ArgumentInfo
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private readonly string title;
		private readonly string tooltip; //mxd
		private readonly bool used;
		private readonly int type;
		private EnumList enumlist;
		private readonly object defaultvalue; //mxd

		#endregion

		#region ================== Properties

		public string Title { get { return title; } }
		public string ToolTip { get { return tooltip; } } //mxd
		public bool Used { get { return used; } }
		public int Type { get { return type; } }
		public EnumList Enum { get { return enumlist; } internal set { enumlist = value; } }
		public object DefaultValue { get { return defaultvalue; } } //mxd

		#endregion

		#region ================== Constructor / Disposer

		// Constructor for argument info from configuration
		internal ArgumentInfo(Configuration cfg, string argspath, int argindex, IDictionary<string, EnumList> enums)
		{
			// Read
			string istr = argindex.ToString(CultureInfo.InvariantCulture);
			this.used = cfg.SettingExists(argspath + ".arg" + istr);
			this.title = cfg.ReadSetting(argspath + ".arg" + istr + ".title", "Argument " + (argindex + 1));
			this.tooltip = cfg.ReadSetting(argspath + ".arg" + istr + ".tooltip", string.Empty); //mxd
			this.type = cfg.ReadSetting(argspath + ".arg" + istr + ".type", 0);
			this.defaultvalue = cfg.ReadSetting(argspath + ".arg" + istr + ".default", 0); //mxd

			// Determine enum type
			IDictionary argdic = cfg.ReadSetting(argspath + ".arg" + istr, new Hashtable());
			if (argdic.Contains("enum"))
			{
				// Enum fully specified?
				if (argdic["enum"] is IDictionary)
				{
					// Create anonymous enum
					this.enumlist = new EnumList(argdic["enum"] as IDictionary);
				}
				else
				{
					// Check if referenced enum exists
					if ((argdic["enum"].ToString().Length > 0) && enums.ContainsKey(argdic["enum"].ToString()))
					{
						// Get the enum list
						this.enumlist = enums[argdic["enum"].ToString()];
					}
					else
					{
						General.ErrorLogger.Add(ErrorType.Warning, "'" + argspath + ".arg" + istr + "' references unknown enumeration '" + argdic["enum"] + "'.");
					}
				}
			}
			
			if (this.enumlist == null) this.enumlist = new EnumList(); //mxd
		}

		//mxd. Constructor for an argument info defined in DECORATE
		internal ArgumentInfo(string title, string tooltip)
		{
			this.used = true;
			this.title = title;
			this.tooltip = tooltip;
			this.type = 0;
			this.enumlist = new EnumList();
			this.defaultvalue = 0;
		}

		// Constructor for unknown argument info
		internal ArgumentInfo(int argindex)
		{
			this.used = false;
			this.title = "Argument " + (argindex + 1);
			this.type = 0;
			this.enumlist = new EnumList();
			this.defaultvalue = 0; //mxd
		}

		#endregion

		#region ================== Methods

		// This gets the description for an argument value
		public string GetValueDescription(int value)
		{
			// TODO: Use the registered type editor to get the description!

			return value.ToString();
		}

		#endregion
	}
}
