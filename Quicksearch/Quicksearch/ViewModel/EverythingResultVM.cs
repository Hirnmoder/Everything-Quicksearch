using Quicksearch.Everything;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksearch.ViewModel
{
    public class EverythingResultVM : BaseVM
    {
        public EverythingResult Model { get; }

        public DateTime DateAccessed => Model.DateAccessed;
        public DateTime DateCreated => Model.DateCreated;
        public DateTime DateModified => Model.DateModified;
        public string Extension => Model.Extension;
        public string HighlightedPath => Model.HighlightedPath;
        public EverythingResultType Type => Model.Type;
        public long Size => Model.Size;
        public string Path => Model.Path;

        private bool _IsSelected;
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                _IsSelected = value;
                RaisePropertyChanged();
            }
        }

        public EverythingResultVM(EverythingResult everythingResult)
        {
            this.Model = everythingResult;
        }

        internal void Start()
        {
            try
            {
                // Start via explorer as explorer will handle if no application can edit / view the specified file
                var psi = new ProcessStartInfo("explorer.exe")
                {
                    Arguments = $"\"{this.Path}\"",
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
