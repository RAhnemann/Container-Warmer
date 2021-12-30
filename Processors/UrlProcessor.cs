using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Diagnostics;

namespace ContainerWarmer.Processors
{
    public class UrlProcessor
    {
        private readonly List<string> _urls = new List<string>();

        private readonly bool _allowCaching;

        private readonly string _baseUrl;

        public UrlProcessor(string allowCaching, string baseUrl)
        {
            bool.TryParse(allowCaching, out _allowCaching);
            _baseUrl = baseUrl;
        }
        public UrlProcessor(string allowCaching)
        {
            bool.TryParse(allowCaching, out _allowCaching);
            _baseUrl = Settings.GetSetting("Warmup.BaseUrl");
        }

        public UrlProcessor()
        {
            _baseUrl = Settings.GetSetting("Warmup.BaseUrl");
        }

        public void Process(WarmupArgs args)
        {
            if (args.IsFailed)
                return;

            foreach (var url in _urls)
            {
                var fullUrl = $"{_baseUrl}/{url}";


                if (_allowCaching && IsCached(fullUrl))
                {
                    Log.Info($"Warmup: Cached Url '{url}'", this);
                    args.Messages.Add($"Success. Url Warmup: '{url}' (from cache)");
                    continue;
                }

                try
                {
                    Log.Info($"Warmup: Loading Url '{url}'", this);
                    using (var client = new WebClient())
                    {
                        client.DownloadString(fullUrl);
                    }
                    args.Messages.Add($"Success. Url Warmup: '{url}'");

                    if (_allowCaching)
                    {
                        AddToCache(fullUrl);
                    }
                }
                catch (WebException wex)
                {
                    var errorResponse = wex.Response as HttpWebResponse;

                    if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        Log.Error($"Warmup: General Web Exception: {wex.Message}", wex, this);

                        args.Messages.Add($"Skipped (404). Url Warmup: '{url}'");

                        if (_allowCaching)
                        {
                            AddToCache(fullUrl);
                        }
                    }
                    else
                    {
                        Log.Error($"Warmup: General Web Exception: {wex.Message}", wex, this);
                        args.IsFailed = true;
                        args.Messages.Add($"Failed. Url Warmup: '{url}'");
                    }
                   
                }
                catch (Exception ex)
                {
                    Log.Error($"Warmup: Unhandled Error fetching page: {url}", ex, this);
                    args.IsFailed = true;
                    args.Messages.Add($"Failed. Url Warmup: '{url}'");
                }
            }
        }

        public bool IsCached(string url)
        {
            return HttpContext.Current.Application[$"Warmup_{url}"] != null;
        }

        public void AddToCache(string url)
        {
            HttpContext.Current.Application.Add($"Warmup_{url}", url);
        }

        public void IncludeUrl(string url)
        {
            Assert.IsNotNullOrEmpty(url, "Warmup: Must specify a Url");

            _urls.Add(url);
        }
    }
}