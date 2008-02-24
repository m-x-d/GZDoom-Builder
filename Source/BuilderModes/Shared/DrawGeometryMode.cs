using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CodeImp.DoomBuilder.BuilderModes.Shared
{
	internal class DrawGeometryMode : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public DrawGeometryMode()
		{
			// Initialize

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

				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		#endregion
	}
}
