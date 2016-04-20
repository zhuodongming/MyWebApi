using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace MyWebApi.Filters
{
    /// <summary>
    /// Https检验过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class RequiredHttpsAttribute : AuthorizationFilterAttribute
    {
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            //如果当前为HTTPS请求，授权通过
            if (actionContext.Request.RequestUri.Scheme == Uri.UriSchemeHttps)
            {
                return base.OnAuthorizationAsync(actionContext, cancellationToken);
            }

            //对于HTTP-GET请求，将Scheme替换成https进行重定向
            if (actionContext.Request.Method == HttpMethod.Get)
            {
                Uri requestUri = actionContext.Request.RequestUri;
                string location = string.Format("Https://{0}/{1}", requestUri.Host, requestUri.LocalPath.TrimStart('/'));
                IHttpActionResult actionResult = new RedirectResult(new Uri(location), actionContext.Request);
                actionContext.Response = actionResult.ExecuteAsync(cancellationToken).Result;
                return Task.FromResult<object>(null);
            }

            //采用其他HTTP方法的请求被视为Bad Request
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "SSL Required" };
            return Task.FromResult<object>(null);
        }
    }
}