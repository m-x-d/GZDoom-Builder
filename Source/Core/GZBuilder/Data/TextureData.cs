using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeImp.DoomBuilder.GZBuilder.Data {
    public struct TextureData {
        public const string INVALID_TEXTURE = "**INVALID_TEXTURE**";
        public static string[] SUPPORTED_TEXTURE_EXTENSIONS = { ".jpg", ".tga", ".png", ".dds" };
    }
}
