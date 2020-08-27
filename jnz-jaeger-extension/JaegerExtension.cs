using Jaeger.Senders.Thrift;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using OpenTracing.Util;

namespace Jnz.JaegerExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJnzTracer(this IServiceCollection services)
        {
            services.TryAddSingleton(serviceProvider =>
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var senderResolver = new Jaeger.Senders.SenderResolver(loggerFactory);

                Jaeger.Configuration.SenderConfiguration
                    .DefaultSenderResolver = senderResolver.RegisterSenderFactory<ThriftSenderFactory>();

                var config = Jaeger.Configuration.FromIConfiguration(loggerFactory, configuration.GetSection("Jaeger"));

                var tracer = config.GetTracer();

                if (!GlobalTracer.IsRegistered())
                    GlobalTracer.Register(tracer);

                return tracer;
            });
            services.AddOpenTracing();
            return services;
        }
    }
}
