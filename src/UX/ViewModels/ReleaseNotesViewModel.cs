using Seemon.Vault.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seemon.Vault.ViewModels
{
    public class ReleaseNotesViewModel : ViewModelBase
    {
        private string _mdContent;

        public string MDContents
        {
            get => _mdContent; set => SetProperty(ref _mdContent, value);
        }

        public override void SetModel(object model)
        {
            MDContents = model.ToString();
        }
    }
}
