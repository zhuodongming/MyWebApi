using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace MyWebApi.Filters
{
    /// <summary>
    /// 异常处理过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ExceptionHandlerFilterAttribute : ExceptionFilterAttribute
    {
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            StringBuilder message = new StringBuilder(200);
            message.Append("URL: " + actionExecutedContext.Request.RequestUri + Environment.NewLine);
            message.Append("Method: " + actionExecutedContext.Request.Method + Environment.NewLine);
            message.Append("Headers: " + JsonConvert.SerializeObject(actionExecutedContext.Request.Headers) + Environment.NewLine);
            message.Append("Controller: " + actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine);
            message.Append("Action: " + actionExecutedContext.ActionContext.ActionDescriptor.ActionName + Environment.NewLine);
            message.Append("Action Parameters: " + JsonConvert.SerializeObject(actionExecutedContext.ActionContext.ActionArguments));

            Logger.Error<Exception>(message.ToString(), actionExecutedContext.Exception);

            var exceptionType = actionExecutedContext.Exception.GetType();

            if (exceptionType == typeof(ValidationException))
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.BadRequest, actionExecutedContext.Exception.Message);
            }
            else if (exceptionType == typeof(UnauthorizedAccessException))
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "身份验证未通过");
            }
            else
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.InternalServerError, "服务器端发生异常");
            }
            return Task.FromResult<object>(null);
        }
    }
}