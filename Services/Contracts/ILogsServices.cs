using QTPCR.Models;

namespace QTPCR.Services.Contracts
{
    public interface ILogsServices
    {
        Task<string> ErrorLog(ErrorModel errorModel);
    }
}
