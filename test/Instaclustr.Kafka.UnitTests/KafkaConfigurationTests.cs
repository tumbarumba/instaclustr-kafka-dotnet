using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using System;
using Xunit;

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
        public void CanRetieveConfigurationSection()
        {
            var config = new KafkaConfiguration();
            var nlogSection = config.GetSection("NLog");
            Assert.IsAssignableFrom<IConfigurationSection>(nlogSection);
            LogManager.Configuration = new NLogLoggingConfiguration(nlogSection);
        }

        [Fact]
        public void CanLoadProducerConfig()
        {
            var config = new KafkaConfiguration();
            var producerConfig = config.GetProducerConfig("Kafka:Producer");

            Assert.IsType<ProducerConfig>(producerConfig);
            Assert.Equal("127.0.0.1:9092", producerConfig.BootstrapServers);
            Assert.Equal("admin", producerConfig.SaslUsername);
        }

        [Fact]
        public void CanLoadConsumerConfig()
        {
            var config = new KafkaConfiguration();
            var consumerConfig = config.GetConsumerConfig("Kafka:Consumer");

            Assert.IsType<ConsumerConfig>(consumerConfig);
            Assert.Equal("10.0.0.1:9092", consumerConfig.BootstrapServers);
            Assert.Equal("TestGroup", consumerConfig.GroupId);
        }
        
        [Fact]
        public void CanLoadSchemaRegistryConfig()
        {
            var config = new KafkaConfiguration();
            var srConfig = config.GetSchemaRegistryConfig("Kafka:SchemaRegistry");

            Assert.IsType<SchemaRegistryConfig>(srConfig);
            Assert.Equal("https://schema-registry.example.com:8085", srConfig.Url);
        }
    }
}
