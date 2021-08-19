// using System;
using Xunit;
// using Confluent.Kafka;
// using Confluent.SchemaRegistry;
// using SAGov.DIS.Kafka.Client;
// using SAGov.DIS.Kafka.ObjectSchema.AvroSchema;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Newtonsoft.Json;
// using Moq;

namespace Instaclustr.Kafka
{
    public class KafkaConfigurationTests
    {
        
        [Fact]
        public void ConfigurationIsLoadedFromAppsettingsJsonByDefault()
        {
            var config = new KafkaConfiguration();

            Assert.Equal("example string",  config.GetValue<string>("ExampleString"));
            Assert.Equal(42,                config.GetValue<int>("ExampleInt"));
            Assert.Equal("127.0.0.1:9092",  config.GetValue<string>("Kafka:Producer:BootstrapServers"));
        }
    
    }
}
