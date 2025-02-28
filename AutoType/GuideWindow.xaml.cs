using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Xps.Packaging;

namespace AutoType
{
	/// <summary>
	/// Логика взаимодействия для GuideWindow.xaml
	/// </summary>
	public partial class GuideWindow : Window
	{
		public GuideWindow()
		{
			InitializeComponent();
		}

		private readonly string Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\AutoType\\guide.xps";

		public void GetGuide()
		{
			string url = "https://drive.google.com/uc?export=download&id=1v8R-pdOtLQ19eG3doc9XVjJlsZxpElKd";
			try
			{
				using (WebClient client = new())
				{
					client.DownloadFileCompleted += client_DownloadFileCompleted;
					client.DownloadFileTaskAsync(url, Path);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		public void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if (!e.Cancelled && e.Error == null)
				LoadDocument();
			else MessageBox.Show($"{e.Error}");
		}

		public void LoadDocument()
		{
			using XpsDocument doc = new(Path, FileAccess.Read);
			documentViewer.Document = doc.GetFixedDocumentSequence();
		}
	}
}
