using Newtonsoft.Json.Linq;

namespace QTPCR.Services.Contracts
{
    public interface ITokenServices
    {
        Task<object> GetTokenResponse(string code);
    }
}
