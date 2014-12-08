#region ================== Namespaces

using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.Types
{
	[TypeHandler(UniversalType.AngleByte, "Byte Angle", true)]
	internal class AngleByteHandler : AngleDegreesHandler
	{
		#region ================== Methods

		public override void Browse(IWin32Window parent) 
		{
			value = (int)Math.Round((float)AngleForm.ShowDialog(parent, (int)Math.Round((float)value / 256 * 360)) / 360 * 256);
		}

		#endregion
	}
}
