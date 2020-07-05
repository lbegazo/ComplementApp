using ComplementApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ComplementApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

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