using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksearch.Everything
{
    [Flags]
    internal enum Data : uint
    {
        FILE_NAME = 0x00000001,
        PATH = 0x00000002,
        FULL_PATH_AND_FILE_NAME = 0x00000004,
        EXTENSION = 0x00000008,
        SIZE = 0x00000010,
        DATE_CREATED = 0x00000020,
        DATE_MODIFIED = 0x00000040,
        DATE_ACCESSED = 0x00000080,
        ATTRIBUTES = 0x00000100,
        FILE_LIST_FILE_NAME = 0x00000200,
        RUN_COUNT = 0x00000400,
        DATE_RUN = 0x00000800,
        DATE_RECENTLY_CHANGED = 0x00001000,
        HIGHLIGHTED_FILE_NAME = 0x00002000,
        HIGHLIGHTED_PATH = 0x00004000,
        HIGHLIGHTED_FULL_PATH_AND_FILE_NAME = 0x00008000,
    }
}
