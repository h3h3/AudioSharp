// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyConverter.cs" company="AudioSharp">
//     Copyright (c) AudioSharp. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace AudioSharp.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Input;

    [ValueConversion(typeof(Key), typeof(string))]
    public class KeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return Binding.DoNothing;
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Key))
            {
                return Binding.DoNothing;
            }

            return Enum.Parse(typeof(Key), value.ToString());
        }
    }
}