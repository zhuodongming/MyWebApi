﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace MyWebApi.Filters
{
    /// <summary>
    /// 认证过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class AuthenticateAttribute : FilterAttribute, IAuthenticationFilter
    {
        private static Dictionary<string, string> userAccounters;
        public AuthenticateAttribute()
        {
            userAccounters = new Dictionary<string, string>();
            userAccounters.Add("user1", "token1");
            userAccounters.Add("user2", "token2");
            userAccounters.Add("user3", "token3");
        }
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var method = context.Request.Method.Method;
            var rawUrl = context.Request.RequestUri.PathAndQuery;
            var date = context.Request.Headers.Date.ToString();
            var bytesContent = await context.Request.Content.ReadAsByteArrayAsync();
            var contentMD5 = "MD5";//MD5(bytesContent);

            IPrincipal user = null;
            var headerValue = context.Request.Headers.Authorization;
            if (headerValue != null && headerValue.Scheme == "webapi")
            {
                string credential = Encoding.UTF8.GetString(Convert.FromBase64String(headerValue.Parameter));
                string[] split = credential.Split(':');
                if (split.Length == 2)
                {
                    string userId = split[0];
                    string token;
                    if (userAccounters.TryGetValue(userId, out token))
                    {
                        var preSignstring = method + "\n" + rawUrl + "\n" + date + "\n" + contentMD5 + "\n" + token;
                        var sign = "sgin";//MD5(preSignstring);
                        if (sign == split[1])
                        {
                            GenericIdentity identity = new GenericIdentity(userId, "webapi");
                            user = new GenericPrincipal(identity, new string[0]);
                        }
                    }
                }
            }
            context.Principal = user;
            await Task.FromResult<object>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            IPrincipal user = context.ActionContext.ControllerContext.RequestContext.Principal;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                string parameter = string.Format("realm=\"{0}\"", context.Request.RequestUri.DnsSafeHost);
                var challenge = new AuthenticationHeaderValue("Basic", parameter);
                context.Result = new UnauthorizedResult(new AuthenticationHeaderValue[] { challenge }, context.Request);
            }
            return Task.FromResult<object>(null);
        }
    }

    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    //public class AuthenticateAttribute : AuthorizationFilterAttribute
    //{
    //    private static Dictionary<string, string> userAccounters;
    //    public AuthenticateAttribute()
    //    {
    //        userAccounters = new Dictionary<string, string>();
    //        userAccounters.Add("Foo", "Password");
    //        userAccounters.Add("Bar", "Password");
    //        userAccounters.Add("Baz", "Password");
    //    }

    //    public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
    //    {
    //        //Authenticate
    //        var headerValue = actionContext.Request.Headers.Authorization;
    //        if (headerValue != null && headerValue.Scheme == "Basic")
    //        {
    //            string credential = Encoding.UTF8.GetString(Convert.FromBase64String(headerValue.Parameter));
    //            string[] split = credential.Split(':');
    //            if (split.Length == 2)
    //            {
    //                string userName = split[0];
    //                string password;
    //                if (userAccounters.TryGetValue(userName, out password))
    //                {
    //                    if (password == split[1])
    //                    {
    //                        GenericIdentity identity = new GenericIdentity(userName, "Basic");
    //                        actionContext.RequestContext.Principal = new GenericPrincipal(identity, new string[0]);
    //                        await Task.FromResult<object>(null);
    //                    }
    //                }
    //            }
    //        }

    //        //Challenge
    //        var response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
    //        string parameter = string.Format("realm=\"{0}\"", actionContext.Request.RequestUri.DnsSafeHost);
    //        var challenge = new AuthenticationHeaderValue("Basic", parameter);
    //        response.Headers.WwwAuthenticate.Add(challenge);
    //        actionContext.Response = response;

    //        await Task.FromResult<object>(null);
    //    }
    //}
}