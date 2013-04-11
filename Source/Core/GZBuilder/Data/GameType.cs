
namespace CodeImp.DoomBuilder.GZBuilder.Data {
    public enum GameType : int {
        UNKNOWN = 0,
        DOOM = 1,
        HERETIC = 2,
        HEXEN = 3,
        STRIFE = 4,
    }

    public struct Gldefs {
        public static string[] GLDEFS_LUMPS_PER_GAME = { "UNKNOWN_GAME", "DOOMDEFS", "HTICDEFS", "HEXNDEFS", "STRFDEFS" };
    }
}
