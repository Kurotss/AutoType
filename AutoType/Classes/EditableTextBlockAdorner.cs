using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AutoType.Classes;
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
		   var textBlock = new TextBlock
		   {
		        Text = _textBox.Text,
		        FontFamily = _textBox.FontFamily,
		        FontSize = _textBox.FontSize,
		        FontStyle = _textBox.FontStyle,
		        FontWeight = _textBox.FontWeight,
		        TextWrapping = TextWrapping.Wrap,
		        Width = _textBox.ActualWidth - _textBox.Padding.Left - _textBox.Padding.Right
		   };

		   textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
		   textBlock.Arrange(new Rect(new System.Windows.Point(0, 0), textBlock.DesiredSize));

		   double textHeight = textBlock.DesiredSize.Height;

		   double newHeight = Math.Min(
		   	textHeight + _textBox.Padding.Top + _textBox.Padding.Bottom + _textBox.BorderThickness.Top
                 + _textBox.BorderThickness.Bottom + 5, _textBox.MaxHeight
		   );
		   _textBox.Height = Math.Max(newHeight, _textBox.MinHeight);

		    _textBox.InvalidateVisual();
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
            // если не определена высота textbox (в первый раз запускаем), то берём высоту textblock
            var height = double.IsNaN(_textBox.Height) ? _textBlock.DesiredSize.Height : _textBox.Height;
			_textBox.Arrange(new Rect(0, 0, _textBlock.DesiredSize.Width + 50, height));
			_textBox.Focus();
			return finalSize;
		}


		protected override void OnRender(DrawingContext drawingContext)
		{
			// если не определена высота textbox (в первый раз запускаем), то берём высоту textblock
			var height = double.IsNaN(_textBox.Height) ? _textBlock.DesiredSize.Height : _textBox.Height;
			drawingContext.DrawRectangle(Brushes.Transparent, new Pen
            {
                Brush = Brushes.Transparent,
                Thickness = 0,
            }, new Rect(0, 0, _textBlock.DesiredSize.Width + 50, height));
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
