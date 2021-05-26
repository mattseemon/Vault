using Seemon.Vault.Contracts.Services;
using Seemon.Vault.Helpers;
using System.Windows.Input;

namespace Seemon.Vault.ViewModels
{
    public class TaskbarIconViewModel : ViewModelBase
    {
        private readonly ITaskbarIconService _taskbarIconService;

        private ICommand _showCommand;
        private ICommand _exitCommand;

        public TaskbarIconViewModel(ITaskbarIconService taskbarIconService) => _taskbarIconService = taskbarIconService;

        public ICommand ShowCommand => _showCommand ??= RegisterCommand(OnShowCommand);

        public ICommand ExitCommand => _exitCommand ??= RegisterCommand(OnExitCommand);

        private void OnExitCommand() => App.Current.Shutdown();

        private void OnShowCommand() => _taskbarIconService.Show();
    }
}
