﻿using AutoType.Classes;
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
			// устанавливаем размеры контроля под размеры необрезанного скриншота
			Height = source.Height;
			Width = source.Width;
			// если не указаны параметры по обрезке скринов, то устанавливаем исходные скрины
			if (CroppedWidth == null || CroppedHeight == null)
				screen.Source = source;
			// иначе обрезаем скрины и устанавливаем их
			else
				screen.Source = new CroppedBitmap(source, new Int32Rect((int)(source.Width - CroppedWidth) / 2, (int)(source.Height - CroppedHeight) / 2, (int)CroppedWidth, (int)CroppedHeight));
			// режим старой рамки
			if (frameMode == FrameMode.Old)
			{
				gridOld.Visibility = Visibility.Visible;
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
					gridFrame.RenderTransform = new ScaleTransform(config.Scale, config.Scale, imgFrameOld.Width / 2, imgFrameOld.Height / 2);
					gridFrame.Margin = config.MarginForDialog;
				}
				//
				imgMenu.RenderTransform = new ScaleTransform(config.Scale, config.Scale, imgFrameOld.Width / 2, imgFrameOld.Height / 2);
				imgMenu.Margin = config.MarginForMenu;
				// логика по тексту в левой рамке
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
			// режим новой рамки
			else if (frameMode == FrameMode.New)
			{
				// высчитываем масштабирование, чтобы растянуть рамку до краёв скриншота
				double horizontalScale = screen.Source.Width / imgFrameNew.Width;
				if (isDialog)
				{
					gridNew.Visibility = Visibility.Visible;
					// растягиваем рамку до краёв скрина
					gridNew.RenderTransform = new ScaleTransform(horizontalScale, horizontalScale, imgFrameNew.Width / 2, imgFrameNew.Height);
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
