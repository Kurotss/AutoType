using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AutoType.Classes;
using System.Drawing;
using Size = System.Windows.Size;
using System;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using System.Globalization;

namespace EditBlockTest
{
    /// <summary>
    /// Adorner class which shows textbox over the text block when the Edit mode is on.
    /// </summary>
    public class EditableTextBlockAdorner : Adorner
    {
        private readonly VisualCollection _collection;

        private readonly TextBox _textBox;

        private readonly OutlinedTextBlock _textBlock;

        public EditableTextBlockAdorner(OutlinedTextBox adornedElement)
            : base(adornedElement)
        {
            _collection = new VisualCollection(this);
            _textBox = new TextBox();
            _textBlock = adornedElement;
            Binding binding = new("Text") {Source = adornedElement};
            _textBox.SetBinding(TextBox.TextProperty, binding);
            _textBox.AcceptsReturn = true;
            _textBox.MaxLength = adornedElement.MaxLength;
            _textBox.TextChanged += AutoSizeTextBox_TextChanged;
			_textBox.Background = Brushes.Transparent;
            _textBox.Foreground = _textBlock.Fill;
            _textBox.FontSize = _textBlock.FontSize;
            _textBox.Background = Brushes.Transparent;
            _textBox.TextWrapping = _textBlock.TextWrapping;
            _textBox.FontFamily = _textBlock.FontFamily;
            _textBox.MaxLines = 9999;
            _collection.Add(_textBox);
        }

		private void AutoSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
		    if (sender is TextBox textBox)
		    {
		    	var textBlock = new TextBlock
		    	{
		    		Text = textBox.Text,
		    		FontFamily = textBox.FontFamily,
		    		FontSize = textBox.FontSize,
		    		FontStyle = textBox.FontStyle,
		    		FontWeight = textBox.FontWeight,
		    		TextWrapping = TextWrapping.Wrap,
		    		Width = textBox.ActualWidth - textBox.Padding.Left - textBox.Padding.Right
		    	};

		    	textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
		    	textBlock.Arrange(new Rect(new System.Windows.Point(0, 0), textBlock.DesiredSize));

		    	double textHeight = textBlock.DesiredSize.Height;

		    	double newHeight = Math.Min(
		    		textHeight + textBox.Padding.Top + textBox.Padding.Bottom + textBox.BorderThickness.Top + textBox.BorderThickness.Bottom + 5,
		    		textBox.MaxHeight
		    	);
		    	textBox.Height = Math.Max(newHeight, textBox.MinHeight);

		    	textBox.InvalidateVisual();
		    }
		}

	    protected override Visual GetVisualChild(int index)
        {
            return _collection[index];
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _collection.Count;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
			_textBox.Arrange(new Rect(0, 0, _textBlock.DesiredSize.Width + 50, _textBlock.DesiredSize.Height));
			_textBox.Focus();
			return finalSize;
		}


		protected override void OnRender(DrawingContext drawingContext)
        {
			drawingContext.DrawRectangle(Brushes.Transparent, new Pen
            {
                Brush = Brushes.Transparent,
                Thickness = 0,
            }, new Rect(0, 0, _textBlock.DesiredSize.Width + 50, _textBlock.DesiredSize.Height));
        }

		private void DynamicTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox)
			{
				// Создаем временный TextBlock для измерения текста
				var textBlock = new TextBlock
				{
					Text = textBox.Text,
					FontFamily = textBox.FontFamily,
					FontSize = textBox.FontSize,
					FontStyle = textBox.FontStyle,
					FontWeight = textBox.FontWeight,
					TextWrapping = TextWrapping.Wrap,
					Width = textBox.ActualWidth - textBox.Padding.Left - textBox.Padding.Right
				};

				// Измеряем размер текста
				textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				double textHeight = textBlock.DesiredSize.Height;

				// Вычисляем новую высоту TextBox
				double newHeight = Math.Min(textHeight + textBox.Padding.Top + textBox.Padding.Bottom, textBox.MaxHeight);
				textBox.Height = Math.Max(newHeight, textBox.MinHeight);
			}
		}

		public event RoutedEventHandler TextBoxLostFocus
        {
            add
            {
                _textBox.LostFocus += value;
            }
            remove
            {
                _textBox.LostFocus -= value;
            }
        }

        public event KeyEventHandler TextBoxKeyUp
        {
            add
            {
                _textBox.KeyUp += value;
            }
            remove
            {
                _textBox.KeyUp -= value;
            }
        }
    }
}
