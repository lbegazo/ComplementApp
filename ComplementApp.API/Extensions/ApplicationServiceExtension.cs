using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ComplementApp.API.Settings;
using ComplementApp.API.Interfaces;
using ComplementApp.API.Services;
using ComplementApp.API.Data;
using ComplementApp.API.Interfaces.Repository;
using ComplementApp.API.Interfaces.Service;

namespace ComplementApp.API.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<MailSettings>(config.GetSection("MailSettings"));
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IProcesoLiquidacionPlanPago, ProcesoLiquidacionPlanPago>();
            services.AddScoped<IProcesoLiquidacionSolicitudPago, ProcesoLiquidacionSolicitudPago>();
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
            services.AddScoped<ISolicitudPagoRepository, SolicitudPagoRepository>();
            services.AddScoped<ITerceroRepository, TerceroRepository>();
            services.AddScoped<IClavePresupuestalContableRepository, ClavePresupuestalContableRepository>();
            services.AddScoped<IProcesoCreacionArchivo, ProcesoCreacionArchivo>();
            services.AddScoped<IProcesoCreacionArchivoExcel, ProcesoCreacionArchivoExcel>();
            services.AddScoped<IContratoRepository, ContratoRepository>();
            services.AddScoped<IActividadGeneralRepository, ActividadGeneralRepository>();
            services.AddScoped<IPlanAdquisicionRepository, PlanAdquisicionRepository>();
            services.AddScoped<IActividadGeneralService, ActividadGeneralService>();
            services.AddScoped<ISolicitudCdpRepository, SolicitudCdpRepository>();

            services.AddDbContext<DataContext>();

            return services;
        }
    }
}