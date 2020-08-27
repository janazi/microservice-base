using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using MicroserviceBase.Api.Events;
using OpenTracing;
using System;
using System.Threading.Tasks;

namespace MicroserviceBase.Api.Consumers
{
    public class ValueConsumer : IConsumer<StartDataPreparationCommand>
    {
        private readonly ITracer tracer;

        public ValueConsumer(ITracer tracer)
        {
            this.tracer = tracer;
        }
        public async Task Consume(ConsumeContext<StartDataPreparationCommand> context)
        {
            tracer.ActiveSpan.SetTag("Correlation-Token", context.CorrelationId.ToString());
            tracer.ActiveSpan.Log("Consumed message");

            await Console.Out.WriteLineAsync($"Consumer: {context.CorrelationId}");
        }
    }

    public class ValueConsumerDefinition : ConsumerDefinition<ValueConsumer>
    {
        public ValueConsumerDefinition()
        {
            var concurrentMessageLimit = Environment.ProcessorCount * 4;
#if (DEBUG)
            concurrentMessageLimit = 1;
#endif

            EndpointName = "start-data-preparation-command";
            ConcurrentMessageLimit = concurrentMessageLimit;

        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<ValueConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
