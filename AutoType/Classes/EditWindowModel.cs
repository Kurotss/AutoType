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
		public EditWindowModel(UserControl userControl, bool isLeftPlace,
			bool isComplexLeftPlace, FrameMode frameMode)
		{
			UserControl = userControl;
			OnPropertyChanged(nameof(UserControl));
			IsComplexLeftPlace = isComplexLeftPlace;
			IsLeftPlace = isLeftPlace;
			FrameMode = frameMode;
		}

		#region Properties

		/// <summary>
		/// Тип рамки: старая, новая
		/// </summary>
		public FrameMode FrameMode { get; set; }

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
				if (UserControl is TypeFrame typeFrame)
				{
					if (FrameMode == FrameMode.Old)
					{
						typeFrame.txtTubeOld.Visibility = IsComplexLeftPlace ? Visibility.Visible : Visibility.Collapsed;
						typeFrame.txtSecondLeftPlace.Visibility = IsComplexLeftPlace ? Visibility.Visible : Visibility.Collapsed;
						if (typeFrame.txtSecondLeftPlace.Visibility == Visibility.Collapsed)
							typeFrame.txtSecondLeftPlace.Text = "Часть 2";
					}
					if (FrameMode == FrameMode.New)
					{
						typeFrame.txtTube.Visibility = IsComplexLeftPlace ? Visibility.Visible : Visibility.Collapsed;
						typeFrame.txtSecondLeftPlaceNew.Visibility = IsComplexLeftPlace ? Visibility.Visible : Visibility.Collapsed;
						if (typeFrame.txtSecondLeftPlaceNew.Visibility == Visibility.Collapsed)
							typeFrame.txtSecondLeftPlaceNew.Text = "Часть 2";
					}
				}
				OnPropertyChanged(nameof(IsComplexLeftPlace));
			}
		}

		#endregion
	}
}
