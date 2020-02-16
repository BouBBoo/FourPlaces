using System;
using System.Globalization;
using TD2.Items;
using Xamarin.Forms;

namespace TD2.Converter
{
    class UserItemToString : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UserItem user = (UserItem)value;

            return user.FirstName + " " + user.LastName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
