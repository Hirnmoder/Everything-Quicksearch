using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksearch.Everything
{
    internal enum Sort : uint
    {
        NAME_ASCENDING = 1,
        NAME_DESCENDING = 2,
        PATH_ASCENDING = 3,
        PATH_DESCENDING = 4,
        SIZE_ASCENDING = 5,
        SIZE_DESCENDING = 6,
        EXTENSION_ASCENDING = 7,
        EXTENSION_DESCENDING = 8,
        TYPE_NAME_ASCENDING = 9,
        TYPE_NAME_DESCENDING = 10,
        DATE_CREATED_ASCENDING = 11,
        DATE_CREATED_DESCENDING = 12,
        DATE_MODIFIED_ASCENDING = 13,
        DATE_MODIFIED_DESCENDING = 14,
        ATTRIBUTES_ASCENDING = 15,
        ATTRIBUTES_DESCENDING = 16,
        FILE_LIST_FILENAME_ASCENDING = 17,
        FILE_LIST_FILENAME_DESCENDING = 18,
        RUN_COUNT_ASCENDING = 19,
        RUN_COUNT_DESCENDING = 20,
        DATE_RECENTLY_CHANGED_ASCENDING = 21,
        DATE_RECENTLY_CHANGED_DESCENDING = 22,
        DATE_ACCESSED_ASCENDING = 23,
        DATE_ACCESSED_DESCENDING = 24,
        DATE_RUN_ASCENDING = 25,
        DATE_RUN_DESCENDING = 26,
    }
}
