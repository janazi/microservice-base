using Jnz.CorrelationTokenMiddleware;
using Jnz.JaegerExtensions;
using MassTransit;
using MicroserviceBase.Api.Consumers;
using MicroserviceBase.Api.Events;
using MicroserviceBase.Api.Options;
using MicroserviceBase.OpenTracing.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace MicroserviceBase.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJnzTracer();
            var rabbitMqOptions = new RabbitMQOptions();
            Configuration.GetSection("RabbitMQOptions").Bind(rabbitMqOptions);

            services.AddControllers();
            services.AddResponseCaching();

            //MASS TRANSIT CONFIG
            services.AddScoped(typeof(MassTransitOpenTracingSendFilter<>));
            services.AddScoped(typeof(MassTransitOpenTracingConsumeFilter<>));
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ValueConsumer>(typeof(ValueConsumerDefinition));
                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.ConfigureEndpoints(ctx);
                    cfg.UseSendFilter(typeof(MassTransitOpenTracingSendFilter<>), ctx);
                    cfg.UseConsumeFilter(typeof(MassTransitOpenTracingConsumeFilter<>), ctx);
                });
            });

            services.AddMassTransitHostedService();
            foreach (var endpoint in rabbitMqOptions.EndPoints)
            {
                var test = endpoint.Key;
                Type interfacequepreciso = Type.GetType(endpoint.Key);

                EndpointConvention.Map<StartDataPreparationCommand>(new Uri($"{rabbitMqOptions.Host}:{rabbitMqOptions.Port}" +
                    $"/{endpoint.Value}"));
            }
            //EndpointConvention.Map<type>(new System.Uri("rabbitmq://localhost:5672/start-data-preparation-command"));
            //MASS TRANSIT CONFIG


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.AddCorrelationToken();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync(
            //        $"Correlation Token from middleware {context.Request.Headers["correlation-token"]}");
            //});


            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseResponseCaching();
        }
    }
}
