using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AutoType.Classes
{
	/// <summary>
	/// Для сортировки файлов в алфавитном порядке
	/// </summary>
	public class MyComparer : IComparer<string>
	{
		[DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		static extern int StrCmpLogicalW(string x, string y);

		public int Compare(string x, string y)
		{
			return StrCmpLogicalW(x, y);
		}
	}
}
