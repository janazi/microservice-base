using System;

namespace MicroserviceBase.Api.Events
{
    public class StartDataPreparationCommand
    {
        public Guid MigrationId { get; set; }
    }
}
