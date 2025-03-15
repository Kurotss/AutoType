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

			ObservableCollection<KeyValuePair<string, FrameworkElement>> editableElements = new();
			bool isComplexLeftPlace = false;

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
						editableElements.Add(new("Реплика и имя", typeFrame.txtDescr));

					if (typeFrame.gridLeftPlace.Visibility == Visibility.Visible)
						editableElements.Add(new("Левая рамка", typeFrame.txtLeftPlace));
				}

				// если новая рамка
				if (frameMode == FrameMode.New)
				{
					if (typeFrame.gridNew.Visibility == Visibility.Visible)
						editableElements.Add(new("Реплика и имя", typeFrame.gridNew));

					if (typeFrame.gridLeftPlaceNew.Visibility == Visibility.Visible)
					{
						editableElements.Add(new("Левая рамка", typeFrame.gridLeftPlaceNew));
						isComplexLeftPlace = typeFrame.txtTube.Visibility == Visibility.Visible;
					}
				}
			}

			viewBox.Child = userControl;

			DataContext = new EditWindowModel(userControl, editableElements, isComplexLeftPlace);
		}

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
