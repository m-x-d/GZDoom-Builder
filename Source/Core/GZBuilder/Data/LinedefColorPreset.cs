using System.Collections.Generic;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Map;

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
    public class LinedefColorPreset
    {
        public bool Valid { get { return valid; } }
        private bool valid;

        public string ErrorDescription { get { return errorDescription; } }
        private string errorDescription;
        public string WarningDescription;

        public string Name;
        public PixelColor Color;
        public int Action;
        public int Activation; //Hexen activation type
        public List<string> Flags;
		public List<string> RestrictedFlags;
        private const string NOT_VALID = "invalid";
        private const string DUPLICATE = "duplicate";

        public LinedefColorPreset(string name, PixelColor lineColor) {
            Name = name;
            Color = lineColor;
            Flags = new List<string>();
			RestrictedFlags = new List<string>();
        }

        public LinedefColorPreset(string name, PixelColor lineColor, int action, int activation, List<string> flags, List<string> restrictedFlags) {
            Name = name;
            Color = lineColor;
            Action = action;
            Activation = activation;
            Flags = flags;
			RestrictedFlags = restrictedFlags;
        }

        public LinedefColorPreset(LinedefColorPreset copyFrom) {
            Name = copyFrom.Name;
            Color = copyFrom.Color;
            Action = copyFrom.Action;
            Activation = copyFrom.Activation;

            Flags = new List<string>();
            Flags.AddRange(copyFrom.Flags);

			RestrictedFlags = new List<string>();
			RestrictedFlags.AddRange(copyFrom.RestrictedFlags);
        }

        public bool Matches(Linedef l) {
            //check action; -1 means Any Action
            if(Action != 0) {
                if((Action == -1 && l.Action == 0) || (Action != -1 && l.Action != Action))
                    return false;
            }

            //check activation; -1 means Any Activation
            if(Activation != 0) {
                if(!General.Map.UDMF && (l.Activate != Activation || (Activation == -1 && l.Activate == 0)))
                    return false;
            }

            //check flags
            if(Flags.Count > 0) {
                foreach(string s in Flags) {
                    if(!l.IsFlagSet(s))
                        return false;
                }
            }

			//check flags, which should be disabled
			if(RestrictedFlags.Count > 0) {
				foreach(string s in RestrictedFlags) {
					if(l.IsFlagSet(s))
						return false;
				}
			}

            return true;
        }

        public void SetValid() {
            valid = true;
            errorDescription = "";
            WarningDescription = "";
        }

        public void SetInvalid(string reason) {
            valid = false;
            errorDescription = reason;
        }

        public override string ToString() {
            List<string> rest = new List<string>();

            if(!valid)
                rest.Add(NOT_VALID);

            if(!string.IsNullOrEmpty(WarningDescription))
                rest.Add(DUPLICATE);

            if(rest.Count > 0)
                return Name + " (" + string.Join(", ", rest.ToArray()) + ")";
            return Name;
        }
    }
}
