using QTPCR.Models;

namespace QTPCR.Services.Contracts
{
    public interface IChangeRequestServices
    {
        Task<ChangeRequestResponse> GetRealisAllTestState(string qtpNumber);
    }
}
