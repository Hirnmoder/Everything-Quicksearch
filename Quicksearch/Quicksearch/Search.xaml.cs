using Quicksearch.Util;
using Quicksearch.ViewModel;
using SourceChord.FluentWPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quicksearch
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class Search : AcrylicWindow
    {
        private bool IsClosing = false;
        private double ScrolledValue = 0.0;

        private MainVM VM { get; }
        public Search(MainVM searchVM)
        {
            this.Loaded += Search_Loaded;
            InitializeComponent();
            this.Icon = Quicksearch.Properties.Resources.icon.ToImageSource();
            this.VM = searchVM;
            this.DataContext = searchVM;
        }

        private void Search_Loaded(object sender, RoutedEventArgs e)
        {
            // Keep Window centered but moved upwards (as of height is set to 500px)
            // When WindowStartupLocation remains "CenterScreen", its location will be adjusted after realizing that 500px are too much
            this.WindowStartupLocation = WindowStartupLocation.Manual;
        }

        private void Search_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Cancel:
                case Key.Escape:
                    this.Close();
                    break;
                case Key.Return:
                    e.Handled = true;
                    VM.RunSelected();
                    if (App.Current.Settings.CloseBehavior == Config.CloseBehavior.CloseOnRun)
                        this.Close();
                    break;
            }
        }

        private void Search_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.PageUp:
                case Key.Up:
                    e.Handled = true;
                    VM.SelectPrevious();
                    break;
                case Key.PageDown:
                case Key.Down:
                case Key.Tab:
                    e.Handled = true;
                    VM.SelectNext();
                    break;
            }
        }

        private void Search_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.ScrolledValue += e.Delta / 120.0;
            if (this.ScrolledValue >= 1.0)
            {
                this.ScrolledValue -= 1.0;
                VM.SelectPrevious();
            }
            else if (this.ScrolledValue <= -1.0)
            {
                this.ScrolledValue += 1.0;
                VM.SelectNext();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            IsClosing = true;
            base.OnClosing(e);
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) ||
                Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                App.Current.Shutdown();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (VM.Query.CanExecute(tbSearch.Text))
                VM.Query.Execute(tbSearch.Text);
        }

        private void border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (sender is Border b && b.DataContext is EverythingResultVM ervm)
                {
                    this.VM.SelectedResultIndex = ervm.Model.Index;
                    ervm.Start();
                    if (App.Current.Settings.CloseBehavior == Config.CloseBehavior.CloseOnRun)
                        this.Close();
                }
            }
        }

        private void Search_Deactivated(object sender, EventArgs e)
        {
            switch (App.Current.Settings.CloseBehavior)
            {
                case Config.CloseBehavior.CloseOnFocusLost:
                    if (!this.IsClosing)
                        this.Close();
                    break;
                case Config.CloseBehavior.NoAutomaticClose:
                case Config.CloseBehavior.CloseOnRun:
                    break;
            }
        }

    }
}
