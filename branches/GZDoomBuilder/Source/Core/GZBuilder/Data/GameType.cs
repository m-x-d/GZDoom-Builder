using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeImp.DoomBuilder.GZBuilder.Data {
    public enum GameType : int {
        DOOM = 0,
        HERETIC = 1,
        HEXEN = 2,
        STRIFE = 3,
        UNKNOWN = 4,
    }

    public struct Gldefs {
        public static string[] GLDEFS_LUMPS_PER_GAME = { "DOOMDEFS", "HTICDEFS", "HEXNDEFS", "STRFDEFS" };
    }
}
