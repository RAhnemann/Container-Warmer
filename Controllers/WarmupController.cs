using System;
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
            try
            {
                CorePipeline.Run("warmup", args);
            }
            catch (Exception e)
            {
                args.IsFailed = true;
                args.Messages.Add("Error Running Pipeline: " + e.Message);
            }

            HttpResponseMessage msg;

            if (args.IsFailed)
            {
                msg = verbose ? Request.CreateResponse(HttpStatusCode.InternalServerError, args.Messages) : Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error in Warmup");

                msg.Headers.Pragma.ParseAdd("no-cache");
                msg.Content.Headers.TryAddWithoutValidation("Expires", "0");

                return msg;
            }
            else
            {
                msg = verbose ? Request.CreateResponse(HttpStatusCode.OK, args.Messages) : Request.CreateResponse(HttpStatusCode.OK, "Healthy");

                msg.Headers.Pragma.ParseAdd("no-cache");
                msg.Content.Headers.TryAddWithoutValidation("Expires", "0");

                return msg;
            }
        }
    }
}