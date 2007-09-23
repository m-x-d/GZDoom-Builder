using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CodeImp.DoomBuilder.Rendering
{
	internal class TextureImageResource : ImageResource
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public TextureImageResource()
		{
			// Initialize

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
