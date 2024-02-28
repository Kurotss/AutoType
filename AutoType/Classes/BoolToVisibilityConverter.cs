using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AutoType.Classes
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		#region TrueValue

		private Visibility _trueValue = Visibility.Visible;

		/// <summary>
		/// что возвращается, если поступает значение true
		/// </summary>
		public Visibility TrueValue
		{
			get => _trueValue;
			set => _trueValue = value;
		}

		#endregion

		#region FalseValue

		private Visibility _falseValue = Visibility.Collapsed;

		/// <summary>
		/// что возвращается, если поступает значение false
		/// </summary>
		public Visibility FalseValue
		{
			get => _falseValue;
			set => _falseValue = value;
		}
		#endregion

		public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool)
			{
				switch (value)
				{
					case true:
						return TrueValue;
					default:
						return FalseValue;
				}
			}
			else
				return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DependencyProperty.UnsetValue;
		}
	}
}
