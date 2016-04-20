using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MyWebApi.Filters
{
    /// <summary>
    /// 授权过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class AuthorizationRequiredFilterAttribute : AuthorizationFilterAttribute
    {
        public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            await Task.Delay(10);//此步骤从IO中获取token
            string serverToken = "1234567890";
            if (actionContext.Request.Headers.Contains("Token"))
            {
                var token = actionContext.Request.Headers.GetValues("Token").FirstOrDefault();
                if (!String.Equals(token, serverToken))
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
            }
            else
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            await base.OnAuthorizationAsync(actionContext, cancellationToken);
        }
    }
}