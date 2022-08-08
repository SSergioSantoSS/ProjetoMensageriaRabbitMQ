using ProjMens.Services.Api.Produces;
using ProjMens.Services.Api.Settings;

namespace ProjMens.Services.Api.Configurations
{
    public static class RabbitMQConfiguration
    {
        public static void Add(WebApplicationBuilder builder)
        {
            builder.Services.Configure<RabbitMQSettings>
                (builder.Configuration.GetSection("RabbitMQSettings"));

            builder.Services.AddTransient<ProduceMessage>();
        }
    }

}
