using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xaml;
using System.Xml.Linq;
using Bitmap = System.Drawing.Bitmap;

namespace AutoType
{
	/// <summary>
	/// Различные расширения
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Перевод BitmapImage в строку, чтобы можно было сохранить в файл
		/// </summary>
		public static string BitmapSourceToBase64(this BitmapSource bitmapImage)
		{
			// Создаем кодировщик для преобразования изображения в байты
			var encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

			// Сохраняем данные в MemoryStream
			using var stream = new MemoryStream();
			encoder.Save(stream);
			return Convert.ToBase64String(stream.ToArray()); // Кодируем в Base64
		}

		/// <summary>
		/// Создание Source, чтобы его можно было подсунуть как источник в контроль Image
		/// </summary>
		public static BitmapImage MakeSource(this Image image)
		{
			try
			{
				image.Measure(new Size(image.Width, image.Height));
				image.Arrange(new Rect(new Point(0, 0), image.DesiredSize));
				RenderTargetBitmap rtb;
				rtb = new((int)image.ActualWidth, (int)image.ActualHeight, 96, 96, PixelFormats.Pbgra32);
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

		/// <summary>
		/// Создание контроля Image из текущего UserControl
		/// </summary>
		public static Image MakeImage(this UserControl userControl)
		{
			try
			{
				userControl.Measure(new Size(userControl.Width, userControl.Height));
				userControl.Arrange(new Rect(new Point(0, 0), userControl.DesiredSize));
				RenderTargetBitmap rtb = new((int)userControl.ActualWidth, (int)userControl.ActualHeight, 96, 96, PixelFormats.Pbgra32);
				rtb.Render(userControl);
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

		public static Bitmap ToImage(this byte[] bytes)
		{
			using var stream = new MemoryStream(bytes);
			using var image = System.Drawing.Image.FromStream(stream, false, true);
			return new Bitmap(image);
		}
		private static ImageCodecInfo GetEncoderInfo(this string mimeType)
		{
			int j;
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			for (j = 0; j < encoders.Length; ++j)
			{
				if (encoders[j].MimeType == mimeType)
					return encoders[j];
			}
			return null;
		}

		public static BitmapImage CreateSource(this byte[] bytes, string? ext)
		{
			var bi = new BitmapImage();
			Bitmap bmp = bytes.ToImage();
			bmp.SetResolution(96, 96); // устанавливаем стандартное разрешение, чтобы изображение не менялось в размерах
			using (var ms = new MemoryStream())
			{
				ImageCodecInfo myImageCodecInfo = ext switch
				{
					".png" => GetEncoderInfo("image/png"),
					".jpeg" => GetEncoderInfo("image/jpeg"),
					_ => GetEncoderInfo("image/jpeg"),
				};

				// Create an Encoder object based on the GUID
				// for the ColorDepth parameter category.
				Encoder myEncoder = Encoder.ColorDepth;

				// Create an EncoderParameters object.
				// An EncoderParameters object has an array of EncoderParameter
				// objects. In this case, there is only one
				// EncoderParameter object in the array.
				EncoderParameters myEncoderParameters = new EncoderParameters(1);

				// Save the image with a color depth of 24 bits per pixel.
				EncoderParameter myEncoderParameter =
					new(myEncoder, 24L);
				myEncoderParameters.Param[0] = myEncoderParameter;
				bmp.Save(ms, myImageCodecInfo, myEncoderParameters);

				ms.Position = 0;
				bi.BeginInit();
				bi.CacheOption = BitmapCacheOption.OnLoad;
				bi.StreamSource = ms;
				bi.EndInit();
			}
			return bi;
		}

		/// <summary>
		/// Обрезка изображения
		/// </summary>
		public static CroppedBitmap CropBitmapImage(this BitmapImage bitmapImage, double? croppedWidth, double? croppedHeight)
		{
			if (bitmapImage.Width - croppedWidth < 0 || bitmapImage.Height - croppedHeight < 0)
				throw new ArgumentException("Параметры для обрезки не должны быть больше исходных размеров картинок.");
			return new CroppedBitmap(bitmapImage,
				new Int32Rect((int)(bitmapImage.Width - (croppedWidth ?? bitmapImage.Width)) / 2, (int)(bitmapImage.Height - (croppedHeight ?? bitmapImage.Height)) / 2,
				(int)(croppedWidth ?? bitmapImage.Width), (int)(croppedHeight ?? bitmapImage.Height)));
		}

		/// <summary>
		/// Преобразование Base64 строки в BitmapImage (распаковка)
		/// </summary>
		public static BitmapImage ConvertBase64ToBitmapImage(this string base64)
		{
			try
			{
				// Декодируем Base64 в массив байтов
				byte[] bytes = Convert.FromBase64String(base64);

				return bytes.CreateSource(null);
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Получает контроль с учётом типа рамки (новой или старой)
		/// </summary>
		/// <returns></returns>
		public static object GetControlWithMode(this UserControl userControl, string baseElementName)
		{
			var controlName = baseElementName + userControl.Tag.ToString();
			return userControl.FindName(controlName);
		}

		/// <summary>
		/// Сохранение XAML с помощью XamlServices
		/// </summary>
		public static string SaveWithXamlServices(this UserControl userControl)
		{
			using var stream = new MemoryStream();
			XamlServices.Save(stream, userControl);
			stream.Position = 0;
			using var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}

		/// <summary>
		/// Загрузка XAML с помощью XamlServices
		/// </summary>
		public static object LoadWithXamlServices(this string xaml)
		{
			using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xaml));
			return XamlServices.Load(stream);
		}

		/// <summary>
		/// Сериализация binding для записи в файл
		/// </summary>
		/// <param name="binding"></param>
		/// <returns></returns>
		public static XDocument ToXml(this Binding binding)
		{
			var xdoc = new XDocument(
				new XElement(nameof(Binding),
					new XElement("Path", binding.Path?.Path),
					new XElement("ElementName", binding.ElementName),
					new XElement("Source", binding.Source?.ToString()),
					new XElement("Mode", (int)binding.Mode),
					new XElement("UpdateSourceTrigger", (int)binding.UpdateSourceTrigger),
					new XElement("ConverterTypeName", binding.Converter?.GetType().AssemblyQualifiedName)
				)
			);
			return xdoc;
		}

		/// <summary>
		/// Десериализация binding из xml
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static Binding FromXml(this string xml)
		{
			XDocument xdoc = XDocument.Load(xml);
			var path = xdoc.Root.Element("Path")?.ToString();
			var elementName = xdoc.Root.Element("ElementName")?.ToString();
			var source = xdoc.Root.Element("Source")?.ToString();
			var mode = (BindingMode)int.Parse(xdoc.Root.Element("Mode")?.Value);
			var updateSourceTrigger = (UpdateSourceTrigger)int.Parse(xdoc.Root.Element("UpdateSourceTrigger")?.Value);
			var converterTypeName = xdoc.Root.Element("ConverterTypeName")?.ToString();

			var binding = new Binding
			{
				Path = !string.IsNullOrEmpty(path) ? new PropertyPath(path) : null,
				ElementName = elementName,
				Source = !string.IsNullOrEmpty(source) ? new Uri(source, UriKind.RelativeOrAbsolute) : null,
				Mode = mode,
				UpdateSourceTrigger = updateSourceTrigger
			};

			if (!string.IsNullOrEmpty(converterTypeName))
			{
				// Создаем экземпляр конвертера по имени типа
				var converterType = Type.GetType(converterTypeName);
				if (converterType != null && typeof(IValueConverter).IsAssignableFrom(converterType))
				{
					binding.Converter = (IValueConverter)Activator.CreateInstance(converterType);
				}
			}

			return binding;
		}
	}
}
