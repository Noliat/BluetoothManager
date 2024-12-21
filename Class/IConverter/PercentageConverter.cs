using System;
using System.Globalization;
using System.Windows.Data;

namespace BluetoothManager.Class.IConverter
{

    // Conversor para exibir o nível de bateria como porcentagem
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int batteryLevel)
            {
                return $"{batteryLevel}%";
            }
            return ""; // Valor padrão para quando a bateria não está disponível
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
