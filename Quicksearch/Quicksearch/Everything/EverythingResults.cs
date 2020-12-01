using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Quicksearch.Everything
{
    internal class EverythingResults : IEnumerable<EverythingResult>
    {
        internal EverythingQueryStats QueryStats { get; }

        internal uint ViewableResultCount { get; }

        internal EverythingResults(EverythingQueryStats eqs)
        {
            this.QueryStats = eqs;
            this.ViewableResultCount = EverythingAPI.GetNumResults();
        }

        public IEnumerator<EverythingResult> GetEnumerator()
        {
            var sb = new StringBuilder(EverythingAPI.MAX_PATH_LENGTH);
            for (uint i = 0; i < ViewableResultCount; i++)
            {
                yield return new EverythingResult(i, QueryStats.ReceivedData, sb);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class EverythingResult
    {
        public uint Index { get; }
        public DateTime DateAccessed { get; } = DateTime.MinValue;
        public DateTime DateCreated { get; } = DateTime.MinValue;
        public DateTime DateModified { get; } = DateTime.MinValue;
        public string Extension { get; } = string.Empty;
        public string Path { get; } = string.Empty;
        public string HighlightedPath { get; } = string.Empty;
        public EverythingResultType Type { get; } = EverythingResultType.None;
        public long Size { get; } = 0;

        private delegate bool DateDelegate(uint index, out long date);

        internal EverythingResult(uint index, Data receivedData, StringBuilder sb)
        {
            this.Index = index;
            if (receivedData.HasFlag(Data.FULL_PATH_AND_FILE_NAME))
                this.Path = ReadEverythingString(EverythingAPI.GetResultFullPathName, sb);

            if (receivedData.HasFlag(Data.HIGHLIGHTED_FULL_PATH_AND_FILE_NAME))
                this.HighlightedPath = Marshal.PtrToStringUni(EverythingAPI.GetResultHighlightedFullPathAndFileName(this.Index));

            if (receivedData.HasFlag(Data.EXTENSION))
                this.Extension = Marshal.PtrToStringUni(EverythingAPI.GetResultExtension(this.Index));

            if (receivedData.HasFlag(Data.DATE_ACCESSED))
                this.DateAccessed = ReadEverythingDate(EverythingAPI.GetResultDateAccessed);
            if (receivedData.HasFlag(Data.DATE_CREATED))
                this.DateCreated = ReadEverythingDate(EverythingAPI.GetResultDateCreated);
            if (receivedData.HasFlag(Data.DATE_MODIFIED))
                this.DateModified = ReadEverythingDate(EverythingAPI.GetResultDateModified);

            if (receivedData.HasFlag(Data.SIZE))
                if (EverythingAPI.GetResultSize(this.Index, out var size))
                    this.Size = size;

            if (EverythingAPI.IsFileResult(this.Index))
                this.Type = EverythingResultType.File;
            else if (EverythingAPI.IsVolumeResult(this.Index))
                this.Type = EverythingResultType.Volume;
            else if (EverythingAPI.IsFolderResult(this.Index))
                this.Type = EverythingResultType.Folder;
        }

        private DateTime ReadEverythingDate(DateDelegate dd)
        {
            if (dd(this.Index, out var date) && date != -1)
                return DateTime.FromFileTime(date);
            return DateTime.MinValue;
        }

        private string ReadEverythingString(Action<uint, StringBuilder, uint> getResultFullPathName, StringBuilder sb)
        {
            sb.Clear();
            getResultFullPathName(this.Index, sb, (uint)sb.Capacity);
            return sb.ToString();
        }
    }

    [Flags]
    public enum EverythingResultType
    {
        None = 0,
        File = 1 << 0,
        Folder = 1 << 2,
        Volume = 1 << 3,

        All = File | Folder | Volume
    }
}
