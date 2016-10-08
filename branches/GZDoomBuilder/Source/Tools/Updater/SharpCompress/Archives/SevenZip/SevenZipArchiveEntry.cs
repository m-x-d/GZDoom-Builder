using System.IO;
using SharpCompress.Common.SevenZip;

namespace SharpCompress.Archives.SevenZip
{
    public class SevenZipArchiveEntry : SevenZipEntry, IArchiveEntry
    {
        internal SevenZipArchiveEntry(SevenZipArchive archive, SevenZipFilePart part)
            : base(part)
        {
            Archive = archive;
        }

		public IArchive Archive { get; private set; }

        public bool IsComplete { get { return true; } }
    }
}