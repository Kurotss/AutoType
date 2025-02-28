using AutoType.UserControls;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;
using Task = System.Threading.Tasks.Task;
using Word = Microsoft.Office.Interop.Word;

namespace AutoType.Classes
{
	public class TypeWindowModel : BaseDataContext
	{
		#region Constructor

		public TypeWindowModel()
		{
			try
			{
				// если нет папки под программу, то создаём её
				if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/AutoType"))
					Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/AutoType");
				string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/AutoType/ConfigurationSekai.txt";
				if (File.Exists(path))
				{
					using StreamReader reader = new(path);
					string text = reader.ReadToEnd();
					AllConfigurationList = new ObservableCollection<Configuration>( JsonSerializer.Deserialize<List<Configuration>>(text));
					CurrentConfig = ConfigurationList.FirstOrDefault();
				}
				OnPropertyChanged(nameof(IsNotExistsConfiguration));
				OnPropertyChanged(nameof(ConfigurationList));
				//}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		#endregion

		#region Properties

		#region Images

		private ObservableCollection<TypeScreen> _images;

		/// <summary>
		/// Коллекция с объектами с инфорацией по скриншотам
		/// </summary>
		public ObservableCollection<TypeScreen> Images
		{
			get => _images;
			set
			{
				_images = value;
				OnPropertyChanged(nameof(Images));
			}
		}

		#endregion

		#region CurrentScreen

		private int _currentScreen;

		public int CurrentScreen
		{
			get => _currentScreen;
			set
			{
				_currentScreen = value;
				OnPropertyChanged(nameof(CurrentScreen));
			}
		}

		#endregion

		#region Max

		private int _max;

		public int Max
		{
			get => _max;
			set
			{
				_max = value;
				OnPropertyChanged(nameof(Max));
			}
		}

		#endregion

		#region IsEditMode

		private bool _isEditMode;

		public bool IsEditMode
		{
			get => _isEditMode;
			set
			{
				_isEditMode = value;
				OnPropertyChanged(nameof(IsEditMode));
			}
		}

		#endregion

		#region IsGetImagesMode

		private bool _isGetImagesMode;

		/// <summary>
		/// Режим получения изображений из папки
		/// </summary>
		public bool IsGetImagesMode
		{
			get => _isGetImagesMode;
			set
			{
				_isGetImagesMode = value;
				OnPropertyChanged(nameof(IsGetImagesMode));
			}
		}

		#endregion

		#region IsSaveImagesMode

		private bool _isSaveImagesMode;

		/// <summary>
		/// Режим сохранения изображений
		/// </summary>
		public bool IsSaveImagesMode
		{
			get => _isSaveImagesMode;
			set
			{
				_isSaveImagesMode = value;
				OnPropertyChanged(nameof(IsSaveImagesMode));
			}
		}

		#endregion

		#region IsNotExistsConfiguration

		private bool _isNotExistsConfiguration;

		/// <summary>
		/// Не определено никаких настроек: да/нет
		/// </summary>
		public bool IsNotExistsConfiguration => ConfigurationList.Count() < 1;
		//{
		//	get => _isNotExistsConfiguration;
		//	set
		//	{
		//		_isNotExistsConfiguration = value;
		//		OnPropertyChanged(nameof(IsNotExistsConfiguration));
		//	}
		//}

		#endregion

		#region ConfigurationList

		/// <summary>
		/// Список настроек наложения под нужную рамку
		/// </summary>
		public ObservableCollection<Configuration> ConfigurationList => new(AllConfigurationList.Where(item => item.FrameMode == FrameMode));

		/// <summary>
		/// Список всех настроек наложения под все рамки
		/// </summary>
		public ObservableCollection<Configuration> AllConfigurationList { get; set; } = new ObservableCollection<Configuration>();

		#endregion

		#region FrameMode

		private FrameMode _frameMode = FrameMode.Old;

		/// <summary>
		/// Режим рамки
		/// </summary>
		public FrameMode FrameMode
		{
			get => _frameMode;
			set
			{
				_frameMode = value;
				OnPropertyChanged(nameof(ConfigurationList));
				OnPropertyChanged(nameof(IsNotExistsConfiguration));
				OnPropertyChanged(nameof(FrameMode));
				CurrentConfig = ConfigurationList.FirstOrDefault();
				EditFileCommand = null;
				NavigateToPreviousConfigCommand = null;
				NavigateToNextConfigCommand = null;
				DeleteConfigCommand = null;
				EditFileCommand = null;
			}
		}

		#endregion

		#region CurrentConfig

		private Configuration _currentConfig;

		/// <summary>
		/// Выбранные настройки для наложения рамок
		/// </summary>
		public Configuration CurrentConfig
		{
			get => _currentConfig;
			set
			{
				_currentConfig = value;
				OnPropertyChanged(nameof(CurrentConfig));
			}
		}

		#endregion

		#region Prefix

		private string _prefix;

		public string Prefix
		{
			get => _prefix;
			set
			{
				var newValue = value.Replace("\\", string.Empty);
				if (CheckPrefix(newValue))
				{
					_prefix = newValue;
					OnPropertyChanged(nameof(Prefix));
				}
			}
		}

		public bool CheckPrefix(string str)
		{
			Regex regex = new Regex(".*[\\<>:\"|?*/]+.*");
			MatchCollection matches = regex.Matches(str);
			if (matches.Count >= 1)
			{
				MessageBox.Show("Введён один из запрещенных символов: < > : / \" | ? *");
				return false;
			}
			return true;
		}

		#endregion

		#region CroppedSizes

		#region CroppedWidth

		private double? _croppedWidth;

		/// <summary>
		/// Ширина для обрезки скринов
		/// </summary>
		public double? CroppedWidth
		{
			get => _croppedWidth;
			set
			{
				if (value <= 0)
					MessageBox.Show("Такое значение недопустимо.");
				else
				{
					_croppedWidth = value;
					OnPropertyChanged(nameof(CroppedWidth));
				}
			}
		}

		#endregion

		#region CroppedHeight

		private double? _croppedHeight;

		public double? CroppedHeight
		{
			get => _croppedHeight;
			set
			{
				if (value <= 0)
					MessageBox.Show("Такое значение недопустимо.");
				else
				{
					_croppedHeight = value;
					OnPropertyChanged(nameof(CroppedHeight));
				}
			}
		}

		#endregion

		#endregion

		public string[] AllText { get; set; }

		public ConfigurationWindow ConfigurationWindow { get; set; }

		#endregion

		#region Commands

		#region SaveFileCommand

		/// <summary>
		/// Сохранение готовых изображений
		/// </summary>
		private RelayCommand? _saveFileCommand;

		public RelayCommand SaveFileCommand
		{
			get => _saveFileCommand ??= new RelayCommand(SaveFiles, param => Images?.Where(item => item.Source != null).ToList().Count > 0);
			set
			{
				_saveFileCommand = value;
				OnPropertyChanged(nameof(SaveFileCommand));
			}
		}

		private async void SaveFiles()
		{
			FolderBrowserDialog dialog = new()
			{
				ShowNewFolderButton = false,
				SelectedPath = AppDomain.CurrentDomain.BaseDirectory
			};
			DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				try
				{
					IsSaveImagesMode = true;
					Max = Images.Count;
					CurrentScreen = 0;
					await Task.Run(() => SaveFilesAsync(dialog.SelectedPath));
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message);
				}
				finally
				{
					IsSaveImagesMode = false;
				}
			}
		}

		async Task SaveFilesAsync(string selectedPath)
		{
			int i = 1;
			foreach (var image in Images)
			{
				var thread = new Thread(() =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{

						var path = selectedPath + '\\' + (string.IsNullOrEmpty(Prefix) ? "" : Prefix + '_') + i + ".png";
						PngBitmapEncoder png = new();
						png.Frames.Add(BitmapFrame.Create(image.Source));
						using (FileStream fs = File.Create(path))
							png.Save(fs);
						CurrentScreen++;
						i++;
					});
				});
				thread.Start();
				thread.Join();
			}
			MessageBox.Show("Все файлы успешно сохранены!");
		}

		#endregion

		#region OpenFolderCommand

		/// <summary>
		/// Выбор папки со скриншотами и преобразование их в BitmapImage
		/// </summary>
		private RelayCommand _openFolderCommand;

		public RelayCommand OpenFolderCommand => _openFolderCommand ??= new RelayCommand(OpenFolder, param => true);

		private async void OpenFolder()
		{
			FolderBrowserDialog dialog = new()
			{
				ShowNewFolderButton = false,
				SelectedPath = AppDomain.CurrentDomain.BaseDirectory
			};
			DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				try
				{
					List<string> files = Directory.GetFiles(dialog.SelectedPath).ToList();
					files.Sort(new MyComparer());
					// очищаем коллекцию скриншотов, если уже были старые
					Images = new ObservableCollection<TypeScreen>();
					SaveFileCommand = null;
					IsGetImagesMode = true;
					CurrentScreen = 0;
					Max = files.Count;
					//
					await Task.Run(() => GetImagesFromFiles(files));
					//
					if (string.IsNullOrEmpty(Prefix))
					{
						string[] fileNames = dialog.SelectedPath.Split('\\');
						Prefix = fileNames[fileNames.Length - 1];
					}
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message);
				}
				finally
				{
					IsGetImagesMode = false;
				}
			}
		}

		/// <summary>
		/// Получение изображений из файлов и преобразование в BitmapImage для дальнейшей работы
		/// </summary>
		async Task GetImagesFromFiles(List<string> files)
		{
			foreach (string fileName in files)
			{
				var thread = new Thread(() =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						// открываем файл и делаем из него изображение
						using Image image = Image.FromFile(fileName);
						// конвертируем в формат BitmapImage, чтобы можно было его подсунуть как источник для контроля Image
						ImageConverter converter = new();
						byte[] data = (byte[])converter.ConvertTo(image, typeof(byte[]));
						var screen = CreateSource(data, Path.GetExtension(fileName));

						// определяем тип скриншота
						FileTypes fileType = FileTypes.Dialog;
						if (fileName.Contains("none"))
							fileType = FileTypes.None;
						else if (fileName.Contains("menu"))
							fileType = FileTypes.Menu;
						else if (fileName.Contains("place"))
							fileType = FileTypes.Place;
						else if (fileName.Contains("left_and_dialog"))
							fileType = FileTypes.LeftAndDialog;
						else if (fileName.Contains("left"))
							fileType = FileTypes.Left;
						// создаём объект, где будут храниться данные по одному скриншоту: контроль для редактирования, результат режим и т.д.
						var item = new TypeScreen()
						{
							FrameMode = FrameMode,
							FileSource = screen,
							Type = fileType
						};
						Images.Add(item);
						CurrentScreen++;
					});
				});
				thread.Start();
				thread.Join();
			}
		}

		#endregion

		#region OpenFileCommand

		private RelayCommand? _openFileCommand;

		/// <summary>
		/// Открытие ворд документа и получение всего текста из него
		/// </summary>
		public RelayCommand OpenFileCommand => _openFileCommand ??= new RelayCommand(OpenFileAndGetText, param => true);

		private void OpenFileAndGetText()
		{
			// Проверка на наличие ворда на компе
			using (var regWord = Registry.ClassesRoot.OpenSubKey("Word.Application"))
			{
				if (regWord == null)
				{
					MessageBox.Show("Не установлен Word на компьютере.");
					return;
				}
			}
			// выбор документа и получение из него текста
			OpenFileDialog dialog = new() { Filter = "Файлы MS Word (*.docx)|*.docx" };
			if ((bool)dialog.ShowDialog())
			{
				Cursor.Current = Cursors.WaitCursor;
				Word.Application app = new();
				try
				{
					Document doc = new();
					object fileNames = dialog.FileName;
					app.Documents.Open(ref fileNames);
					doc = app.ActiveDocument;
					AllText = doc.Content.Text.Split('\r');
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message);
				}
				finally
				{
					app.Quit();
					Cursor.Current = Cursors.Default;
				}
			}
		}

		#endregion

		#region EditFileCommand

		/// <summary>
		/// Наложение рамки на скриншоты
		/// </summary>
		private RelayCommand? _editFileCommand;

		public RelayCommand? EditFileCommand
		{
			get => _editFileCommand ??= new RelayCommand(EditFile, param => !IsNotExistsConfiguration);
			set
			{
				_editFileCommand = value;
				OnPropertyChanged(nameof(EditFileCommand));
			}
		}

		/// <summary>
		/// Проверка всех условий для начала тайпа
		/// </summary>
		private bool CheckBeforeType()
		{
			if (AllText == null || AllText?.Length < 1)
			{
				MessageBox.Show("Не выбран файл с текстом или же он пустой.");
				return false;
			}
			if (Images == null || Images?.Count < 1)
			{
				MessageBox.Show("Не выбрана папка с изображениями или же она пустая.");
				return false;
			}
			// Проверка на равное количество строк текста и скриншотов с текстом
			int stringCnt = AllText.Where(item => !string.IsNullOrEmpty(item)).Count();
			int filesCnt = Images.Where(item => item.Type == FileTypes.Place || item.Type == FileTypes.Dialog || item.Type == FileTypes.Left).Count();
			filesCnt += Images.Where(item => item.Type == FileTypes.LeftAndDialog).Count() * 2;
			if (stringCnt != filesCnt)
			{
				MessageBox.Show($"Количество скриншотов ({filesCnt}) не соотвествует количеству строк текста ({stringCnt}). Проверьте и выберите папку со скриншотами или файл с текстом заново.");
				return false;
			}
			var img = Images.FirstOrDefault();
			if (img?.FileSource.Width - CroppedWidth < 0 || img?.FileSource.Height - CroppedHeight < 0)
			{
				MessageBox.Show("Параметры для обрезки не должны быть больше исходных размеров картинок.");
				return false;
			}
			if (CroppedWidth != null && CroppedHeight == null || CroppedWidth == null && CroppedHeight != null)
			{
				MessageBox.Show("Укажите все параметры для обрезки.");
				return false;
			}
			return true;
		}

		/// <summary>
		/// Запуск тайпа всех скриншотов
		/// </summary>
		private async void EditFile()
		{
			// если все условия соблюдены, то выполняется тайп
			if (CheckBeforeType())
			{
				IsEditMode = true;
				CurrentScreen = 0;
				try
				{
					await Task.Run(Type);
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message);
				}
				finally
				{
					IsEditMode = false;
				}
			}
		}

		// прогон пустых строк до следующей строки с текстом
		public string GetLine(ref int i)
		{
			// переменная с текстом для скриншота
			string line;
			do
			{
				line = AllText[i];
				i++;
			}
			while (string.IsNullOrEmpty(line));
			return line;
		}

		public static string[] GetDialog(string line, int i)
		{
			// делим строку на имя и реплику. если в реплике было :, то склеиваем реплику
			string[] lines = line.Split(':');
			if (lines.Length > 2)
				for (int j = 2; j < lines.Length; j++)
					lines[1] += ':' + lines[j];
			return lines;
		}

		/// <summary>
		/// Получение всех строк с переводом из документа и цикл с перебором всех скриншотов
		/// </summary>
		async Task Type()
		{
			// цикл с перебором всех скриншотов для наложения
			var i = 0;
			foreach (TypeScreen typeScreen in Images)
			{
				if (!IsEditMode)
					break;
				var thread = new Thread(() =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						try
						{
							// переменная с текстом для скриншота
							string line = string.Empty;
							// пропускаем пустые строки и ищем следующую строку с текстом
							if (typeScreen.Type == FileTypes.Place || typeScreen.Type == FileTypes.Dialog || typeScreen.Type == FileTypes.Left || typeScreen.Type == FileTypes.LeftAndDialog)
							{
								line = GetLine(ref i);
							}
							//
							switch (typeScreen.Type)
							{
								case FileTypes.Menu:
									{
										typeScreen.UserControl = new TypeLocation(string.Empty, false, CurrentConfig ?? new Configuration(), typeScreen.FileSource, CroppedWidth, CroppedHeight);
										break;
									}
								case FileTypes.Place:
									{
										typeScreen.UserControl = new TypeLocation(line, true, CurrentConfig ?? new Configuration(), typeScreen.FileSource, CroppedWidth, CroppedHeight);
										break;
									}
								case FileTypes.Left:
									{
										typeScreen.UserControl = new TypeFrame(string.Empty, string.Empty, CurrentConfig ?? new Configuration(), FrameMode, typeScreen.FileSource, CroppedWidth, CroppedHeight, false, true,
											line.Split("|"));
										break;
									}
								case FileTypes.Dialog:
									{
										var lines = GetDialog(line, i);
										if (lines.Length <= 1)
										{
											MessageBox.Show($"Неправильная разметка на строке {i} ({lines[0]})");
											IsEditMode = false;
											return;
										}
										typeScreen.UserControl = new TypeFrame(lines[0].Trim(), lines[1].Trim(), CurrentConfig ?? new Configuration(), FrameMode, typeScreen.FileSource, CroppedWidth, CroppedHeight, true, false);
										break;
									}
								case FileTypes.None:
									break;
								case FileTypes.LeftAndDialog:
									{
										// сначала прогоняем текст до следующей строки, потом делим на имя и реплику
										var lines = GetDialog(GetLine(ref i), i);
										if (lines.Length <= 1)
										{
											MessageBox.Show($"Неправильная разметка на строке {i} ({lines[0]})");
											IsEditMode = false;
											return;
										}
										typeScreen.UserControl = new TypeFrame(lines[0].Trim(), lines[1].Trim(), CurrentConfig ?? new Configuration(), FrameMode, typeScreen.FileSource, CroppedWidth, CroppedHeight, true, true,
											line.Split("|"));
										break;
									}
							}
							// делаем Source, чтобы отобразить на превью в списке
							// если тип none, то просто делаем control image и в качестве источника подсовываем скрин, иначе берём usercontrol и преобразовываем к image
							typeScreen.Source = typeScreen.MakeSource(typeScreen.Type == FileTypes.None ?
								new System.Windows.Controls.Image() { Source = typeScreen.FileSource, Width = typeScreen.FileSource.Width, Height = typeScreen.FileSource.Height, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Top } :
								typeScreen.MakeImage(), CroppedWidth, CroppedHeight);
							CurrentScreen++;
						}
						catch (Exception e)
						{
							IsEditMode = false;
							MessageBox.Show(e.Message);
							return;
						}
					});
				});
				thread.Start();
				thread.Join();
			}
			IsEditMode = false;
			SaveFileCommand = null;
		}

		#endregion

		#region CreateConfigurationCommand

		private RelayCommand _createConfigurationCommand;

		public RelayCommand CreateConfigurationCommand => _createConfigurationCommand ??= new RelayCommand(CreateConfiguration, param => true);

		private void CreateConfiguration()
		{
			if (Images == null || Images?.Count < 1)
			{
				MessageBox.Show("Для осуществления настройки сначала выберите папку с изображениями");
				return;
			}

			if (CroppedWidth != null && CroppedHeight == null || CroppedWidth == null && CroppedHeight != null)
			{
				MessageBox.Show("Укажите все параметры для обрезки.");
				return;
			}

			try
			{
				IEnumerable<TypeScreen> dialogScreens = Images.Where(item => item.Type == FileTypes.Dialog || item.Type == FileTypes.LeftAndDialog);
				if (!dialogScreens.Any())
				{
					MessageBox.Show("Отсутствует изображение с диалогом!");
					return;
				}

				if (FrameMode == FrameMode.Old)
				{
					// в первую очередь ищем скриншот с рамкой диалога и левой рамкой
					var imageExample = dialogScreens.FirstOrDefault(item => item.Type == FileTypes.LeftAndDialog);
					// если не нашлось, то ищем просто с диалогом
					imageExample ??= dialogScreens.FirstOrDefault(item => item.Type == FileTypes.Dialog);
					ConfigurationWindow = new ConfigurationWindow
					{
						DataContext = new ConfigurationFrameModel(imageExample.Type, imageExample.FileSource, null, CroppedWidth, CroppedHeight, FrameMode) { Save = GetConfiguration }
					};
					ConfigurationWindow.Show();
				}
				else if (FrameMode == FrameMode.New)
					OpenPlaceConfiguration(null);

			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				return;
			}
		}

		/// <summary>
		/// Открыть окно с настройками для рамки места
		/// </summary>
		private void OpenPlaceConfiguration(Configuration? config)
		{
			var placeScreen = Images.Where(item => item.Type == FileTypes.Place).FirstOrDefault();
			if (placeScreen is null)
			{
				MessageBox.Show("Отсутствует изображение с местом!");
				return;
			}

			ConfigurationWindow = new ConfigurationWindow
			{
				DataContext = new ConfigurationFrameModel(placeScreen.Type, placeScreen.FileSource, config, CroppedWidth, CroppedHeight, FrameMode) { Save = GetConfiguration }
			};
			ConfigurationWindow.Show();
		}

		private void GetConfiguration()
		{
			ConfigurationFrameModel dataContext = ConfigurationWindow.DataContext as ConfigurationFrameModel;
			Configuration config = dataContext.MakeConfiguration();
			ConfigurationWindow.Close();
			switch (dataContext.FileType)
			{
				case FileTypes.Dialog:
					var leftPlaceScreen = Images.Where(item => item.Type == FileTypes.Left).FirstOrDefault();
					if (leftPlaceScreen is not null)
					{
						ConfigurationWindow = new ConfigurationWindow
						{
							DataContext = new ConfigurationFrameModel(leftPlaceScreen.Type, leftPlaceScreen.FileSource, config, CroppedWidth, CroppedHeight, FrameMode) { Save = GetConfiguration }
						};
						ConfigurationWindow.Show();
					}
					else OpenPlaceConfiguration(config);
					break;
				case FileTypes.Left:
				case FileTypes.LeftAndDialog:
					OpenPlaceConfiguration(config);
					break;
				case FileTypes.Place:
					AllConfigurationList.Add(config);
					OnPropertyChanged(nameof(IsNotExistsConfiguration));
					OnPropertyChanged(nameof(ConfigurationList));
					EditFileCommand = null;
					CurrentConfig = config;
					NavigateToPreviousConfigCommand = null;
					NavigateToNextConfigCommand = null;
					DeleteConfigCommand = null;
					break;
			}
		}

		#endregion

		#region NavigateToPreviousConfigCommand

		private RelayCommand _navigateToPreviousConfigCommand;

		public RelayCommand? NavigateToPreviousConfigCommand
		{
			get => _navigateToPreviousConfigCommand ??= new RelayCommand(NavigateToPrevious, param => ConfigurationList.IndexOf(CurrentConfig) - 1 >= 0);
			set
			{
				_navigateToPreviousConfigCommand = value;
				OnPropertyChanged(nameof(NavigateToPreviousConfigCommand));
			}
		}

		private void NavigateToPrevious()
		{
			CurrentConfig = ConfigurationList[ConfigurationList.IndexOf(CurrentConfig) - 1];
			NavigateToPreviousConfigCommand = null;
			NavigateToNextConfigCommand = null;
			DeleteConfigCommand = null;
		}

		#endregion

		#region NavigateToNextConfigCommand

		private RelayCommand _navigateToNextConfigCommand;

		public RelayCommand? NavigateToNextConfigCommand
		{
			get => _navigateToNextConfigCommand ??= new RelayCommand(NavigateToNext, param => ConfigurationList.IndexOf(CurrentConfig) + 1 < ConfigurationList.Count);
			set
			{
				_navigateToNextConfigCommand = value;
				OnPropertyChanged(nameof(NavigateToNextConfigCommand));
			}
		}

		private void NavigateToNext()
		{
			CurrentConfig = ConfigurationList[ConfigurationList.IndexOf(CurrentConfig) + 1];
			NavigateToPreviousConfigCommand = null;
			NavigateToNextConfigCommand = null;
			DeleteConfigCommand = null;
		}

		#endregion

		#region DeleteConfigCommand

		private RelayCommand _deleteConfigCommand;

		public RelayCommand? DeleteConfigCommand
		{
			get => _deleteConfigCommand ??= new RelayCommand(DeleteConfig, param => CurrentConfig != null);
			set
			{
				_deleteConfigCommand = value;
				OnPropertyChanged(nameof(DeleteConfigCommand));
			}
		}

		private void DeleteConfig()
		{
			MessageBoxResult result = MessageBox.Show("Вы точно хотите удалить данную настройку?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
			if (result != MessageBoxResult.Yes)
				return;
			int index = ConfigurationList.IndexOf(CurrentConfig);
			if (ConfigurationList.Count == 1)
			{
				CurrentConfig = null;
			}
			else if (index == ConfigurationList.Count - 1)
				CurrentConfig = ConfigurationList[index - 1];
			else
				CurrentConfig = ConfigurationList[index + 1];
			AllConfigurationList.RemoveAt(index);
			NavigateToPreviousConfigCommand = null;
			NavigateToNextConfigCommand = null;
			DeleteConfigCommand = null;
			EditFileCommand = null;
			OnPropertyChanged(nameof(IsNotExistsConfiguration));
			OnPropertyChanged(nameof(ConfigurationList));
		}

		#endregion

		#region OpenGuideCommand

		/// <summary>
		/// Открыть инструкцию
		/// </summary>
		private RelayCommand _openGuideCommand;

		public RelayCommand? OpenGuideCommand => _openGuideCommand ??= new RelayCommand(OpenGuide, param => true);

		private void OpenGuide()
		{
			var window = new GuideWindow();
			window.GetGuide();
			window.Show();
		}

		#endregion

		#endregion

		#region Methods

		public static Bitmap ToImage(byte[] bytes)
		{
			using (var stream = new MemoryStream(bytes))
			using (var image = Image.FromStream(stream, false, true))
			{
				return new Bitmap(image);
			}
		}
		private static ImageCodecInfo GetEncoderInfo(String mimeType)
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

		private static BitmapImage CreateSource(byte[] bytes, string ext)
		{
			var bi = new BitmapImage();
			Bitmap bmp = ToImage(bytes);
			using (var ms = new MemoryStream())
			{
				ImageCodecInfo myImageCodecInfo;

				switch (ext)
				{
					case ".png":
						myImageCodecInfo = GetEncoderInfo("image/png");
						break;
					case ".jpeg":
						myImageCodecInfo = GetEncoderInfo("image/jpeg");
						break;
					default:
						myImageCodecInfo = GetEncoderInfo("image/jpeg");
						break;
				}

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
					new EncoderParameter(myEncoder, 24L);
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
		/// Для сортировки файлов в алфавитном порядке
		/// </summary>
		public class MyComparer : IComparer<string>
		{

			[DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern int StrCmpLogicalW(String x, String y);

			public int Compare(string x, string y)
			{
				return StrCmpLogicalW(x, y);
			}

		}

		#endregion
	}
}
