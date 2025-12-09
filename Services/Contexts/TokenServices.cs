using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QTPCR.Services.Contracts;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Dependencies;
using System.Web.Mvc;


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

            string wAccessToken;

            ////--------------------------- Approch-1 to get token using HttpClient -------------------------------------------------------------------------------------
            //HttpResponseMessage responseMessage;
            //using (HttpClient client = new HttpClient())
            //{
            //    HttpRequestMessage tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
            //    HttpContent httpContent = new FormUrlEncodedContent(
            //            new[]
            //            {
            //                            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            //            });
            //    tokenRequest.Content = httpContent;
            //    tokenRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(wClientId + ":" + wClientSecretKey)));
            //    responseMessage = client.SendAsync(tokenRequest).Result;
            //}
            //string ResponseJSON = responseMessage.Content.ReadAsStringAsync().Result;


            ////--------------------------- Approch-2 to get token using HttpWebRequest and deserialize json object into ResponseModel class -------------------------------------------------------------------------------------


            //byte[] byte1 = Encoding.ASCII.GetBytes("grant_type=client_credentials");

            //HttpWebRequest oRequest = WebRequest.Create(tokenEndpoint) as HttpWebRequest;
            //oRequest.Accept = "application/json";
            //oRequest.Method = "POST";
            //oRequest.ContentType = "application/x-www-form-urlencoded";
            //oRequest.ContentLength = byte1.Length;
            //oRequest.KeepAlive = false;
            //oRequest.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(wClientId + ":" + wClientSecretKey)));
            //Stream newStream = oRequest.GetRequestStream();
            //newStream.Write(byte1, 0, byte1.Length);

            //WebResponse oResponse = oRequest.GetResponse();

            //using (var reader = new StreamReader(oResponse.GetResponseStream(), Encoding.UTF8))
            //{
            //    var oJsonReponse = reader.ReadToEnd();
            //    ResponseModel oModel = JsonConvert.DeserializeObject<ResponseModel>(oJsonReponse);
            //    wAccessToken = oModel.access_token;
            //}


            //var client = new RestClient(tokenEndpoint);
            //var request = new RestRequest(tokenEndpoint, Method.Post);
            //request.AddHeader("cache-control", "no-cache");
            //request.AddHeader("content-type", "application/x-www-form-urlencoded");
            //request.AddParameter("application/x-www-form-urlencoded", $"grant_type=client_credentials&client_id={wClientId}&client_secret={wClientSecretKey}", ParameterType.RequestBody);
            //var response = client.Execute(request);




            //var client = new RestClient(tokenEndpoint);
            //var request = new RestRequest(tokenEndpoint, Method.Post);
            //request.AddHeader("cache-control", "no-cache");
            //request.AddHeader("content-type", "application/x-www-form-urlencoded");
            //request.AddParameter("application/x-www-form-urlencoded", $"grant_type=client_credentials&scope=all&client_id={wClientId}&client_secret={wClientSecretKey}", ParameterType.RequestBody);
            //RestResponse response = client.Execute(request);

            //dynamic resp = JObject.Parse(response.Content);
            //string token = resp.access_token;

            //client = new RestClient("https://xxx.xxx.com/services/api/x/users/v1/employees");
            //request = new RestRequest(tokenEndpoint, Method.Post);
            //request.AddHeader("authorization", "Bearer " + token);
            //request.AddHeader("cache-control", "no-cache");
            //response = client.Execute(request);


            //using var client = new HttpClient();
            //var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

            //var formData = new Dictionary<string, string>
            //{
            //    { "client_id", clientId },
            //    { "client_secret", clientSecret },
            //    { "grant_type", "client_credentials" }
            //};

            //request.Content = new FormUrlEncodedContent(formData);

            //var response = await client.SendAsync(request);
            //response.EnsureSuccessStatusCode();

            //var tokenResponse = await response.Content.ReadAsStringAsync();
            //var token = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse);


            string tokenEndpoint = _configuration["Issuer"] + "/as/token.oauth2";
            string clientId = _configuration["ClientId"];
            string clientSecret = _configuration["ClientSecret"];
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

            var formData = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "grant_type", "client_credentials" }
            };

            request.Content = new FormUrlEncodedContent(formData);

            try
            {
                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<OAuthError>(responseContent);

                    Console.WriteLine($"OAuth Error: {error.Error} - {error.ErrorDescription}");

                    throw new HttpRequestException(
                        $"OAuth Request Failed - Status Code: {response.StatusCode}, Error: {error.Error}, Description: {error.ErrorDescription}");
                }

                var tokenResponse = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<TokenResponse>(tokenResponse);
                return token.AccessToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAccessTokenAsync: {ex.Message}");
                throw;
            }



            return null;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //---------------------------------- Response Class---------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// De-serialize Web response Object into model class to read  
    /// </summary>
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
    }

    public class OAuthError
    {
        public string Error { get; set; }
        public string ErrorDescription { get; set; }
    }
}
