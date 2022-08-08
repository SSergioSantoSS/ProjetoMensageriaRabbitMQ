using Microsoft.EntityFrameworkCore;
using ProjMens.Services.Api.Contexts.Entities;

namespace ProjMens.Services.Api.Contexts
{
    public class SqlServerContext : DbContext
    {
        public SqlServerContext(DbContextOptions<SqlServerContext> options) : base(options)
        {

        }
        public DbSet<Usuario>? Usuarios { get; set; }
    }

}
