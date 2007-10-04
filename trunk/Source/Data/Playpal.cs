using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.Rendering;

namespace CodeImp.DoomBuilder.Data
{
	internal class Playpal
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private PixelColor[] colors;

		#endregion

		#region ================== Properties

		public PixelColor this[int a] { get { return colors[a]; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Playpal(IDataReader source, int lumpindex)
		{
			// Initialize

			// We have no destructor
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ================== Methods

		#endregion
	}
}
