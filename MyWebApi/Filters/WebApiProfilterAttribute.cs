using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MyWebApi.Filters
{
    /// <summary>
    /// webapi处理性能监视器
    /// </summary>
    public class WebApiProfilterAttribute : ActionFilterAttribute
    {
        int executionTime = 50;
        private Stopwatch timer = null;
        private readonly static Logger Logger = LogManager.GetCurrentClassLogger();
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            timer = new Stopwatch();
            timer.Start();
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            timer.Stop();
            if (timer.ElapsedMilliseconds >= executionTime)
            {
                StringBuilder message = new StringBuilder(200);
                message.Append("URL: " + actionExecutedContext.Request.RequestUri + Environment.NewLine);
                message.Append("Method: " + actionExecutedContext.Request.Method + Environment.NewLine);
                message.Append("Headers: " + JsonConvert.SerializeObject(actionExecutedContext.Request.Headers) + Environment.NewLine);
                message.Append("Controller: " + actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine);
                message.Append("Action: " + actionExecutedContext.ActionContext.ActionDescriptor.ActionName + Environment.NewLine);
                message.Append("Action Parameters: " + JsonConvert.SerializeObject(actionExecutedContext.ActionContext.ActionArguments));

                Logger.Info("webapi execution time is {0} ms: {1}", timer.ElapsedMilliseconds, message.ToString());
            }
            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }
    }
}