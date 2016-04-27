using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using EF.Diagnostics.Profiling;

namespace MyWebApi.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        [OverrideActionFilters]
        [OverrideAuthentication]
        [OverrideAuthorization]
        public async Task<IHttpActionResult> Index()
        {
            using (ProfilingSession.Current.Step("HomeController.Index"))
            {
                await Task.Delay(100);
                using (ProfilingSession.Current.Step(() => "Print item: "))
                {
                    return Ok("Index");
                }
            }
        }
    }
}
