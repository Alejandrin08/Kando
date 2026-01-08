using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace kando_desktop.Converters
{
    public class IndexToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is View view && view.BindingContext is Models.Member member)
            {
                Element parent = view.Parent;
                while (parent != null)
                {
                    var itemsSource = BindableLayout.GetItemsSource(parent as BindableObject);
                    if (itemsSource != null)
                    {
                        var list = itemsSource as IList<Models.Member> ?? itemsSource.Cast<Models.Member>().ToList();
                        int index = list.IndexOf(member);

                        if (index == 0)
                            return new Thickness(20, 0, 0, 0);
                        else
                            return new Thickness(0);
                    }
                    parent = parent.Parent;
                }
            }
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
