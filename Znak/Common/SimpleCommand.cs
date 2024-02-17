using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Znak.Common
{
    public class SimpleCommand : ICommand
    {
        public bool CommandSucceeded { get; set; }

        public Predicate<object> CanExecuteDelegate { get; set; }

        public Action<object> ExecuteDelegate { get; set; }

        public SimpleCommand(Action p_action, Func<bool> canExecute = null)
        {
            ExecuteDelegate = o => p_action();
            if (canExecute != null)
                CanExecuteDelegate = o => canExecute();
        }
        public SimpleCommand(Action<object> p_action, Predicate<object> canExecute = null)
        {
            ExecuteDelegate = p_action;
            CanExecuteDelegate = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return CanExecuteDelegate?.Invoke(parameter) ?? true;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null)
            {
                ExecuteDelegate(parameter);
                CommandSucceeded = true;
            }
        }
    }
}
