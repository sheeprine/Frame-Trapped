﻿namespace FrameTrapped.Common.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;

    public class BoolToOpacityConverter : IValueConverter
    {
        #region IValueConverter Member

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? b = (bool?)value;

            if (b.GetValueOrDefault(false))
            {
                return (double) 1.0;
            }
            else
            {
                return (double)0.25;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}