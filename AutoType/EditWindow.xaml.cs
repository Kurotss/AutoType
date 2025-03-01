using AutoType.Classes;
using AutoType.UserControls;
using System.Collections.Generic;
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

			Item = userControl;
			viewBox.Child = userControl;

			viewBox.Height = height;
			viewBox.Width = width;

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
					{
						EditableElemets.Add(new("Реплика", typeFrame.txtDescr));
						EditableElemets.Add(new("Имя", typeFrame.txtName));
					}

					if (typeFrame.gridLeftPlace.Visibility == Visibility.Visible)
						EditableElemets.Add(new("Левая рамка", typeFrame.txtLeftPlace));
				}

				// если новая рамка
				if (frameMode == FrameMode.New)
				{
					if (typeFrame.gridNew.Visibility == Visibility.Visible)
					{
						EditableElemets.Add(new("Реплика", typeFrame.txtDescrNew));
						EditableElemets.Add(new("Имя", typeFrame.txtNameNew));
					}

					if (typeFrame.gridLeftPlaceNew.Visibility == Visibility.Visible)
						EditableElemets.Add(new("Левая рамка", typeFrame.txtLeftPlaceNew));
				}
			}

			if (EditableElemets.Count > 0)
			{
				cmbFrames.Visibility = Visibility.Visible;
				SelectedElement = EditableElemets.FirstOrDefault();
			}

			DataContext = this;
		}

		#region Properties

		public UserControl Item { get; set; }

		/// <summary>
		/// Список элементов, которые можно отредактировать в скрине
		/// </summary>
		public List<KeyValuePair<string, FrameworkElement>> EditableElemets { get; set; } = new();

		/// <summary>
		/// Выбранный элемент для редактирования
		/// </summary>
		private KeyValuePair<string, FrameworkElement>? _selectedElement;
		public KeyValuePair<string, FrameworkElement>? SelectedElement
		{
			get => _selectedElement;
			set
			{
				// если элемент поменялся, то выставляем новый элемент на передний план
				if (value is not null)
				{
					Canvas.SetZIndex(value.Value.Value, 0);
					Canvas.SetZIndex(_selectedElement.Value.Value, 1);
					_selectedElement = value;
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
