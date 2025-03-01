using System.Windows;

namespace AutoType.Classes
{
	/// <summary>
	/// Настройка наложения
	/// </summary>
	public class Configuration
	{
		/// <summary>
		/// Отспупы для рамки диалога
		/// </summary>
		public Thickness MarginForDialog { get; set; }

		/// <summary>
		/// Отступы для рамки места
		/// </summary>
		public Thickness MarginForPlace { get; set; }

		/// <summary>
		/// Отступы для меню
		/// </summary>
		public Thickness MarginForMenu { get; set; }
		
		/// <summary>
		/// Отступы для левой рамки места
		/// </summary>
		public Thickness MarginForLeftPlace { get; set; }

		/// <summary>
		/// Масштаб для всего
		/// </summary>
		public double Scale { get; set; } = 1;

		/// <summary>
		/// Название конфигурации
		/// </summary>
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
