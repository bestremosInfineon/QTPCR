using QTPCR.Services.Contracts;


namespace QTPCR.Services.Contexts
{
    public class TokenServices : ITokenServices
    {
        private readonly IConfiguration _configuration;

        public TokenServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<object> GetTokenResponse(string code)
        {
            string tokenEndpoint = $"{Environment.GetEnvironmentVariable("Issuer") ?? _configuration["Issuer"]}/as/token.oauth2";
            string clientId = Environment.GetEnvironmentVariable("ClientId") ?? _configuration["ClientId"];
            string clientSecret = Environment.GetEnvironmentVariable("ClientSecret") ?? _configuration["ClientSecret"];

            HttpClient client = new();
            var uri = new Uri(tokenEndpoint);
            var values = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "scope", "openid" },
                { "grant_type", "password" },
            };

            var body = new FormUrlEncodedContent(values);
            Task<HttpResponseMessage> responseTask = client.PostAsync(uri, body);
            responseTask.Wait();
            HttpResponseMessage response = responseTask.Result;

            Task<string> responseStringTask = response.Content.ReadAsStringAsync();
            responseStringTask.Wait();
            string responseString = responseStringTask.Result;

            return responseString;

        }
    }
}
