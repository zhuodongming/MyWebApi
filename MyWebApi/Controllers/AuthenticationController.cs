using MyWebApi.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MyWebApi.Controllers
{
    public class AuthenticationController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> Authenticate()
        {
            IPrincipal principal = Thread.CurrentPrincipal;
            if (principal != null && principal.Identity.IsAuthenticated)
            {
                var baseAuthenticationIdentiy = principal.Identity as BasicAuthenticationIdentity;
                if (baseAuthenticationIdentiy != null)
                {
                    var userId = baseAuthenticationIdentiy.UserId;
                    return ResponseMessage(await GetAuthToken(userId));
                }
            }
            return null;
        }

        private async Task<HttpResponseMessage> GetAuthToken(int userId)
        {
            await Task.Delay(100);
            string token = Guid.NewGuid().ToString();
            var response = Request.CreateResponse(HttpStatusCode.OK, "Authorized");
            response.Headers.Add("Token", token);
            response.Headers.Add("TokenExpiry", ConfigurationManager.AppSettings["TokenExpiry"]);
            response.Headers.Add("Access-Control-Expose-Headers", "Token,TokenExpiry");
            return response;
        }
    }
}