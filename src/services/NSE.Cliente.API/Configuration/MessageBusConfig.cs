using NSE.Cliente.API.Services;
using NSE.Core.Utils;
using NSE.Messagebus;

namespace NSE.Cliente.API.Configuration
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"))
                    .AddHostedService<RegistroclienteIntegrationHandler>();
        }
    }
}
