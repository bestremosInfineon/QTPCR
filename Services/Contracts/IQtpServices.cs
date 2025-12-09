using QTPCR.Models;

namespace QTPCR.Services.Contracts
{
    public interface IQtpServices
    {
        Task<List<StressTestDetails>> StressTableRealisTestAll(string qtpNumber);
        Task<List<TestState>> GetAllowedTestStateForCR(string enable_cr);
        Task<string> GetConfigByAttribute(string attribute);
        Task<string> GetVersion(string endpointName);
    }
}
