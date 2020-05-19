using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Quicksearch.Util
{
    public class FilesizeConverter : IValueConverter
    {
        private static readonly string[] Units =
        {
            "B",
            "KB",
            "MB",
            "GB",
            "TB"
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is long l)
            {
                double d = l;
                int unitIdx = 0;
                while(d > 1024 && unitIdx < Units.Length)
                {
                    d /= 1024.0;
                    unitIdx++;
                }
                return $"{d:n2} {Units[unitIdx]}";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
