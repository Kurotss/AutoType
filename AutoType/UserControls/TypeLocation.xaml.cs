using AutoType.Classes;
using System.ComponentModel;
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
		public TypeLocation(string place, bool isPlace, Configuration config, FrameMode frameMode, BitmapImage source)
		{
			InitializeComponent();
			//
			Height = frameMode == FrameMode.Old ? source.Height : (source.Height > 1080 ? 1080 : source.Height);
			Width = frameMode == FrameMode.Old ? source.Width : (source.Width > 2280 ? 2280 : source.Width);
			screen.Source = source;
			if (frameMode == FrameMode.Old)
			{
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
				gridNew.Visibility = Visibility.Visible;
				//
				txtPlaceNew.Text = place;
			}
			DataContext = this;
		}
	}
}
