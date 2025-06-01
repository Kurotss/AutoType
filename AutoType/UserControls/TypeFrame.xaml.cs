using AutoType.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutoType.UserControls
{
	/// <summary>
	/// Логика окна с тайпом рамки диалога
	/// </summary>
	public partial class TypeFrame : UserControl
	{
		public TypeFrame() { }

		public TypeFrame(string characterName, string description, Configuration config, TypeScreen typeScreen, string[]? leftPlaceDescr = null, string? note = null)
		{
			InitializeComponent();
			Tag = config.FrameMode;
			// устанавливаем размеры контроля под размеры обрезанного скриншота
			screen.Source = typeScreen.CroppedImage;
			Height = screen.Source.Height;
			Width = screen.Source.Width;

			// получаем контроли из обеих версий рамки
			var gridMain = this.GetControlWithMode(Definitions.GRID) as Grid;
			var gridLeftPlace = this.GetControlWithMode(Definitions.GRID_LEFT_PLACE) as Grid;
			var imgFrame = this.GetControlWithMode(Definitions.IMG_FRAME) as Image;
			dynamic txtName = this.GetControlWithMode(Definitions.TXT_NAME);
			dynamic txtDescr = this.GetControlWithMode(Definitions.TXT_DESCR);
			var txtLeftPlace = this.GetControlWithMode(Definitions.TXT_LEFT_PLACE) as TextBox;
			var txtTube = this.GetControlWithMode(Definitions.TXT_TUBE) as TextBox;
			var txtSecondLeftPlace = this.GetControlWithMode(Definitions.TXT_SECOND_LEFT_PLACE) as TextBox;

			// проставляем папраметры наложения главному контейнеру
			gridMain.Visibility = Visibility.Visible;
			gridMain.LayoutTransform = new ScaleTransform(config.Scale, config.Scale, imgFrame.Width / 2, imgFrame.Height / 2);
			imgMenu.Margin = config.MarginForMenu;
			// если это репдика, то проставляем текст и параметры наложения
			if (typeScreen.IsDialog)
			{
				gridFrame.Visibility = Visibility.Visible;
				gridFrame.Margin = config.MarginForDialog;

				txtName.Text = characterName;
				if (characterName.Length > 16) // если длинное имя
				{
					txtName.FontSize = config.FrameMode == FrameMode.Old ? 46 : 43.3;
					txtName.Margin = config.FrameMode == FrameMode.Old ?
						new Thickness(txtName.Margin.Left, txtName.Margin.Top, txtName.Margin.Right, txtName.Margin.Bottom - 4) :
						new Thickness(0, 0, 1084.4, 275.4);
				}
				txtDescr.Text = description;
			}
			// если это левая рамка мест, то заполняем её и настраиваем
			if (typeScreen.IsLeftPlace)
			{
				gridLeftPlace.Visibility = Visibility.Visible;
				if (leftPlaceDescr is null || leftPlaceDescr.Length < 1)
					throw new Exception("Некорректная разметка для места слева.");
				txtLeftPlace.Text = leftPlaceDescr[0];
				// если составное место слева
				if (leftPlaceDescr.Length == 2)
				{
					txtTube.Visibility = Visibility.Visible;
					txtSecondLeftPlace.Text = leftPlaceDescr[1].TrimStart();
				}
				gridLeftPlace.LayoutTransform = new ScaleTransform(config.Scale, config.Scale, imgFrame.Width / 2, imgFrame.Height / 2);
				gridLeftPlace.Margin = config.MarginForLeftPlace;
			}
			// если есть примечание, то тайпим
			if (typeScreen.Type == FileTypes.Note)
			{
				txtNote.Visibility = Visibility.Visible;
				txtNote.Text = note;
				txtNote.Width = (Width - 200) / config.Scale; // устанавливаем ширину примечание, чтобы не заползало на меню
				txtNote.Height = Height; // устанавливаем высоту примечания размером со скрин
				txtNote.LayoutTransform = new ScaleTransform(config.Scale, config.Scale, 0, 0);
			}
			if (config.FrameMode == FrameMode.New && typeScreen.IsDialog)
			{
				var scaleWidthFrame = imgFrame.Width * config.Scale;
				if (screen.Source.Width > scaleWidthFrame)
				{
					// высчитываем масштабирование, чтобы растянуть рамку до краёв скриншота
					double horizontalScale = screen.Source.Width / imgFrame.Width;
					// растягиваем рамку до краёв скрина
					gridMain.LayoutTransform = new ScaleTransform(horizontalScale, horizontalScale, imgFrame.Width / 2, imgFrame.Height);
				}
			}
			DataContext = this;
		}
	}
}
