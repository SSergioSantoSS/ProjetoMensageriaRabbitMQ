using Microsoft.IdentityModel.Logging;
using ProjMens.Services.Api.Settings;

namespace ProjMens.Services.Api.Configurations
{
    public static class LogConfiguration
    {
        public static void Add(WebApplicationBuilder builder)
        {
            builder.Services.Configure<LogSettings>
                (builder.Configuration.GetSection("LogSettings"));

            builder.Services.AddTransient<Helpers.LogHelper>();
        }
    }

}
