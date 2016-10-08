using System;
using System.Collections.Generic;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace SharpCompress.Archives
{
    public interface IArchive : IDisposable
    {
		int NumEntries { get; } //mxd
        IReader ExtractAllEntries();
        bool IsComplete { get; }
    }
}