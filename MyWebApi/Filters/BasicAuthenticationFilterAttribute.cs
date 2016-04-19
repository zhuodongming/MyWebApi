using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MyWebApi.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class BasicAuthenticationFilterAttribute : AuthorizationFilterAttribute
    {
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var userIdentity = ParseHeader(actionContext);
            if (userIdentity == null)
            {
                Challenge(actionContext);
                return null;
            }

            if (!OnAuthorize(userIdentity.UserName, userIdentity.UserPassword, actionContext))
            {
                Challenge(actionContext);
                return null;
            }

            var principal = new GenericPrincipal(userIdentity, null);

            Thread.CurrentPrincipal = principal;

            return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }

        public virtual BasicAuthenticationIdentity ParseHeader(HttpActionContext actionContext)
        {
            string authParameter = null;

            var authValue = actionContext.Request.Headers.Authorization;
            if (authValue != null && authValue.Scheme == "Basic")
                authParameter = authValue.Parameter;

            if (string.IsNullOrEmpty(authParameter))

                return null;

            authParameter = Encoding.UTF8.GetString(Convert.FromBase64String(authParameter));

            var authToken = authParameter.Split(':');
            if (authToken.Length < 2)
                return null;

            return new BasicAuthenticationIdentity(authToken[0], authToken[1]);
        }

        void Challenge(HttpActionContext actionContext)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));
        }

        public virtual bool OnAuthorize(string userName, string userPassword, HttpActionContext actionContext)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPassword))

                return false;
            else
                return true;

        }
    }
}