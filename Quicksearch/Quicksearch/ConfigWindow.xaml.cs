using Quicksearch.Util;
using SourceChord.FluentWPF;

namespace Quicksearch
{
    /// <summary>
    /// Interaktionslogik für ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : AcrylicWindow
    {
        public ConfigWindow()
        {
            InitializeComponent();
            this.Icon = Quicksearch.Properties.Resources.icon.ToImageSource();
        }
    }
}
