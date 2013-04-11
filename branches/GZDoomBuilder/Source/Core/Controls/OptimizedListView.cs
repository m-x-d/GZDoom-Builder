
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

using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public class OptimizedListView : ListView
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor

		// Constructor
		public OptimizedListView() : base()
		{
			this.DoubleBuffered = true;
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}

		#endregion

		#region ================== Methods

		#endregion
	}
}
