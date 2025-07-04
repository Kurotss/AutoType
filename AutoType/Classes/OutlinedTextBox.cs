﻿using EditBlockTest;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace AutoType.Classes
{
	public class OutlinedTextBox : OutlinedTextBlock
	{
		public bool IsInEditMode
		{
			get
			{
				return (bool)GetValue(IsInEditModeProperty);
			}
			set
			{
				SetValue(IsInEditModeProperty, value);
			}
		}

		/// <summary>
		/// Делегат для действий при выходе из режима редактирования
		/// </summary>
		public Action UpdateEditModeActions;

		private EditableTextBlockAdorner _adorner;

		// Using a DependencyProperty as the backing store for IsInEditMode.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsInEditModeProperty =
			DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(OutlinedTextBox), new UIPropertyMetadata(false, IsInEditModeUpdate));

		/// <summary>
		/// Determines whether [is in edit mode update] [the specified obj].
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void IsInEditModeUpdate(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			OutlinedTextBox textBlock = obj as OutlinedTextBox;
			if (null != textBlock)
			{
				//Get the adorner layer of the uielement (here TextBlock)
				AdornerLayer layer = AdornerLayer.GetAdornerLayer(textBlock);

				//If the IsInEditMode set to true means the user has enabled the edit mode then
				//add the adorner to the adorner layer of the TextBlock.
				if (textBlock.IsInEditMode)
				{
					textBlock.Opacity = 0;
					if (null == textBlock._adorner)
					{
						textBlock._adorner = new EditableTextBlockAdorner(textBlock);

						//Events wired to exit edit mode when the user presses Enter key or leaves the control.
						textBlock._adorner.TextBoxKeyUp += textBlock.TextBoxKeyUp;
						textBlock._adorner.TextBoxLostFocus += textBlock.TextBoxLostFocus;
					}
					layer.Add(textBlock._adorner);
				}
				else
				{
					//Remove the adorner from the adorner layer.
					Adorner[] adorners = layer?.GetAdorners(textBlock);
					if (adorners != null)
					{
						foreach (Adorner adorner in adorners)
						{
							if (adorner is EditableTextBlockAdorner)
							{
								layer.Remove(adorner);
							}
						}
					}

					//Update the textblock's text binding.
					BindingExpression expression = textBlock.GetBindingExpression(TextProperty);
					expression?.UpdateTarget();
					textBlock.Opacity = 1;
				}

				textBlock.UpdateEditModeActions?.Invoke();
			}
		}

		/// <summary>
		/// Gets or sets the length of the max.
		/// </summary>
		/// <value>The length of the max.</value>
		public int MaxLength
		{
			get
			{
				return (int)GetValue(MaxLengthProperty);
			}
			set
			{
				SetValue(MaxLengthProperty, value);
			}
		}

		// Using a DependencyProperty as the backing store for MaxLength.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MaxLengthProperty =
			DependencyProperty.Register("MaxLength", typeof(int), typeof(OutlinedTextBox), new UIPropertyMetadata(0));

		private void TextBoxLostFocus(object sender, RoutedEventArgs e)
		{
			IsInEditMode = false;
		}

		/// <summary>
		/// release the edit mode when user presses escape.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
		private void TextBoxKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				IsInEditMode = false;
			}
		}

		/// <summary>
		/// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed)
			{
				IsInEditMode = true;
			}
			else if (e.ClickCount == 2)
			{
				IsInEditMode = true;
			}
		}
	}
}
