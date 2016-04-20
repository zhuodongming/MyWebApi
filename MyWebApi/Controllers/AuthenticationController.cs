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
using System.Net.Http.Headers;

namespace MyWebApi.Controllers
{
    public class AuthenticationController : ApiController
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [Authenticate]//启用认证过滤器
        [OverrideAuthorization]//不启用授权过滤器
        [HttpPost]
        public async Task<HttpResponseMessage> Validate()
        {
            string token = "1234567890";//令牌
            var response = Request.CreateResponse(HttpStatusCode.OK, "登录成功");
            response.Headers.Add("Token", token);
            response.Headers.Add("TokenExpiry", ConfigurationManager.AppSettings["TokenExpiry"]);
            response.Headers.Add("Access-Control-Expose-Headers", "Token,TokenExpiry");

            CookieHeaderValue[] cookies = new CookieHeaderValue[] { new CookieHeaderValue("Token", token) };
            response.Headers.AddCookies(cookies);
            return await Task.FromResult<HttpResponseMessage>(response);
        }

        public async Task<IHttpActionResult> Get()
        {
            return await Task.FromResult(Ok("成功调用"));
        }
    }
}