using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Claims;
using Microsoft.Owin.Security.Cookies;
using System.Threading;


namespace QTPCR.Helpers
{
    public class ClaimsHelper
    {
        //public static string GetAccessToken()
        //{
        //    var owinContext = HttpContext.Current.GetOwinContext();
        //    var authentication = owinContext.Authentication;
        //    var authenticationTicket = authentication.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationType).Result;
        //    var accessToken = authenticationTicket?.Identity?.FindFirst("AccessToken")?.Value;
        //    return accessToken ?? string.Empty;
        //}

        public static string GetEnviromentAccessToken()
        {
            var accessToken = Environment.GetEnvironmentVariable("AccessToken");
            return accessToken ?? string.Empty;
        }

      
    }
}
