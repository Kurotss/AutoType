using System;

namespace AutoType.Classes
{
	/// <summary>
	/// Класс для одного сохранения
	/// </summary>
	public class SaveFolder : BaseDataContext
	{
		public SaveFolder(string fullPath, string folderName, DateTime creationDate)
		{
			FullPath = fullPath;
			FolderName = folderName;
			CreationDate = creationDate;
		}

		/// <summary>
		/// Полный путь к сохранению
		/// </summary>
		public string FullPath { get; set; }

		/// <summary>
		/// Название последней папки
		/// </summary>
		public string FolderName
		{
			get => _folderName;
			set
			{
				_folderName = value;
				OnPropertyChanged(nameof(FolderName));
			}
		}

		private string _folderName;

		/// <summary>
		/// Дата создания папки
		/// </summary>
		public DateTime CreationDate { get; set; }
	}
}
