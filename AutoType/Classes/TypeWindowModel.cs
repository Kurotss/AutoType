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
				// либо файла с настройками нет, либо получаем из файла все настройки и загружаем в программу 
				if (!File.Exists(path))
					IsNotExistsConfiguration = true;
				else
				{
					using StreamReader reader = new(path);
					string text = reader.ReadToEnd();
					ConfigurationList = JsonSerializer.Deserialize<List<Configuration>>(text);
					CurrentConfig = ConfigurationList.Find(item => item.IsSelected);
				}
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
		public bool IsNotExistsConfiguration
		{
			get => _isNotExistsConfiguration;
			set
			{
				_isNotExistsConfiguration = value;
				OnPropertyChanged(nameof(IsNotExistsConfiguration));
			}
		}

		#endregion

		#region ConfigurationList

		private List<Configuration> _сonfigurationList;

		/// <summary>
		/// Список настроек наложения
		/// </summary>
		public List<Configuration> ConfigurationList
		{
			get => _сonfigurationList ??= new List<Configuration>();
			set => _сonfigurationList = value;
		}

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
				EditFileCommand = null;
				OnPropertyChanged(nameof(FrameMode));
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
				ConfigurationList.ForEach(item => item.IsSelected = false);
				if (_currentConfig != null)
					_currentConfig.IsSelected = true;
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
			get => _editFileCommand ??= new RelayCommand(EditFile, param => !IsNotExistsConfiguration || FrameMode == FrameMode.New);
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
			int filesCnt = Images.Where(item => item.Type == FileTypes.Place || (item.Type != FileTypes.Menu && item.Type != FileTypes.None)).Count();
			if (stringCnt != filesCnt)
			{
				MessageBox.Show($"Количество скриншотов ({filesCnt}) не соотвествует количеству строк текста ({stringCnt}). Проверьте и выберите папку со скриншотами или файл с текстом заново.");
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

		/// <summary>
		/// Получение всех строк с переводом из документа и цикл с перебором всех скриншотов
		/// </summary>
		async Task Type()
		{
			// цикл с перебором всех скриншотов для наложения
			var i = 0;
			foreach (TypeScreen typeScreen in Images)
			{
				var thread = new Thread(() =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						try
						{
							// переменная с текстом для скриншота
							string line = string.Empty;
							// пропускаем пустые строки и ищем следующую строку с текстом
							if (typeScreen.Type == FileTypes.Place || typeScreen.Type == FileTypes.Dialog)
							{
								do
								{
									line = AllText[i];
									i++;
								}
								while (string.IsNullOrEmpty(line));
							}
							//
							switch (typeScreen.Type)
							{
								case FileTypes.Menu:
									{
										typeScreen.UserControl = new TypeLocation("", false, CurrentConfig ?? new Configuration(), FrameMode, typeScreen.FileSource);
										break;
									}
								case FileTypes.Place:
									{
										typeScreen.UserControl = new TypeLocation(line, true, CurrentConfig ?? new Configuration(), FrameMode, typeScreen.FileSource);
										break;
									}
								case FileTypes.None:
									break;
								default:
									{
										// делим строку на имя и реплику. если в реплике было :, то склеиваем реплику
										string[] lines = line.Split(':');
										if (lines.Length > 2)
											for (int j = 2; j < lines.Length; j++)
												lines[1] += ':' + lines[j];
										try
										{
											typeScreen.UserControl = new TypeFrame(lines[0].Trim(), lines[1].Trim(), CurrentConfig ?? new Configuration(), FrameMode, typeScreen.FileSource);
										}
										catch
										{
											MessageBox.Show($"Неправильная разметка на строке {i} ({lines[0]})");
											IsEditMode = false;
											return;
										}
										break;
									}
							}
							// делаем Source, чтобы отобразить на превью в списке
							typeScreen.Source = typeScreen.Type == FileTypes.None ? typeScreen.FileSource : typeScreen.MakeSource(typeScreen.MakeImage());
							CurrentScreen++;
						}
						catch (Exception e)
						{
							throw;
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

			try
			{
				IEnumerable<TypeScreen> dialogScreens = Images.Where(item => item.Type == FileTypes.Dialog);
				if (dialogScreens.Count() < 1)
				{
					MessageBox.Show("Отсутствует изображение с диалогом!");
					return;
				}

				ConfigurationWindow = new ConfigurationWindow
				{
					DataContext = new ConfigurationFrameModel(FileTypes.Dialog, dialogScreens.FirstOrDefault().FileSource, null) { Save = GetConfiguration }
				};
				ConfigurationWindow.Show();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
				return;
			}
		}

		private void GetConfiguration()
		{
			ConfigurationFrameModel dataContext = ConfigurationWindow.DataContext as ConfigurationFrameModel;
			Configuration config = dataContext.MakeConfiguration();
			ConfigurationWindow.Close();
			if (dataContext.FileType == FileTypes.Dialog)
			{
				IEnumerable<TypeScreen> placeScreens = Images.Where(item => item.Type == FileTypes.Place);
				if (placeScreens.Count() < 1)
				{
					MessageBox.Show("Отсутствует изображение с местом!");
					return;
				}

				ConfigurationWindow = new ConfigurationWindow
				{
					DataContext = new ConfigurationFrameModel(FileTypes.Place, placeScreens.FirstOrDefault().FileSource, config) { Save = GetConfiguration }
				};
				ConfigurationWindow.Show();
			}
			else
			{
				ConfigurationList.Add(config);
				IsNotExistsConfiguration = false;
				OnPropertyChanged(nameof(IsNotExistsConfiguration));
				EditFileCommand = null;
				CurrentConfig = config;
				NavigateToPreviousConfigCommand = null;
				NavigateToNextConfigCommand = null;
				DeleteConfigCommand = null;
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
				IsNotExistsConfiguration = true;
			}
			else if (index == ConfigurationList.Count - 1)
				CurrentConfig = ConfigurationList[index - 1];
			else
				CurrentConfig = ConfigurationList[index + 1];
			ConfigurationList.RemoveAt(index);
			NavigateToPreviousConfigCommand = null;
			NavigateToNextConfigCommand = null;
			DeleteConfigCommand = null;
			EditFileCommand = null;
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

		private static BitmapImage CreateSource(byte[] bytes, string ext)
		{
			var bi = new BitmapImage();
			MemoryStream stream = new(bytes);
			Bitmap bmp = (Bitmap)Image.FromStream(stream);
			using (var ms = new MemoryStream())
			{
				switch (ext)
				{
					case ".png":
						bmp.Save(ms, ImageFormat.Png);
						break;
					case ".jpeg":
						bmp.Save(ms, ImageFormat.Jpeg);
						break;
					default:
						bmp.Save(ms, ImageFormat.Jpeg);
						break;
				}
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
