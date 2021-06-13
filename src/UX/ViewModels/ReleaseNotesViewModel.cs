using Seemon.Vault.Helpers.ViewModels;

namespace Seemon.Vault.ViewModels
{
    public class ReleaseNotesViewModel : ViewModelBase
    {
        private string _mdContent;

        public string MDContents
        {
            get => _mdContent; set => SetProperty(ref _mdContent, value);
        }

        public override void SetModel(object model) => MDContents = model.ToString();
    }
}
