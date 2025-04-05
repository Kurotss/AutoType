using AutoType.Classes;
using System;
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
			viewBox.Child = userControl;
			DataContext = new EditWindowModel(userControl);
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
