﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AutoType.Classes
{
	public class StringToDoubleConverter : IValueConverter
	{
		public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

		public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || string.IsNullOrEmpty(value.ToString()))
				return null;

			return value;
		}
	}
}
