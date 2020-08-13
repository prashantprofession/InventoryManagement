using System;
using System.Globalization;
using System.Windows.Data;

namespace Gasware.Common
{
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, 
            System.Globalization.CultureInfo culture)
        {
            if (value is Boolean)
            {
                return ((Boolean)value) == true ? "Yes" : "No";
            }
            return "Other";
        }

        public object ConvertBack(object value, Type targetType, object parameter, 
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
