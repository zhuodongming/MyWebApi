using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MyWebApi.Filters;

namespace MyWebApi.Controllers
{
    [Authenticate]
    public class DemoController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok("成功调用");
        }
    }
}