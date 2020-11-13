using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ComplementApp.API.Settings;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Services;
using ComplementApp.API.Data;
using System;
using Microsoft.EntityFrameworkCore;

namespace ComplementApp.API.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<MailSettings>(config.GetSection("MailSettings"));
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IProcesoLiquidacionPlanPago, ProcesoLiquidacionPlanPago>();
            services.AddScoped<IProcesoDocumentoExcel, ProcesoDocumentoExcel>();
            services.AddScoped<IGeneralInterface, GeneralService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IDocumentoRepository, DocumentoRepository>();
            services.AddScoped<ICDPRepository, CDPRepository>();
            services.AddScoped<IListaRepository, ListaRepository>();
            services.AddScoped<IPlanPagoRepository, PlanPagoRepository>();
            services.AddScoped<IDetalleLiquidacionRepository, DetalleLiquidacionRepository>();
            services.AddScoped<ITransaccionRepository, TransaccionRepository>();

            services.AddDbContext<DataContext>();
            
            return services;
        }
    }
}