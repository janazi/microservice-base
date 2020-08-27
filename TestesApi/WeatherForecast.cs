using System;
using System.Runtime.Serialization;

namespace TestesApi
{
    [Serializable]
    [DataContract(IsReference = true, Name = "WeatherForecast", Namespace = "MicroserviceBase.Api")]

    public class WeatherForecast
    {
        [DataMember(Name = "Date", IsRequired = false)]
        public DateTime Date { get; set; }

        [DataMember(Name = "TemperatureC", IsRequired = false)]
        public int TemperatureC { get; set; }

        [DataMember(Name = "TemperatureF", IsRequired = false)]
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        [DataMember(Name = "Summary", IsRequired = false)]
        public string Summary { get; set; }
    }
}
