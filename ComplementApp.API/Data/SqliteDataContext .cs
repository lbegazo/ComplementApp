using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ComplementApp.API.Data
{
    public class SqliteDataContext: DataContext
    {
        public SqliteDataContext(IConfiguration configuration) : base(configuration) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
        }
    }
}