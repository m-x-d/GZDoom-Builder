
namespace CodeImp.DoomBuilder.Actions
{
	public class HintsManager
	{
		public static string GetRtfString(string text) {
			text = text.Replace("<b>", "{\\b ").Replace("</b>", "}").Replace("<br>", "\\par\\par ");
			return "{\\rtf1" + text + "}";
		}
	}
}
