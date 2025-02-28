using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
			if (userControl.Height > SystemParameters.PrimaryScreenHeight)
			{
				height = SystemParameters.PrimaryScreenHeight * 0.9;
				width = userControl.Width * (SystemParameters.PrimaryScreenHeight * 0.9 / userControl.Height);
			}

			if (width > SystemParameters.PrimaryScreenWidth)
			{
				width = SystemParameters.PrimaryScreenWidth * 0.9;
				height = height * (SystemParameters.PrimaryScreenWidth * 0.9 / width);
			}

			Item = userControl;
			viewBox.Child = userControl;

			viewBox.Height = height;
			viewBox.Width = width;

			DataContext = this;
		}

		#region Properties

		public UserControl Item { get; set; }

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
