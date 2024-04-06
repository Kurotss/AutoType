using System.Windows;

namespace AutoType.Classes
{
	public class Configuration
	{
		public Thickness MarginForDialog { get; set; }

		public Thickness MarginForPlace { get; set; }

		public Thickness MarginForMenu { get; set; }
		public Thickness MarginForLeftPlace { get; set; }

		/// <summary>
		/// Масштаб для всего
		/// </summary>
		public double Scale { get; set; } = 1;

		public string ConfigurationName { get; set; }

		public bool IsSelected { get; set; }
	}
}
