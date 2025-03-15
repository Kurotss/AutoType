using AutoType.UserControls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AutoType.Classes
{
	/// <summary>
	/// Вью модель для окна редактирования
	/// </summary>
	public class EditWindowModel : BaseDataContext
	{
		public EditWindowModel(UserControl userControl, ObservableCollection<KeyValuePair<string, FrameworkElement>> editableElements,
			bool isComplexLeftPlace)
		{
			UserControl = userControl;
			OnPropertyChanged(nameof(UserControl));
			EditableElements = editableElements;
			OnPropertyChanged(nameof(EditableElements));
			if (EditableElements.Any())
				SelectedControl = editableElements.FirstOrDefault();
			IsComplexLeftPlace = isComplexLeftPlace;
		}

		#region Properties

		/// <summary>
		/// Юзер контроль с содержимым скрина с тайпом
		/// </summary>
		public UserControl UserControl { get; set; }

		/// <summary>
		/// Список элементов, которые можно отредактировать в скрине
		/// </summary>
		public ObservableCollection<KeyValuePair<string, FrameworkElement>> EditableElements { get; set; } = new();

		/// <summary>
		/// Выбранный элемент для редактирования
		/// </summary>
		private KeyValuePair<string, FrameworkElement>? _selectedControl;
		public KeyValuePair<string, FrameworkElement>? SelectedControl
		{
			get => _selectedControl;
			set
			{
				// если элемент поменялся, то выставляем новый элемент на передний план
				if (value is not null)
				{
					Panel.SetZIndex(value.Value.Value, 1);
					if (_selectedControl is not null)
						Panel.SetZIndex(_selectedControl.Value.Value, 0);
					_selectedControl = value;
					IsLeftPlace = _selectedControl.Value.Key == "Левая рамка";
					OnPropertyChanged(nameof(SelectedControl));
				}
			}
		}

		private bool _isLeftPlace;

		public bool IsLeftPlace
		{
			get => _isLeftPlace;
			set
			{
				_isLeftPlace = value;
				OnPropertyChanged(nameof(IsLeftPlace));
			}
		}


		/// <summary>
		/// Составная ли это рамка левого места
		/// </summary>
		private bool _isComplexLeftPlace;
		public bool IsComplexLeftPlace
		{
			get => _isComplexLeftPlace;
			set
			{
				_isComplexLeftPlace = value;
				if (UserControl is TypeFrame typeFrame)
				{
					typeFrame.txtTube.Visibility = IsComplexLeftPlace ? Visibility.Visible : Visibility.Collapsed;
					typeFrame.txtSecondLeftPlaceNew.Visibility = IsComplexLeftPlace ? Visibility.Visible : Visibility.Collapsed;
					if (typeFrame.txtSecondLeftPlaceNew.Visibility == Visibility.Collapsed)
						typeFrame.txtSecondLeftPlaceNew.Text = "Часть 2";
				}
				OnPropertyChanged(nameof(IsComplexLeftPlace));
			}
		}

		#endregion
	}
}
