using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BluetoothManager.Class
{
    public class BooleanToVisProgBarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean)
            {
                return boolean ? Visibility.Visible : Visibility.Hidden; // Alterado de Collapsed para Hidden
            }
            return Visibility.Hidden; // Alterado de Collapsed para Hidden
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
