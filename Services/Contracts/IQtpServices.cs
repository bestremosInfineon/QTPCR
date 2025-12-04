using QTPCR.Models;

namespace QTPCR.Services.Contracts
{
    public interface IQtpServices
    {
        Task<List<StressTestDetails>> StressTableRealisTestAll(string qtpNumber);
    }
}
