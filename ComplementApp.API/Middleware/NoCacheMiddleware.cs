using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ComplementApp.API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ComplementApp.API.Middleware
{
    public class NoCacheMiddleware
    {
        private readonly RequestDelegate m_next;

        public NoCacheMiddleware(RequestDelegate next)
        {
            m_next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Response.OnStarting((state) =>
           {
                // ref: http://stackoverflow.com/questions/49547/making-sure-a-web-page-is-not-cached-across-all-browsers
               httpContext.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
               httpContext.Response.Headers.Append("Pragma", "no-cache");
               httpContext.Response.Headers.Append("Expires", "0");
               return Task.FromResult(0);
           }, null);

            await m_next.Invoke(httpContext);
        }
    }
}