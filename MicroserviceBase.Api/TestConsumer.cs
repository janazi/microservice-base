using MassTransit;
using System;
using System.Threading.Tasks;

namespace MicroserviceBase.Api
{
    public class TestConsumer : IConsumer<Message>
    {
        public Task Consume(ConsumeContext<Message> context)
        {
            throw new NotImplementedException();
        }
    }
}
