using Microsoft.EntityFrameworkCore;
using ProjMens.Services.Api.Contexts;

namespace ProjMens.Services.Api.Configurations
{
    public static class SqlServerConfiguration
    {
        public static void Add(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DB_MENS_API");

            builder.Services.AddDbContext<SqlServerContext>
                (options => options.UseSqlServer(connectionString));
        }
    }

}
