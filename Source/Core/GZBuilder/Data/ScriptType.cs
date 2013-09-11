
namespace CodeImp.DoomBuilder.GZBuilder.Data {
	internal enum ScriptType {
		UNKNOWN = 0,
		ACS = 1,
		MODELDEF = 2,
		DECORATE = 3,
	}
	
	internal struct ScriptTypes {
		private static string[] knownScriptTypes = { "UNKNOWN SCRIPT", "ZDoom ACS script", "GZDoom MODELDEF", "ZDoom DECORATE" };
		internal static string[] TYPES { get { return knownScriptTypes; } } 
	}
}
