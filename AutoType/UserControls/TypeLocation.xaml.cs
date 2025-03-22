using AutoType.Classes;
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
		public TypeLocation() { }

		public TypeLocation(string place, FileTypes fileType, Configuration config, CroppedBitmap source, string note = null)
		{
			InitializeComponent();
			Tag = config.FrameMode;
			// устанавливаем размеры контроля под размеры обрезанного скриншота
			screen.Source = source;
			Height = screen.Source.Height;
			Width = screen.Source.Width;
			// режим старой рамки
			if (config?.FrameMode == FrameMode.Old)
			{
				txtPlace.Text = place;
				//
				gridOld.Visibility = Visibility.Visible;
				//
				if (fileType == FileTypes.Menu)
					gridPlace.Visibility = Visibility.Collapsed;
				//
				gridPlace.LayoutTransform = new ScaleTransform(config.Scale, config.Scale, 1167.5, 540);
				gridPlace.Margin = config.MarginForPlace;
				imgMenu.LayoutTransform = new ScaleTransform(config.Scale, config.Scale, 1167.5, 540);
				imgMenu.Margin = config.MarginForMenu;
			}
			// режим новой рамки
			else
			{
				gridNew.Visibility = Visibility.Visible;
				//
				txtPlaceNew.Text = place;
				gridNew.LayoutTransform = new ScaleTransform(config.Scale, config.Scale, imgPlaceNew.Width / 2, imgPlaceNew.Height / 2);
			}

			if (fileType == FileTypes.PlaceAndNote)
			{
				txtNote.Visibility = Visibility.Visible;
				txtNote.Text = note;
				txtNote.Width = (Width - 200) / config.Scale; // устанавливаем ширину примечание, чтобы не заползало на меню
				txtNote.LayoutTransform = new ScaleTransform(config.Scale, config.Scale, 0, 0);
			}

			DataContext = this;
		}
	}
}
