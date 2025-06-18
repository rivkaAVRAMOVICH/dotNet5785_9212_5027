using System;
using System.Windows.Input;

namespace PL.Volunteer
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        private Action<object> chooseCall;
        private Func<object, bool> canChooseCall;
        private Action updateAddress;

        public RelayCommand(Action updateAddress)
        {
            this.updateAddress = updateAddress;
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object> chooseCall, Func<object, bool> canChooseCall)
        {
            this.chooseCall = chooseCall;
            this.canChooseCall = canChooseCall;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();

        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
