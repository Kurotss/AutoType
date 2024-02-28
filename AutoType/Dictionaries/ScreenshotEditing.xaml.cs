using System;
using System.Windows;
using System.Windows.Controls;

namespace AutoType.Dictionaries
{
	public partial class ScreenshotEditing : ResourceDictionary
	{
		private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
		{
			((MediaElement)sender).Position = ((MediaElement)sender).Position.Add(TimeSpan.FromMilliseconds(1));
		}
	}
}
