using System;
using System.Windows.Input;

namespace DominatorHouseCore.Command
{
 public   class BaseCommand<T>: ICommand
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

        #endregion

        //Variables
        #region Variables

        Func<object, bool> CanExecuteMethod { get; set; }

        Action<object> ExecuteMethod { get; set; }

        #endregion

        //Implementation of ICommand
        #region Implementation of ICommand

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return this.CanExecuteMethod(parameter);
        }

        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged.Invoke(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {

            this.ExecuteMethod(parameter);
        }

        #endregion
    }
}
