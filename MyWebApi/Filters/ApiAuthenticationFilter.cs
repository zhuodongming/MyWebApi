using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;

namespace MyWebApi.Filters
{
    public class ApiAuthenticationFilter : BasicAuthenticationFilter
    {
        public override bool OnAuthorize(string userName, string userPassword, HttpActionContext actionContext)
        {
            //验证用户身份

            var basicAuthenticationIdentity = Thread.CurrentPrincipal.Identity as BasicAuthenticationIdentity;
            if (basicAuthenticationIdentity != null)
                basicAuthenticationIdentity.UserId = 123;
            return true;

        }
    }
}