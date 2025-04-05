using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace AutoType.Classes
{
	/// <summary>
	/// Всякие данные о затайпленном скриншоте
	/// </summary>
	public class TypeScreen : BaseDataContext
	{
		#region конструкторы

		public TypeScreen() { }

		public TypeScreen(string filepath)
		{
			FromXml(filepath);
		}

		#endregion

		#region свойства

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
				OnPropertyChanged(nameof(IsNotNullSource));
			}
		}

		/// <summary>
		/// Источник с изначальным скриншотом
		/// </summary>
		public BitmapImage FileSource { get; set; }

		/// <summary>
		/// Обрезанный скриншот с учётом настроек
		/// </summary>
		public CroppedBitmap CroppedImage { get; set; }

		/// <summary>
		/// Ширина картинки для превью в списке
		/// </summary>
		public double WidthForPreview => SystemParameters.PrimaryScreenWidth / 3.2;

		/// <summary>
		/// Заполнен ли Source
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

		/// <summary>
		/// Окно для редактирования
		/// </summary>
		public EditWindow Win { get; set; }

		/// <summary>
		/// Режим рамки
		/// </summary>
		public FrameMode FrameMode { get; set; }

		/// <summary>
		/// Имеет ли этот скрин реплику
		/// </summary>
		public bool IsDialog => Type == FileTypes.Dialog || Type == FileTypes.LeftAndDialog || Type == FileTypes.Note;

		/// <summary>
		/// Имеет ли этот скрин левую рамку места
		/// </summary>
		public bool IsLeftPlace => Type == FileTypes.Left || Type == FileTypes.LeftAndDialog;

		#endregion

		#region методы

		/// <summary>
		/// Получение нового скриншота после редактирования текста
		/// </summary>
		private void GetNewImage()
		{
			try
			{
				UserControl = (UserControl)Win.viewBox.Child;
				Win.viewBox.Child = null;
				Source = UserControl.MakeImage().MakeSource();
				Win.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		/// <summary>
		/// Сохранение экземпляра класса в формате в текстовый файл
		/// </summary>
		public void SaveTypeToFile(string filepath)
		{
			var xdoc = ToXml();
			xdoc.Save(filepath);
		}

		#endregion

		#region команды

		#region OpenControlCommand

		private RelayCommand _openControlCommand;

		/// <summary>
		/// Открытие окна с редактированием
		/// </summary>
		public RelayCommand OpenControlCommand => _openControlCommand ??= (_openControlCommand = new RelayCommand(OpenControl));

		/// <summary>
		/// Открытие окна для редактирования текста контроля
		/// </summary>
		private void OpenControl()
		{
			if (Type != FileTypes.None && Type != FileTypes.Menu)
			{
				Win = new(UserControl);
				Win.SaveAndClose += GetNewImage;
				Win.Show();
			}
			else MessageBox.Show("Редактировать можно только изображения с рамкой места или рамкой диалога.");
		}

		#endregion

		#endregion

		#region сериализация

		/// <summary>
		/// Сериализация для записи в файл
		/// </summary>
		private XDocument ToXml()
		{
			string? xaml = null;
			// зачищаем изображение, т.к. оно потом может сломать распаковку контрола
			if (UserControl != null) // не у всех модет быть создан UserControl
			{
				(UserControl.FindName("screen") as Image).Source = null;
				xaml = XamlWriter.Save(UserControl);
			}

			var xdoc = new XDocument(
				new XElement(nameof(TypeScreen),
					new XElement(nameof(CroppedImage), CroppedImage.BitmapSourceToBase64()),
					new XElement(nameof(UserControl), xaml),
					new XElement(nameof(Type), (int)Type),
					new XElement(nameof(FrameMode), (int)FrameMode)
				)
			);
			return xdoc;
		}

		/// <summary>
		/// Десериализация данных из текстового файла
		/// </summary>
		private void FromXml(string filepath)
		{
			XDocument xdoc = XDocument.Load(filepath);
			Type = (FileTypes)int.Parse(xdoc.Root.Element(nameof(Type))?.Value);
			FrameMode = (FrameMode)int.Parse(xdoc.Root.Element(nameof(FrameMode))?.Value);

			var croppedBitmapImage = xdoc.Root.Element(nameof(CroppedImage))?.Value.ConvertBase64ToBitmapImage();
			CroppedImage = new CroppedBitmap(croppedBitmapImage, new Int32Rect(0, 0, (int)croppedBitmapImage.Width, (int)croppedBitmapImage.Height));
			FileSource = croppedBitmapImage;

			var xaml = xdoc.Root.Element(nameof(UserControl))?.Value;
			if (!string.IsNullOrEmpty(xaml))
			{
				UserControl = (UserControl)XamlReader.Parse(xaml);
				// устанавливаем затёртую картинку
				(UserControl.FindName("screen") as Image).Source = CroppedImage;
			}


			var image = Type == FileTypes.None ?
								new System.Windows.Controls.Image()
								{
									Source = CroppedImage,
									Width = CroppedImage.Width,
									Height = CroppedImage.Height,
									HorizontalAlignment = HorizontalAlignment.Center,
									VerticalAlignment = VerticalAlignment.Top
								} : UserControl.MakeImage();
			Source = image.MakeSource();
		}

		#endregion
	}
}
