using Quicksearch.Config;
using Quicksearch.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quicksearch.ViewModel
{
    public class ConfigVM : BaseVM
    {
        public List<Setting> Settings { get; }

        public RelayCommand ConfirmCommand { get; }
        public RelayCommand CancelCommand { get; }

        public Action CloseAction { private get; set; }

        public ConfigVM()
        {
            this.Settings = new List<Setting>();
            AddSettings();

            this.ConfirmCommand = new RelayCommand(ConfirmAndSave, SomethingChanged);
            this.CancelCommand = new RelayCommand(Cancel);
        }

        internal void AddSettings()
        {
            this.Settings.Clear();
            this.Settings.Add(new CloseBehaviorSetting("Close Behavior", () => App.Current.Settings.CloseBehavior, cb => App.Current.Settings.CloseBehavior = cb));
            this.Settings.Add(new CultureSetting("Display Culture", () => App.Current.Settings.UICulture, c => App.Current.Settings.UICulture = c));
            this.Settings.Add(new AutorunSetting("Start with Windows", () => App.Current.Settings.Autostart, a => App.Current.Settings.SetAutostart(a)));
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

    public class CloseBehaviorSetting : Setting
    {
        public override SettingType Type => SettingType.CloseBehavior;

        private CloseBehavior _Value;
        public CloseBehavior Value
        {
            get => _Value;
            set
            {
                _Value = value;
                RaisePropertyChanged();
            }
        }
        public CloseBehavior[] Values { get; }

        private Func<CloseBehavior> GetValue { get; }
        private Action<CloseBehavior> SetValue { get; }

        public CloseBehaviorSetting(string name, Func<CloseBehavior> getValue, Action<CloseBehavior> setValue) : base(name)
        {
            this.GetValue = getValue;
            this.SetValue = setValue;
            this.Values = Enum.GetValues(typeof(CloseBehavior)) as CloseBehavior[];
            this.Reset();
        }

        public override void Apply()
        {
            this.SetValue(this.Value);
            this.Reset();
        }

        public override bool HasChanged()
        {
            return this.GetValue() != this.Value;
        }

        public override void Reset()
        {
            this.Value = this.GetValue();
        }
    }

    public class CultureSetting : Setting
    {

        public override SettingType Type => SettingType.CultureInfo;

        private string _Value;
        public string Value
        {
            get => _Value;
            set
            {
                _Value = value;
                RaisePropertyChanged();
            }
        }
        public string[] Values { get; }

        private Func<string> GetValue { get; }
        private Action<string> SetValue { get; }

        public CultureSetting(string name, Func<string> getValue, Action<string> setValue) : base(name)
        {
            this.GetValue = getValue;
            this.SetValue = setValue;
            this.Values = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(c => c.Name).ToArray();
            this.Reset();
        }

        public override void Apply()
        {
            this.SetValue(this.Value);
            this.Reset();
        }

        public override bool HasChanged()
        {
            return this.GetValue() != this.Value;
        }

        public override void Reset()
        {
            this.Value = this.GetValue();
        }
    }

    public class AutorunSetting : Setting
    {
        public override SettingType Type => SettingType.Autorun;

        private bool _Value;
        public bool Value
        {
            get => _Value;
            set
            {
                _Value = value;
                RaisePropertyChanged();
            }
        }

        private Func<bool> GetValue { get; }
        private Action<bool> SetValue { get; }
        public AutorunSetting(string name, Func<bool> getValue, Action<bool> setValue) : base(name)
        {
            this.GetValue = getValue;
            this.SetValue = setValue;
            this.Reset();
        }


        public override void Apply()
        {
            this.SetValue(this.Value);
            this.Reset();
        }

        public override bool HasChanged()
        {
            return this.GetValue() ^ this.Value;
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
        Autorun
    }
}
