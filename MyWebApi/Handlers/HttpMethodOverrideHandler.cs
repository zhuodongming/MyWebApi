using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MyWebApi.Handlers
{
    /// <summary>
    /// 用于解决浏览器、服务器网关不支持(PUT、Delete、Patch)等http请求的消息处理器
    /// </summary>
    public class HttpMethodOverrideHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //获取X-HTTP-Method-Override，覆盖当前http请求的Method
            IEnumerable<string> methodOverrideHeader;
            if (request.Headers.TryGetValues("X-HTTP-Method-Override", out methodOverrideHeader))
            {
                request.Method = new HttpMethod(methodOverrideHeader.First());
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}