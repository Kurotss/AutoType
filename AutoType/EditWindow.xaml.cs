using AutoType.Classes;
using AutoType.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
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
			bool isComplexLeftPlace = false, isLeftPlace = false;
			FrameMode frameMode = FrameMode.Old;

			// если это скрин с диалогом/местом слева/именем, то ему потребуется редактирование слоёв
			if (userControl is TypeFrame typeFrame)
			{
				//var gridLeftPlace = typeFrame.FindName("gridLeftPlace") as Grid;
				//var gridLeftPlaceNew = typeFrame.FindName("gridLeftPlaceNew") as Grid;
				//var txtTubeOld = typeFrame.FindName("txtTubeOld") as TextBox;
				//var txtTube = typeFrame.FindName("txtTube") as TextBox;


				//if (gridLeftPlace.Visibility == Visibility.Visible ||
				//	gridLeftPlaceNew.Visibility == Visibility.Visible)
				//{
				//	isLeftPlace = true;
				//	if (gridLeftPlace.Visibility == Visibility.Visible)
				//	{
				//		frameMode = FrameMode.Old;
				//		isComplexLeftPlace = txtTubeOld.Visibility == Visibility.Visible;
				//	}
				//	if (gridLeftPlaceNew.Visibility == Visibility.Visible)
				//	{
				//		frameMode = FrameMode.New;
				//		isComplexLeftPlace = txtTube.Visibility == Visibility.Visible;
				//	}
				//}
			}

			viewBox.Child = userControl;

			DataContext = new EditWindowModel(userControl, false, false, frameMode);
		}

		#region Methods

		public Action SaveAndClose;

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
