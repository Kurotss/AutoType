using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AutoType.Classes
{
	public class ObjectToVisibilityConverter : IValueConverter
	{
		#region NotNullValue

		private Visibility _notNullValue = Visibility.Visible;

		/// <summary>
		/// что возвращается, если поступает объект со значение null
		/// </summary>
		public Visibility NotNullValue
		{
			get => _notNullValue;
			set => _notNullValue = value;
		}

		#endregion

		#region NullValue

		private Visibility _nullValue = Visibility.Collapsed;

		/// <summary>
		/// что возвращается, если поступает объект с ненулевым значением
		/// </summary>
		public Visibility NullValue
		{
			get => _nullValue;
			set => _nullValue = value;
		}
		#endregion

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
				return NotNullValue;
			else
				return NullValue;

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DependencyProperty.UnsetValue;
		}
	}
}
