using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CodeImp.DoomBuilder.Data
{
	internal sealed class FlatImage : ImageData
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		private DataReader source;
		private string lumpname;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public FlatImage(string name, DataReader source, string lumpname)
		{
			// Initialize
			this.source = source;
			this.lumpname = lumpname;
			SetName(name);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public override void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up

				// Done
				base.Dispose();
			}
		}

		#endregion

		#region ================== Methods

		// This loads the image
		public override void LoadImage()
		{
			// Leave when already loaded
			if(this.IsLoaded) return;
			
			
			
			// Pass on to base
			base.LoadImage();
		}

		#endregion
	}
}
