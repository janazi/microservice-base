using System.Collections.Generic;

namespace MicroserviceBase.Api.Options
{
    public class RabbitMQOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public Dictionary<string, string> EndPoints { get; set; }
    }
}
