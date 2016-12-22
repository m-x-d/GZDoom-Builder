#region ================== Namespaces

using System;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	internal class ImageBrowserCategoryItem : ImageBrowserItem
	{
		#region ================== Variables

		private string groupname;

		#endregion

		#region ================== Properties

		public override bool IsPreviewLoaded { get { return true; } }
		public override string TextureName { get { return groupname; } }

		#endregion

		#region ================== Constructors

		private ImageBrowserCategoryItem(ImageData icon, string tooltip, bool showfullname) : base(icon, tooltip, showfullname) { }
		public ImageBrowserCategoryItem(ImageBrowserItemType itemtype, string groupname)
		{
			this.groupname = groupname;
			this.itemtype = itemtype;
			
			switch(itemtype)
			{
				case ImageBrowserItemType.FOLDER:
					icon = General.Map.Data.FolderTexture;
					break;

				case ImageBrowserItemType.FOLDER_UP:
					icon = General.Map.Data.FolderUpTexture;
					break;

				default:
					throw new NotImplementedException("Unsupported ItemType");
			}
		}

		#endregion
	}
}
