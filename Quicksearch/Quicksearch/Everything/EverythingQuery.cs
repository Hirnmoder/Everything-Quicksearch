using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksearch.Everything
{
    internal class EverythingQuery
    {
        internal string Search { get; }
        internal Data Data { get; }
        internal Sort Sort { get; }
        internal bool MatchPath { get; }
        internal bool MatchWholeWord { get; }
        internal bool MatchCase { get; }
        internal uint ResultCount { get; }
        internal uint ResultOffset { get; }
        internal bool Executed { get; private set; }
        internal Status ErrorCode { get; private set; }
        internal EverythingQueryStats QueryStats { get; private set; }

        internal EverythingQuery(string search, Data data, Sort sort, bool matchPath, bool matchWholeWord, bool matchCase, uint resultCount, uint resultOffset)
        {
            this.Search = search;
            this.Data = data;
            this.Sort = sort;
            this.MatchPath = matchPath;
            this.MatchWholeWord = matchWholeWord;
            this.MatchCase = matchCase;
            this.ResultCount = resultCount;
            this.ResultOffset = resultOffset;
            this.Executed = false;
            this.ErrorCode = Status.OK;
            this.QueryStats = null;
        }

        internal bool Execute()
        {
            try
            {
                EverythingAPI.SetSearch(Search);
                EverythingAPI.SetMatchPath(MatchPath);
                EverythingAPI.SetMatchWholeWord(MatchWholeWord);
                EverythingAPI.SetMatchCase(MatchCase);
                EverythingAPI.SetRequestFlags((uint)Data);
                EverythingAPI.SetSort((uint)Sort);
                EverythingAPI.SetMax(ResultCount);
                EverythingAPI.SetOffset(ResultOffset);
                if (EverythingAPI.Query(true))
                {
                    Executed = true;
                    this.QueryStats = new EverythingQueryStats(EverythingAPI.GetTotResults(),
                        EverythingAPI.GetTotFileResults(),
                        EverythingAPI.GetTotFolderResults(),
                        (Data)EverythingAPI.GetResultListRequestFlags());
                    return true;
                }
                else
                {
                    ErrorCode = (Status)EverythingAPI.GetLastError();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }
    }

    internal class EverythingQueryStats
    {
        internal Data ReceivedData { get; }
        internal uint ResultCount { get; }
        internal uint FileCount { get; }
        internal uint FolderCount { get; }

        internal EverythingQueryStats(uint resultCount, uint fileCount, uint folderCount, Data receivedData)
        {
            this.ReceivedData = receivedData;
            this.ResultCount = resultCount;
            this.FileCount = fileCount;
            this.FolderCount = folderCount;
        }
    }
}
