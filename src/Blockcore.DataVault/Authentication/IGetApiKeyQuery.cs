using System.Threading.Tasks;

namespace Blockcore.DataVault.Authentication
{
    public interface IGetApiKeyQuery
    {
        Task<ApiKey> Execute(string providedApiKey);
    }
}
