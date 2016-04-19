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
        private static Dictionary<string, string> userAccounters;
        public AuthenticationController()
        {
            userAccounters = new Dictionary<string, string>();
            userAccounters.Add("Foo", "Password");
            userAccounters.Add("Bar", "Password");
            userAccounters.Add("Baz", "Password");
        }
        [OverrideExceptionFilters]
        [HttpGet]
        [Route("api/auth/{username}/{password}")]
        public Task<IHttpActionResult> Validate(string userName, string password)
        {
            string pwd;
            if (userAccounters.TryGetValue(userName, out pwd))
            {
                if (password != pwd)
                {
                    throw new ArgumentException("密码错误");
                }
                return Task.FromResult<IHttpActionResult>(Ok("认证成功"));
            }
            throw new ArgumentException("用户名不存在");
        }


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