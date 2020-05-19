using Quicksearch.Everything;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Quicksearch.Util
{
    public class IconConverter : IValueConverter
    {
        private ImageSource FolderImageSource;
        private Dictionary<string, ImageSource> Cache;
        private object CacheLock;

        public IconConverter()
        {
            this.FolderImageSource = Quicksearch.Properties.Resources.folder.ToImageSource();
            this.CacheLock = new object();
            this.Cache = new Dictionary<string, ImageSource>();
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path)
            {
                try
                {
                    if (Cache.ContainsKey(path))
                    {
                        lock (CacheLock)
                        {
                            if (Cache.ContainsKey(path))
                                return Cache[path];
                        }
                    }
                    var attr = File.GetAttributes(path);
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        AddToCache(path, FolderImageSource);
                        return FolderImageSource;
                    }
                    else
                    {
                        using (var icon = System.Drawing.Icon.ExtractAssociatedIcon(path))
                        {
                            var img = icon.ToImageSource();
                            AddToCache(path, img);
                            return img;
                        }
                    }
                }
                catch(FileNotFoundException) { }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
            return null;

            void AddToCache(string p, ImageSource img)
            {
                if (!Cache.ContainsKey(p))
                {
                    lock (CacheLock)
                    {
                        if (!Cache.ContainsKey(p))
                        {
                            Cache.Add(p, img);
                        }
                    }
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class IconHelper
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource ToImageSource(this System.Drawing.Icon icon)
        {

            var b = icon.ToBitmap();
            IntPtr hb = b.GetHbitmap();

            var img = Imaging.CreateBitmapSourceFromHBitmap(hb, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hb))
                throw new Win32Exception();

            return img;
        }
    }
}
