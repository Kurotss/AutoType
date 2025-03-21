﻿using AutoType.Classes;
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
		public TypeLocation(string place, bool isPlace, Configuration config, BitmapImage source, double? CroppedWidth, double? CroppedHeight)
		{
			InitializeComponent();
			// если не указаны параметры по обрезке скринов, то устанавливаем исходные скрины
			if (CroppedWidth == null || CroppedHeight == null)
				screen.Source = source;
			else
				screen.Source = new CroppedBitmap(source, new Int32Rect((int)(source.Width - CroppedWidth) / 2, (int)(source.Height - CroppedHeight) / 2, (int)CroppedWidth, (int)CroppedHeight));
			// устанавливаем размеры контроля под размеры обрезанного скриншота
			Height = screen.Source.Height;
			Width = screen.Source.Width;
			// режим старой рамки
			if (config?.FrameMode == FrameMode.Old)
			{
				txtPlace.Text = place;
				//
				gridOld.Visibility = Visibility.Visible;
				//
				if (!isPlace)
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
			DataContext = this;
		}
	}
}
