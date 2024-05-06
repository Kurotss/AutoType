using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutoType.Classes
{
	public class TypeScreen : BaseDataContext
	{
		/// <summary>
		/// Источник для Image Control с тайпом 
		/// </summary>
		private BitmapImage? _source;

		public BitmapImage? Source
		{
			get => _source;
			set
			{
				_source = value;
				OnPropertyChanged(nameof(Source));
				OnPropertyChanged(nameof(WidthForPreview));
				OnPropertyChanged(nameof(IsNotNullSource));
			}
		}

		/// <summary>
		/// Источник с изначальным скриншотом
		/// </summary>
		public BitmapImage FileSource { get; set; }

		/// <summary>
		/// Ширина картинки для превью в списке
		/// </summary>
		public double? WidthForPreview => SystemParameters.PrimaryScreenWidth / 3.2;

		/// <summary>
		/// Заполнен ли Dource
		/// </summary>
		public bool IsNotNullSource => Source != null;

		/// <summary>
		/// UserControl с тайпом
		/// </summary>
		public UserControl UserControl { get; set; }

		/// <summary>
		/// Тип скриншота
		/// </summary>
		public FileTypes Type { get; set; }

		#region OpenControlCommand

		private RelayCommand _openControlCommand;

		/// <summary>
		/// Открытие окна с редактированием
		/// </summary>
		public RelayCommand OpenControlCommand => _openControlCommand ??= (_openControlCommand = new RelayCommand(OpenControl));

		#endregion

		/// <summary>
		/// Окно для редактирования
		/// </summary>
		public EditWindow Win { get; set; }

		/// <summary>
		/// Режим рамки
		/// </summary>
		public FrameMode FrameMode { get; set; }

		#region Methods

		/// <summary>
		/// Открытие окна для редактирования текста контроля
		/// </summary>
		private void OpenControl()
		{
			if (Type == FileTypes.Place || Type == FileTypes.Dialog || Type == FileTypes.LeftAndDialog)
			{
				Win = new(UserControl) { SaveAndClose = GetNewImage };
				Win.Show();
			}
			else MessageBox.Show("Редактировать можно только изображения с рамкой места или рамкой диалога.");
		}

		/// <summary>
		/// Получение нового скриншота после редактирования текста
		/// </summary>
		private void GetNewImage()
		{
			try
			{
				UserControl = (UserControl)Win.viewBox.Child;
				Win.viewBox.Child = null;
				Source = MakeSource(MakeImage(), Source.Width, Source.Height);
				Win.Close();
				OnPropertyChanged(nameof(Source));
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		/// <summary>
		/// Создание конроля Image из текущего UserControl
		/// </summary>
		public Image MakeImage()
		{
			try
			{
				UserControl.Measure(new Size(UserControl.Width, UserControl.Height));
				UserControl.Arrange(new Rect(new Point(0, 0), UserControl.DesiredSize));
				RenderTargetBitmap rtb = new((int)UserControl.ActualWidth, (int)UserControl.ActualHeight, 96, 96, PixelFormats.Pbgra32);
				rtb.Render(UserControl);
				PngBitmapEncoder png = new();
				png.Frames.Add(BitmapFrame.Create(rtb));
				MemoryStream stream = new();
				png.Save(stream);
				BitmapImage bmImage = new();
				bmImage.BeginInit();
				bmImage.CacheOption = BitmapCacheOption.OnLoad;
				bmImage.StreamSource = stream;
				bmImage.EndInit();

				Image img = new() { Source = bmImage, Width = bmImage.Width, Height = bmImage.Height, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
				return img;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Создание Source, чтобы его можно было подсунуть как источник в контроль Image
		/// </summary>
		public BitmapImage MakeSource(Image image, double? newWidth, double? newHeight)
		{
			try
			{
				if (newWidth != null && newHeight != null)
				{
					image.Measure(new Size((double)newWidth, (double)newHeight));
				}
				else
					image.Measure(new Size(image.Width, image.Height));
				image.Arrange(new Rect(new Point(0, 0), image.DesiredSize));
				RenderTargetBitmap rtb;
				if (newWidth != null && newHeight != null)
				{
					rtb = new((int)newWidth, (int)newHeight, 96, 96, PixelFormats.Pbgra32);
				}
				else
				{
					rtb = new((int)image.ActualWidth, (int)image.ActualHeight, 96, 96, PixelFormats.Pbgra32);
				}
				rtb.Render(image);
				//
				PngBitmapEncoder png = new();
				png.Frames.Add(BitmapFrame.Create(rtb));
				MemoryStream stream = new();
				png.Save(stream);
				BitmapImage bmImage = new();
				bmImage.BeginInit();
				bmImage.CacheOption = BitmapCacheOption.OnLoad;
				bmImage.StreamSource = stream;
				bmImage.EndInit();
				return bmImage;
			}
			catch (Exception)
			{
				throw;
			}
		}

		#endregion
	}
}
