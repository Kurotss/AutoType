using AutoType.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutoType.UserControls
{
	/// <summary>
	/// Логика окна с тайпом рамки диалога
	/// </summary>
	public partial class TypeFrame : UserControl
	{
		public TypeFrame(string characterName, string description, Configuration config, FrameMode frameMode, BitmapImage source, double? CroppedWidth, double? CroppedHeight,
			bool isDialog, bool isLeftPlace, string[]? leftPlaceDescr = null)
		{
			InitializeComponent();
			//
			Height = frameMode == FrameMode.Old ? source.Height : (source.Height > 1080 ? 1080 : source.Height);
			Width = frameMode == FrameMode.Old ? source.Width : (source.Width > 2280 ? 2280 : source.Width);
			if (frameMode == FrameMode.Old)
			{
				gridOld.Visibility = Visibility.Visible;
				if (CroppedWidth == null || CroppedHeight == null)
					screen.Source = source;
				else
					screen.Source = new CroppedBitmap(source, new Int32Rect((int)(source.Width - CroppedWidth) / 2, (int)(source.Height - CroppedHeight) / 2, (int)CroppedWidth, (int)CroppedHeight));
				if (isDialog)
				{
					gridFrame.Visibility = Visibility.Visible;
					//
					txtName.Text = characterName;
					if (characterName.Length > 16)
					{
						txtName.FontSize = 43;
						txtName.Margin = new Thickness(txtName.Margin.Left, txtName.Margin.Top, txtName.Margin.Right, txtName.Margin.Bottom - 6);
					}
					txtDescr.Text = description;
					gridFrame.RenderTransform = new ScaleTransform(config.Scale, config.Scale, 1167.5, 540);
					gridFrame.Margin = config.MarginForDialog;
				}
				//
				imgMenu.RenderTransform = new ScaleTransform(config.Scale, config.Scale, 1167.5, 540);
				imgMenu.Margin = config.MarginForMenu;
				//
				if (isLeftPlace)
				{
					gridLeftPlace.Visibility = Visibility.Visible;
					if (leftPlaceDescr is null || leftPlaceDescr.Length < 1)
						throw new Exception("Некорректная разметка для места слева.");
					else if (leftPlaceDescr.Length == 1)
						txtLeftPlace.Text = leftPlaceDescr[0];
					// если составное место слева
					else if (leftPlaceDescr.Length == 2)
					{
						txtLeftPlace.Text = leftPlaceDescr[0] + " " + leftPlaceDescr[1];
						spSecondLeftPlace.Visibility = Visibility.Visible;
						txtSecondLeftPlace.Text = leftPlaceDescr[0];
					}

					gridLeftPlace.RenderTransform = new ScaleTransform(config.Scale, config.Scale, 1167.5, 540);
					gridLeftPlace.Margin = config.MarginForLeftPlace;
				}
			}
			else if (frameMode == FrameMode.New)
			{
				double newWidth = source.Width > 2280 ? 2280 : source.Width;
				double newHeight = source.Height > 1080 ? 1080 : source.Height;
				screen.Source = new CroppedBitmap(source, new Int32Rect((int)(source.Width - newWidth) / 2, (int)(source.Height - newHeight) / 2, (int)newWidth, (int)newHeight));
				if (isDialog)
				{
					gridNew.Visibility = Visibility.Visible;
					// пробел - костыль, ибо слева обрезается обводка имени
					txtNameNew.Text = " " + characterName;
					if (characterName.Length > 16)

						txtNameNew.FontSize = 43.3;
					txtDescrNew.Text = description;
				}
			}
			DataContext = this;
		}
	}
}
