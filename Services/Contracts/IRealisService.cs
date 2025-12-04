namespace QTPCR.Services.Contracts
{
    public interface IRealisService
    {
        Task<HttpResponseMessage> GetRealisStressStatus(string json_standard);
        string GetVersion(string endpointName);
    }
}
