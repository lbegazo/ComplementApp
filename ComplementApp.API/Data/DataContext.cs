using System;
using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ComplementApp.API.Data
{

    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // protected override void OnConfiguring(DbContextOptionsBuilder options)
        // {
        //     // connect to sql server database
        //     options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        // }

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
            options.UseSqlServer(connStr);
        }


        public DbSet<Tercero> Tercero { get; set; }

        public DbSet<Estado> Estado { get; set; }

        public DbSet<ActividadGeneral> ActividadGeneral { get; set; }

        public DbSet<ActividadEspecifica> ActividadEspecifica { get; set; }

        public DbSet<Dependencia> Dependencia { get; set; }

        public DbSet<TipoOperacion> TipoOperacion { get; set; }

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

            modelBuilder.Entity<TerceroDeduccion>()
          .HasKey(bc => new { bc.TerceroId, bc.DeduccionId });
            modelBuilder.Entity<TerceroDeduccion>()
                .HasOne(bc => bc.Tercero)
                .WithMany(b => b.DeduccionesXTercero)
                .HasForeignKey(bc => bc.TerceroId);
            modelBuilder.Entity<TerceroDeduccion>()
                .HasOne(bc => bc.Deduccion)
                .WithMany(c => c.DeduccionesXTercero)
                .HasForeignKey(bc => bc.DeduccionId);

            modelBuilder.Entity<RubroPresupuestal>()
                .Property(b => b.PadreRubroId)
                .HasDefaultValue(0);

            modelBuilder.Entity<DetalleLiquidacion>()
                .Property(b => b.Procesado)
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

        }

    }
}