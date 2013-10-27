using System;
using System.Globalization;
using System.Windows.Data;

namespace DayzServerManager
{
    class InnerElementsScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double) value;
            
            return val >= 1 ? 1 : 1 / val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double) value;

            return val >= 1 ? 1 : 1/val;
        }
    }
}
