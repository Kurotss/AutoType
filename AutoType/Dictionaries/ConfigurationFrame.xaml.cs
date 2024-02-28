using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoType.Dictionaries
{
	public partial class ConfigurationFrame : ResourceDictionary
	{
		private void DoDragFrame(object sender, MouseButtonEventArgs e)
		{
			// Initiate the drag-and-drop operation.
			DragDrop.DoDragDrop((Image)sender, new DataObject(DataFormats.Serializable, (Image)sender), DragDropEffects.Copy | DragDropEffects.Move);
		}

		private void Grid_DragOver(object sender, DragEventArgs e)
		{
			object obj = e.Data.GetData(DataFormats.Serializable);
			Point dropPosition = e.GetPosition((Canvas)sender);
			double? actualWidth = (obj as Image)?.Width;
			double? actualHeight = (obj as Image)?.Height;
			if (dropPosition.X + actualWidth <= ((Canvas)sender).Width && dropPosition.X >= 0 &&
				dropPosition.Y + actualHeight <= ((Canvas)sender).Height && dropPosition.Y >= 0)
			{
				Canvas.SetLeft(obj as Image, dropPosition.X);
				Canvas.SetTop(obj as Image, dropPosition.Y);
			}
		}
	}
}
