using AspNetCore.Authentication.ApiKey;
using System.Threading.Tasks;

namespace SampleWebApi.Repositories
{
    /// <summary>
    /// NOTE: DO NOT USE THIS IMPLEMENTATION. THIS IS FOR DEMO PURPOSE ONLY
    /// </summary>
    public interface IApiKeyRepository
    {
        Task<IApiKey> GetApiKeyAsync(string key);

        Task<IApiKey> GetApiKeyAsync(ApiKeyValidateKeyContext context);
    }
}