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

		private FrameMode _frameMode;

		/// <summary>
		/// Тип рамки: новая или старая, по умолчанию старая
		/// </summary>
		public FrameMode FrameMode
		{
			get => _frameMode;
			set
			{
				if (value == 0)
					_frameMode = FrameMode.Old;
				else
					_frameMode = value;
			}
		}
	}
}
