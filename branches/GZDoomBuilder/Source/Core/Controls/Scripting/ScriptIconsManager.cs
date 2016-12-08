using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;

namespace CodeImp.DoomBuilder.Controls.Scripting
{
	internal class ScriptIconsManager
	{
		internal const int SCRIPT_TYPE_ICONS_OFFSET = 4;
		internal const int SCRIPT_GROUP_ICONS_OFFSET = 23;
		internal const int SCRIPT_GROUP_OPEN_ICONS_OFFSET = 42;
		
		private ImageList icons;
		public ImageList Icons { get { return icons; } }

		public ScriptIconsManager(ImageList icons)
		{
			this.icons = icons;
		}

		public int GetResourceIcon(int datalocationtype)
		{
			return datalocationtype;
		}

		public int GetScriptIcon(ScriptType type)
		{
			int scripttype = (int)type + SCRIPT_TYPE_ICONS_OFFSET;
			if(scripttype >= SCRIPT_GROUP_ICONS_OFFSET) scripttype = SCRIPT_TYPE_ICONS_OFFSET;
			return scripttype;
		}

		public int GetScriptFolderIcon(ScriptType type, bool opened)
		{
			int scripttype = (int)type;
			if(scripttype >= SCRIPT_GROUP_ICONS_OFFSET - SCRIPT_TYPE_ICONS_OFFSET)
				scripttype = SCRIPT_TYPE_ICONS_OFFSET;

			if(opened) return SCRIPT_GROUP_OPEN_ICONS_OFFSET + scripttype;
			return SCRIPT_GROUP_ICONS_OFFSET + scripttype;
		}
	}
}
