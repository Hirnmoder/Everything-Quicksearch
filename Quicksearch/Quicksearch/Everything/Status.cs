using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksearch.Everything
{
    internal enum Status
    {
        OK = 0,
        ERROR_MEMORY = 1,
        ERROR_IPC = 2,
        ERROR_REGISTERCLASSEX = 3,
        ERROR_CREATEWINDOW = 4,
        ERROR_CREATETHREAD = 5,
        ERROR_INVALIDINDEX = 6,
        ERROR_INVALIDCALL = 7,
    }

    internal class ErrorMessages
    {
        internal const string IPC = "Cannot communicate with Everything-service. Please make sure that Everything is started in the background!";
        internal const string LoadingDB = "Loading Database...";
        internal const string UnknownError = "Unknown Error";
        internal const string Memory = "Failed to allocate memory";
        internal const string Thread = "Failed to create thread";
        internal const string RegisterClass = "Failed to register search query window class";
        internal const string Window = "Failed to create search query window";
        internal const string InvalidCall = "Internal error resulted in invalid call";

    }
}
