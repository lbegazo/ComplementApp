using System.Globalization;
using System.Text;
using AutoMapper;
using ComplementApp.API.Data;
using ComplementApp.API.Extensions;
using ComplementApp.API.Helpers;
using ComplementApp.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ComplementApp.API
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _config = configuration;
            _env = env;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*The order is not important*/

        services.AddApplicationServices(_config);
            services.AddControllers()
            .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling =
                                     Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddAutoMapper(typeof(DatingRepository).Assembly);
            services.AddCors();
            services.AddIdentityServices(_config);
            
            services.AddScoped<LogUserActivity>();
            services.AddScoped<LogActividadUsuario>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var espanolCulture = "es-CO";
            var cultureInfo = new CultureInfo(espanolCulture);
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            /*The order is extremely important*/

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseRouting();

            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthentication();

            app.UseAuthorization();

            // this will serve wwwroot/index.html when path is '/'
            app.UseDefaultFiles();

            // this will serve js, css, images etc.
            app.UseStaticFiles();

            app.UseMiddleware<NoCacheMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Fallback");
            });

        }
    }
}
