using ProjMens.Services.Api.Helpers;
using ProjMens.Services.Api.Settings;

namespace ProjMens.Services.Api.Configurations
{
    public static class MailConfiguration
    {
        public static void Add(WebApplicationBuilder builder)
        {
            builder.Services.Configure<MailSettings>
                (builder.Configuration.GetSection("MailSettings"));

            builder.Services.AddTransient<EmailHelper>();
        }
    }

}
