using AutoType.Classes;
using AutoType.UserControls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Window = System.Windows.Window;

namespace AutoType
{
	/// <summary>
	/// Логика окна с редактированием текста готового скриншота
	/// </summary>
	public partial class EditWindow : Window
	{
		public EditWindow(UserControl userControl)
		{
			InitializeComponent();

			// если это скрин с диалогом/местом слева/именем, то ему потребуется редактирование слоёв
			if (userControl is TypeFrame typeFrame)
			{
				FrameMode frameMode;
				// определяем тип рамки (старая, новая) по гриду
				if (typeFrame.gridOld.Visibility == Visibility.Visible)
					frameMode = FrameMode.Old;
				else
					frameMode = FrameMode.New;

				// наполняем комбобокс типами файлов для редактирования
				// если старая рамка
				if (frameMode == FrameMode.Old)
				{
					if (typeFrame.gridFrame.Visibility == Visibility.Visible)
						EditableElements.Add(new("Реплика и имя", typeFrame.txtDescr));

					if (typeFrame.gridLeftPlace.Visibility == Visibility.Visible)
						EditableElements.Add(new("Левая рамка", typeFrame.txtLeftPlace));
				}

				// если новая рамка
				if (frameMode == FrameMode.New)
				{
					if (typeFrame.gridNew.Visibility == Visibility.Visible)
						EditableElements.Add(new("Реплика и имя", typeFrame.gridNew));

					if (typeFrame.gridLeftPlaceNew.Visibility == Visibility.Visible)
					{
						EditableElements.Add(new("Левая рамка", typeFrame.gridLeftPlaceNew));
						IsComplexLeftPlace = typeFrame.txtTube.Visibility == Visibility.Visible;
					}
				}
			}

			if (EditableElements.Count > 0)
			{
				SelectedControl = EditableElements.FirstOrDefault();
				spFrames.Visibility = Visibility.Visible;
			}

			double width = userControl.Width, height = userControl.Height;
			// если скриншот больше ширины монитора, то уменьшаем
			if (userControl.Height > SystemParameters.PrimaryScreenHeight)
			{
				height = SystemParameters.PrimaryScreenHeight * 0.8;
				width = userControl.Width * (SystemParameters.PrimaryScreenHeight * 0.8 / userControl.Height);
			}

			// если скриншот больше высоты монитора, то уменьшаем
			if (width > SystemParameters.PrimaryScreenWidth)
			{
				width = SystemParameters.PrimaryScreenWidth * 0.8;
				height *= (SystemParameters.PrimaryScreenWidth * 0.8 / width);
			}

			viewBox.Child = userControl;

			viewBox.Height = height;
			viewBox.Width = width;

			DataContext = this;
		}

		#region Properties

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
					cbLeftPlace.Visibility = _selectedControl.Value.Key == "Левая рамка" ? Visibility.Visible: Visibility.Collapsed;
				}
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
				if (viewBox.Child is TypeFrame typeFrame)
				{
					typeFrame.txtTube.Visibility = IsComplexLeftPlace ? Visibility.Visible : Visibility.Collapsed;
					typeFrame.txtSecondLeftPlaceNew.Visibility = IsComplexLeftPlace ? Visibility.Visible : Visibility.Collapsed;
				}
			}
		}

		#endregion

		#region Methods

		public System.Action SaveAndClose;

		private void SaveAndCloseAction(object sender, RoutedEventArgs e)
		{
			SaveAndClose.Invoke();
		}

		#endregion

		private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			viewBox.Child = null;
		}
	}
}
