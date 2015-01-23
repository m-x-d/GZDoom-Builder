#region ================== Namespaces

using System.Collections.Generic;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.SoundPropagationMode
{
	public class SoundEnvironment
	{
		#region ================== Properties

		public List<Sector> Sectors { get; private set; }
		public List<Thing> Things { get; set; }
		public List<Linedef> Linedefs { get; set; }
		public PixelColor Color { get; set; }
		public int ID { get; set; }

		#endregion

		public SoundEnvironment()
		{
			Sectors = new List<Sector>();
			Things = new List<Thing>();
			Linedefs = new List<Linedef>();
			Color = General.Colors.Background;
			ID = -1;
		}
	}
}
