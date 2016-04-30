using MyWebApi.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace MyWebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            GlobalConfiguration.Configuration.MessageHandlers.Add(new HttpMethodOverrideHandler());//添加消息管道
        }

        protected void Application_BeginRequest(object source, EventArgs e)
        {

        }

        protected void Application_EndRequest(object source, EventArgs e)
        {

        }
    }
}
