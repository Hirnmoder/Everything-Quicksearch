using Quicksearch.Config;
using Quicksearch.Properties;
using Quicksearch.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quicksearch.ViewModel
{
    public class ConfigVM : BaseVM
    {
        public List<Setting> Settings { get; }

        public string LicenseNote { get; }

        public RelayCommand ConfirmCommand { get; }
        public RelayCommand CancelCommand { get; }

        public Action CloseAction { private get; set; }

        public ConfigVM()
        {
            this.LicenseNote = LoadLicense();

            this.Settings = new List<Setting>();
            AddSettings();

            this.ConfirmCommand = new RelayCommand(ConfirmAndSave, SomethingChanged);
            this.CancelCommand = new RelayCommand(Cancel);
        }

        private string LoadLicense()
        {
            try
            {
                var license = ResourceLoader.ReadResourceFile("LICENSE");
                var thirdparty = ResourceLoader.ReadResourceFile("ThirdPartyLicenseNote.txt");

                return license + "\n\n\n" + thirdparty;
            }
            catch
            {
                return "Unable to load license file. Check Github-Repository for license information.";
            }
        }

        internal void AddSettings()
        {
            this.Settings.Clear();
            this.Settings.Add(new MultiValueSetting<CloseBehavior>("Close Behavior",
                                                                   () => App.Current.Settings.CloseBehavior,
                                                                   cb => App.Current.Settings.CloseBehavior = cb,
                                                                   SettingType.CloseBehavior,
                                                                   Enum.GetValues(typeof(CloseBehavior)) as CloseBehavior[]));
            this.Settings.Add(new MultiValueSetting<string>("Display Culture",
                                                            () => App.Current.Settings.UICulture,
                                                            c => App.Current.Settings.UICulture = c,
                                                            SettingType.CultureInfo,
                                                            CultureInfo.GetCultures(CultureTypes.AllCultures).Select(c => c.Name).ToArray()));
            this.Settings.Add(new MultiValueSetting<bool>("Start with Windows",
                                                          () => App.Current.Settings.Autostart,
                                                          a => App.Current.Settings.SetAutostart(a),
                                                          SettingType.Autorun,
                                                          new bool[] { true, false }));
            this.Settings.Add(new MultiValueSetting<bool>("Close Everything with Quicksearch",
                                                          () => App.Current.Settings.CloseEverythingOnExit,
                                                          c => App.Current.Settings.CloseEverythingOnExit = c,
                                                          SettingType.CloseEverythingOnExit,
                                                          new bool[] { true, false }));
        }

        private void Cancel()
        {
            foreach (var s in this.Settings)
                s.Reset();
            CloseAction?.Invoke();
        }

        private bool SomethingChanged()
        {
            foreach (var s in this.Settings)
                if (s.HasChanged())
                    return true;
            return false;
        }

        private void ConfirmAndSave()
        {
            foreach (var s in this.Settings)
                s.Apply();
            App.Current.SaveSettings();
            CloseAction?.Invoke();
        }
    }

    public abstract class Setting : BaseVM
    {
        public abstract SettingType Type { get; }
        public string Name { get; }

        public Setting(string name)
        {
            this.Name = name;
        }

        public abstract void Apply();
        public abstract bool HasChanged();
        public abstract void Reset();
    }

    public class MultiValueSetting<T> : Setting
    {
        public override SettingType Type { get; }

        private T _Value;
        public T Value
        {
            get => _Value;
            set
            {
                _Value = value;
                RaisePropertyChanged();
            }
        }
        public T[] Values { get; }

        private Func<T> GetValue { get; }
        private Action<T> SetValue { get; }

        public MultiValueSetting(string name, Func<T> getValue, Action<T> setValue, SettingType type, T[] values) : base(name)
        {
            this.Type = type;
            this.GetValue = getValue;
            this.SetValue = setValue;
            this.Values = values;
            this.Reset();
        }

        public override void Apply()
        {
            this.SetValue(this.Value);
            this.Reset();
        }

        public override bool HasChanged()
        {
            return !this.GetValue().Equals(this.Value);
        }

        public override void Reset()
        {
            this.Value = this.GetValue();
        }
    }

    public enum SettingType
    {
        CloseBehavior,
        CultureInfo,
        Autorun,
        CloseEverythingOnExit
    }
}
