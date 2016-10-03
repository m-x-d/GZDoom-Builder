#region ================== Namespaces

using System;
using System.Collections.Generic;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public class SoundInfo
	{
		#region ================== Enums

		public enum RolloffType
		{
			NONE,
			INVALID,
			CUSTOM,
			LINEAR,
			LOG
		}

		public enum SoundInfoType
		{
			SOUND,
			GROUP_RANDOM,
		}

		#endregion

		#region ================== Constants

		//TODO: this probably should be defined in the Game Configuration...
		public static readonly HashSet<string> SPECIAL_SOUNDS = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "*death", "*xdeath", "*wimpydeath", "*crazydeath", "*burndeath", "*gibbed", "*splat", "*pain100", "*pain75",
            "*pain50", "*pain25", "*grunt", "*land", "*falling", "*jump", "*fist", "*fistgrunt", "*usefail", "*evillaugh",
            "*weaponlaugh", "*puzzfail", "*poison", "*dive", "*surface", "*gasp", "*taunt", "*regenerate", "*drainhealth"         
        };

		#endregion

		#region ================== Variables

		private string name;
		private List<SoundInfo> children;
		private SoundInfoType type;

		#endregion

		#region ================== Properties

		public string Name { get { return name; } }
		public List<SoundInfo> Children { get { return children; } }
		public SoundInfoType Type { get { return type; } internal set { type = value; } }

		// Sound settings
		public string LumpName;
		public float Volume;
		public float Attenuation;
		public int MinimumDistance;
		public int MaximumDistance;
		public RolloffType Rolloff;
		public float RolloffFactor;

		#endregion

		#region ================== Constructor

		public SoundInfo(string name)
		{
			this.name = name;
			children = new List<SoundInfo>();
			type = SoundInfoType.SOUND;

			// Set non-existent settings
			Volume = float.MinValue;
			Attenuation = float.MinValue;
			MinimumDistance = int.MinValue;
			MaximumDistance = int.MinValue;
			Rolloff = RolloffType.INVALID;
			RolloffFactor = float.MinValue;
		}

		// Default props constructor
		internal SoundInfo()
		{
			this.name = "#GLOBAL_PROPERTIES#";
			children = new List<SoundInfo>();
			type = SoundInfoType.SOUND;

			// Set non-existent settings
			Volume = 1.0f;
			Attenuation = 1.0f;
			MinimumDistance = 200;
			MaximumDistance = 1200;
			Rolloff = RolloffType.NONE;
			RolloffFactor = 1.0f; // Is this the default value?
		}

		#endregion
	}
}
