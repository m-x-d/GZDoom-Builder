using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace SharpCompress.Archives
{
	public abstract class AbstractArchive<TEntry, TVolume> : IArchive, IArchiveExtractionListener
		where TEntry : IArchiveEntry
		where TVolume : IVolume
	{
		private readonly LazyReadOnlyCollection<TVolume> lazyVolumes;
		private readonly LazyReadOnlyCollection<TEntry> lazyEntries;

		public event EventHandler<ArchiveExtractionEventArgs<IArchiveEntry>> EntryExtractionBegin;
		public event EventHandler<ArchiveExtractionEventArgs<IArchiveEntry>> EntryExtractionEnd;

		public event EventHandler<EventArgs> CompressedBytesRead;
		public event EventHandler<FilePartExtractionBeginEventArgs> FilePartExtractionBegin;

		protected ReaderOptions ReaderOptions { get; private set; }

		private bool disposed;

#if !NO_FILE
		internal AbstractArchive(ArchiveType type, FileInfo fileInfo, ReaderOptions readerOptions)
		{
			Type = type;
			if(!fileInfo.Exists)
			{
				throw new ArgumentException("File does not exist: " + fileInfo.FullName);
			}
			ReaderOptions = readerOptions;
			readerOptions.LeaveStreamOpen = false;
			lazyVolumes = new LazyReadOnlyCollection<TVolume>(LoadVolumes(fileInfo));
			lazyEntries = new LazyReadOnlyCollection<TEntry>(LoadEntries(Volumes));
		}


		protected abstract IEnumerable<TVolume> LoadVolumes(FileInfo file);
#endif

		public ArchiveType Type { get; private set; }

		/// <summary>
		/// Returns an ReadOnlyCollection of all the RarArchiveEntries across the one or many parts of the RarArchive.
		/// </summary>
		public virtual ICollection<TEntry> Entries { get { return lazyEntries; } }
		public int NumEntries { get { return lazyEntries.Count; } } //mxd

		/// <summary>
		/// Returns an ReadOnlyCollection of all the RarArchiveVolumes across the one or many parts of the RarArchive.
		/// </summary>
		public ICollection<TVolume> Volumes { get { return lazyVolumes; } }

		/// <summary>
		/// The total size of the files compressed in the archive.
		/// </summary>
		//public virtual long TotalSize { get { return Entries.Aggregate(0L, (total, cf) => total + cf.CompressedSize); } }

		/// <summary>
		/// The total size of the files as uncompressed in the archive.
		/// </summary>
		//public virtual long TotalUncompressSize { get { return Entries.Aggregate(0L, (total, cf) => total + cf.Size); } }

		//protected abstract IEnumerable<TVolume> LoadVolumes(IEnumerable<Stream> streams);
		protected abstract IEnumerable<TEntry> LoadEntries(IEnumerable<TVolume> volumes);

		//IEnumerable<IArchiveEntry> IArchive.Entries { get { return Entries.Cast<IArchiveEntry>(); } }

		//IEnumerable<IVolume> IArchive.Volumes { get { return lazyVolumes.Cast<IVolume>(); } }

		public virtual void Dispose()
		{
			if(!disposed)
			{
				lazyVolumes.ForEach(v => v.Dispose());
				lazyEntries.GetLoaded().Cast<Entry>().ForEach(x => x.Close());
				disposed = true;
			}
		}

		void IArchiveExtractionListener.EnsureEntriesLoaded()
		{
			lazyEntries.EnsureFullyLoaded();
			lazyVolumes.EnsureFullyLoaded();
		}

		/// <summary>
		/// Use this method to extract all entries in an archive in order.
		/// This is primarily for SOLID Rar Archives or 7Zip Archives as they need to be 
		/// extracted sequentially for the best performance.
		/// 
		/// This method will load all entry information from the archive.
		/// 
		/// WARNING: this will reuse the underlying stream for the archive.  Errors may 
		/// occur if this is used at the same time as other extraction methods on this instance.
		/// </summary>
		/// <returns></returns>
		public IReader ExtractAllEntries()
		{
			((IArchiveExtractionListener)this).EnsureEntriesLoaded();
			return CreateReaderForSolidExtraction();
		}

		protected abstract IReader CreateReaderForSolidExtraction();

		/// <summary>
		/// Archive is SOLID (this means the Archive saved bytes by reusing information which helps for archives containing many small files).
		/// </summary>
		public virtual bool IsSolid { get { return false; } }

		/// <summary>
		/// The archive can find all the parts of the archive needed to fully extract the archive.  This forces the parsing of the entire archive.
		/// </summary>
		public bool IsComplete
		{
			get
			{
				((IArchiveExtractionListener)this).EnsureEntriesLoaded();
				return Entries.All(x => x.IsComplete);
			}
		}
	}
}