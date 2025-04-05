using AutoType.UserControls;
using System.Windows;
using System.Windows.Controls;

namespace AutoType.Classes
{
	/// <summary>
	/// Вью модель для окна редактирования
	/// </summary>
	public class EditWindowModel : BaseDataContext
	{
		public EditWindowModel(UserControl userControl)
		{
			UserControl = userControl;
			OnPropertyChanged(nameof(UserControl));

			var gridLeftPlace = UserControl.GetControlWithMode(Definitions.GRID_LEFT_PLACE) as Grid;
			var txtTube = UserControl.GetControlWithMode(Definitions.TXT_TUBE) as TextBox;

			IsLeftPlace = gridLeftPlace.Visibility == Visibility.Visible;
			IsComplexLeftPlace = txtTube.Visibility == Visibility.Visible;
		}

		#region Properties

		/// <summary>
		/// Юзер контроль с содержимым скрина с тайпом
		/// </summary>
		public UserControl UserControl { get; set; }

		/// <summary>
		///  Имеется ли место слева
		/// </summary>
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
				if (UserControl is TypeFrame)
				{
					var txtTube = UserControl.GetControlWithMode(Definitions.TXT_TUBE) as TextBox;
					var txtSecondLeftPlace = UserControl.GetControlWithMode(Definitions.TXT_SECOND_LEFT_PLACE) as TextBox;

					txtTube.Visibility = IsComplexLeftPlace ? Visibility.Visible : Visibility.Collapsed;
					txtSecondLeftPlace.Visibility = IsComplexLeftPlace ? Visibility.Visible : Visibility.Collapsed;
					if (txtSecondLeftPlace.Visibility == Visibility.Collapsed)
						txtSecondLeftPlace.Text = "Часть 2";
				}
				OnPropertyChanged(nameof(IsComplexLeftPlace));
			}
		}

		#endregion
	}
}
