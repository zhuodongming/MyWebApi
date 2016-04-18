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
    public class ActionFilterRequiredAttribute : ActionFilterAttribute
    {
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var token = actionContext.Request.Headers.GetValues("Token").FirstOrDefault();
            if (!String.IsNullOrWhiteSpace(token))
            {
                if (!String.Equals(token, Guid.NewGuid().ToString()))
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
            }
            else {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }
    }
}