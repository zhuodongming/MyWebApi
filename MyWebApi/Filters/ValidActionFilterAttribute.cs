using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using NLog;

namespace MyWebApi.Filters
{
    /// <summary>
    /// Action的参数验证过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ValidActionFilterAttribute : ActionFilterAttribute
    {
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (!actionContext.ModelState.IsValid)
            {
                StringBuilder message = new StringBuilder(200);
                message.Append("URL: " + actionContext.Request.RequestUri + Environment.NewLine);
                message.Append("Method: " + actionContext.Request.Method + Environment.NewLine);
                message.Append("Headers: " + JsonConvert.SerializeObject(actionContext.Request.Headers) + Environment.NewLine);
                message.Append("Controller: " + actionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine);
                message.Append("Action: " + actionContext.ActionDescriptor.ActionName + Environment.NewLine);
                message.Append("Action Parameters: " + JsonConvert.SerializeObject(actionContext.ActionArguments));

                Logger.Info("参数绑定失败:" + message.ToString());

                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
            }
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }
    }
}