﻿#region ================== Namespaces

using SlimDX;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.Data 
{
	public sealed class MapInfo
	{
		#region ================== Variables

		private bool isdefined;

		private string title;
		private string sky1;
		private float sky1scrollspeed;
		private string sky2;
		private float sky2scrollspeed;
		private bool doublesky;
		private bool hasfadecolor;
		private Color4 fadecolor;
		private bool hasoutsidefogcolor;
		private Color4 outsidefogcolor;
		private int fogdensity;
		private int outsidefogdensity;

		private bool evenlighting;
		private bool smoothlighting;
		private int vertwallshade;
		private int horizwallshade;

		#endregion

		#region ================== Properties

		public bool IsDefined { get { return isdefined; } }

		public string Title { get { return title; } internal set { title = value; isdefined = true; } }
		public string Sky1 { get { return sky1; } internal set { sky1 = value; isdefined = true; } }
		public float Sky1ScrollSpeed { get { return sky1scrollspeed; } internal set { sky1scrollspeed = value; isdefined = true; } }
		public string Sky2 { get { return sky2; } internal set { sky2 = value; isdefined = true; } }
		public float Sky2ScrollSpeed { get { return sky2scrollspeed; } internal set { sky2scrollspeed = value; isdefined = true; } }
		public bool DoubleSky { get { return doublesky; } internal set { doublesky = value; isdefined = true; } }
		public bool HasFadeColor { get { return hasfadecolor; } internal set { hasfadecolor = value; isdefined = true; } }
		public Color4 FadeColor { get { return fadecolor; } internal set { fadecolor = value; isdefined = true; } }
		public bool HasOutsideFogColor { get { return hasoutsidefogcolor; } internal set { hasoutsidefogcolor = value; isdefined = true; } }
		public Color4 OutsideFogColor { get { return outsidefogcolor; } internal set { outsidefogcolor = value; isdefined = true; } }
		public int FogDensity { get { return fogdensity; } internal set { fogdensity = value; isdefined = true; } }
		public int OutsideFogDensity { get { return outsidefogdensity; } internal set { outsidefogdensity = value; isdefined = true; } }

		public bool EvenLighting { get { return evenlighting; } internal set { evenlighting = value; isdefined = true; } }
		public bool SmoothLighting { get { return smoothlighting; } internal set { smoothlighting = value; isdefined = true; } }
		public int VertWallShade { get { return vertwallshade; } internal set { vertwallshade = value; isdefined = true; } }
		public int HorizWallShade { get { return horizwallshade; } internal set { horizwallshade = value; isdefined = true; } }

		#endregion

		#region ================== Constructor

		public MapInfo() 
		{
			vertwallshade = 16;
			horizwallshade = -16;
		}

		#endregion
	}
}
