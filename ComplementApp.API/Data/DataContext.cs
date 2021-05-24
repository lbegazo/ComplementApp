using System;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace ComplementApp.API.Data
{

    public class DataContext : DbContext
    {
        public static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration, DbContextOptions<DataContext> options) : base(options)
        {
            Configuration = configuration;
        }

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            string connStr;

            // Depending on if in development or production, use either Heroku-provided
            // connection string, or development connection string from env var.
            if (env == "Development")
            {
                // Use connection string from file.
                connStr = Configuration.GetConnectionString("DefaultConnection");
            }
            else
            {
                connStr = Configuration.GetConnectionString("DefaultConnection");
            }
            //options.UseLoggerFactory(_loggerFactory);
            //options.EnableSensitiveDataLogging();
            options.UseSqlServer(connStr);
        }


        public DbSet<Tercero> Tercero { get; set; }
        public DbSet<Estado> Estado { get; set; }
        public DbSet<ActividadGeneral> ActividadGeneral { get; set; }
        public DbSet<ActividadEspecifica> ActividadEspecifica { get; set; }
        public DbSet<Dependencia> Dependencia { get; set; }
        public DbSet<TipoOperacion> TipoOperacion { get; set; }
        public DbSet<Estado> EstadoSolicitudCDP { get; set; }
        public DbSet<Cargo> Cargo { get; set; }
        public DbSet<Area> Area { get; set; }
        public DbSet<RubroPresupuestal> RubroPresupuestal { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<CDP> CDP { get; set; }
        public DbSet<DetalleCDP> DetalleCDP { get; set; }
        public DbSet<TipoDetalleCDP> TipoDetalleModificacion { get; set; }
        public DbSet<UsoPresupuestal> UsoPresupuestal { get; set; }
        public DbSet<PlanPago> PlanPago { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Value> Values { get; set; }
        public DbSet<Perfil> Perfil { get; set; }
        public DbSet<Transaccion> Transaccion { get; set; }
        public DbSet<PerfilTransaccion> PerfilTransaccion { get; set; }
        public DbSet<UsuarioPerfil> UsuarioPerfil { get; set; }
        public DbSet<ParametroGeneral> ParametroGeneral { get; set; }
        public DbSet<TipoBaseDeduccion> TipoBaseDeduccion { get; set; }
        public DbSet<Deduccion> Deduccion { get; set; }
        public DbSet<TerceroDeduccion> TerceroDeducciones { get; set; }
        public DbSet<ParametroLiquidacionTercero> ParametroLiquidacionTercero { get; set; }
        public DbSet<CriterioCalculoReteFuente> CriterioCalculoReteFuente { get; set; }
        public DbSet<DetalleLiquidacion> DetalleLiquidacion { get; set; }
        public DbSet<LiquidacionDeduccion> LiquidacionDeducciones { get; set; }
        public DbSet<ArchivoDetalleLiquidacion> ArchivoDetalleLiquidacion { get; set; }
        public DbSet<DetalleArchivoLiquidacion> DetalleArchivoLiquidacion { get; set; }
        public DbSet<SolicitudCDP> SolicitudCDP { get; set; }
        public DbSet<DetalleSolicitudCDP> DetalleSolicitudCDP { get; set; }
        public DbSet<ActividadEconomica> ActividadEconomica { get; set; }
        public DbSet<TipoGasto> TipoGasto { get; set; }
        public DbSet<SituacionFondo> SituacionFondo { get; set; }
        public DbSet<FuenteFinanciacion> FuenteFinanciacion { get; set; }
        public DbSet<RecursoPresupuestal> RecursoPresupuestal { get; set; }
        public DbSet<AtributoContable> AtributoContable { get; set; }
        public DbSet<CuentaContable> CuentaContable { get; set; }
        public DbSet<RelacionContable> RelacionContable { get; set; }
        public DbSet<ClavePresupuestalContable> ClavePresupuestalContable { get; set; }
        public DbSet<TipoCuentaXPagar> TipoCuentaXPagar { get; set; }
        public DbSet<TipoDePago> TipoDePago { get; set; }
        public DbSet<TipoDocumentoSoporte> TipoDocumentoSoporte { get; set; }
        public DbSet<TipoIva> TipoIva { get; set; }
        public DbSet<TipoModalidadContrato> TipoModalidadContrato { get; set; }
        public DbSet<TipoDocumentoIdentidad> TipoDocumentoIdentidad { get; set; }
        public DbSet<Contrato> Contrato { get; set; }
        public DbSet<FormatoSolicitudPago> FormatoSolicitudPago { get; set; }
        public DbSet<DetalleFormatoSolicitudPago> DetalleFormatoSolicitudPago { get; set; }
        public DbSet<TipoAdminPila> TipoAdminPila { get; set; }
        public DbSet<Numeracion> Numeracion { get; set; }
        public DbSet<TipoContrato> TipoContrato { get; set; }
        public DbSet<Pci> Pci { get; set; }
        public DbSet<ParametroSistema> ParametroSistema { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsuarioPerfil>()
                .HasKey(bc => new { bc.UsuarioId, bc.PerfilId });
            modelBuilder.Entity<UsuarioPerfil>()
                .HasOne(bc => bc.Usuario)
                .WithMany(b => b.UsuarioPerfiles)
                .HasForeignKey(bc => bc.UsuarioId);
            modelBuilder.Entity<UsuarioPerfil>()
                .HasOne(bc => bc.Perfil)
                .WithMany(c => c.UsuarioPerfiles)
                .HasForeignKey(bc => bc.PerfilId);


            modelBuilder.Entity<PerfilTransaccion>()
            .HasKey(bc => new { bc.PerfilId, bc.TransaccionId });
            modelBuilder.Entity<PerfilTransaccion>()
                .HasOne(bc => bc.Perfil)
                .WithMany(b => b.PerfilTransacciones)
                .HasForeignKey(bc => bc.PerfilId);
            modelBuilder.Entity<PerfilTransaccion>()
                .HasOne(bc => bc.Transaccion)
                .WithMany(c => c.PerfilTransacciones)
                .HasForeignKey(bc => bc.TransaccionId);


            modelBuilder.Entity<RubroPresupuestal>()
                .Property(b => b.PadreRubroId)
                .HasDefaultValue(0);

            modelBuilder.Entity<DetalleLiquidacion>()
                .Property(b => b.Procesado)
                .HasDefaultValue(0);

            modelBuilder.Entity<Tercero>()
                .Property(b => b.DeclaranteRenta)
                .HasDefaultValue(0);

            modelBuilder.Entity<DetalleArchivoLiquidacion>()
          .HasKey(bc => new { bc.ArchivoDetalleLiquidacionId, bc.DetalleLiquidacionId });
            modelBuilder.Entity<DetalleArchivoLiquidacion>()
                .HasOne(bc => bc.ArchivoDetalleLiquidacion)
                .WithMany(b => b.DetalleArchivo)
                .HasForeignKey(bc => bc.ArchivoDetalleLiquidacionId);
            modelBuilder.Entity<DetalleArchivoLiquidacion>()
                .HasOne(bc => bc.DetalleLiquidacion)
                .WithMany(c => c.DetalleArchivo)
                .HasForeignKey(bc => bc.DetalleLiquidacionId);

            modelBuilder.Entity<ParametroLiquidacionTercero>()
                .Property(b => b.FacturaElectronica)
                .HasDefaultValue(0);

            modelBuilder.Entity<ParametroLiquidacionTercero>()
            .Property(b => b.NotaLegal1)
            .HasDefaultValue(0);
            modelBuilder.Entity<ParametroLiquidacionTercero>()
            .Property(b => b.NotaLegal2)
            .HasDefaultValue(0);
            modelBuilder.Entity<ParametroLiquidacionTercero>()
            .Property(b => b.NotaLegal3)
            .HasDefaultValue(0);
            modelBuilder.Entity<ParametroLiquidacionTercero>()
            .Property(b => b.NotaLegal4)
            .HasDefaultValue(0);
            modelBuilder.Entity<ParametroLiquidacionTercero>()
            .Property(b => b.NotaLegal5)
            .HasDefaultValue(0);
            modelBuilder.Entity<ParametroLiquidacionTercero>()
            .Property(b => b.NotaLegal6)
            .HasDefaultValue(0);

            modelBuilder.Entity<Numeracion>()
                .Property(b => b.Utilizado)
                .HasDefaultValue(0);

            modelBuilder.Entity<Pci>()
                .Property(b => b.Estado)
                .HasDefaultValue(0);

            modelBuilder.Entity<Contrato>()
            .Property(b => b.EsPagoMensual)
            .HasDefaultValue(0);
        }

    }
}