using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Znak
{
    public class SimpleCommand : ICommand
    {
        public bool CommandSucceeded { get; set; }

        public Func<object, bool> CanExecuteDelegate { get; set; }

        public Action<object> ExecuteDelegate { get; set; }

        public SimpleCommand(Action p_action, Func<bool> canExecute = null)
        {
            ExecuteDelegate = _ => p_action();
            CanExecuteDelegate = canExecute == null ? null : _ => canExecute();
        }
        public SimpleCommand(Action<object> p_action, Func<object, bool> canExecute = null)
        {
            ExecuteDelegate = p_action;
            CanExecuteDelegate = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return CanExecuteDelegate == null || CanExecuteDelegate(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null && CanExecute(parameter))
            {
                ExecuteDelegate(parameter);
                CommandSucceeded = true;
            }
        }
    }
}
