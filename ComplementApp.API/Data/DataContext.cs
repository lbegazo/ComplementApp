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

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        }

        public DbSet<Value> Values { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Photo> Photos { get; set; }

        public DbSet<TipoOperacion> TipoOperacion { get; set; }

        public DbSet<Cargo> Cargo { get; set; }

        public DbSet<Area> Area { get; set; }

        public DbSet<RubroPresupuestal> RubroPresupuestal { get; set; }

        public DbSet<Usuario> Usuario { get; set; }

        public DbSet<CDP> CDP { get; set; }

        public DbSet<DetalleCDP> DetalleCDP { get; set; }

        public DbSet<TipoDetalleModificacion> TipoDetalleModificacion { get; set; }

    }
}