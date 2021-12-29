using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using Sitecore.Pipelines;

namespace ContainerWarmer.Controllers
{
    public class HealthzController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Warm(bool verbose = false)
        {
            var args = new WarmupArgs();

            CorePipeline.Run("warmup", args);

            if (args.IsFailed)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                msg.Headers.Pragma.ParseAdd("no-cache");
                msg.Content.Headers.TryAddWithoutValidation("Expires", "0");

                if (verbose)
                {
                    msg.Content = new StringContent(JsonConvert.SerializeObject(args.Messages), Encoding.UTF8, "text/plain");
                    msg.Content.Headers.TryAddWithoutValidation("Expires", "0");
                }

                return msg;
            }
            else
            {
                var msg = new HttpResponseMessage(HttpStatusCode.OK);
                msg.Headers.Pragma.ParseAdd("no-cache");


                if (verbose)
                {
                    msg.Content = new StringContent(JsonConvert.SerializeObject(args.Messages), Encoding.UTF8, "text/plain");
                    msg.Content.Headers.TryAddWithoutValidation("Expires", "0");
                }

                return msg;
            }
        }
    }
}