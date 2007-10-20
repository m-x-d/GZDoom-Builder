
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
using CodeImp.DoomBuilder.IO;

#endregion

namespace CodeImp.DoomBuilder
{
	internal class GameConfigurationCache
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private int soundlinedefflags;

		#endregion

		#region ================== Properties

		public int SoundLinedefFlags { get { return soundlinedefflags; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public GameConfigurationCache(Configuration cfg)
		{
			// Initialize
			soundlinedefflags = cfg.ReadSetting("soundlinedefflags", 0);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		#endregion
	}
}
