using Oracle.ManagedDataAccess.Client;
using QTPCR.Helpers;
using QTPCR.Services.Contracts;
using System.Text;

namespace QTPCR.Services.Contexts
{
    public class RealisServices : IRealisService

    {
        private readonly IConfiguration _configuration;

        public RealisServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<HttpResponseMessage> GetRealisStressStatus(string json_standard)
        {
            string connString = Environment.GetEnvironmentVariable("RealistHostName") ?? _configuration["RealisHostname:Dev"];
            try
            {
                string access_token = ClaimsHelper.GetEnviromentAccessToken();
                var handler = new WinHttpHandler();
                using (var client = new HttpClient(handler))
                {
                    var realis_hostname = connString;

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);


                    var content = new System.Net.Http.StringContent(json_standard, Encoding.UTF8, "application/json");
                    string version = GetVersion("GET-TESTS-STATE");
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri(realis_hostname + "/realis" + version + "/project/tests-state"),
                        Content = content
                    };

                    HttpResponseMessage message = new HttpResponseMessage();
                    message = await client.SendAsync(request).ConfigureAwait(false);

                    return message;
                }
            }
            catch (HttpRequestException ex)
            {

                var responseText = ex.InnerException.Message;
                throw;
            }

        }

        public string GetVersion(string endpointName)
        {
            string connString = Environment.GetEnvironmentVariable("QTPConnectionString") ?? _configuration["ConnectionStrings:QTP"];

            using (var connection = new OracleConnection(connString))
            {
                connection.Open();
                using (var command = new OracleCommand("SELECT VERSION FROM QTP_REALIS_ENDPOINT_VERSION WHERE ENDPOINT_NAME = '" + endpointName + "'  AND IS_USED = 'Y'", connection))
                {
                    command.Parameters.Add(new OracleParameter("endpointName", endpointName));
                    var result = command.ExecuteScalar();
                    if (result == null)
                    {
                        throw new InvalidOperationException($"The endpoint '{endpointName}' is deprecated.");
                    }
                    return result.ToString();
                }
            }
        }
 
    }
}
