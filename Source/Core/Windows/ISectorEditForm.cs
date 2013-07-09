using System;
using System.Collections.Generic;
using System.Text;
using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.Windows
{
	public interface ISectorEditForm
	{
		event EventHandler OnValuesChanged;
		ICollection<Sector> Selection { get; }
	}
}
