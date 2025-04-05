using System;
using System.Windows.Input;

namespace AutoType.Classes
{
	public class RelayCommand : ICommand
	{
		public RelayCommand(Action execute, Func<object, bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		// Конструктор для команд с параметром
		public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
		{
			_executeWithParameter = execute ?? throw new ArgumentNullException(nameof(execute));
			_canExecute = canExecute;
		}

		private Action _execute;
		private readonly Action<object> _executeWithParameter;
		private Func<object, bool> _canExecute;

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			if (_executeWithParameter != null)
			{
				_executeWithParameter(parameter);
			}
			else if (_execute != null)
			{
				_execute();
			}
		}
	}
}
