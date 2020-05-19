using Quicksearch.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Quicksearch.Util
{
    public class SettingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CloseBehaviorTemplate { get; set; }
        public DataTemplate CultureTemplate { get; set; }
        public DataTemplate CheckboxTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            if(item is Setting s)
            {
                switch (s.Type)
                {
                    case SettingType.CloseBehavior:
                        return CloseBehaviorTemplate;
                    case SettingType.CultureInfo:
                        return CultureTemplate;
                    case SettingType.Autorun:
                        return CheckboxTemplate;
                    default:
                        break;
                }
            }
            return null;
        }
    }
}
