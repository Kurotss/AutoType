using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoType.Classes
{
	/// <summary>
	/// Класс по работе с сохранениями
	/// </summary>
	public class SaveManager
	{
		public static void CreateFolderForSave(string folderName)
		{
			string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/AutoType/Saves/" + folderName;
			Directory.CreateDirectory(saveFolder);


		}
	}
}
