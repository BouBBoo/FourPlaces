using System;
using System.Globalization;
using TD2.Items;
using Xamarin.Forms;

namespace TD2.Converter
{
	public class SelectedItemEventArgsToSelectedItemConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var eventArgs = value as SelectedItemChangedEventArgs;
			return ((PlaceItemSummary) eventArgs.SelectedItem).Id;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
