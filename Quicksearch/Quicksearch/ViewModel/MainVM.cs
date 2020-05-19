using Quicksearch.Everything;
using Quicksearch.Util;
using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Quicksearch.ViewModel
{
    public class MainVM : BaseVM
    {
        private bool _Ready;
        public bool Ready
        {
            get
            {
                _Ready = EverythingAPI.IsDBLoaded();
                if (!_Ready)
                {
                    var e = (Status)EverythingAPI.GetLastError();
                    switch (e)
                    {
                        case Status.OK:
                            this.Error = ErrorMessages.LoadingDB;
                            break;
                        case Status.ERROR_IPC:
                            this.Error = ErrorMessages.IPC;
                            break;
                        default:
                            this.Error = ErrorMessages.UnknownError;
                            break;
                    }
                }
                else
                {
                    this.Error = string.Empty;
                }

                return _Ready;
            }
        }

        private string _Error;
        public string Error
        {
            get => _Error;
            set
            {
                this._Error = value;
                RaisePropertyChanged();
            }
        }

        private uint _ResultCount;
        public uint ResultCount
        {
            get => _ResultCount;
            private set
            {
                _ResultCount = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ResultCounts));
            }
        }

        public string ResultCounts => $"Total results: {ResultCount}";

        public RelayCommand<string> Query { get; }
        public RelayCommand OpenSettingsCommand { get; }

        public ObservableCollection<EverythingResultVM> Results { get; }

        private long _SelectedResultIndex;
        public long SelectedResultIndex
        {
            get => _SelectedResultIndex;
            set
            {
                _SelectedResultIndex = value;
                for (int i = 0; i < this.Results.Count; i++)
                    this.Results[i].IsSelected = this.Results[i].Model.Index == _SelectedResultIndex;
            }
        }

        private const int VisibleResultsCount = 10;
        private uint _ResultOffset = 0;
        public uint ResultOffset
        {
            get => _ResultOffset;
            set
            {
                _ResultOffset = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ResultOffsets));
            }
        }
        public string ResultOffsets => $"Showing results {Math.Min(ResultOffset + 1, ResultCount)} to {Math.Min(ResultOffset + VisibleResultsCount, ResultCount)}";

        private string LastQuery = "";

        private DispatcherTimer UpdateReadyDispatcherTimer { get; }
        private Workhorse QueryWorkhorse { get; }

        public MainVM()
        {
            this.Results = new ObservableCollection<EverythingResultVM>();
            this.Query = new RelayCommand<string>(EnqueueQuery, CanDoQuery);
            this.OpenSettingsCommand = new RelayCommand(App.Current.OpenSettings);
            this.UpdateReadyDispatcherTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Background, (s, e) => RaisePropertyChanged(nameof(Ready)), App.Current.Dispatcher);
            this.UpdateReadyDispatcherTimer.Start();
            this.QueryWorkhorse = new Workhorse();
        }

        internal void SelectNext()
        {
            if (this.Results.Count > 0)
            {
                if (this.Results[this.Results.Count - 1].Model.Index < this.SelectedResultIndex + 1)
                {
                    if (ResultOffset + VisibleResultsCount < ResultCount)
                    {
                        DoQuery(LastQuery, VisibleResultsCount, ResultOffset + VisibleResultsCount, 0);
                    }
                }
                else
                {
                    this.SelectedResultIndex++;
                }
            }
        }

        internal void SelectPrevious()
        {
            if (this.Results.Count > 0)
            {
                if (this.Results[0].Model.Index > this.SelectedResultIndex - 1)
                {
                    if (ResultOffset >= VisibleResultsCount)
                    {
                        DoQuery(LastQuery, VisibleResultsCount, ResultOffset - VisibleResultsCount, -2);
                    }
                }
                else
                {
                    this.SelectedResultIndex--;
                }
            }
        }

        internal void RunSelected()
        {
            foreach (var r in this.Results)
                if (r.IsSelected)
                    r.Start();
        }

        private void EnqueueQuery(string s)
        {
            this.QueryWorkhorse.Enqueue(() => DoQuery(s, VisibleResultsCount, 0));
        }

        private void EnqueueQuery(string s, uint resultOffset, long selectedIndex)
        {
            this.QueryWorkhorse.Enqueue(() => DoQuery(s, VisibleResultsCount, resultOffset, selectedIndex));
        }

        private void DoQuery(string s, uint visibleResultsCount, uint resultOffset, long selectedIndex = -1L)
        {
            System.Diagnostics.Debug.WriteLine(s + "   " + resultOffset);
            var data = Data.DATE_ACCESSED |
                       Data.DATE_CREATED |
                       Data.DATE_MODIFIED |
                       Data.EXTENSION |
                       Data.FULL_PATH_AND_FILE_NAME |
                       Data.HIGHLIGHTED_FULL_PATH_AND_FILE_NAME |
                       Data.SIZE;
            var q = new EverythingQuery(s, data, Sort.DATE_RECENTLY_CHANGED_DESCENDING, true, false, false, visibleResultsCount, resultOffset);
            if (q.Execute())
            {
                this.ResultCount = q.QueryStats.ResultCount;
                var results = new EverythingResults(q.QueryStats);
                App.Current.Dispatcher.Invoke(() =>
                {
                    this.Results.Clear();
                    foreach (var result in results)
                        this.Results.Add(new EverythingResultVM(result));
                    this.SelectedResultIndex = this.Results.Count > 0 ? (selectedIndex != -1L ? (selectedIndex == -2L ? this.Results[this.Results.Count - 1].Model.Index : selectedIndex) : this.Results[0].Model.Index) : -1L;
                });
                this.ResultOffset = resultOffset;
                this.LastQuery = s;
            }
            else
            {
                switch (q.ErrorCode)
                {
                    case Status.ERROR_MEMORY:
                        this.Error = ErrorMessages.Memory;
                        break;
                    case Status.ERROR_IPC:
                        this.Error = ErrorMessages.IPC;
                        break;
                    case Status.ERROR_REGISTERCLASSEX:
                        this.Error = ErrorMessages.RegisterClass;
                        break;
                    case Status.ERROR_CREATEWINDOW:
                        this.Error = ErrorMessages.Window;
                        break;
                    case Status.ERROR_CREATETHREAD:
                        this.Error = ErrorMessages.Thread;
                        break;
                    case Status.ERROR_INVALIDCALL:
                        this.Error = ErrorMessages.InvalidCall;
                        break;
                    default:
                        this.Error = ErrorMessages.UnknownError;
                        break;
                }
            }
        }

        private bool CanDoQuery(string s)
        {
            return Ready;
        }
    }
}
