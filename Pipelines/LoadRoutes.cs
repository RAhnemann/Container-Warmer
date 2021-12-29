using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Sitecore;
using Sitecore.Pipelines;

namespace ContainerWarmer.Pipelines
{
    public class LoadRoutes
    {
        public void Process(PipelineArgs args)
        {
            // Registration
            RouteTable.Routes.MapHttpRoute("warmup", "healthz/warm", new { action = "Warm", controller = "Healthz" });
        }
    }
}