using System;
using System.Globalization;
using Xamarin.Forms;

namespace TD2.Converter
{
    class IdToImageConverter : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
            {
                return "https://td-api.julienmialon.com/images/1";
            }
            else
            {
                int ImageID = (int)value;

                return "https://td-api.julienmialon.com/images/" + ImageID;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
