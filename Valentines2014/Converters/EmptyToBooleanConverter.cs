using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Valentines2015.Converters
{
    public class EmptyToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            IEnumerable enumerable = value as IEnumerable;
            if (enumerable != null)
            {
                return enumerable.GetEnumerator().MoveNext();
            }

            long numVal = System.Convert.ToInt64(value);
            if (numVal == 0)
            {
                return false;
            }

            string strVal = value.ToString();
            if (strVal == String.Empty)
            {
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}