using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Quicksearch.Everything
{

    internal class EverythingAPI
    {
        [DllImport("Everything64.dll", CharSet = CharSet.Unicode, EntryPoint = "Everything_SetSearch")]
        public static extern int SetSearch(string lpSearchString);

        [DllImport("Everything64.dll", EntryPoint = "Everything_SetMatchPath")]
        public static extern void SetMatchPath(bool bEnable);

        [DllImport("Everything64.dll", EntryPoint = "Everything_SetMatchCase")]
        public static extern void SetMatchCase(bool bEnable);

        [DllImport("Everything64.dll", EntryPoint = "Everything_SetMatchWholeWord")]
        public static extern void SetMatchWholeWord(bool bEnable);

        [DllImport("Everything64.dll", EntryPoint = "Everything_SetRegex")]
        public static extern void SetRegex(bool bEnable);

        [DllImport("Everything64.dll", EntryPoint = "Everything_SetMax")]
        public static extern void SetMax(UInt32 dwMax);

        [DllImport("Everything64.dll", EntryPoint = "Everything_SetOffset")]
        public static extern void SetOffset(UInt32 dwOffset);


        [DllImport("Everything64.dll", EntryPoint = "Everything_GetMatchPath")]
        public static extern bool GetMatchPath();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetMatchCase")]
        public static extern bool GetMatchCase();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetMatchWholeWord")]
        public static extern bool GetMatchWholeWord();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetRegex")]
        public static extern bool GetRegex();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetMax")]
        public static extern UInt32 GetMax();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetOffset")]
        public static extern UInt32 GetOffset();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetSearch", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetSearch();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetLastError")]
        public static extern int GetLastError();


        [DllImport("Everything64.dll", EntryPoint = "Everything_Query", CharSet = CharSet.Unicode)]
        public static extern bool Query(bool bWait);


        [DllImport("Everything64.dll", EntryPoint = "Everything_SortResultsByPath")]
        public static extern void SortResultsByPath();


        [DllImport("Everything64.dll", EntryPoint = "Everything_GetNumFileResults")]
        public static extern UInt32 GetNumFileResults();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetNumFolderResults")]
        public static extern UInt32 GetNumFolderResults();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetNumResults")]
        public static extern UInt32 GetNumResults();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetTotFileResults")]
        public static extern UInt32 GetTotFileResults();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetTotFolderResults")]
        public static extern UInt32 GetTotFolderResults();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetTotResults")]
        public static extern UInt32 GetTotResults();

        [DllImport("Everything64.dll", EntryPoint = "Everything_IsVolumeResult")]
        public static extern bool IsVolumeResult(UInt32 nIndex);

        [DllImport("Everything64.dll", EntryPoint = "Everything_IsFolderResult")]
        public static extern bool IsFolderResult(UInt32 nIndex);

        [DllImport("Everything64.dll", EntryPoint = "Everything_IsFileResult")]
        public static extern bool IsFileResult(UInt32 nIndex);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultFullPathName", CharSet = CharSet.Unicode)]
        public static extern void GetResultFullPathName(UInt32 nIndex, StringBuilder lpString, UInt32 nMaxCount);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultPath", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetResultPath(UInt32 nIndex);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultFileName", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetResultFileName(UInt32 nIndex);


        [DllImport("Everything64.dll", EntryPoint = "Everything_Reset")]
        public static extern void Reset();

        [DllImport("Everything64.dll", EntryPoint = "Everything_CleanUp")]
        public static extern void CleanUp();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetMajorVersion")]
        public static extern UInt32 GetMajorVersion();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetMinorVersion")]
        public static extern UInt32 GetMinorVersion();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetRevision")]
        public static extern UInt32 GetRevision();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetBuildNumber")]
        public static extern UInt32 GetBuildNumber();

        [DllImport("Everything64.dll", EntryPoint = "Everything_Exit")]
        public static extern bool Exit();

        [DllImport("Everything64.dll", EntryPoint = "Everything_IsDBLoaded")]
        public static extern bool IsDBLoaded();

        [DllImport("Everything64.dll", EntryPoint = "Everything_IsAdmin")]
        public static extern bool IsAdmin();

        [DllImport("Everything64.dll", EntryPoint = "Everything_IsAppData")]
        public static extern bool IsAppData();

        [DllImport("Everything64.dll", EntryPoint = "Everything_RebuildDB")]
        public static extern bool RebuildDB();

        [DllImport("Everything64.dll", EntryPoint = "Everything_UpdateAllFolderIndexes")]
        public static extern bool UpdateAllFolderIndexes();

        [DllImport("Everything64.dll", EntryPoint = "Everything_SaveDB")]
        public static extern bool SaveDB();

        [DllImport("Everything64.dll", EntryPoint = "Everything_SaveRunHistory")]
        public static extern bool SaveRunHistory();

        [DllImport("Everything64.dll", EntryPoint = "Everything_DeleteRunHistory")]
        public static extern bool DeleteRunHistory();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetTargetMachine")]
        public static extern UInt32 GetTargetMachine();


        // Everything 1.4
        [DllImport("Everything64.dll", EntryPoint = "Everything_SetSort")]
        public static extern void SetSort(UInt32 dwSortType);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetSort")]
        public static extern UInt32 GetSort();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultListSort")]
        public static extern UInt32 GetResultListSort();

        [DllImport("Everything64.dll", EntryPoint = "Everything_SetRequestFlags")]
        public static extern void SetRequestFlags(UInt32 dwRequestFlags);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetRequestFlags")]
        public static extern UInt32 GetRequestFlags();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultListRequestFlags")]
        public static extern UInt32 GetResultListRequestFlags();

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultExtension", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetResultExtension(UInt32 nIndex);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultSize")]
        public static extern bool GetResultSize(UInt32 nIndex, out long lpFileSize);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultDateCreated")]
        public static extern bool GetResultDateCreated(UInt32 nIndex, out long lpFileTime);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultDateModified")]
        public static extern bool GetResultDateModified(UInt32 nIndex, out long lpFileTime);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultDateAccessed")]
        public static extern bool GetResultDateAccessed(UInt32 nIndex, out long lpFileTime);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultAttributes")]
        public static extern UInt32 GetResultAttributes(UInt32 nIndex);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultFileListFileName", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetResultFileListFileName(UInt32 nIndex);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultRunCount")]
        public static extern UInt32 GetResultRunCount(UInt32 nIndex);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultDateRun")]
        public static extern bool GetResultDateRun(UInt32 nIndex, out long lpFileTime);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultDateRecentlyChanged")]
        public static extern bool GetResultDateRecentlyChanged(UInt32 nIndex, out long lpFileTime);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultHighlightedFileName", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetResultHighlightedFileName(UInt32 nIndex);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultHighlightedPath", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetResultHighlightedPath(UInt32 nIndex);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetResultHighlightedFullPathAndFileName", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetResultHighlightedFullPathAndFileName(UInt32 nIndex);

        [DllImport("Everything64.dll", EntryPoint = "Everything_GetRunCountFromFileName")]
        public static extern UInt32 GetRunCountFromFileName(string lpFileName);

        [DllImport("Everything64.dll", EntryPoint = "Everything_SetRunCountFromFileName")]
        public static extern bool SetRunCountFromFileName(string lpFileName, UInt32 dwRunCount);

        [DllImport("Everything64.dll", EntryPoint = "Everything_IncRunCountFromFileName")]
        public static extern UInt32 IncRunCountFromFileName(string lpFileName);


        public const UInt32 MAX_RESULTS_ALL = 0xFFFFFFFF;
        public const int MAX_PATH_LENGTH = 32768;
    }
}