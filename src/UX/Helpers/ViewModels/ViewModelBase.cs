using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Seemon.Vault.Core.Contracts.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Seemon.Vault.Helpers.ViewModels
{
    public class ViewModelBase : ObservableValidator, IViewModel
    {
        private readonly IList<ICommand> _commands;

        public ViewModelBase() => _commands = new List<ICommand>();

        public string PageKey => this.GetType().FullName;

        public IList<ICommand> Commands => _commands;

        public ICommand RegisterCommand<T>(Action<T> execute, Predicate<T> canExecute = null)
        {
            var command = new RelayCommand<T>(execute, canExecute);
            _commands.Add(command);
            return command;
        }

        public ICommand RegisterCommand(Action execute, Func<bool> canExecute = null)
        {
            var command = new RelayCommand(execute, canExecute);
            _commands.Add(command);
            return command;
        }

        public void RaiseCommandsCanExecute()
        {
            foreach (IRelayCommand command in _commands)
            {
                command.NotifyCanExecuteChanged();
            }
        }

        public virtual void SetModel(object model) { }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            RaiseCommandsCanExecute();
        }
    }
}
