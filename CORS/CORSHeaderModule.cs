using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CORS
{
    public class CORSHeaderModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += OptionsHandler;
            context.PreSendRequestHeaders += Handler;
        }

        public void Dispose()
        {
        }

        private static void OptionsHandler(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            if (app == null)
            {
                return;
            }

            var requestsToMatch = new List<Predicate<HttpRequest>> {r => r.HttpMethod == "OPTIONS"};
            if (!requestsToMatch.Any(test => test(app.Request)))
            {
                return;
            }

            app.Response.StatusCode = 200;
            app.Response.End();
        }

        private static void Handler(object sender, EventArgs e)
        {
            var httpApplication = sender as HttpApplication;
            if (httpApplication == null)
            {
                return;
            }

            AddCORSHeaders(httpApplication.Request, httpApplication.Response);
        }

        private static void AddCORSHeaders(HttpRequest request, HttpResponse response)
        {
            var origin = request.Url.GetLeftPart(UriPartial.Authority);
            var requestedOrigin = request.Headers["Origin"];
            if (requestedOrigin != null && requestedOrigin.Trim().Length > 0)
            {
                origin = requestedOrigin;
            }
            response.AddHeader("Access-Control-Allow-Origin", origin);
        }
    }
}