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
			Item = userControl;
			DataContext = this;
		}

		#region Properties

		public UserControl Item { get; set; }

		/// <summary>
		/// Высота скриншота
		/// </summary>
		public double ImageHeight => Item.Height / 2;
		
		/// <summary>
		/// Ширина скриншота
		/// </summary>
		public double ImageWidth => Item.Width / 2;

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
