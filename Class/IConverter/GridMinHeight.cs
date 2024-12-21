using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothManager.Class.IConverter
{
    public class GridMinHeight
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int gridHeight = (int)value;
            double minHeight = 280; // Tamanho mínimo da grade
            return Math.Max(gridHeight, minHeight);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
