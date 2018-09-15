using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LauncherMvvmLight.Domain.Utils.Convertors
{
    public class StringToIntConvertor: IValueConverter {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int i = 0;

            string s = value as string;
            if (s != null)
                int.TryParse(s, out i);

            return i;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
             throw new NotImplementedException();
        }
    }
}