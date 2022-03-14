## Purpose:

Container Warmer allows for the execution of a custom pipeline that can be patched with processors and invoked during the "ready" check portion of the k8s initialization process.  This, in the basic model here, translates into HTTP Requests to specific pages to pre-load the HTML cache.
There are a few aspects to the solution, some of which are not in this repository. For a full writeup, see the corresponding blog entry at [Rock, Paper, Sitecore](http://rockpapersitecore.com/2022/03/aks-quality-of-life-warming-your-containers-in-k8s).

This solution does three things:
1. Register the Web API Routes in the initialize pipeline
2. Invoke the warmup pipeline and execute the processors
3. Run the provided Url Processor, which calls a list of urls from the site.

## Configuration:

There is one setting which controls the base URL for all requests (unless overridden): **Warmup.BaseUrl**. Note that this should be http as all connectivity should happen behind your ingress.  If it's happening outside your ingress, it's going to the up and running container!

The out of the box configuration has three processors. One for CD, one for CM and one for both CM and CD.  The processor "healtcheck" should not be disabled, as it invokes the default Healthcheck endpoint to check for SQL and XConnect connectivity (why rewrite this?)

The UrlProcessor takes in two optional parameters:  allowCaching and baseUrl
- **allowCaching** - This parameter will cache the request in case there is a failure down the line, which would cause it to re-invoke the pipeline.
- **baseUrl** - This parameter sets the base url for the processor.  If not set, the processor will use the Setting above.  This setting comes into play more with multi-site implementations and should be used sparingly to avoid confusion.

Note: You're going to want to include your own config file in your solution for this to add additional URLs to the processor. Or new processors.  That's the beauty of the pipeline: this is just a start.  Hell, rip out what's here and write your own. It's up to you!

## Image:

You can retreive this image from Docker Hub: [https://hub.docker.com/r/rahnemann/container-warmer/tags](https://hub.docker.com/r/rahnemann/container-warmer/tags)