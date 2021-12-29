using System;
using System.Collections.Generic;
using System.Web;
using Sitecore.Pipelines;

namespace ContainerWarmer
{
    public class WarmupArgs : PipelineArgs
    {
        public List<string> Messages { get; set; } = new List<string>();

        public bool IsFailed { get; set; }
    }

}