using System;
using System.Windows.Input;

namespace DominatorHouseCore.Command
{
    public class BaseCommand<T> : ICommand
    {
        //Constructor
        #region Constructor

        public BaseCommand()
        {

        }

        public BaseCommand(Func<object, bool> CanExecuteMethod, Action<object> ExecuteMethod)
        {
            this.CanExecuteMethod = CanExecuteMethod;
            this.ExecuteMethod = ExecuteMethod;
        }

        public BaseCommand(Action<object,bool> ExecuteMethod)
        {
            this.ExecuteMethod2 = ExecuteMethod;
        }

        #endregion

        //Variables
        #region Variables

        Func<object, bool> CanExecuteMethod { get; set; }
        Func<object,bool, bool> CanExecuteMethod2 { get; set; }

        Action<object> ExecuteMethod { get; set; }
        Action<object,bool> ExecuteMethod2 { get; set; }

        #endregion

        //Implementation of ICommand
        #region Implementation of ICommand

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return CanExecuteMethod != null && CanExecuteMethod(parameter);
        }
        public bool CanExecute(object parameter1, bool parameter2)
        {
            return CanExecuteMethod2 != null && CanExecuteMethod2(parameter1,parameter2);
        }

        public void Execute(object parameter)
        {
            ExecuteMethod(parameter);
        }
        public void Execute(object parameter1,bool parameter2)
        {
            ExecuteMethod2(parameter1,parameter2);
        }

        #endregion
    }
}
