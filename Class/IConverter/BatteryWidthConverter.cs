using System;
using System.Globalization;
using System.Windows.Data;

namespace BluetoothManager.Class.IConverter
{
    public class BatteryWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int batteryLevel = (int)value;
            double maxWidth = 100; // Tamanho máximo do retângulo
            return (batteryLevel / 100.0) * maxWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
