using AutoType.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutoType.UserControls
{
	/// <summary>
	/// Логика окна с тайпом рамки диалога
	/// </summary>
	public partial class TypeFrame : UserControl
	{
		public TypeFrame(string characterName, string description, Configuration config, FrameMode frameMode, BitmapImage source)
		{
			InitializeComponent();
			//
			Height = frameMode == FrameMode.Old ? source.Height : (source.Height > 1080 ? 1080 : source.Height);
			Width = frameMode == FrameMode.Old ? source.Width : (source.Width > 2280 ? 2280 : source.Width);
			screen.Source = source;
			//
			if (frameMode == FrameMode.Old)
			{
				gridOld.Visibility = Visibility.Visible;
				//
				txtName.Text = characterName;
				if (characterName.Length > 16)
				{
					txtName.FontSize = 43;
					txtName.Margin = new Thickness(txtName.Margin.Left, txtName.Margin.Top, txtName.Margin.Right, txtName.Margin.Bottom - 6);
				}
				txtDescr.Text = description;
				gridFrame.RenderTransform = new ScaleTransform(config.Scale, config.Scale, 1167.5, 540);
				gridFrame.Margin = config.MarginForDialog;
				imgMenu.RenderTransform = new ScaleTransform(config.Scale, config.Scale, 1167.5, 540);
				imgMenu.Margin = config.MarginForMenu;
			}
			else if (frameMode == FrameMode.New)
			{
				gridNew.Visibility = Visibility.Visible;
				// пробел - костыль, ибо слева обрезается обводка имени
				txtNameNew.Text = " " + characterName;
				if (characterName.Length > 16)
					
					txtNameNew.FontSize = 43.3;
				txtDescrNew.Text = description;
			}
			//
			DataContext = this;
		}
	}
}
