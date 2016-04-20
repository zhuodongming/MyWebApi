using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using MyWebApi.Filters;

namespace MyWebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            //配置WebApi可跨域访问：全局启用无限制跨域访问
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            config.Filters.Add(new ExceptionHandlerFilterAttribute());//全局启用异常处理过滤器
            config.Filters.Add(new ValidActionFilterAttribute());//全局启用验证参数绑定过滤器
            config.Filters.Add(new AuthorizationRequiredFilterAttribute());//全局启用验证验证token

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
