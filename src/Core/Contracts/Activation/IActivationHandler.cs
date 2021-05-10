using System.Threading.Tasks;

namespace Seemon.Vault.Core.Contracts.Activation
{
    public interface IActivationHandler
    {
        bool CanHandle();

        Task HandleAsync();
    }
}
