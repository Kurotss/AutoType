using AutoType.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Configuration = AutoType.Classes.Configuration;

namespace AutoType.UserControls
{
	/// <summary>
	/// Логика окна с тайпом рамки места
	/// </summary>
	public partial class TypeLocation : UserControl
	{
		public TypeLocation(string place, bool isPlace, Configuration config, FrameMode frameMode, BitmapImage source, double? CroppedWidth, double? CroppedHeight)
		{
			InitializeComponent();
			//
			Height = frameMode == FrameMode.Old ? source.Height : (source.Height > 1080 ? 1080 : source.Height);
			Width = frameMode == FrameMode.Old ? source.Width : (source.Width > 2280 ? 2280 : source.Width);

			if (frameMode == FrameMode.Old)
			{
				if (CroppedWidth == null || CroppedHeight == null)
					screen.Source = source;
				else
					screen.Source = new CroppedBitmap(source, new Int32Rect((int)(source.Width - CroppedWidth) / 2, (int)(source.Height - CroppedHeight) / 2, (int)CroppedWidth, (int)CroppedHeight));
				txtPlace.Text = place;
				//
				gridOld.Visibility = Visibility.Visible;
				//
				if (!isPlace)
					gridPlace.Visibility = Visibility.Collapsed;
				//
				gridPlace.RenderTransform = new ScaleTransform(config.Scale, config.Scale, 1167.5, 540);
				gridPlace.Margin = config.MarginForPlace;
				imgMenu.RenderTransform = new ScaleTransform(config.Scale, config.Scale, 1167.5, 540);
				imgMenu.Margin = config.MarginForMenu;
			}
			else
			{
				double newWidth = source.Width > 2280 ? 2280 : source.Width;
				double newHeight = source.Height > 1080 ? 1080 : source.Height;
				screen.Source = new CroppedBitmap(source, new Int32Rect((int)(source.Width - newWidth) / 2, (int)(source.Height - newHeight) / 2, (int)newWidth, (int)newHeight));
				gridNew.Visibility = Visibility.Visible;
				//
				txtPlaceNew.Text = place;
			}
			DataContext = this;
		}
	}
}
