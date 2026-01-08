using kando_desktop.Enums;
using kando_desktop.Resources.Strings;
using System.Globalization;

namespace kando_desktop.Converters
{
    public class TeamRoleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TeamRole role)
            {
                return role switch
                {
                    TeamRole.Owner => AppResources.Role_Owner,
                    TeamRole.Member => AppResources.Role_Member,
                    TeamRole.Pending => AppResources.Role_Pending,
                    _ => string.Empty
                };
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
