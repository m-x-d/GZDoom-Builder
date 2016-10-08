using System;
using System.IO;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace SharpCompress.Archives
{
    public class ArchiveFactory
    {
		/// <summary>
		/// Constructor expects a filepath to an existing file.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="options"></param>
		public static IArchive Open(string filePath)
		{
			//filePath.CheckNotNullOrEmpty("filePath");
			return Open(new FileInfo(filePath), new ReaderOptions());
		}

		/// <summary>
		/// Constructor with a FileInfo object to an existing file.
		/// </summary>
		/// <param name="fileInfo"></param>
		/// <param name="options"></param>
		public static IArchive Open(FileInfo fileInfo, ReaderOptions options)
		{
			using(var stream = fileInfo.OpenRead())
			{
				if(SevenZipArchive.IsSevenZipFile(stream))
				{
					stream.Dispose();
					return SevenZipArchive.Open(fileInfo, options);
				}
				throw new InvalidOperationException("Cannot determine compressed stream type. Supported Archive Formats: Zip, GZip, Tar, Rar, 7Zip");
			}
		}
    }
}