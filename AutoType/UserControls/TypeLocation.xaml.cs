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

		public TypeLocation(string place, Configuration config, TypeScreen typeScreen, string note = null)
		{
			InitializeComponent();
			Tag = config.FrameMode;
			// устанавливаем размеры контроля под размеры обрезанного скриншота
			screen.Source = typeScreen.CroppedImage;
			Height = screen.Source.Height;
			Width = screen.Source.Width;

			var gridMain = this.GetControlWithMode(Definitions.GRID) as Grid;
			var txtPlace = this.GetControlWithMode(Definitions.TXT_PLACE) as TextBox;
			var imgPlace = this.GetControlWithMode(Definitions.IMG_PLACE) as Image;

			gridMain.Visibility = Visibility.Visible;
			gridMain.LayoutTransform = new ScaleTransform(config.Scale, config.Scale, imgPlace.Width / 2, imgPlace.Height / 2);
			txtPlace.Text = place;

			if (typeScreen.Type == FileTypes.Menu)
				gridPlace.Visibility = Visibility.Collapsed;

			gridPlace.Margin = config.MarginForPlace;
			imgMenu.Margin = config.MarginForMenu;

			if (typeScreen.Type == FileTypes.PlaceAndNote)
			{
				txtNote.Visibility = Visibility.Visible;
				txtNote.Text = note;
				txtNote.Width = (Width - 200) / config.Scale; // устанавливаем ширину примечание, чтобы не заползало на меню
				txtNote.Height = Height; // устанавливаем высоту примечания размером со скрин
				txtNote.LayoutTransform = new ScaleTransform(config.Scale, config.Scale, 0, 0);
			}

			DataContext = this;
		}
	}
}
