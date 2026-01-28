using kando_desktop.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kando_desktop.Converters
{
    public class BusyToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isBusy = value is bool b && b;
            string context = parameter as string ?? "Login"; 

            if (isBusy)
            {
                return context == "Register"
                    ? AppResources.Registering 
                    : AppResources.LoggingIn; 
            }

            return context == "Register"
                ? AppResources.CreateAccount 
                : AppResources.SignIn;       
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
