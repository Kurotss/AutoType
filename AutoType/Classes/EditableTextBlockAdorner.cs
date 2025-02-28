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
            Binding binding = new Binding("Text") {Source = adornedElement};
            _textBox.SetBinding(TextBox.TextProperty, binding);
            _textBox.AcceptsReturn = true;
            _textBox.MaxLength = adornedElement.MaxLength;
            //_textBox.LostFocus += _textBox_LostFocus;
            _textBox.Background = Brushes.Transparent;
            _textBox.Foreground = _textBlock.Fill;
            _textBox.FontSize = _textBlock.FontSize;
            _textBox.Background = Brushes.Transparent;
            _textBox.TextWrapping = _textBlock.TextWrapping;
            _textBox.FontFamily = _textBlock.FontFamily;
            _collection.Add(_textBox);
        }

		//void _textBox_LostFocus(object sender, RoutedEventArgs e)
		//{
		//    BindingExpression expression = _textBox.GetBindingExpression(TextBox.TextProperty);
		//    if (null != expression)
		//    {
		//        expression.UpdateSource();
		//    }
		//}

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
