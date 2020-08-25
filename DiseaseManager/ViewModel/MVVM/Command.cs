using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DiseaseManager.ViewModel.MVVM
{
    class Command : ICommand
    {
        private readonly Action<object> action;
        readonly Predicate<object> canExecute;
        public Command(Action<object> action) : this(action, null) { }
        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }
        public Command(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            action = execute;
            this.canExecute = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }
    }
}
