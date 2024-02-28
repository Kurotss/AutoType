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

		private Action _execute;
		private Func<object, bool> _canExecute;

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			_execute();
		}
	}
}
