using Quicksearch.Config;
using Quicksearch.Everything;
using Quicksearch.Properties;
using Quicksearch.Util;
using Quicksearch.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Quicksearch
{
    public partial class App : Application
    {
        private HotKey OpenWindowHotkey, CloseApplicationHotkey;
        private MainVM SearchVM;
        private ConfigVM ConfigVM;
        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private ConfigWindow ConfigWindow;
        private Search SearchWindow;
        private const string SettingsPath = "quicksearch.settings";

        public UserSettings Settings { get; private set; }

        public App()
        {
            App.Current = this;
            OpenWindowHotkey = new HotKey(System.Windows.Input.Key.Y, KeyModifier.Win, OpenWindowHotkeyHandler);
            CloseApplicationHotkey = new HotKey(System.Windows.Input.Key.Y, KeyModifier.Win | KeyModifier.Shift, CloseApplicationHotkeyHandler);
            this.Exit += App_Exit;
            this.SessionEnding += App_SessionEnding;
        }

        public new static App Current { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            ApplicationInstanceWatcher.ShutdownRequest += () => this.Dispatcher.Invoke(() => this.Shutdown());
            ApplicationInstanceWatcher.CreatePipeAndShutdownOtherInstances();

            LoadConfig(e.Args);

            var culture = new CultureInfo(this.Settings.UICulture);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            base.OnStartup(e);

            this.SearchVM = new MainVM();
            this.ConfigVM = new ConfigVM();

            if (!this.Settings.Silent)
                OpenWindow();
            ShowTrayIcon();
        }

        private void LoadConfig(string[] args)
        {
            // If program runs on windows startup the CurrentDirectory is %windir%\system32
            // So we reset it to the directory that contains the executable
            Environment.CurrentDirectory = new FileInfo(typeof(App).Assembly.Location).DirectoryName;
            try
            {
                this.Settings = UserSettings.Load(SettingsPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                this.Settings = new UserSettings();
                SaveSettings();
            }

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-silent":
                        this.Settings.Silent = true;
                        break;
                    case "-culture":
                        if (args.Length >= i)
                            this.Settings.UICulture = args[i + 1];
                        break;
                    default:
                        break;
                }
            }
        }

        internal bool SaveSettings()
        {
            try
            {
                this.Settings.Save(SettingsPath);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        public void ShowTrayIcon()
        {
            if (NotifyIcon == null)
            {
                NotifyIcon = new System.Windows.Forms.NotifyIcon();
                NotifyIcon.Text = "Quicksearch Everything";
                NotifyIcon.Icon = Quicksearch.Properties.Resources.icon;
                NotifyIcon.MouseClick += (s, e) => { if (e.Button == System.Windows.Forms.MouseButtons.Left) OpenWindow(); };
                NotifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]
                {
                    new System.Windows.Forms.MenuItem("Open Window", (s, e) => OpenWindow()),
                    new System.Windows.Forms.MenuItem("Close Application", (s, e) => Shutdown())
                });
                NotifyIcon.ContextMenu.MenuItems[0].DefaultItem = true;
            }
            NotifyIcon.Visible = true;
        }

        private void CloseApplicationHotkeyHandler(HotKey obj)
        {
            Shutdown();
        }

        private void OpenWindowHotkeyHandler(HotKey obj)
        {
            OpenWindow();
        }

        private void OpenWindow()
        {
            if (this.SearchWindow == null)
            {
                this.SearchWindow = new Search(SearchVM);
                this.SearchWindow.Closing += (s, e) => this.SearchWindow = null;
            }

            if (!this.SearchWindow.IsVisible)
            {
                this.SearchWindow.Show();
            }

            if (this.SearchWindow.WindowState == WindowState.Minimized)
            {
                this.SearchWindow.WindowState = WindowState.Normal;
            }

            this.SearchWindow.Activate();
            this.SearchWindow.Topmost = true;
            this.SearchWindow.Topmost = false;
            this.SearchWindow.Focus();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            ApplicationInstanceWatcher.Exit();
            EverythingAPI.CleanUp();
            if (this.Settings.CloseEverythingOnExit)
                EverythingAPI.Exit();
            if (this.NotifyIcon != null)
            {
                this.NotifyIcon.Visible = false;
                this.NotifyIcon?.Dispose();
                this.NotifyIcon = null;
            }
            this.OpenWindowHotkey?.Dispose();
            this.OpenWindowHotkey = null;
            this.CloseApplicationHotkey?.Dispose();
            this.CloseApplicationHotkey = null;
        }

        private void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            if(e.ReasonSessionEnding == ReasonSessionEnding.Logoff
                || e.ReasonSessionEnding == ReasonSessionEnding.Shutdown)
            {
                // Cancel to give us more time
                e.Cancel = true;
                // Shutdown to avoid crash of Everything
                Shutdown();
            }
        }

        public void OpenSettings()
        {
            if (ConfigWindow == null)
            {
                this.ConfigWindow = new ConfigWindow();
                this.ConfigWindow.Closed += (s, e) => this.ConfigWindow = null;
                this.ConfigWindow.DataContext = this.ConfigVM;
                this.ConfigVM.CloseAction = () => this.ConfigWindow.Close();
            }
            else if (!ConfigWindow.IsVisible)
            {
                this.ConfigVM.AddSettings();
            }
            ConfigWindow.Show();
        }
    }
}
