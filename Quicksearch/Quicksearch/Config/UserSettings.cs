using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Quicksearch.Config
{
    public class UserSettings
    {
        public const string ApplicationName = "Quicksearch Everything";
        private const string StartupRegKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public CloseBehavior CloseBehavior = CloseBehavior.CloseOnFocusLost;
        public string UICulture = CultureInfo.CurrentUICulture.Name;

        public bool CloseEverythingOnExit = false;


        private bool _Autostart;
        [XmlIgnore]
        public bool Autostart => _Autostart;

        [XmlIgnore]
        public bool Silent = false;

        public UserSettings()
        {
            this._Autostart = GetAutostart();
        }

        public bool GetAutostart()
        {
            try
            {
                var k = Registry.CurrentUser.OpenSubKey(StartupRegKey, false);
                if(k != null)
                {
                    return k.GetValue(ApplicationName) != null;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return false;
        }

        public void SetAutostart(bool autostart)
        {
            try
            {
                var k = Registry.CurrentUser.OpenSubKey(StartupRegKey, true);
                if(k != null)
                {
                    if (autostart)
                        k.SetValue(ApplicationName, $"\"{Application.ExecutablePath}\" -silent");
                    else
                        k.DeleteValue(ApplicationName, false);
                    this._Autostart = autostart;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public static UserSettings Load(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                var xml = new XmlSerializer(typeof(UserSettings));
                var us = xml.Deserialize(fs) as UserSettings;
                fs.Close();
                return us;
            }
        }

        public void Save(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                new XmlSerializer(typeof(UserSettings)).Serialize(fs, this);
                fs.Close();
            }
        }
    }
}
