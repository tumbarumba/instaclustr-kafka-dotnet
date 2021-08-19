using Confluent.Kafka;
using System;
using Xunit;
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

        [Fact]
        public void ConfigurationCanBeOverridenByEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("ExampleString", "overridden 1");
            Environment.SetEnvironmentVariable("Kafka__Producer__BootstrapServers", "overridden 2");
            var config = new KafkaConfiguration();

            Assert.Equal("overridden 1",    config.GetValue<string>("ExampleString"));
            Assert.Equal(42,                config.GetValue<int>("ExampleInt"));
            Assert.Equal("overridden 2",    config.GetValue<string>("Kafka:Producer:BootstrapServers"));

            Environment.SetEnvironmentVariable("ExampleString", null);
            Environment.SetEnvironmentVariable("Kafka__Producer__BootstrapServers", null);
        }

        [Fact]
        public void CanLoadProducerConfiguration()
        {
            var config = new KafkaConfiguration();
            var producerConfig = config.GetProducerConfig("Kafka:Producer");

            Assert.IsType<ProducerConfig>(producerConfig);
            Assert.Equal("127.0.0.1:9092", producerConfig.BootstrapServers);
            Assert.Equal("admin", producerConfig.SaslUsername);
        }

    }
}
