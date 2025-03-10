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
			// если не указаны параметры по обрезке скринов, то устанавливаем исходные скрины
			if (CroppedWidth == null || CroppedHeight == null)
				screen.Source = source;
			// иначе обрезаем скрины и устанавливаем их
			else
				screen.Source = new CroppedBitmap(source, new Int32Rect((int)(source.Width - CroppedWidth) / 2, (int)(source.Height - CroppedHeight) / 2, (int)CroppedWidth, (int)CroppedHeight));
			// устанавливаем размеры контроля под размеры необрезанного скриншота
			Height = screen.Source.Height;
			Width = screen.Source.Width;
			// режим старой рамки
			if (frameMode == FrameMode.Old)
			{
				gridOld.Visibility = Visibility.Visible;
				// если скриншот с репликой
				if (isDialog)
				{
					gridFrame.Visibility = Visibility.Visible;
					//
					txtName.Text = characterName;
					if (characterName.Length > 16)
					{
						txtName.FontSize = 46;
						txtName.Margin = new Thickness(txtName.Margin.Left, txtName.Margin.Top, txtName.Margin.Right, txtName.Margin.Bottom - 4);
					}
					txtDescr.Text = description;
					gridFrame.RenderTransform = new ScaleTransform(config.Scale, config.Scale, imgFrameOld.Width / 2, imgFrameOld.Height / 2);
					gridFrame.Margin = config.MarginForDialog;
				}
				//
				imgMenu.RenderTransform = new ScaleTransform(config.Scale, config.Scale, imgFrameOld.Width / 2, imgFrameOld.Height / 2);
				imgMenu.Margin = config.MarginForMenu;
				// если скриншот с левой рамкой места
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

					gridLeftPlace.RenderTransform = new ScaleTransform(config.Scale, config.Scale, imgFrameOld.Width / 2, imgFrameOld.Height / 2);
					gridLeftPlace.Margin = config.MarginForLeftPlace;
				}
			}
			else if (frameMode == FrameMode.New)
			{
				if (isDialog)
				{
					gridNew.Visibility = Visibility.Visible;

					// пробел - костыль, ибо слева обрезается обводка имени
					txtNameNew.Text = " " + characterName;
					if (characterName.Length > 16)
					{
						txtNameNew.FontSize = 43.3;
						txtNameNew.Margin = new Thickness(0, 0, 1103.6, 275.4);

					}
					txtDescrNew.Text = description;

					var scaleWidthFrame = imgFrameNew.Width * config.Scale;
					// если ширина скриншота меньше рамки со скейлом, то надо её сначала обрезать
					if (screen.Source.Width < scaleWidthFrame)
					{
						var croppedHorizontalPart = (scaleWidthFrame - screen.Source.Width) / config.Scale;
						var scaleHeightFrame = imgFrameNew.Height * config.Scale;
						double croppedVerticalPart;
						// если высота скриншота меньше рамки со скейлом, то надо её и по высоте обрезать
						if (screen.Source.Height < scaleHeightFrame)
						{
							croppedVerticalPart = (scaleHeightFrame - screen.Source.Height) / config.Scale;
						}
						else
							croppedVerticalPart = 0;
						var image = new BitmapImage(new Uri("pack://application:,,,/resources/frame_new.png"));
						imgFrameNew.Source = new CroppedBitmap(image, new Int32Rect((int)croppedHorizontalPart / 2,
							(int)croppedVerticalPart, (int)(imgFrameNew.Width - croppedHorizontalPart),
							(int)(imgFrameNew.Height - croppedVerticalPart)));
						imgFrameNew.Width -= croppedHorizontalPart;
						imgFrameNew.Height -= croppedVerticalPart;
					}

					gridNew.RenderTransform = new ScaleTransform(config.Scale, config.Scale, imgFrameNew.Width / 2, imgFrameNew.Height);
					// если скриншот больше рамки, то рамку надо растянуть
					if (screen.Source.Width > scaleWidthFrame)
					{
						// высчитываем масштабирование, чтобы растянуть рамку до краёв скриншота
						double horizontalScale = screen.Source.Width / imgFrameNew.Width;
						// растягиваем рамку до краёв скрина
						gridNew.RenderTransform = new ScaleTransform(horizontalScale, horizontalScale, imgFrameNew.Width / 2, imgFrameNew.Height);
					}
				}

				if (isLeftPlace)
				{
					gridLeftPlaceNew.Visibility = Visibility.Visible;
					if (leftPlaceDescr is null || leftPlaceDescr.Length < 1)
						throw new Exception("Некорректная разметка для места слева.");
					txtLeftPlaceNew.Text = leftPlaceDescr[0];
					// если составное место слева
					if (leftPlaceDescr.Length == 2)
					{
						txtTube.Visibility = Visibility.Visible;
						txtSecondLeftPlaceNew.Text = leftPlaceDescr[1];
					}

					gridLeftPlaceNew.RenderTransform = new ScaleTransform(config.Scale, config.Scale, 0, 0);
				}
			}
			DataContext = this;
		}
	}
}
