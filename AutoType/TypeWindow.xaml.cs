using AutoType.Classes;
using System.IO;
using System.Text.Json;
using System;
using System.Windows;

namespace AutoType
{
	/// <summary>
	/// Логика взаимодействия для TypeWindow.xaml
	/// </summary>
	public partial class TypeWindow : Window
	{
		public TypeWindow()
		{
			InitializeComponent();
			DataContext = new TypeWindowModel();
		}

		/// <summary>
		/// Записываем текущие настройки наложения в файл во время закрытия программы
		/// </summary>
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			TypeWindowModel context = (TypeWindowModel)DataContext;


			if (context.ConfigurationList.Count > 0)
			{
				try
				{
					string json = JsonSerializer.Serialize(context.AllConfigurationList);
					using StreamWriter writer = new(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/AutoType/ConfigurationSekai.txt", false);
					writer.WriteLineAsync(json);
				}
				catch (Exception exception)
				{
					MessageBox.Show(exception.Message);
				}
			}
			else
				try
				{
					File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/AutoType/ConfigurationSekai.txt");
				}
				catch (Exception exception)
				{
					MessageBox.Show(exception.Message);
				}
		}
    }
}
