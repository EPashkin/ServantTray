using System;
using System.Globalization;
using System.Windows.Data;

namespace ServantTray
{
    public class DataTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
          CultureInfo culture)
        {
            if (value == null)
                return null;
            return value.GetType().Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
          CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
