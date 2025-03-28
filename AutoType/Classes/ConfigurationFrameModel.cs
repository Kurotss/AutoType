﻿using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AutoType.Classes
{
	public class ConfigurationFrameModel : BaseDataContext
	{
		public ConfigurationFrameModel(FileTypes fileType, CroppedBitmap image, Configuration? config, FrameMode frameMode)
		{
			ImageExample = image;
			FileType = fileType;
			Config = config;
			if (config != null)
			{
				if (config.MarginForMenu.Left == 0 && config.MarginForMenu.Right == 0)
					ValueLrMenu = 0;
				else
				{
					ValueLrMenu = (config.MarginForMenu.Left == 0) ? Convert.ToInt32(config.MarginForMenu.Right * -1) : Convert.ToInt32(config.MarginForMenu.Left);
				}
				if (config.MarginForMenu.Top == 0 && config.MarginForMenu.Bottom == 0)
					ValueTbMenu = 0;
				else
				{
					ValueTbMenu = (config.MarginForMenu.Bottom == 0) ? Convert.ToInt32(config.MarginForMenu.Top * -1) : Convert.ToInt32(config.MarginForMenu.Bottom);
				}
				Scale = config.Scale;
			}
			FrameMode = frameMode;
		}

		#region Properties

		#region ImageExample

		/// <summary>
		/// Картинка, по образцу которой делается настройка
		/// </summary>
		public BitmapSource ImageExample { get; set; }

		#endregion

		#region MarginForFrame

		/// <summary>
		/// Значения для настройки местоположения рамки/места
		/// </summary>

		#region ValueLrFrame

		private int _valueLrFrame;

		/// <summary>
		/// Левый и правый отступ для рамки
		/// </summary>
		public int ValueLrFrame
		{
			get => _valueLrFrame;
			set
			{
				_valueLrFrame = value;
				OnPropertyChanged(nameof(MarginForFrame));
			}
		}

		#endregion

		#region ValueTbFrame

		private int _valueTbFrame;

		/// <summary>
		/// Верхний и нижний отступ для рамки
		/// </summary>
		public int ValueTbFrame
		{
			get => _valueTbFrame;
			set
			{
				_valueTbFrame = value;
				OnPropertyChanged(nameof(MarginForFrame));
			}
		}

		#endregion

		/// <summary>
		/// Общий margin для всей рамки
		/// </summary>
		public Thickness MarginForFrame
		{
			get
			{
				int valueL = ValueLrFrame;
				int valueR = 0;
				int valueT = 0;
				int valueB = ValueTbFrame;
				return new Thickness(valueL, valueT, valueR, valueB);
			}
		}

		#endregion

		#region MarginForMenu

		#region ValueLrMenu

		private int _valueLrMenu;

		/// <summary>
		/// Левый и правый отступ для меню
		/// </summary>
		public int ValueLrMenu
		{
			get => _valueLrMenu;
			set
			{
				_valueLrMenu = value;
				OnPropertyChanged(nameof(MarginForMenu));
			}
		}

		#endregion

		#region ValueTbMenu

		private int _valueTbMenu;

		/// <summary>
		/// Верхний и нижний отступ для меню
		/// </summary>
		public int ValueTbMenu
		{
			get => _valueTbMenu;
			set
			{
				_valueTbMenu = value;
				OnPropertyChanged(nameof(MarginForMenu));
			}
		}

		#endregion

		/// <summary>
		/// Значения для настройки местоположения меню
		/// </summary>
		public Thickness MarginForMenu
		{
			get
			{
				int valueL = ValueLrMenu;
				int valueR = 0;
				int valueT = 0;
				int valueB = ValueTbMenu;
				return new Thickness(valueL, valueT, valueR, valueB);
			}
		}

		#endregion

		#region MarginForLeftPlace

		#region ValueLrLeftPlace

		private int _valueLrLeftPlace;

		/// <summary>
		/// Левый и правый отступ для левой рамки места
		/// </summary>
		public int ValueLrLeftPlace
		{
			get => _valueLrLeftPlace;
			set
			{
				_valueLrLeftPlace = value;
				OnPropertyChanged(nameof(MarginForLeftPlace));
			}
		}

		#endregion

		#region ValueTbLeftPlace

		private int _valueTbLeftPlace;

		/// <summary>
		/// Верхний и нижний отступ для левой рамки места
		/// </summary>
		public int ValueTbLeftPlace
		{
			get => _valueTbLeftPlace;
			set
			{
				_valueTbLeftPlace = value;
				OnPropertyChanged(nameof(MarginForLeftPlace));
			}
		}

		#endregion

		/// <summary>
		/// Значения для настройки местоположения левой рамки места
		/// </summary>
		public Thickness MarginForLeftPlace
		{
			get
			{
				int valueL = 0;
				int valueR = -ValueLrLeftPlace;
				int valueT = 0;
				int valueB = ValueTbLeftPlace;
				return new Thickness(valueL, valueT, valueR, valueB);
			}
		}

		#endregion

		#region Scale

		private double _scale = 1;

		/// <summary>
		/// Масштаб для всего
		/// </summary>
		public double Scale
		{
			get => _scale;
			set
			{
				_scale = value;
				OnPropertyChanged(nameof(Scale));
			}
		}

		#endregion

		#region Screen

		/// <summary>
		/// Половина ширины экрана
		/// </summary>
		public double ScreenWidth => SystemParameters.PrimaryScreenWidth / 2;

		/// <summary>
		/// Половина высоты экрана
		/// </summary>
		public double ScreenHeight => SystemParameters.PrimaryScreenHeight / 2;

		#endregion

		#region OtherProperties

		/// <summary>
		/// Тип скриншота
		/// </summary>
		public FileTypes FileType { get; set; }

		/// <summary>
		/// Настройки
		/// </summary>
		public Configuration? Config { get; set; }

		/// <summary>
		/// Тип рамки
		/// </summary>
		public FrameMode FrameMode { get; set; }

		/// <summary>
		/// Это старая рамка?
		/// </summary>
		public bool IsOldFrame => FrameMode == FrameMode.Old;

		#endregion

		#endregion

		#region SaveAndCloseCommand

		public Action Save;

		private RelayCommand _saveAndCloseCommand;

		/// <summary>
		/// Сохранение настройки и закрытие окна с ней
		/// </summary>
		public RelayCommand SaveAndCloseCommand => _saveAndCloseCommand ??= new RelayCommand(SaveAndClose, param => true);

		private void SaveAndClose()
		{
			Save?.Invoke();
		}

		#endregion

		#region MakeConfiguration

		/// <summary>
		/// Создание настройки
		/// </summary>
		public Configuration MakeConfiguration()
		{
			var config = Config ?? new Configuration();
			config.FrameMode = FrameMode;
			config.ConfigurationName = Math.Round(ImageExample.Width).ToString() + " x " + Math.Round(ImageExample.Height).ToString();
			switch (FileType)
			{
				case FileTypes.Dialog:
					config.MarginForDialog = new Thickness(MarginForFrame.Left, MarginForFrame.Top, MarginForFrame.Right, MarginForFrame.Bottom);
					config.MarginForMenu = new Thickness(MarginForMenu.Left, MarginForMenu.Top, MarginForMenu.Right, MarginForMenu.Bottom);
					config.Scale = Scale;
					break;
				case FileTypes.LeftAndDialog:
					config.MarginForDialog = new Thickness(MarginForFrame.Left, MarginForFrame.Top, MarginForFrame.Right, MarginForFrame.Bottom);
					config.MarginForMenu = new Thickness(MarginForMenu.Left, MarginForMenu.Top, MarginForMenu.Right, MarginForMenu.Bottom);
					config.MarginForLeftPlace = new Thickness(MarginForLeftPlace.Left, MarginForLeftPlace.Top, MarginForLeftPlace.Right, MarginForLeftPlace.Bottom);
					config.Scale = Scale;
					break;
				case FileTypes.Left:
					config.MarginForLeftPlace = new Thickness(MarginForLeftPlace.Left, MarginForLeftPlace.Top, MarginForLeftPlace.Right, MarginForLeftPlace.Bottom);
					break;
				case FileTypes.Place:
					config.MarginForPlace = new Thickness(MarginForFrame.Left, MarginForFrame.Top, MarginForFrame.Right, MarginForFrame.Bottom);
					config.Scale = Scale;
					break;
			}
			return config;
		}

		#endregion
	}
}
