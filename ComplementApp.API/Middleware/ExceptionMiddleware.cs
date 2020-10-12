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
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            this._env = env;
            this._logger = logger;
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                string mensajeErrorNoManejado = string.Empty;

                if (ex.InnerException != null)
                    mensajeErrorNoManejado = ex.InnerException.Message.Replace("'", string.Empty);
                else
                    mensajeErrorNoManejado = ex.Message.Replace("'", string.Empty);

                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment()
                                ? new ApiException(context.Response.StatusCode, mensajeErrorNoManejado, ex.StackTrace?.ToString())
                                : new ApiException(context.Response.StatusCode, mensajeErrorNoManejado);

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}